using System;

namespace Utilities
{
    /// <summary>
    /// Process-wide accessor for EarthData credentials/config.
    /// Reads exclusively from environment variables (set via ConfigMap/Secret in k8s).
    /// </summary>
    public static class EarthDataConfig
    {
        public static string? AccessToken => Get("EARTHDATA_ACCESS_TOKEN");
        public static string? Username => Get("EARTHDATA_USERNAME");
        public static string? Password => Get("EARTHDATA_PASSWORD");

        public static string FindOrCreateTokenUrl =>
            Get("EarthData__FindOrCreateTokenUrl")
            ?? "https://urs.earthdata.nasa.gov/api/users/find_or_create_token";

        public static int TokenLifetimeDays
        {
            get
            {
                var raw = Get("EARTHDATA_TOKEN_LIFETIME_DAYS");
                return int.TryParse(raw, out var days) && days > 0 ? days : 50;
            }
        }

        public static bool HasToken => !string.IsNullOrWhiteSpace(AccessToken);
        public static bool HasCredentials =>
            !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);

        private static string? Get(string key)
        {
            var v = Environment.GetEnvironmentVariable(key);
            return string.IsNullOrWhiteSpace(v) ? null : v;
        }
    }
}