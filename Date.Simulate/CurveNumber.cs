using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;

namespace Data.Simulate
{
    /// <summary>
    /// Curve Number base class.
    /// Classified as simulation data, simulation takes place on a different server so only a standard data call is made.
    /// </summary>
    public class CurveNumber
    {

        /// <summary>
        /// Get simulated curve number data.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public string Simulate(out string errorMsg, ITimeSeriesInput input)
        {
            errorMsg = "";

            string url = ConstructURL(out errorMsg);
            if (errorMsg.Contains("ERROR")) { return null; }

            byte[] body = ConstructPOST(out errorMsg, input);
            if (errorMsg.Contains("ERROR")) { return null; }

            string data = DownloadData(out errorMsg, url, body);
            if (errorMsg.Contains("ERROR")) { return null; }

            return data;
        }

        /// <summary>
        /// Constructs the url to retrieve curvenumber data.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        private string ConstructURL(out string errorMsg)
        {
            errorMsg = "";
            string source_url = "CURVE_NUMBER_INT";
            try
            {
                Dictionary<string, string> urls = (Dictionary<string, string>)HttpContext.Current.Application["urlList"];
                return urls[source_url];
            }
            catch(Exception ex)
            {
                errorMsg = "ERROR: Unable to curve number url to retrieve data." + ex.Message;
                return null;
            }
        }

        /// <summary>
        /// Constructs the body of the post for retrieving curvenumber data.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private byte[] ConstructPOST(out string errorMsg, ITimeSeriesInput input)
        {
            errorMsg = "";
            string parameters = "startdate={" + input.DateTimeSpan.StartDate.ToString() + "}&enddate={" + input.DateTimeSpan.EndDate.ToString() +
                "}&latitude={" + input.Geometry.Point.Latitude.ToString() + "}&longitude={" + input.Geometry.Point.Longitude.ToString() + "}";
            return Encoding.UTF8.GetBytes(parameters);
        }

        /// <summary>
        /// Sends request for curvenumber timeseries data.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="url"></param>
        /// <param name="postBody"></param>
        /// <returns></returns>
        private string DownloadData(out string errorMsg, string url, byte[] postBody)
        {
            errorMsg = "";
            string data = "";
            try
            {
                int retries = 5;
                string status = "";
                while (retries > 0 && !status.Contains("OK"))
                {
                    Thread.Sleep(100);
                    WebRequest wr = WebRequest.Create(url);

                    wr.Method = "POST";
                    wr.ContentType = "application/x-www-form-urlencoded";
                    wr.ContentLength = postBody.Length;
                    using (Stream stream = wr.GetRequestStream())
                    {
                        stream.Write(postBody, 0, postBody.Length);
                    }

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
                errorMsg = "ERROR: Unable to download requested curve number data. " + ex.Message;
                return null;
            }

            return data;
        }

        /// <summary>
        /// Parse data string and set to output object.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="dataset"></param>
        /// <param name="data"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public ITimeSeriesOutput SetDataToOutput(out string errorMsg, string dataset, string data, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";
            //TODO: Format data to output object.
            return null; 
        }
    }
}
