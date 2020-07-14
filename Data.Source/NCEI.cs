using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Data.Source
{

    public class NCEI<T>
    {
        /// <summary>
        /// Get data from NCEI source for the specific datatype expecting a List of type T
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="dataTypeID"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public List<T> GetData(out string errorMsg, string dataTypeID, ITimeSeriesInput input)
        {
            errorMsg = "";

            List<T> data = DownLoadData(out errorMsg, dataTypeID, input);
            if (errorMsg.Contains("ERROR")) { return null; }

            return data;
        }

        /// <summary>
        /// Starts the sequence of functions to download ncdc data.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="dataset"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private List<T> DownLoadData(out string errorMsg, string dataTypeID, ITimeSeriesInput input)
        {
            errorMsg = "";

            DateTime tempStartDate = input.DateTimeSpan.StartDate;
            DateTime tempEndDate = input.DateTimeSpan.EndDate;
            string stationID = "";
            if (input.Geometry.StationID != null)
            {
                stationID = input.Geometry.StationID;
            }
            else
            {
                stationID = input.Geometry.GeometryMetadata["stationID"];
            }
            string token = input.Geometry.GeometryMetadata["token"];

            string url = ConstructURL(out errorMsg, dataTypeID, stationID, input.BaseURL.First(), tempStartDate, tempEndDate);
            if (errorMsg.Contains("ERROR")) { return null; }

            string json = SendRequest(token, url, 0).Result;
            if (errorMsg.Contains("ERROR")) { return null; }

            List<T> data = new List<T>();
            if (!json.Equals("{}"))
            {
                try
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,                       
                        IgnoreNullValues = true
                    };
                    options.Converters.Add(new Utilities.DoubleConverter());
                    data = JsonSerializer.Deserialize<List<T>>(json, options);
                }
                catch (System.Text.Json.JsonException ex)
                {
                    errorMsg = ex.Message;
                    Log.Warning("Erroring serializing json response from NCEI data request. URL: {0}", url);
                }               
            }
            return data;
        }

        /// <summary>
        /// Constructs url for ncdc download
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="station">stationID</param>
        /// <param name="dateTime">IDateTimeSpan</param>
        /// <returns></returns>
        private string ConstructURL(out string errorMsg, string dataTypeID, string station, string baseURL, DateTime startDate, DateTime endDate)
        {
            errorMsg = "";
            StringBuilder sb = new StringBuilder();

            if (station.Contains("GHCND"))
            {
                sb.Append(baseURL);
                station = station.Remove(0, 6);
                sb.Append("dataset=daily-summaries" + "&dataTypes=" + dataTypeID + "&stations=" + station + "&startDate=" + startDate.ToString("yyyy-MM-dd") + "&endDate=" + endDate.ToString("yyyy-MM-dd") + "&format=json" + "&includeAttributes=true" + "&units=metric");
            }
            else
            {
                errorMsg = "ERROR: NCEI web service does not currently support the dataset for this station.";
            }
            return sb.ToString();
        }

        /// <summary>
        /// Downloads NCDC data using the provided token and url string.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="token">Required for accessing ncdc data.</param>
        /// <param name="url"></param>
        /// <returns></returns>
        private async Task<string> SendRequest(string token, string url, int retries)
        {
            string data = "";
            HttpClient hc = new HttpClient();
            HttpResponseMessage wm = new HttpResponseMessage();
            hc.DefaultRequestHeaders.Add("token", token);
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
                    Log.Warning("Error: Failed to download ncei data. Retry {0}:{1}", retries, maxRetries);
                    Random r = new Random();
                    Thread.Sleep(5000 + (r.Next(10) * 1000));
                    return this.SendRequest(token, url, retries).Result;
                }
                wm.Dispose();
                hc.Dispose();
                Log.Warning(ex, "Error: Failed to download ncei data.");
                return null;
            }
            wm.Dispose();
            hc.Dispose();
            return data;
        }

        /// <summary>
        /// Gets NCEI station details.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="stationID"></param>
        /// <param name="token">Token required for accessing NCEI services.</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetStationDetails(out string errorMsg, string url, string stationID, string token)
        {
            errorMsg = "";

            Dictionary<string, string> stationDetails = new Dictionary<string, string>();
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true
            };

            url = url + stationID;
            if (String.IsNullOrWhiteSpace(token))
            {
                errorMsg = "ERROR: No token provided for retrieving NCEI station details.";
                return new Dictionary<string, string>();
            }
            try
            {
                int retries = 5;                                        // Max number of request retries
                string status = "";                                     // response status code
                Dictionary<string, object> details = new Dictionary<string, object>();

                while (retries > 0 && !status.Contains("OK"))
                {
                    Thread.Sleep(333);
                    WebRequest wr = WebRequest.Create(url);
                    wr.Headers.Add("token", token);
                    HttpWebResponse response = (HttpWebResponse)wr.GetResponse();
                    status = response.StatusCode.ToString();
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    string dataBuffer = reader.ReadToEnd();
                    details = JsonSerializer.Deserialize<Dictionary<string, object>>(dataBuffer, options);
                    response.Close();
                    retries -= 1;
                }
                foreach (KeyValuePair<string, object> kv in details)
                {
                    stationDetails[kv.Key] = kv.Value.ToString();
                }
            }
            catch (Exception ex)
            {
                errorMsg = "ERROR: Unable to download NCEI station details. " + ex.Message;
                return new Dictionary<string, string>();
            }
            return stationDetails;
        }

        /// <summary>
        /// Checks the attributes string for a specific value and returns the altered value based upon what attributes are present.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="value"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static double AttributeCheck(out string errorMsg, double value, string attribute)
        {
            //Explanation of quality flags: https://www1.ncdc.noaa.gov/pub/data/cdo/documentation/GHCND_documentation.pdf
            char[] qualityFlags = new char[] { 'D', 'G', 'I', 'K', 'L', 'M', 'N', 'O', 'R', 'S', 'T', 'W', 'X', 'Z' };

            List<string> tables = new List<string>();
            if (attribute != null)
            {
                tables = attribute.Split(',').ToList();
            }
            else
            {
                attribute = "";
            }

            errorMsg = "";
            if (attribute.Contains("[") || attribute.Contains("]"))             // Begin and end of deleted period, during the given hour
            {
                return -9999;
            }
            else if (attribute.Contains("{") || attribute.Contains("}"))        // Begin and end of missing period, during the given hour
            {
                return -9999;
            }
            if(tables.Count > 1){
                if (tables[0].Contains("M") || tables[1].IndexOfAny(qualityFlags) != -1)        // One period of missing data or failed QA check
                {
                    return -9999;
                }
            }
            return value;
        }

    }
}
