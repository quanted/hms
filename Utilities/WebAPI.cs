using Serilog;
using System;
using System.Collections.Generic;
using System.Net.Http;

using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

//using System.Net;
//using Newtonsoft.Json;
//using System.IO;

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
        public static async Task<T> RequestData<T>(string queryString, int pings = 100, string flaskURL = null, string dataURL = null)
        {
            flaskURL = flaskURL is null ? Environment.GetEnvironmentVariable("FLASK_SERVER") : flaskURL;
            if(flaskURL == null)
            {
                flaskURL = "localhost:7777";
            }

            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true,
                IgnoreNullValues = true
            };
            string requestURL = flaskURL + queryString;
            dataURL = dataURL is null ? flaskURL + "/hms/data?job_id=" : dataURL + "?job_id=";
            string data = "";

            HttpClient hc = new HttpClient();
            hc.Timeout = TimeSpan.FromMinutes(10);
            HttpResponseMessage wm = new HttpResponseMessage();
            try
            {
                int retries = 0;
                int maxRetries = 10;
                string status = "";

                while (retries < maxRetries && !status.Contains("OK"))
                {
                    wm = await hc.GetAsync(requestURL).ConfigureAwait(false);   //JSC 5/16/2022 add ConfigureAwait(false) see https://devblogs.microsoft.com/dotnet/configureawait-faq/
                    var response = wm.Content;
                    status = wm.StatusCode.ToString();
                    data = await wm.Content.ReadAsStringAsync();
                    retries += 1;
                    if (!status.Contains("OK"))
                    {
                        Thread.Sleep(100 * retries);
                    }
                }
            }
            catch (Exception ex)
            {
                wm.Dispose();
                hc.Dispose();
                Log.Warning(ex, "Error: Failed to send flask request");
                return default(T);
            }
            string jobID = JsonSerializer.Deserialize<Dictionary<string, string>>(data, options)["job_id"];

            Thread.Sleep(1000);

            dynamic taskData = "";
            try
            {
                int retries = 0;
                int maxRetries = pings;
                string status = "";
                bool success = false;
                wm = await hc.GetAsync(dataURL + jobID);
                dataURL = dataURL + jobID;
                while (retries < maxRetries && !success && !jobID.Equals(""))
                {
                    wm = await hc.GetAsync(dataURL);
                    var response = wm.Content;
                    status = wm.StatusCode.ToString();
                    data = await wm.Content.ReadAsStringAsync();
                    retries += 1;
                    taskData = JsonSerializer.Deserialize<T>(data, options);
                    if (taskData.status == "SUCCESS" && taskData.data != null)
                    {
                        success = true;
                    }
                    else if (taskData.status == "FAILURE")
                    {
                        Log.Warning("Warning: Task failed to complete, status: " + taskData.status);
                        break;
                    }
                    else
                    {
                        Thread.Sleep(5000);
                    }
                    retries += 1;
                }
                Log.Warning("TASK END, ID: " + jobID + ", STATUS: " + taskData.status + ", TRY COUNT: " + retries.ToString() + "/" + pings.ToString());
                if(taskData.status != "SUCCESS")    // Send task cancel request on non-success
                {
                    string revokeUrl = flaskURL + "/hms/task/revoke/?task_id=" + jobID;
                    await hc.GetAsync(revokeUrl);
                }
            }
            catch (Exception ex)
            {
                wm.Dispose();
                hc.Dispose();
                Log.Warning(ex, "ERROR: unable to complete request to flask request.");
                return default(T);
            }

            return taskData;
        }


//        public static async Task<T> RequestData<T>(HttpWebRequest request, int pings = 100, string flaskURL = null, string dataURL = null)
//        {
//            var response = (HttpWebResponse)request.GetResponse();
            

//            HttpClient hc = new HttpClient();
//            hc.Timeout = TimeSpan.FromMinutes(10);
//            HttpResponseMessage wm = new HttpResponseMessage();
//            try
//            {
//                int retries = 0;
//                int maxRetries = 10;
//                string status = "";

//                while (retries < maxRetries && !status.Contains("OK"))
//                {
//                    wm = await hc.GetAsync(request).ConfigureAwait(false);   //JSC 5/16/2022 add ConfigureAwait(false) see https://devblogs.microsoft.com/dotnet/configureawait-faq/
//                    var response = wm.Content;
//                    status = wm.StatusCode.ToString();
//                    data = await wm.Content.ReadAsStringAsync();
//                    retries += 1;
//                    if (!status.Contains("OK"))
//                    {
//                        Thread.Sleep(100 * retries);
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                wm.Dispose();
//                hc.Dispose();
//                Log.Warning(ex, "Error: Failed to send flask request");
//                return default(T);
//            }
            

//            string taskID = JsonConvert.DeserializeObject<Dictionary<string, string>>(new StreamReader(response.GetResponseStream()).ReadToEnd())["job_id"];


   
//            string jobID = JsonSerializer.Deserialize<Dictionary<string, string>>(data, options)["job_id"];

//            Thread.Sleep(1000);

//            dynamic taskData = "";
//            try
//            {
//                int retries = 0;
//                int maxRetries = pings;
//                string status = "";
//                bool success = false;
//                wm = await hc.GetAsync(dataURL + jobID);
//                dataURL = dataURL + jobID;
//                while (retries < maxRetries && !success && !jobID.Equals(""))
//                {
//                    wm = await hc.GetAsync(dataURL);
//                    var response = wm.Content;
//                    status = wm.StatusCode.ToString();
//                    data = await wm.Content.ReadAsStringAsync();
//                    retries += 1;
//                    taskData = JsonSerializer.Deserialize<T>(data, options);
//                    if (taskData.status == "SUCCESS" && taskData.data != null)
//                    {
//                        success = true;
//                    }
//                    else if (taskData.status == "FAILURE")
//                    {
//                        Log.Warning("Warning: Task failed to complete, status: " + taskData.status);
//                        break;
//                    }
//                    else
//                    {
//                        Thread.Sleep(5000);
//                    }
//                    retries += 1;
//                }
//                Log.Warning("TASK END, ID: " + jobID + ", STATUS: " + taskData.status + ", TRY COUNT: " + retries.ToString() + "/" + pings.ToString());
//                if (taskData.status != "SUCCESS")    // Send task cancel request on non-success
//                {
//                    string revokeUrl = flaskURL + "/hms/task/revoke/?task_id=" + jobID;
//                    await hc.GetAsync(revokeUrl);
//                }
//            }
//            catch (Exception ex)
//            {
//                wm.Dispose();
//                hc.Dispose();
//                Log.Warning(ex, "ERROR: unable to complete request to flask request.");
//                return default(T);
//            }

//            return taskData;
//        }

    }



public class FlaskData<T>
    {
        public string id { get; set; }
        public string status { get; set; }
        public T data { get; set; }
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
