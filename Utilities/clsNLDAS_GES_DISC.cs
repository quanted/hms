using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    /// <summary>
    /// Where EarthData credentials are looked up.
    /// </summary>
    public enum CredentialSource
    {
        /// <summary>Environment variables first; fall back to .edl_token / .netrc on disk. (Default)</summary>
        EnvironmentThenFile,
        /// <summary>Only environment variables.</summary>
        EnvironmentOnly,
        /// <summary>Only files on disk (original behavior).</summary>
        FileOnly
    }

    public class clsNLDAS_GES_DISC : IDisposable
    {
        // Environment variable names (same names NASA's earthaccess library uses)
        public const string TokenEnvVar = "EARTHDATA_TOKEN";
        public const string UsernameEnvVar = "EARTHDATA_USERNAME";
        public const string PasswordEnvVar = "EARTHDATA_PASSWORD";
        /// <summary>Optional override for the .netrc location (standard curl/Python convention).</summary>
        public const string NetrcPathEnvVar = "NETRC";

        private const string NetrcFileName = ".netrc";
        private const string TokenFileName = ".edl_token";

        private readonly HttpClient _httpClient;
        private readonly string _basePath;
        private readonly CredentialSource _credentialSource;
        //private const string TimeSeriesUrl = "https://api.giovanni.earthdata.nasa.gov/timeseries";
        private const string TimeSeriesUrl = "https://api.giovanni.earthdata.nasa.gov/proxy-timeseries?";
        private const string UserAgent = "GESDISC.Net v1.0";

        // Token cache (per instance; reuse the instance to benefit from it)
        private readonly object _tokenLock = new();
        private string? _cachedToken;
        private DateTimeOffset? _cachedTokenRefreshAtUtc;   // null = expiry unknown; kept until the server rejects it
        private readonly HashSet<string> _rejectedTokens = new(StringComparer.Ordinal);

        /// <summary>
        /// How long before a token's expiry it is considered "about to expire" and replaced.
        /// </summary>
        public TimeSpan TokenRefreshMargin { get; set; } = TimeSpan.FromHours(1);

        public clsNLDAS_GES_DISC(string? basePath = null,
            CredentialSource credentialSource = CredentialSource.EnvironmentThenFile)
        {
            _basePath = basePath ?? AppContext.BaseDirectory;
            _credentialSource = credentialSource;

            var handler = new HttpClientHandler()
            {
                UseCookies = true,
                CookieContainer = new System.Net.CookieContainer()
            };
            _httpClient = new HttpClient(handler);
            _httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);
        }

        private bool UseEnvironment => _credentialSource != CredentialSource.FileOnly;
        private bool UseFiles => _credentialSource != CredentialSource.EnvironmentOnly;

        #region Environment lookups

        private static string? GetEnv(string name)
        {
            var value = Environment.GetEnvironmentVariable(name);
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        /// <summary>
        /// Returns the token from EARTHDATA_TOKEN, or null if it is not set.
        /// </summary>
        public string? ReadEnvironmentToken() => GetEnv(TokenEnvVar);

        /// <summary>
        /// Returns credentials from EARTHDATA_USERNAME / EARTHDATA_PASSWORD, or null if neither is set.
        /// Throws if only one of the pair is set, so a half-configured environment isn't silently ignored.
        /// </summary>
        public (string username, string password)? ReadEnvironmentCredentials()
        {
            var username = GetEnv(UsernameEnvVar);
            var password = GetEnv(PasswordEnvVar);

            if (username == null && password == null)
                return null;

            if (username == null || password == null)
                throw new InvalidOperationException(
                    $"Incomplete EarthData credentials in environment: both {UsernameEnvVar} and {PasswordEnvVar} must be set.");

            return (username, password);
        }

        #endregion

        #region File lookups

        private string ResolveNetrcPath()
        {
            if (UseEnvironment)
            {
                var overridePath = GetEnv(NetrcPathEnvVar);
                if (overridePath != null)
                    return overridePath;
            }
            return Path.Combine(_basePath, NetrcFileName);
        }

        /// <summary>
        /// Returns the token stored in .edl_token (whole file, trimmed), or null if the file is missing or empty.
        /// </summary>
        public string? ReadTokenFile()
        {
            string tokenPath = Path.Combine(_basePath, TokenFileName);
            if (!File.Exists(tokenPath))
                return null;

            var token = File.ReadAllText(tokenPath).Trim();
            return string.IsNullOrEmpty(token) ? null : token;
        }

        public (string username, string password) ReadNetrcCredentials()
        {
            string netrcPath = ResolveNetrcPath();
            if (!File.Exists(netrcPath))
                throw new FileNotFoundException($"No .netrc file found at {netrcPath}. Please create one with your EarthData credentials.");
            var lines = File.ReadAllLines(netrcPath);
            string? username = null;
            string? password = null;

            var tokens = lines
                .SelectMany(line => line.Split('#')[0].Split((char[])null, StringSplitOptions.RemoveEmptyEntries))
                .ToArray();

            bool foundMachine = false;
            bool isEarthDataMachine = false;

            for (int i = 0; i < tokens.Length; i++)
            {
                if (tokens[i].Equals("machine", StringComparison.OrdinalIgnoreCase) && i + 1 < tokens.Length)
                {
                    foundMachine = true;
                    isEarthDataMachine = tokens[i + 1].Contains("earthdata.nasa.gov", StringComparison.OrdinalIgnoreCase);
                    i++;
                    continue;
                }

                if ((!foundMachine || isEarthDataMachine) && i + 1 < tokens.Length)
                {
                    if (tokens[i].Equals("login", StringComparison.OrdinalIgnoreCase))
                    {
                        username = tokens[++i];
                    }
                    else if (tokens[i].Equals("password", StringComparison.OrdinalIgnoreCase))
                    {
                        password = tokens[++i];
                    }
                }
            }

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                throw new InvalidOperationException("Could not find valid EarthData credentials in .netrc file.");
            return (username, password);
        }

        #endregion

        #region Token resolution and caching

        /// <summary>
        /// Returns a cached EarthData bearer token, fetching a new one only when there is none cached
        /// or the cached one is within <see cref="TokenRefreshMargin"/> of expiring.
        ///
        /// Lookup order with the default <see cref="CredentialSource.EnvironmentThenFile"/>:
        ///   1. EARTHDATA_TOKEN
        ///   2. EARTHDATA_USERNAME + EARTHDATA_PASSWORD (exchanged for a token)
        ///   3. .edl_token file in the base path
        ///   4. .netrc (from NETRC if set, else the base path), exchanged for a token
        /// A static token (1 or 3) that is expired, about to expire, or was rejected by the server
        /// is skipped in favor of the next source.
        /// </summary>
        public string GetAccessToken()
        {
            lock (_tokenLock)
            {
                if (_cachedToken != null &&
                    (_cachedTokenRefreshAtUtc == null || DateTimeOffset.UtcNow < _cachedTokenRefreshAtUtc))
                {
                    return _cachedToken;
                }

                try
                {
                    var (token, expiresUtc) = ResolveToken();
                    _cachedToken = token;
                    _cachedTokenRefreshAtUtc = ComputeRefreshAt(expiresUtc);
                    return token;
                }
                catch (Exception ex)
                {
                    _cachedToken = null;
                    _cachedTokenRefreshAtUtc = null;
                    throw new InvalidOperationException("Authentication with NASA EarthData failed. " +
                        $"Please set {TokenEnvVar} (or {UsernameEnvVar}/{PasswordEnvVar}) in the environment, " +
                        "or ensure that your .edl_token or .netrc file is stored and contains valid credentials. " +
                        $"Error: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Drops the cached token so the next <see cref="GetAccessToken"/> call fetches a new one.
        /// Pass the token the server rejected; a static token (env var or .edl_token) with that value
        /// won't be used again by this instance. If another caller has already replaced the cached
        /// token, the newer token is kept.
        /// </summary>
        public void InvalidateToken(string? rejectedToken = null)
        {
            lock (_tokenLock)
            {
                if (rejectedToken != null)
                {
                    _rejectedTokens.Add(rejectedToken);
                    if (_cachedToken != rejectedToken)
                        return;
                }
                _cachedToken = null;
                _cachedTokenRefreshAtUtc = null;
            }
        }

        private (string token, DateTimeOffset? expiresUtc) ResolveToken()
        {
            string? skippedReason = null;
            (string token, DateTimeOffset? expiresUtc)? lastResort = null;

            // Accepts a static token only if it isn't rejected and isn't about to expire.
            // A still-valid-but-expiring token is kept as a last resort in case no credentials exist.
            bool TryStaticToken(string? token, string sourceName, out (string token, DateTimeOffset? expiresUtc) result)
            {
                result = default;
                if (token == null)
                    return false;

                if (_rejectedTokens.Contains(token))
                {
                    skippedReason ??= $"The token from {sourceName} was rejected by the server.";
                    return false;
                }

                var expiresUtc = GetJwtExpiration(token);
                if (expiresUtc.HasValue && expiresUtc.Value - TokenRefreshMargin <= DateTimeOffset.UtcNow)
                {
                    skippedReason ??= $"The token from {sourceName} expires at {expiresUtc.Value:u}.";
                    if (expiresUtc.Value > DateTimeOffset.UtcNow)
                        lastResort ??= (token, expiresUtc);
                    return false;
                }

                result = (token, expiresUtc);
                return true;
            }

            if (UseEnvironment)
            {
                if (TryStaticToken(ReadEnvironmentToken(), TokenEnvVar, out var envToken))
                    return envToken;

                if (ReadEnvironmentCredentials() is { } creds)
                    return TokenFromCredentials(creds.username, creds.password);
            }

            if (UseFiles)
            {
                if (TryStaticToken(ReadTokenFile(), TokenFileName, out var fileToken))
                    return fileToken;

                // With nothing skipped, a missing .netrc should raise its usual FileNotFoundException.
                if (skippedReason == null || File.Exists(ResolveNetrcPath()))
                {
                    var (username, password) = ReadNetrcCredentials();
                    return TokenFromCredentials(username, password);
                }
            }

            if (lastResort is { } fallback)
                return fallback;

            if (skippedReason != null)
                throw new InvalidOperationException(
                    $"{skippedReason} No username/password credentials are available to obtain a new token.");

            throw new InvalidOperationException(
                $"No EarthData credentials found in environment. Set {TokenEnvVar}, " +
                $"or {UsernameEnvVar} and {PasswordEnvVar}.");
        }

        private (string token, DateTimeOffset? expiresUtc) TokenFromCredentials(string username, string password)
        {
            var token = FindOrCreateToken(username, password);
            if (string.IsNullOrEmpty(token))
                throw new InvalidOperationException("Failed to obtain access token");
            return (token, GetJwtExpiration(token));
        }

        /// <summary>
        /// Refresh a margin before expiry. If the token is already inside the margin (the login server
        /// returned an existing token that is close to expiring), keep it until it actually expires,
        /// since asking again before then would just return the same token.
        /// </summary>
        private DateTimeOffset? ComputeRefreshAt(DateTimeOffset? expiresUtc)
        {
            if (expiresUtc == null)
                return null;
            var refreshAt = expiresUtc.Value - TokenRefreshMargin;
            return refreshAt > DateTimeOffset.UtcNow ? refreshAt : expiresUtc;
        }

        /// <summary>
        /// Reads the "exp" claim from an EarthData Login token (a JWT). Returns null if the token
        /// isn't a JWT or has no expiry, in which case it is kept until the server rejects it.
        /// </summary>
        private static DateTimeOffset? GetJwtExpiration(string token)
        {
            try
            {
                var parts = token.Split('.');
                if (parts.Length < 2)
                    return null;

                var payload = parts[1].Replace('-', '+').Replace('_', '/');
                payload = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');
                var claims = JObject.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(payload)));

                var exp = claims["exp"];
                return exp == null ? null : DateTimeOffset.FromUnixTimeSeconds(exp.Value<long>());
            }
            catch
            {
                return null;
            }
        }

        #endregion

        public string? FindOrCreateToken(string username, string password)
        {
            try
            {
                var findOrCreateTokenUrl = "https://urs.earthdata.nasa.gov/api/users/find_or_create_token";
                var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
                var request = new HttpRequestMessage(HttpMethod.Post, findOrCreateTokenUrl);
                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);
                request.Headers.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                var response = _httpClient.Send(request);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    var tokenResponse = JObject.Parse(responseContent);
                    return tokenResponse["access_token"]?.ToString();
                }
                else
                {
                    var errorContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    throw new HttpRequestException($"Token request failed with status {response.StatusCode}: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to find or create token: {ex.Message}", ex);
            }
        }

        public string CallTimeSeries(double lat, double lon, string timeStart, string timeEnd, string dataVariable, string accessToken)
        {
            var requestUrl = BuildTimeSeriesUrl(lat, lon, timeStart, timeEnd, dataVariable);
            var uriBuilder = new UriBuilder(requestUrl);
            var request = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            var response = _httpClient.Send(request);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                // StatusCode is carried on the exception so callers can detect 401s.
                throw new HttpRequestException(
                    $"API request failed with status {response.StatusCode}: {errorContent}",
                    null,
                    response.StatusCode);
            }
            return response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        }

        public static string BuildTimeSeriesUrl(double lat, double lon, string timeStart, string timeEnd, string dataVariable, string? baseUrl = null)
        {
            var queryParams = new Dictionary<string, string>
            {
                ["data"] = dataVariable,
                ["location"] = $"[{lat.ToString(CultureInfo.InvariantCulture)},{lon.ToString(CultureInfo.InvariantCulture)}]",
                ["time"] = $"{timeStart}/{timeEnd}"
            };
            var uriBuilder = new UriBuilder(string.IsNullOrWhiteSpace(baseUrl) ? TimeSeriesUrl : baseUrl.Split('?')[0]);
            var query = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
            uriBuilder.Query = query;
            return uriBuilder.Uri.ToString();
        }

        public (Dictionary<string, string> headers, List<TimeSeriesDataPoint> dataPoints) ParseCsv(string csvData)
        {
            var lines = csvData.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var headers = new Dictionary<string, string>();
            var dataPoints = new List<TimeSeriesDataPoint>();
            if (lines.Length < 15)
            {
                throw new InvalidOperationException(
                    "The returned CSV is empty or incomplete.\n" +
                    "Please ensure that your subsetting bounds are within the extent of your dataset\n" +
                    "or that your credentials (environment variables, .edl_token or .netrc) are valid.");
            }
            for (int i = 0; i < 13 && i < lines.Length; i++)
            {
                var parts = lines[i].Split(',', 2);
                if (parts.Length >= 2)
                {
                    headers[parts[0]] = parts[1].Trim();
                }
            }
            for (int i = 15; i < lines.Length; i++)
            {
                var parts = lines[i].Split(',');
                if (parts.Length >= 2)
                {
                    if (DateTime.TryParse(parts[0], out var timestamp) &&
                        double.TryParse(parts[1], out var value))
                    {
                        dataPoints.Add(new TimeSeriesDataPoint
                        {
                            Timestamp = timestamp,
                            Value = value
                        });
                    }
                }
            }
            return (headers, dataPoints);
        }

        public (Dictionary<string, string> headers, List<TimeSeriesDataPoint> dataPoints)
            GetTimeSeriesData(double lat, double lon, string timeStart, string timeEnd,
            string dataVariable = "NLDAS_FORA0125_H_2_0_Rainf")
        {
            var accessToken = GetAccessToken();
            string csvData;
            try
            {
                csvData = CallTimeSeries(lat, lon, timeStart, timeEnd, dataVariable, accessToken);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                // Token was revoked or expired early: drop it, get a fresh one, and retry once.
                InvalidateToken(accessToken);
                accessToken = GetAccessToken();
                csvData = CallTimeSeries(lat, lon, timeStart, timeEnd, dataVariable, accessToken);
            }
            return ParseCsv(csvData);
        }

        public bool SaveToCsv(Dictionary<string, string> headers, List<TimeSeriesDataPoint> dataPoints, string filePath)
        {
            bool success = false;
            try
            {
                using var writer = new StreamWriter(filePath);
                foreach (var header in headers)
                {
                    writer.WriteLine($"{header.Key},{header.Value}");
                }
                writer.WriteLine();
                writer.WriteLine("Timestamp,Value");
                foreach (var point in dataPoints)
                {
                    writer.WriteLine($"{point.Timestamp:yyyy-MM-ddTHH:mm:ss},{point.Value}");
                }
                success = true;
            }
            catch (Exception ex)
            {

            }
            return success;
        }


        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        public static string GetUrlInfoValue()
        {
            throw new NotImplementedException();
        }

        public object FindOrCreateToken(string accessToken)
        {
            throw new NotImplementedException();
        }
    }

    public class TimeSeriesDataPoint
    {
        public DateTime Timestamp { get; set; }
        public double Value { get; set; }
    }

}