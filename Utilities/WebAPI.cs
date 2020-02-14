using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace Utilities
{
    public class WebAPI
    {
        /// <summary>
        /// Method for querying the HMS Flask API endpoint, which generates a jobID for the requested task->data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="errorMsg"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public static T RequestData<T>(out string errorMsg, string queryString)
        {
            errorMsg = "";
            string flaskURL = Environment.GetEnvironmentVariable("FLASK_SERVER");
            if (flaskURL == null)
            {
                flaskURL = "http://localhost:7777";
            }
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true,
                IgnoreNullValues = true
            };
            string requestURL = flaskURL + queryString;
            string dataURL = flaskURL + "/hms/data?job_id=";
            WebClient myWC = new WebClient();
            string data = "";
            dynamic taskData = "";
            try
            {
                int retries = 5;                                        // Max number of request retries
                string status = "";                                     // response status code
                string jobID = "";
                while (retries > 0 && !status.Contains("OK"))
                {
                    WebRequest wr = WebRequest.Create(requestURL);
                    HttpWebResponse response = (HttpWebResponse)wr.GetResponse();
                    status = response.StatusCode.ToString();
                    System.IO.Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    jobID = JsonSerializer.Deserialize<Dictionary<string, string>>(reader.ReadToEnd(), options)["job_id"];
                    reader.Close();
                    response.Close();
                    retries -= 1;
                    if (!status.Contains("OK"))
                    {
                        Thread.Sleep(100);
                    }
                }

                retries = 50;
                status = "";
                taskData = "";
                bool success = false;
                while (retries > 0 && !success && !jobID.Equals(""))
                {
                    Thread.Sleep(6000);
                    WebRequest wr = WebRequest.Create(dataURL + jobID);
                    HttpWebResponse response = (HttpWebResponse)wr.GetResponse();
                    status = response.StatusCode.ToString();
                    System.IO.Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    data = reader.ReadToEnd();
                    taskData = JsonSerializer.Deserialize<T>(data, options);
                    if (taskData.status == "SUCCESS")
                    {
                        success = true;
                    }
                    else if (taskData.status == "FAILURE" || taskData.status == "PENDING")
                    {
                        break;
                    }
                    reader.Close();
                    response.Close();
                    retries -= 1;
                }
            }
            catch (Exception ex)
            {
                errorMsg = "ERROR: Could not find NCEI stations for the given geometry." + ex.Message;
            }
            return taskData;
        }

    }

    public class NCEIResult
    {
        public string id { get; set; }
        public string status { get; set; }
        public List<NCEIStations> data { get; set; }
    }

    public class NCEIStations
    {
        public string id { get; set; }
        public double distance { get; set; }
        public NCEIStationData data { get; set; }
        public Dictionary<string, string> metadata { get; set; }
    }

    public class NCEIStationData
    {
        public double elevation { get; set; }
        public string mindate { get; set; }
        public string maxdate { get; set; }
        public double latitude { get; set; }
        public string name { get; set; }
        public double datacoverage { get; set; }
        public string id { get; set; }
        public string elevationUnit { get; set; }
        public double longitude { get; set; }
    }
}
