using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Web.Services
{
    /// <summary>
    /// </summary>
    public class WebApiApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// </summary>
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            FileToApplicationDict("url_info.txt", "urlList");       // Loads url list into Application variable as "urlList"

            // Token for accessing ncdc data.
            Application["ncdc_token"] = "RUYNSTvfSvtosAoakBSpgxcHASBxazzP";
            
        }

        /// <summary>
        /// Reads the contents of a space delimited file and populates the specified Application variable.
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <param name="variableName">Name for the Application variable</param>
        public void FileToApplicationDict(string fileName, string variableName)
        {
            Dictionary<string, string> fileValues = new Dictionary<string, string>();
            foreach (string line in File.ReadLines(Server.MapPath("~/App_Data/" + fileName)))
            {
                string[] lineValues = line.Split(' ');
                if (!fileValues.ContainsKey(lineValues[0]) && lineValues.Length > 1)
                {
                    fileValues.Add(lineValues[0], lineValues[1]);
                }
            }
            Application[variableName] = fileValues;
        }
    }
}
