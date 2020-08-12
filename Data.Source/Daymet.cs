using Serilog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Data.Source
{
    public class Daymet
    {

        /// <summary>
        /// GetData function for Daymet base class.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="dataSet"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public string GetData(out string errorMsg, string dataSet, ITimeSeriesInput input, int retries = 0)
        {
            errorMsg = "";

            string url = ConstructURL(out errorMsg, dataSet, input);
            if (errorMsg.Contains("ERROR")) { return null; }

            string data = DownloadData(url, retries).Result;
            if (errorMsg.Contains("ERROR")) { return null; }

            return data;
        }

        /// <summary>
        /// Downloads the Daymet data using the constructed URL.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        private async Task<string> DownloadData(string url, int retries)
        {
            string data = "";
            HttpClient hc = new HttpClient();
            HttpResponseMessage wm = new HttpResponseMessage();
            int maxRetries = 10;

            try
            {
                string status = "";

                while (retries < maxRetries && !status.Contains("OK"))
                {
                    wm = await hc.GetAsync(url);
                    var response = wm.Content;
                    status = wm.StatusCode.ToString();
                    data = await wm.Content.ReadAsStringAsync();
                    retries += 1;
                    if (!status.Contains("OK"))
                    {
                        Thread.Sleep(1000 * retries);
                    }
                }
            }
            catch (Exception ex)
            {
                if (retries < maxRetries)
                {
                    retries += 1;
                    Log.Warning("Error: Failed to download daymet data. Retry {0}:{1}", retries, maxRetries);
                    Random r = new Random();
                    Thread.Sleep(5000 + (r.Next(10) * 1000));
                    return this.DownloadData(url, retries).Result;
                }
                wm.Dispose();
                hc.Dispose();
                Log.Warning(ex, "Error: Failed to download daymet data.");
                return null;
            }
            wm.Dispose();
            hc.Dispose();
            return data;
        }

        /// <summary>
        /// Constructs the url for Daymet rest API.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="dataSet"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string ConstructURL(out string errorMsg, string dataSet, ITimeSeriesInput input)
        {
            errorMsg = "";
            StringBuilder sb = new StringBuilder();
            if(input.BaseURL == null)
            {
                errorMsg = "ERROR: Unable to build url for dataset = " + dataSet + " and source = " + input.Source;
                return null;
            }
            sb.Append(input.BaseURL[0]);
            sb.Append("lat=" + input.Geometry.Point.Latitude + "&lon=" + input.Geometry.Point.Longitude);                   // Adds coordinates to url variable string
            sb.Append("&measuredParams=" + GetMeasuredParam(out errorMsg, dataSet));                                        // Adds dataset variable to string
            string years = GetListOfYears(out errorMsg, input.DateTimeSpan.StartDate, input.DateTimeSpan.EndDate);
            sb.Append("&year=" + years);                                                                                    // Adds start and end dates, only takes years
            if (errorMsg.Contains("ERROR")) { return null; }
            return sb.ToString();
        }

        /// <summary>
        /// Returns the measured parameter string used in constructing the URL for Daymet data.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="dataset"></param>
        /// <returns></returns>
        private static string GetMeasuredParam(out string errorMsg, string dataset)
        {
            errorMsg = "";
            switch (dataset.ToLower())
            {
                case "precip":
                case "precipitation":
                    return "prcp";
                case "temp":
                case "temperature":
                    return "tmax,tmin";
                case "radiation":
                case "rad":
                    return "srad";
                default:
                    errorMsg = "ERROR: Parameter for Daynet did not load.";
                    return null;
            }
        }

        /// <summary>
        /// Converts date parameters to list of years.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        private static string GetListOfYears(out string errorMsg, DateTime startDate, DateTime endDate)
        {
            errorMsg = "";
            StringBuilder st = new StringBuilder();
            int yearDif = (endDate.Year - startDate.Year);
            for (int i = 0; i <= yearDif; i++)
            {
                string year = startDate.AddYears(i).Year.ToString();
                st.Append(year + ",");
            }
            st.Remove(st.Length - 1, 1);
            return st.ToString();
        }

        /// <summary>
        /// Directly downloads from the source using the testInput object. Used for checking the status of the GLDAS endpoints.
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="testInput"></param>
        /// <returns></returns>
        public static Dictionary<string, string> CheckStatus(string dataset, ITimeSeriesInput testInput)
        {
            string url = ConstructURL(out string errorMsg, dataset, testInput);
            try
            {
                WebRequest wr = WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)wr.GetResponse();
                string status = response.StatusCode.ToString();
                string description = response.StatusDescription;
                response.Close();
                return new Dictionary<string, string>()
                {
                    { "status", status },
                    { "description", description}
                };
            }
            catch (Exception ex)
            {
                return new Dictionary<string, string>()
                {
                    { "status", "ERROR" },
                    { "description", ex.Message }
                };
            }
        }
    }
}
