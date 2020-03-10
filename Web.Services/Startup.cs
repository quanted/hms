using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Filters;
using Web.Services.Controllers;
using Utilities;
using Serilog;

namespace Web.Services
{
    /// <summary>
    /// Web Services Startup Class
    /// </summary>
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        /// <summary>
        /// Configure services function
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.WriteIndented = true;
                options.JsonSerializerOptions.AllowTrailingCommas = true;
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.Converters.Add(new Utilities.IntegerConverter());
                options.JsonSerializerOptions.Converters.Add(new Utilities.DoubleConverter());
                options.JsonSerializerOptions.Converters.Add(new Utilities.BooleanConverter());
                options.JsonSerializerOptions.Converters.Add(new Utilities.DateTimeConverterUsingDateTimeParse());
                options.JsonSerializerOptions.IgnoreNullValues = true;
            });

            services.AddLogging();

            services.AddCors();

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {
                    Title = "HMS REST API",
                    Version = "v1",
                    Description = "Swagger documentation for HMS REST API with example requests and responses."
                });

                var xmlPath = Path.Combine(AppContext.BaseDirectory, "XmlComments.xml");
                var xmlDataPath = Path.Combine(AppContext.BaseDirectory, "XmlCommentsData.xml");

                c.IncludeXmlComments(xmlPath);
                c.IncludeXmlComments(xmlDataPath);

                c.ExampleFilters();
            });

            // ---- Swashbuckle Swagger Examples for POST requests ---- //

            services.AddSwaggerExamplesFromAssemblyOf<ContaminantLoaderInputExample>();
            services.AddSwaggerExamplesFromAssemblyOf<DewPointInputExample>();
            services.AddSwaggerExamplesFromAssemblyOf<EvapotranspirationInputExample>();
            services.AddSwaggerExamplesFromAssemblyOf<HumidityInputExample>();
            services.AddSwaggerExamplesFromAssemblyOf<PrecipitationInputExample>();
            services.AddSwaggerExamplesFromAssemblyOf<PressureInputExample>();
            services.AddSwaggerExamplesFromAssemblyOf<RadiationInputExample>();
            services.AddSwaggerExamplesFromAssemblyOf<SoilMoistureInputExample>();
            services.AddSwaggerExamplesFromAssemblyOf<SolarInputExample>();
            services.AddSwaggerExamplesFromAssemblyOf<SolarCalcInputExample>();
            services.AddSwaggerExamplesFromAssemblyOf<SubSurfaceFlowInputExample>();
            services.AddSwaggerExamplesFromAssemblyOf<SurfaceRunoffInputExample>();
            services.AddSwaggerExamplesFromAssemblyOf<TemperatureInputExample>();
            services.AddSwaggerExamplesFromAssemblyOf<TotalFlowInputExample>();
            services.AddSwaggerExamplesFromAssemblyOf<WatershedDelineationInputExample>();
            services.AddSwaggerExamplesFromAssemblyOf<WatershedWorkflowInputExample>();
            services.AddSwaggerExamplesFromAssemblyOf<WindInputExample>();
            services.AddSwaggerExamplesFromAssemblyOf<PrecipitationCompareInputExample>();
            services.AddSwaggerExamplesFromAssemblyOf<PrecipitationExtractionInputExample>();
            services.AddSwaggerExamplesFromAssemblyOf<WaterQualityInputExample>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// <summary>
        /// Configure function
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="loggerFactory"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

            app.UseMiddleware<GCMiddleware>();
            app.UseSerilogRequestLogging();
            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader());

            // Enable static files middleware.
            app.UseStaticFiles();
            // app.UseMvcWithDefaultRoute();
;
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                // Routing through IIS as a subdomain requires the following  line for swagger.json to be accessible.
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "HMS REST API V1");
            });

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
