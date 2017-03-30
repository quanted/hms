using HMSLDAS;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HMSNCDC
{
    /// <summary>
    /// Metadata of json string retrieved from ncdc.
    /// </summary>
    public class MetaData
    {
        public Resultset resultset { get; set; }
    }

    /// <summary>
    /// Result structure of the json string retrieved from ncdc.
    /// </summary>
    public class Result
    {
        public string date { get; set; }
        public string datatype { get; set; }
        public string station { get; set; }
        public string attributes { get; set; }
        public double value { get; set; }
    }

    /// <summary>
    /// Details for metadata.
    /// </summary>
    public class Resultset
    {
        public int offset { get; set; }
        public int count { get; set; }
        public int limit { get; set; }
    }

    /// <summary>
    /// Complete json object returned from NCDC.
    /// </summary>
    public class NCDCJson
    {
        public MetaData metadata { get; set; }
        public List<Result> results { get; set; }
    }

    public class HMSNCDC
    {

        public Dictionary<DateTime, double> timeSeries;             //Final timeSeries is stored while it is being constructed.

        /// <summary>
        /// Begins the sequence of methods to retrieve NCDC data.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="module"></param>
        /// <param name="dataset"></param>
        /// <param name="station"></param>
        /// <param name="newTS"></param>
        public void BeginNCDCSequence(out string errorMsg, IHMSModule module, string dataset, string station, HMSTimeSeries.HMSTimeSeries newTS)
        {
            errorMsg = "";
            string data = "";
            if (String.IsNullOrWhiteSpace(station))
            {
                //use lat/lon to find closest station and return data from there.
            }
            else
            {
                data = GetData(out errorMsg, module.startDate, module.endDate, module.ts[0], "PRECIP_HLY", station);
                if (errorMsg.Contains("ERROR")) { return; }
                module.ts[0].ConvertTSDictToTS(out errorMsg);
                if (errorMsg.Contains("ERROR")) { return; }
            }
        }

        /// <summary>
        /// Method containing core logic for handling the variables for obtaining the NCDC data.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="ts"></param>
        /// <param name="dataset"></param>
        /// <param name="station"></param>
        /// <returns></returns>
        private string GetData(out string errorMsg, DateTime startDate, DateTime endDate, HMSTimeSeries.HMSTimeSeries ts, string dataset, string station)
        {
            errorMsg = "";
            this.timeSeries = new Dictionary<DateTime, double>();
            string data = "";
            DateTime tempStartDate = startDate;
            DateTime tempEndDate = endDate;
            string url = "";

            //Check if date difference is greater than one year, if true splits dates apart into multiple requests that are equal to 1y-1day.
            int requiredCalls = Convert.ToInt16(Math.Ceiling((endDate - startDate).TotalDays / 365));
            for (int i = 0; i < requiredCalls; i++)
            {
                if (i == 0 && requiredCalls == 1)       //url constructed for a single call being made
                {
                    tempStartDate = startDate;
                    tempEndDate = endDate;
                    url = ConstructURL(out errorMsg) + SetVariables(out errorMsg, tempStartDate, tempEndDate, station);
                    if (errorMsg.Contains("ERROR")) { return null; }
                }
                else if (i == 0 && requiredCalls != 1)  //url constructed for first call of multiple
                {
                    tempStartDate = startDate;
                    tempEndDate = tempStartDate.AddYears(1).AddDays(-1);
                    url = ConstructURL(out errorMsg) + SetVariables(out errorMsg, tempStartDate, tempEndDate, station);
                    if (errorMsg.Contains("ERROR")) { return null; }
                }
                else if (i != 0 && i == requiredCalls - 1) //url constructed for last call of multiple
                {
                    tempStartDate = tempEndDate;
                    tempEndDate = endDate;
                    url = ConstructURL(out errorMsg) + SetVariables(out errorMsg, tempStartDate, tempEndDate, station);
                    if (errorMsg.Contains("ERROR")) { return null; }
                }
                else                                   //url constructed for calls that are not the start or end
                {
                    tempStartDate = tempEndDate;
                    tempEndDate = tempStartDate.AddYears(1).AddDays(-1);
                    url = ConstructURL(out errorMsg) + SetVariables(out errorMsg, tempStartDate, tempEndDate, station);
                    if (errorMsg.Contains("ERROR")) { return null; }
                }
                string json = DownloadData(out errorMsg, url);
                if (errorMsg.Contains("ERROR")) { return null; }
                if (!json.Equals("{}"))
                {
                    NCDCJson results = JsonConvert.DeserializeObject<NCDCJson>(json);
                    double total = results.metadata.resultset.count;        //checking if available results exceed 1000 entry limit.
                    if (total > 1000)
                    {
                        for (int j = 1; j < Math.Ceiling(total / 1000); j++)
                        {
                            url = url + "&offset=" + (j) * 1000;
                            json = DownloadData(out errorMsg, url);
                            if (errorMsg.Contains("ERROR")) { return null; }
                            NCDCJson tempResults = JsonConvert.DeserializeObject<NCDCJson>(json);
                            results.results.AddRange(tempResults.results);                              //Adds the additional calls to the results.results variable
                        }
                    }

                    Dictionary<string, double> sumValues = SumDailyValues(out errorMsg, results);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    AddValuesToTS(out errorMsg, sumValues, tempStartDate, tempEndDate);
                    if (errorMsg.Contains("ERROR")) { return null; }
                }
                else
                {
                    AddValuesToTS(out errorMsg, new Dictionary<string, double>(), tempStartDate, tempEndDate);
                    if (errorMsg.Contains("ERROR")) { return null; }
                }
            }
            //Need to set metadata
            ts.timeSeriesDict = timeSeries;
            return data;
        }

        /// <summary>
        /// Constructs a url string for collecting NCDC data.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        private string ConstructURL(out string errorMsg)
        {
            errorMsg = "";
            string url = "";
            string prepInfo = System.AppDomain.CurrentDomain.BaseDirectory + @"bin\url_info.txt";  // URL configuration info.
            string[] lineData;
            try
            {
                foreach (string line in File.ReadLines(prepInfo))
                {
                    lineData = line.Split(' ');
                    if (lineData[0].Equals("NCDC_URL", StringComparison.OrdinalIgnoreCase))
                    {
                        url = lineData[1];
                        break;
                    }
                }
            }
            catch
            {
                errorMsg = "ERROR: Unable to load URL details from configuration file.";
                return null;
            }
            return url;
        }

        /// <summary>
        /// Sets the variable string for the url.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="station"></param>
        /// <returns></returns>
        private string SetVariables(out string errorMsg, DateTime startDate, DateTime endDate, string station)
        {
            errorMsg = "";
            return "datasetid=PRECIP_HLY" + "&stationid=" + station + "&units=metric" + "&startdate=" + startDate.ToString("yyyy-MM-dd") + "&enddate=" + endDate.ToString("yyyy-MM-dd") + "&limit=1000";
        }

        /// <summary>
        /// Downloads data from NCDC using the constructed url.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        private string DownloadData(out string errorMsg, string url)
        {
            errorMsg = "";
            string data = "";
            string token = "RUYNSTvfSvtosAoakBSpgxcHASBxazzP";          //required for accessing ncdc data
            WebClient wc = new WebClient();
            Thread.Sleep(333);
            try
            {
                wc.Headers.Add("token", token);
                byte[] dataBuffer = wc.DownloadData(url);
                data = Encoding.UTF8.GetString(dataBuffer);
            }
            catch
            {
                errorMsg = "ERROR: Unable to download ncdc station data.";
                return null;
            }
            wc.Dispose();
            return data;
        }

        /// <summary>
        /// Checks the attributes string for a specific value and returns the altered value based upon what attributes are present.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="value"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        private double NCDCAttributeCheck(out string errorMsg, double value, string attribute)
        {
            errorMsg = "";
            if (attribute.Contains("[") || attribute.Contains("]"))             // Begin and end of deleted period, during the given hour
            {
                return 0.0;
            }
            else if (attribute.Contains("{") || attribute.Contains("}"))        // Begin and end of missing period, during the given hour
            {
                return 0.0;
            }
            else
            {
                return value;
            }
            // place additional checks for attribute values
        }

        /// <summary>
        /// Converts a double value in Inches to MM. OBSOLETE. Data is retrieved already converted to Metric.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private double ConvertInchesToMM(double value)
        {
            return (value * 25.4);      // inches to mm eq: x[In] * (25.4[mm]/1[In]) = result[mm]
        }

        /// <summary>
        /// Sums the values for each recorded value to return a dictoinary of daily summed values.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private Dictionary<string, double> SumDailyValues(out string errorMsg, NCDCJson data)
        {
            errorMsg = "";
            Dictionary<string, double> dict = new Dictionary<string, double>();
            DateTime iDate;
            DateTime newDate;
            DateTime.TryParse(data.results[0].date, out newDate);
            double sum = 0.0;
            for (int i = 0; i < data.results.Count - 1; i++)
            {
                DateTime.TryParse(data.results[i].date, out iDate);
                if (iDate.Date == newDate.Date)
                {
                    sum += NCDCAttributeCheck(out errorMsg, data.results[i].value, data.results[i].attributes);
                    if (errorMsg.Contains("ERROR")) { return null; }
                }
                else
                {
                    dict.Add(newDate.ToString("yyyy-MM-dd"), sum);
                    newDate = iDate;
                    sum = NCDCAttributeCheck(out errorMsg, data.results[i].value, data.results[i].attributes);
                    if (errorMsg.Contains("ERROR")) { return null; }
                }
            }
            return dict;
        }

        /// <summary>
        /// Adds the values in data to the timeseries variable for the dates between startdate and endDate.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="data"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        private void AddValuesToTS(out string errorMsg, Dictionary<string, double> data, DateTime startDate, DateTime endDate)
        {
            errorMsg = "";
            int days = Convert.ToInt16((endDate - startDate).TotalDays);
            double value = 0.0;
            for (int i = 0; i < days; i++)
            {
                DateTime date = startDate.AddDays(i);
                if (data.ContainsKey(date.ToString("yyyy-MM-dd")))
                {
                    //value = ConvertInchesToMM(data[date.ToString("yyyy-MM-dd")]);             //Only required if not requesting metric values
                    value = data[date.ToString("yyyy-MM-dd")];
                }
                else
                {
                    value = 0.0;
                }
                timeSeries.Add(date, value);
            }
        }

        public Dictionary<string, string> GetStationDetails(out string errorMsg, string stationID)
        {
            errorMsg = "";
            Dictionary<string, string> stationDetails = new Dictionary<string, string>();
            string urlFile = System.AppDomain.CurrentDomain.BaseDirectory + @"bin\url_info.txt";
            string url = "";
            string[] lineData;
            try
            {
                foreach (string line in File.ReadLines(urlFile))
                {
                    lineData = line.Split(' ');
                    if (lineData[0].Equals("NCDC_STATION_URL", StringComparison.OrdinalIgnoreCase))
                    {
                        url = lineData[1];
                        break;
                    }
                }
            }
            catch
            {
                errorMsg = "ERROR: Unable to load URL details from configuration file.";
                return null;
            }
            url = url + stationID;
            string token = "RUYNSTvfSvtosAoakBSpgxcHASBxazzP";
            WebClient wc = new WebClient();
            Thread.Sleep(333);
            try
            {
                wc.Headers.Add("token", token);
                byte[] dataBuffer = wc.DownloadData(url);
                stationDetails = JsonConvert.DeserializeObject<Dictionary<string, string>>(Encoding.UTF8.GetString(dataBuffer));
            }
            catch
            {
                errorMsg = "ERROR: Unable to download ncdc station details.";
                return null;
            }
            wc.Dispose();
            return stationDetails;
        }

    }
}
