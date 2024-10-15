using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Swashbuckle.AspNetCore.Filters;

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
                var builder = CreateHostBuilder(args);
                builder.ConfigureServices(services => services.AddSerilog());

                var app = builder.Build();
                app.Run();
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

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseSetting(WebHostDefaults.DetailedErrorsKey, "true");
                webBuilder.ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                          .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                    config.AddEnvironmentVariables();

                });
                webBuilder.UseStartup<Startup>();
            });

        /// <summary>
        /// Web Services build Web host method
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        //public static IHostedService BuildWebHost(string[] args)
        //{
        //    await Host.CreateDefaultBuilder(args)
        //        .UseSetting(WebHostDefaults.DetailedErrorsKey, "true")
        //        .ConfigureAppConfiguration((hostingContext, config) =>
        //        {
        //            var env = hostingContext.HostingEnvironment;
        //            config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        //                  .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
        //            config.AddEnvironmentVariables();
        //        })
        //        .UseStartup<Startup>()
        //        .Build();
        //}

    }
}
