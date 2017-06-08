using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;

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
        public string GetData(out string errorMsg, string dataSet, ITimeSeriesInput input)
        {
            errorMsg = "";

            string url = ConstructURL(out errorMsg, dataSet, input);
            if (errorMsg.Contains("ERROR")) { return null; }

            string data = DownloadData(out errorMsg, url);
            if (errorMsg.Contains("ERROR")) { return null; }

            return data;
        }

        /// <summary>
        /// Downloads the Daymet data using the constructed URL.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        private string DownloadData(out string errorMsg, string url)
        {
            errorMsg = "";
            string data = "";
            WebClient myWC = new WebClient();
            try
            {
                int retries = 5;                                        // Max number of request retries
                string status = "";                                     // response status code

                while (retries > 0 && !status.Contains("OK"))
                {
                    Thread.Sleep(100);
                    WebRequest wr = WebRequest.Create(url);
                    HttpWebResponse response = (HttpWebResponse)wr.GetResponse();
                    status = response.StatusCode.ToString();
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    data = reader.ReadToEnd();
                    reader.Close();
                    response.Close();
                    retries -= 1;
                }
            }
            catch (Exception ex)
            {
                errorMsg = "ERROR: Unable to download data from Daymet. " + ex.Message;
                return null;
            }
            return data;
        }

        /// <summary>
        /// Constructs the url for Daymet rest API.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="dataSet"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private string ConstructURL(out string errorMsg, string dataSet, ITimeSeriesInput input)
        {
            errorMsg = "";
            StringBuilder sb = new StringBuilder();
            try
            {
                Dictionary<string, string> urls = (Dictionary<string, string>)HttpContext.Current.Application["urlList"];
                sb.Append(urls["Daymet_" + dataSet + "_URL"]);
            }
            catch(Exception ex)
            {
                errorMsg = "ERROR: Unable to load url for " + dataSet + " dataset for Daymet. " + ex.Message;
                return null;
            }
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
        private string GetMeasuredParam(out string errorMsg, string dataset)
        {
            errorMsg = "";
            switch (dataset)
            {
                case "Precip":
                    return "prcp";
                case "Temp":
                    return "tmax,tmin";
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
        private string GetListOfYears(out string errorMsg, DateTime startDate, DateTime endDate)
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

    }
}
