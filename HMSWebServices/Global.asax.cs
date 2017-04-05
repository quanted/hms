using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Http;
using System.Web.Services;
using System.IO;
using System.Collections;

namespace HMSWebServices
{
    public class Global : HttpApplication
    {
		//public static Dictionary<string, string> urlList { get; set; }

        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            GlobalConfiguration.Configure(WebApiConfig.Register);

			// Load url_info.txt file from App_Data
			ReadURLFile("url_info.txt");


        }




		/// <summary>
		/// Reads the contents of a space delimited file and populates urlList dictionary adding it to the application variables.
		/// </summary>
		/// <param name="fileName">File name.</param>
		private void ReadURLFile(string fileName)
		{
			Dictionary<string, string> urls = new Dictionary<string, string>();
			foreach (string url in File.ReadLines(fileName))
			{
				string[] urlDetails = url.Split(' ');
				if (!urls.ContainsKey(urlDetails[0]) || urlDetails.Length > 1)
				{
					urls.Add(urlDetails[0], urlDetails[1]);
				}
			}
			Application["urlList"] = urls;
		}
    }
}