using Data;
using System;
using System.Collections.Generic;
using Utilities;

namespace Precipitation
{
    public class DataDict
    {
        public Dictionary<string, string> data { get; set; }
        public Dictionary<string, string> metadata { get; set; }
    }
    /// <summary>
    /// Result structure of the json string retrieved from nwm.
    /// </summary>
    public class Result
    {
        public string id { get; set; }
        public string status { get; set; }
        public DataDict data;
    }

    public class NWM
    {
        public ITimeSeriesOutput GetData(out string errorMsg, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";

            if(input.Geometry.Point == null)
            {
                Dictionary<string, string> centroidDict = Utilities.SQLite.GetData("./App_Data/catchments.sqlite", "SELECT CentroidLatitude, CentroidLongitude FROM PlusFlowlineVAA WHERE ComID =='" + input.Geometry.ComID + "' AND CentroidLatitude IS NOT NULL AND CentroidLongitude IS NOT NULL");
                input.Geometry.Point.Latitude = double.Parse(centroidDict["CentroidLatitude"]);
                input.Geometry.Point.Longitude = double.Parse(centroidDict["CentroidLongitude"]);
            }
            
            string url = "/hms/nwm/data/?dataset=precipitation&long=" + input.Geometry.Point.Longitude + "&lat=" + input.Geometry.Point.Latitude + "&startDate=" + input.DateTimeSpan.StartDate.ToString("yyyy-MM-dd") + "&endDate=" + input.DateTimeSpan.EndDate.ToString("yyyy-MM-dd");
            ITimeSeriesOutput nwmOutput = output;
            
            string data = DownloadData(out errorMsg, url);
            Data.Source.NLDAS nldas = new Data.Source.NLDAS();

            if (errorMsg.Contains("ERROR"))
            {
                Utilities.ErrorOutput err = new Utilities.ErrorOutput();
                output = err.ReturnError("Precipitation", "nwm", errorMsg);
                errorMsg = "";
                return output;
            }
            else
            {
                nwmOutput = SetDataToOutput(out errorMsg, data, output, input);
            }
            nwmOutput = TemporalAggregation(out errorMsg, output, input);
            if (errorMsg.Contains("ERROR")) { return null; }
            
            return nwmOutput;
        }

        /// <summary>
        /// Checks for temporal resolution and runs appropriate aggregation function.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private ITimeSeriesOutput TemporalAggregation(out string errorMsg, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";
            output.Metadata.Add("nwm_temporalresolution", input.TemporalResolution);
            output.Metadata.Add("column_1", "Date");
            if (input.Units.Contains("imperial")) { output.Metadata["nwm_unit"] = "mm"; }

            // NLDAS static methods used for aggregation as NWM is identical in function. Modifier refers to the 3hr different to nldas's hourly resolution.
            switch (input.TemporalResolution)
            {
                case "daily":
                    output.Data = NLDAS.DailyAggregatedSum(out errorMsg, 23, 1.0, output, input);
                    output.Metadata.Add("column_2", "Daily Total");
                    return output;
                case "weekly":
                    output.Data = NLDAS.WeeklyAggregatedSum(out errorMsg, 1.0, output, input);
                    output.Metadata.Add("column_2", "Weekly Total");
                    return output;
                case "monthly":
                    output.Data = NLDAS.MonthlyAggregatedSum(out errorMsg, 1.0, output, input);
                    output.Metadata.Add("column_2", "Monthly Total");
                    return output;
                case "yearly":
                    output.Data = NLDAS.YearlyAggregatedSum(out errorMsg, 23, 1.0, output, input);
                    output.Metadata.Add("column_2", "Yearly Total");
                    return output;
                default:
                    output.Metadata.Add("column_2", "Hourly Total");
                    return output;
            }
        }

        private string DownloadData(out string errorMsg, string url)
        {
            errorMsg = "";
            string result = Utilities.WebAPI.RequestData<string>(url).Result;
            return result;

            //string flaskURL = Environment.GetEnvironmentVariable("FLASK_SERVER");
            //if (flaskURL == null)
            //{
            //    flaskURL = "http://localhost:7777";
            //}
            //Debug.WriteLine("Flask Server URL: " + flaskURL);

            //string dataURL = flaskURL + "/hms/data?job_id=";
            //WebClient myWC = new WebClient();
            //string data = "";
            //dynamic taskData = "";
            //try
            //{
            //    int retries = 5;                                        // Max number of request retries
            //    string status = "";                                     // response status code
            //    string jobID = "";
            //    while (retries > 0 && !status.Contains("OK"))
            //    {
            //        WebRequest wr = WebRequest.Create(flaskURL + url);
            //        HttpWebResponse response = (HttpWebResponse)wr.GetResponse();
            //        status = response.StatusCode.ToString();
            //        Stream dataStream = response.GetResponseStream();
            //        StreamReader reader = new StreamReader(dataStream);
            //        jobID = JSON.Deserialize<Dictionary<string, string>>(reader.ReadToEnd())["job_id"];
            //        reader.Close();
            //        response.Close();
            //        retries -= 1;
            //        if (!status.Contains("OK"))
            //        {
            //            Thread.Sleep(500);
            //        }
            //    }

            //    Thread.Sleep(500);
            //    int maxRetries = 100;
            //    retries = 0;
            //    status = "";
            //    taskData = "";
            //    bool success = false;
            //    while (retries < maxRetries && !success && !jobID.Equals(""))
            //    {
            //        WebRequest wr = WebRequest.Create(dataURL + jobID);
            //        HttpWebResponse response = (HttpWebResponse)wr.GetResponse();
            //        status = response.StatusCode.ToString();
            //        Stream dataStream = response.GetResponseStream();
            //        StreamReader reader = new StreamReader(dataStream);
            //        data = reader.ReadToEnd();
            //        taskData = JSON.Deserialize<dynamic>(data);
            //        if (taskData["status"] == "SUCCESS")
            //        {
            //            success = true;
            //        }
            //        else if (taskData["status"] == "FAILURE" || taskData["status"] == "PENDING")
            //        {
            //            reader.Close();
            //            response.Close();
            //            break;
            //        }
            //        else
            //        {
            //            retries += 1;
            //            Thread.Sleep(5000);
            //        }
            //        reader.Close();
            //        response.Close();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    errorMsg = "ERROR: Could not find National Water Model data for the given input. " + ex.Message;
            //}
            //return data;
        }

        private ITimeSeriesOutput SetDataToOutput(out string errorMsg, string data, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";
            Result result = JSON.Deserialize<Result>(data);
            
            foreach (KeyValuePair<string, string> kvp in result.data.data)
            {
                List<string> timestepData = new List<string>();
                double mmsval = (Convert.ToDouble(kvp.Value) / 60) / 0.0393701;
                timestepData.Add(mmsval.ToString());
                DateTime date = DateTime.ParseExact(kvp.Key, "yyyy-MM-ddTHH:mm:ss", null);
                output.Data.Add(date.ToString("yyyy-MM-dd HH"), timestepData);
            }
            output.Dataset = "Precipitation";
            output.DataSource = "nwm";
            output.Metadata.Add("date_of_creation", result.data.metadata["nwm_date_of_creation"].ToString());
            output.Metadata.Add("dataset_info", result.data.metadata["nwm_dataset_info"].ToString());
            output.Metadata.Add("units", result.data.metadata["nwm_units"].ToString());
            return output;
        }
    }
}
