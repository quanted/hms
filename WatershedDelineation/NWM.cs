using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using Utilities;

namespace WatershedDelineation
{
    public class NWMData
    {
        public Dictionary<string, List<String>> data { get; set; }
        public Dictionary<string, string> metadata { get; set; }
    }

    public class NWMObject
    {
        public string id { get; set; }
        public string status { get; set; }
        public string data { get; set; }
    }

    public class NWM
    {
        public string GetData(out string errorMsg, string url)
        {
            errorMsg = "";
            string flaskURL = Environment.GetEnvironmentVariable("FLASK_SERVER");
            if (flaskURL == null)
            {
                flaskURL = "http://localhost:7777";
            }
            Debug.WriteLine("Flask Server URL: " + flaskURL);

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
                    WebRequest wr = WebRequest.Create(url);
                    HttpWebResponse response = (HttpWebResponse)wr.GetResponse();
                    status = response.StatusCode.ToString();
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    jobID = JSON.Deserialize<Dictionary<string, string>>(reader.ReadToEnd())["job_id"];
                    reader.Close();
                    response.Close();
                    retries -= 1;
                    if (!status.Contains("OK"))
                    {
                        Thread.Sleep(100);
                    }
                }
                Thread.Sleep(1000);
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
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    data = reader.ReadToEnd();
                    taskData = JSON.Deserialize<NWMObject>(data);
                    if (taskData.status == "SUCCESS")
                    {
                        success = true;
                    }
                    else if (taskData.status == "FAILURE")
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
                errorMsg = "ERROR: Could not find National Water Model data for the given input. " + ex.Message;
            }
            return data;
        }
    }
}
