using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;
using Swashbuckle.AspNetCore.Examples;
using Swashbuckle.AspNetCore.SwaggerGen;

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
            services.AddMvc();
            services.AddLogging();

            services.AddCors();

            // Add our repository type
            // services.AddSingleton<ITodoRepository, TodoRepository>();

            //Set the comments path for the swagger json and ui.
            var basePath = PlatformServices.Default.Application.ApplicationBasePath;
            var xmlPath = Path.Combine(basePath, "App_Data\\XmlComments.xml");
            var xmlDataPath = Path.Combine(basePath, "App_Data\\XmlCommentsData.xml");

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info {
                    Title = "HMS REST API",
                    Version = "v1",
                    Description = "Swagger documentation for HMS REST API with example requests and responses."
                });
                
                c.IncludeXmlComments(xmlPath);
                c.IncludeXmlComments(xmlDataPath);
                c.OperationFilter<ExamplesOperationFilter>();
                c.OperationFilter<DescriptionOperationFilter>();
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// <summary>
        /// Configure function
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});

            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader());

            // Enable static files middleware.
            app.UseStaticFiles();
            app.UseMvc();
            //app.UseMvcWithDefaultRoute();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            //app.UseSwagger(c =>
            //{
            //    c.RouteTemplate = "HMSWS/swagger/{documentName}/swagger.json";
            //});
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                // Routing through IIS as a subdomain requires the following two lines for swagger.json to be accessible.
                // c.SwaggerEndpoint("/HMSWS/swagger/v1/swagger.json", "HMS REST API V1");
                //c.RoutePrefix = "HMSWS/swagger";
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "HMS REST API V1");
                c.DocExpansion("none");
                c.ShowRequestHeaders();
                //c.ShowJsonEditor();
            });
        }
    }
}
