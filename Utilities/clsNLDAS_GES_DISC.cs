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

        /// <summary>
        /// Resolves an EarthData bearer token. With the default <see cref="CredentialSource.EnvironmentThenFile"/>:
        ///   1. EARTHDATA_TOKEN
        ///   2. EARTHDATA_USERNAME + EARTHDATA_PASSWORD (exchanged for a token)
        ///   3. .edl_token file in the base path
        ///   4. .netrc (from NETRC if set, else the base path), exchanged for a token
        /// </summary>
        public string GetAccessToken()
        {
            try
            {
                if (UseEnvironment)
                {
                    var envToken = ReadEnvironmentToken();
                    if (envToken != null)
                        return envToken;

                    var envCreds = ReadEnvironmentCredentials();
                    if (envCreds is { } creds)
                        return RequireToken(FindOrCreateToken(creds.username, creds.password));
                }

                if (UseFiles)
                {
                    var fileToken = ReadTokenFile();
                    if (fileToken != null)
                        return fileToken;

                    var (username, password) = ReadNetrcCredentials();
                    return RequireToken(FindOrCreateToken(username, password));
                }

                throw new InvalidOperationException(
                    $"No EarthData credentials found in environment. Set {TokenEnvVar}, " +
                    $"or {UsernameEnvVar} and {PasswordEnvVar}.");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Authentication with NASA EarthData failed. " +
                    $"Please set {TokenEnvVar} (or {UsernameEnvVar}/{PasswordEnvVar}) in the environment, " +
                    "or ensure that your .edl_token or .netrc file is stored and contains valid credentials. " +
                    $"Error: {ex.Message}", ex);
            }
        }

        private static string RequireToken(string? token)
        {
            if (string.IsNullOrEmpty(token))
                throw new InvalidOperationException("Failed to obtain access token");
            return token;
        }

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
                throw new HttpRequestException($"API request failed with status {response.StatusCode}: {errorContent}");
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
            var csvData = CallTimeSeries(lat, lon, timeStart, timeEnd, dataVariable, accessToken);
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