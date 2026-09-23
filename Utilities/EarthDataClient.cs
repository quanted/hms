using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Utilities
{
    /// <summary>
    /// EarthData/NLDAS Giovanni client that resolves credentials from environment/config
    /// (via <see cref="EarthDataConfig"/>) instead of a .netrc file.
    /// </summary>
    public class EarthDataClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private const string TimeSeriesUrl = "https://api.giovanni.earthdata.nasa.gov/proxy-timeseries?";
        private const string UserAgent = "GESDISC.Net v1.0";

        // Process-wide, auto-reloading token cache.
        private static string? _cachedToken;
        private static DateTime _tokenExpiresUtc = DateTime.MinValue;
        private static readonly object _tokenLock = new();

        public EarthDataClient()
        {
            var handler = new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = new System.Net.CookieContainer()
            };
            _httpClient = new HttpClient(handler);
            _httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);
        }

        public string GetAccessToken()
        {
            // Fast path: a valid cached token.
            if (IsCacheValid())
                return _cachedToken!;

            lock (_tokenLock)
            {
                if (IsCacheValid())
                    return _cachedToken!;

                // 1) Pre-minted token from config. We don't know its exact issue date,
                //    re-read from config on expiry so a rotated token
                //    (new pod env) is picked up.
                if (EarthDataConfig.HasToken)
                {
                    _cachedToken = EarthDataConfig.AccessToken!;
                    _tokenExpiresUtc = DateTime.UtcNow.AddDays(EarthDataConfig.TokenLifetimeDays);
                    return _cachedToken;
                }

                // 2) Mint from username/password.
                if (!EarthDataConfig.HasCredentials)
                    throw new InvalidOperationException(
                        "No EarthData auth configured. Set EarthData__AccessToken, " +
                        "or EarthData__Username and EarthData__Password " +
                        "(env vars / appsettings / user-secrets).");

                var token = FindOrCreateToken(EarthDataConfig.Username!, EarthDataConfig.Password!);
                if (string.IsNullOrEmpty(token))
                    throw new InvalidOperationException("Failed to obtain access token from EarthData.");

                _cachedToken = token;
                _tokenExpiresUtc = DateTime.UtcNow.AddDays(EarthDataConfig.TokenLifetimeDays);
                return _cachedToken!;
            }
        }

        private static bool IsCacheValid() =>
            !string.IsNullOrEmpty(_cachedToken) && DateTime.UtcNow < _tokenExpiresUtc;

        /// <summary>Force the next GetAccessToken() call to re-resolve/re-mint.</summary>
        public static void InvalidateToken()
        {
            lock (_tokenLock)
            {
                _cachedToken = null;
                _tokenExpiresUtc = DateTime.MinValue;
            }
        }

        public string? FindOrCreateToken(string username, string password)
        {
            try
            {
                var url = EarthDataConfig.FindOrCreateTokenUrl;
                var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);
                request.Headers.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var response = _httpClient.Send(request);
                if (response.IsSuccessStatusCode)
                {
                    var body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    return JObject.Parse(body)["access_token"]?.ToString();
                }

                var err = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                throw new HttpRequestException($"Token request failed with status {response.StatusCode}: {err}");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to find or create token: {ex.Message}", ex);
            }
        }

        public string CallTimeSeries(double lat, double lon, string timeStart, string timeEnd, string dataVariable, string accessToken)
        {
            var requestUrl = BuildTimeSeriesUrl(lat, lon, timeStart, timeEnd, dataVariable);
            var request = new HttpRequestMessage(HttpMethod.Get, new UriBuilder(requestUrl).Uri);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            var response = _httpClient.Send(request);
            if (!response.IsSuccessStatusCode)
            {
                // If the token was rejected, drop the cache so the next call re-mints.
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                    response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    InvalidateToken();
                }
                var err = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                throw new HttpRequestException($"API request failed with status {response.StatusCode}: {err}");
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
            uriBuilder.Query = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
            return uriBuilder.Uri.ToString();
        }

        public (Dictionary<string, string> headers, List<TimeSeriesDataPoint> dataPoints) ParseCsv(string csvData)
        {
            var lines = csvData.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var headers = new Dictionary<string, string>();
            var dataPoints = new List<TimeSeriesDataPoint>();
            if (lines.Length < 15)
                throw new InvalidOperationException(
                    "The returned CSV is empty or incomplete.\n" +
                    "Please ensure that your subsetting bounds are within the extent of your dataset\n" +
                    "or that your EarthData credentials are valid.");
            for (int i = 0; i < 13 && i < lines.Length; i++)
            {
                var parts = lines[i].Split(',', 2);
                if (parts.Length >= 2) headers[parts[0]] = parts[1].Trim();
            }
            for (int i = 15; i < lines.Length; i++)
            {
                var parts = lines[i].Split(',');
                if (parts.Length >= 2 &&
                    DateTime.TryParse(parts[0], out var timestamp) &&
                    double.TryParse(parts[1], out var value))
                {
                    dataPoints.Add(new TimeSeriesDataPoint { Timestamp = timestamp, Value = value });
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
            try
            {
                using var writer = new StreamWriter(filePath);
                foreach (var header in headers)
                    writer.WriteLine($"{header.Key},{header.Value}");
                writer.WriteLine();
                writer.WriteLine("Timestamp,Value");
                foreach (var point in dataPoints)
                    writer.WriteLine($"{point.Timestamp:yyyy-MM-ddTHH:mm:ss},{point.Value}");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Dispose() => _httpClient?.Dispose();
    }
}