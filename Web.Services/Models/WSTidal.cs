using System;
using System.Net;
using System.Reflection;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Web.Services.Models
{
    /// <summary>
    /// Model object class for all Tidal parameters and making an API request to:
    /// https://api.tidesandcurrents.noaa.gov/api/prod/
    /// </summary>
    public class WSTidal
    {
        public string station { get; set; }
        public string begin_date { get; set; }
        public string end_date { get; set; }
        public string date { get; set; }
        public string range { get; set; }
        public string product { get; set; }
        public string datum { get; set; }
        public string vel_type { get; set; }
        public string units { get; set; }
        public string time_zone { get; set; }
        public string interval { get; set; }
        public string bin { get; set; }
        public string application { get; set; }
        public const string format = "json";

        /// <summary>
        /// Makes a GET request with the properties of this class. Returns the request response
        /// to the front-end/user. 
        /// </summary>
        /// <returns>JSON object of the response.</returns>
        public JObject GetTidalData()
        {
            try
            {
                WebClient webClient = new WebClient();
                const string url = "https://api.tidesandcurrents.noaa.gov/api/prod/datagetter";

                // Itterate through class properties.
                foreach (PropertyInfo prop in this.GetType().GetProperties())
                {
                    string value = prop.GetValue(this)?.ToString();
                    if (!String.IsNullOrEmpty(value))
                    {
                        webClient.QueryString.Add(prop.Name, value);
                    }
                }
                // Add format to query.
                webClient.QueryString.Add("format", format);

                // Parse string result to json JObject and return.
                return JObject.Parse(webClient.DownloadString(url));
            }
            catch (WebException exception)
            {
                // Catch bad request response and return as json to front-end/user.
                string responseText = "{\"Error\":\"unkown error\"}";
                var responseStream = exception.Response?.GetResponseStream();
                if (responseStream != null)
                {
                    using var reader = new StreamReader(responseStream);
                    responseText = reader.ReadToEnd();
                }
                return JObject.Parse(responseText);
            }
        }
    }
}
