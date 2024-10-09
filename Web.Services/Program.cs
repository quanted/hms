using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Web.Services
{
    /// <summary>
    /// Program class
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Web Services Main method
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.AspNewCore", Serilog.Events.LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(Serilog.Filters.Matching.WithProperty<string>("Type", t => t == "exception"))
                    .WriteTo.File("App_Data//exception_log.txt", restrictedToMinimumLevel:Serilog.Events.LogEventLevel.Warning)
                )
                .CreateLogger();
            
            try
            {
                Log.Information("HMS web host starting");
                BuildWebHost(args).Run();
            }
            catch(Exception ex)
            {
                Log.Fatal(ex, "HMS terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }

        }

        /// <summary>
        /// Web Services build Web host method
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .CaptureStartupErrors(true)
                .UseSetting(WebHostDefaults.DetailedErrorsKey, "true")
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                          .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                    config.AddEnvironmentVariables();
                })
                .UseSerilog()
                .UseStartup<Startup>()
                .Build();
    }
}
