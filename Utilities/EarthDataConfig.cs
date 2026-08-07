using Microsoft.Extensions.Configuration;
using System;

namespace Utilities
{
    /// <summary>
    /// Process-wide accessor for EarthData credentials/config.
    /// Precedence: environment variables > user-secrets (Development) > appsettings.json.
    /// </summary>
    public static class EarthDataConfig
    {
        private static readonly Lazy<IConfiguration> _config = new(BuildConfiguration);

        private static IConfiguration BuildConfiguration()
        {
            var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
                      ?? "Production";

            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: false);

            if (env.Equals("Development", StringComparison.OrdinalIgnoreCase))
            {
                try { builder.AddUserSecrets(System.Reflection.Assembly.GetEntryAssembly()!, optional: true); }
                catch { /* no user-secrets configured; ignore */ }
            }

            builder.AddEnvironmentVariables();
            return builder.Build();
        }

        public static string? AccessToken => Get("EarthData:AccessToken");
        public static string? Username => Get("EarthData:Username");
        public static string? Password => Get("EarthData:Password");

        public static string FindOrCreateTokenUrl =>
            Get("EarthData:FindOrCreateTokenUrl")
            ?? "https://urs.earthdata.nasa.gov/api/users/find_or_create_token";

        /// <summary>
        /// How long a minted token is treated as valid before re-minting.
        /// EarthData tokens last 60 days; default to 50 to re-mint with margin.
        /// </summary>
        public static int TokenLifetimeDays
        {
            get
            {
                var raw = Get("EarthData:TokenLifetimeDays");
                return int.TryParse(raw, out var days) && days > 0 ? days : 50;
            }
        }

        public static bool HasToken => !string.IsNullOrWhiteSpace(AccessToken);
        public static bool HasCredentials =>
            !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);

        private static string? Get(string key)
        {
            var v = _config.Value[key];
            return string.IsNullOrWhiteSpace(v) ? null : v;
        }
    }
}