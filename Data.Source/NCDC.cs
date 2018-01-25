//using Newtonsoft.Json;
using System;
using System.Collections;
using System.Runtime.Serialization.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Runtime.Serialization;

namespace Data.Source
{

    // ---------------- NCDC return object ---------------- //

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
    [DataContract]
    public class NCDCJson
    {
        [DataMember]
        public MetaData metadata { get; set; }
        [DataMember]
        public List<Result> results { get; set; }
    }

    /// <summary>
    /// Base NCDC class.
    /// </summary>
    public class NCDC
    {
        /// <summary>
        /// GetData function for NCDC. 
        /// NCDC requires two metadata values in Geometry.GeometryMetaData: 
        /// 1. stationID - ncdc station ID
        /// 2. token - users ncdc token used for accessing ncdc services
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="dataset"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public Dictionary<string, double> GetData(out string errorMsg, string dataset, ITimeSeriesInput input)
        {
            errorMsg = "";

            // Add one day to the end of the requested time span, to include the end date. NCDC cuts off at the requested end date.
            input.DateTimeSpan.EndDate = input.DateTimeSpan.EndDate.AddDays(1);

            // Begins sequence to download ncdc data.
            Dictionary<string, double> data = BeginDataDownload(out errorMsg, dataset, input);
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
        private Dictionary<string, double> BeginDataDownload(out string errorMsg, string dataset, ITimeSeriesInput input)
        {
            errorMsg = "";

            Dictionary<string, double> data = new Dictionary<string, double>();
            DateTime tempStartDate = input.DateTimeSpan.StartDate;
            DateTime tempEndDate = input.DateTimeSpan.EndDate;

            string station = input.Geometry.GeometryMetadata["stationID"];
            string token = input.Geometry.GeometryMetadata["token"];

            //string token = (input.Geometry.GeometryMetadata.ContainsKey("token")) ? input.Geometry.GeometryMetadata["token"] : (string)HttpContext.Current.Application["ncdc_token"];
            string url = "";

            //Check if date difference is greater than one year, if true splits dates apart into multiple requests that are equal to 1y-1day.
            int requiredCalls = Convert.ToInt16(Math.Ceiling((input.DateTimeSpan.EndDate - input.DateTimeSpan.StartDate).TotalDays / 365));
            for (int i = 0; i < requiredCalls; i++)
            {
                if (i == 0 && requiredCalls == 1)       //url constructed for a single call being made
                {
                    tempStartDate = input.DateTimeSpan.StartDate;
                    tempEndDate = input.DateTimeSpan.EndDate;
                    url = ConstructURL(out errorMsg, station, input.BaseURL.First(), tempStartDate, tempEndDate);
                    if (errorMsg.Contains("ERROR")) { return null; }
                }
                else if (i == 0 && requiredCalls != 1)  //url constructed for first call of multiple
                {
                    tempStartDate = input.DateTimeSpan.StartDate;
                    tempEndDate = tempStartDate.AddYears(1).AddDays(-1);
                    url = ConstructURL(out errorMsg, station, input.BaseURL.First(), tempStartDate, tempEndDate);
                    if (errorMsg.Contains("ERROR")) { return null; }
                }
                else if (i != 0 && i == requiredCalls - 1) //url constructed for last call of multiple
                {
                    tempStartDate = tempEndDate;
                    tempEndDate = input.DateTimeSpan.EndDate;
                    url = ConstructURL(out errorMsg, station, input.BaseURL.First(), tempStartDate, tempEndDate);
                    if (errorMsg.Contains("ERROR")) { return null; }
                }
                else                                   //url constructed for calls that are not the start or end
                {
                    tempStartDate = tempEndDate;
                    tempEndDate = tempStartDate.AddYears(1).AddDays(-1);
                    url = ConstructURL(out errorMsg, station, input.BaseURL.First(), tempStartDate, tempEndDate);
                    if (errorMsg.Contains("ERROR")) { return null; }
                }
                string json = DownloadData(out errorMsg, token, url);
                if (errorMsg.Contains("ERROR")) { return null; }
                if (!json.Equals("{}"))
                {
                    //NCDCJson results = JsonConvert.DeserializeObject<NCDCJson>(json);
                    MemoryStream mStream1 = new MemoryStream(Encoding.UTF8.GetBytes(json));
                    DataContractJsonSerializer ser1 = new DataContractJsonSerializer(typeof(NCDCJson));
                    NCDCJson results = ser1.ReadObject(mStream1) as NCDCJson;
                    mStream1.Close();

                    double total = results.metadata.resultset.count;        //checking if available results exceed 1000 entry limit.
                    if (total > 1000)
                    {
                        for (int j = 1; j < Math.Ceiling(total / 1000); j++)
                        {
                            url = url + "&offset=" + (j) * 1000;
                            json = DownloadData(out errorMsg, input.Geometry.GeometryMetadata["token"], url);
                            if (errorMsg.Contains("ERROR")) { return null; }

                            //NCDCJson tempResults = JsonConvert.DeserializeObject<NCDCJson>(json);                    
                            MemoryStream mStream2 = new MemoryStream(Encoding.UTF8.GetBytes(json));
                            DataContractJsonSerializer ser2 = new DataContractJsonSerializer(typeof(NCDCJson));
                            NCDCJson tempResults = ser2.ReadObject(mStream2) as NCDCJson;
                            mStream2.Close();

                            results.results.AddRange(tempResults.results);                              //Adds the additional calls to the results.results variable
                        }
                    }
                    Dictionary<string, double> sumValues = AggregateData(out errorMsg, input, results, tempStartDate, tempEndDate);
                    AppendData(ref data, sumValues, input.DateTimeSpan.DateTimeFormat, tempStartDate, tempEndDate, input.TemporalResolution);
                    if (errorMsg.Contains("ERROR")) { return null; }
                }
                else
                {
                    AppendData(ref data, new Dictionary<string, double>(), input.DateTimeSpan.DateTimeFormat, tempStartDate, tempEndDate, input.TemporalResolution);
                    if (errorMsg.Contains("ERROR")) { return null; }
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
        private string ConstructURL(out string errorMsg, string station, string baseURL, DateTime startDate, DateTime endDate)
        {
            errorMsg = "";
            StringBuilder sb = new StringBuilder();
            sb.Append(baseURL);
            if (station.Contains("GHCND"))
            {
                sb.Append("datasetid=GHCND&datatypeid=PRCP" + "&stationid=" + station + "&units=metric" + "&startdate=" + startDate.ToString("yyyy-MM-dd") + "&enddate=" + endDate.ToString("yyyy-MM-dd") + "&limit=1000");
            }
            else
            {
                sb.Append("datasetid=PRECIP_HLY" + "&stationid=" + station + "&units=metric" + "&startdate=" + startDate.ToString("yyyy-MM-dd") + "&enddate=" + endDate.ToString("yyyy-MM-dd") + "&limit=1000");
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
        private string DownloadData(out string errorMsg, string token, string url)
        {
            errorMsg = "";
            try
            {              
                int retries = 5;                                        // Max number of request retries
                string status = "";                                     // response status code
                string data = "";

                while (retries > 0 && !status.Contains("OK"))
                {
                    Thread.Sleep(333);
                    WebRequest wr = WebRequest.Create(url);
                    wr.Headers.Add("token", token);
                    HttpWebResponse response = (HttpWebResponse)wr.GetResponse();
                    status = response.StatusCode.ToString();
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    data = reader.ReadToEnd();
                    reader.Close();
                    response.Close();
                    retries -= 1;
                }
                return data;
            }
            catch (Exception ex)
            {
                errorMsg = "ERROR: Unable to download ncdc station data. " + ex.Message;
                return null;
            }
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

            // TODO: include a count of thrown out data entries in meta data.
            // TODO: add attribute column to data output for data entries handling

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
        /// Calls the appropriate aggregation function.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="inputData"></param>
        /// <param name="results"></param>
        /// <param name="tempStartDate"></param>
        /// <param name="tempEndDate"></param>
        /// <returns></returns>
        private Dictionary<string, double> AggregateData(out string errorMsg, ITimeSeriesInput inputData, NCDCJson results, DateTime tempStartDate, DateTime tempEndDate)
        {
            Dictionary<string, double> sumValues = new Dictionary<string, double>();
            switch (inputData.TemporalResolution)
            {
                case "hourly":
                    sumValues = SumHourlyValues(out errorMsg, inputData.DateTimeSpan.DateTimeFormat, results);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                case "daily":
                case "default":
                default:
                    sumValues = SumDailyValues(out errorMsg, inputData.DateTimeSpan.DateTimeFormat, results);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                case "weekly":
                    sumValues = SumDailyValues(out errorMsg, inputData.DateTimeSpan.DateTimeFormat, results);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    // Weekly aggregation of ncdc data requires daily summed values.
                    sumValues = SumWeeklyValues(out errorMsg, inputData.DateTimeSpan.DateTimeFormat, sumValues);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                case "monthly":
                    sumValues = SumDailyValues(out errorMsg, inputData.DateTimeSpan.DateTimeFormat, results);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    // Weekly aggregation of ncdc data requires daily summed values.
                    sumValues = SumWeeklyValues(out errorMsg, inputData.DateTimeSpan.DateTimeFormat, sumValues);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
            }
            return sumValues;
        }

        /// <summary>
        /// Sums the values for each recorded value to return a dictionary of monthly summed values.
        /// Requires summing of daily values first.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private Dictionary<string, double> SumMonthlyValues(out string errorMsg, string dateFormat, Dictionary<string, double> dailyData)
        {
            errorMsg = "";
            Dictionary<string, double> dict = new Dictionary<string, double>();
            DateTime iDate;
            DateTime newDate;
            DateTime.TryParse(dailyData.Keys.ElementAt(0), out newDate);
            double sum = 0.0;
            for (int i = 0; i < dailyData.Count - 1; i++)
            {
                DateTime.TryParse(dailyData.Keys.ElementAt(i), out iDate);
                if (iDate.Month == newDate.Month)
                {
                    sum += dailyData[dailyData.Keys.ElementAt(i)];
                    if (errorMsg.Contains("ERROR")) { return null; }
                }
                else
                {
                    dict.Add(newDate.ToString(dateFormat), sum);
                    newDate = iDate;
                    sum = dailyData[dailyData.Keys.ElementAt(i)];
                    if (errorMsg.Contains("ERROR")) { return null; }
                }
            }
            return dict;
        }

        /// <summary>
        /// Sums the values for each recorded value to return a dictionary of weekly summed values.
        /// Requires summing of daily values first.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private Dictionary<string, double> SumWeeklyValues(out string errorMsg, string dateFormat, Dictionary<string, double> dailyData)
        {
            errorMsg = "";
            Dictionary<string, double> dict = new Dictionary<string, double>();
            DateTime iDate;
            DateTime newDate;
            DateTime.TryParse(dailyData.Keys.ElementAt(0), out newDate);
            double sum = 0.0;
            int week = 0;
            for (int i = 0; i < dailyData.Count - 1; i++)
            {
                DateTime.TryParse(dailyData.Keys.ElementAt(i), out iDate);
                if (week == 7)
                {
                    week++;
                    sum += dailyData[dailyData.Keys.ElementAt(i)];
                    if (errorMsg.Contains("ERROR")) { return null; }
                }
                else
                {
                    week = 0;
                    dict.Add(newDate.ToString(dateFormat), sum);
                    newDate = iDate;
                    sum = dailyData[dailyData.Keys.ElementAt(i)];
                    if (errorMsg.Contains("ERROR")) { return null; }
                }
            }
            return dict;
        }

        /// <summary>
        /// Sums the values for each recorded value to return a dictionary of daily summed values.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private Dictionary<string, double> SumDailyValues(out string errorMsg, string dateFormat, NCDCJson data)
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
                    dict.Add(newDate.ToString(dateFormat), sum);
                    newDate = iDate;
                    sum = NCDCAttributeCheck(out errorMsg, data.results[i].value, data.results[i].attributes);
                    if (errorMsg.Contains("ERROR")) { return null; }
                }
            }
            return dict;
        }

        /// <summary>
        /// Sums the values for each recorded value to return a dictionary of hourly summed values.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private Dictionary<string, double> SumHourlyValues(out string errorMsg, string dateFormat, NCDCJson data)
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
                if (iDate.Hour == newDate.Hour)
                {
                    sum += NCDCAttributeCheck(out errorMsg, data.results[i].value, data.results[i].attributes);
                    if (errorMsg.Contains("ERROR")) { return null; }
                }
                else
                {
                    dict.Add(newDate.ToString(dateFormat), sum);
                    newDate = iDate;
                    sum = NCDCAttributeCheck(out errorMsg, data.results[i].value, data.results[i].attributes);
                    if (errorMsg.Contains("ERROR")) { return null; }
                }
            }
            return dict;
        }

        /// <summary>
        /// Adds the key/value pairs from the second dictionary into the first if the key/value does not exist.
        /// </summary>
        /// <param name="firstDict"></param>
        /// <param name="secondDict"></param>
        private static void AppendData(ref Dictionary<string, double> firstDict, Dictionary<string, double> secondDict, string dateFormat, DateTime startDate, DateTime endDate, string tRes)
        {

            switch (tRes)
            {
                case "hourly":
                    int hours = Convert.ToInt16((endDate - startDate).TotalHours);
                    for (int i = 0; i < hours; i++)
                    {
                        DateTime date = startDate.AddHours(i);
                        if (secondDict.ContainsKey(date.ToString(dateFormat)))
                        {
                            firstDict.Add(date.ToString(dateFormat), secondDict[date.ToString(dateFormat)]);
                        }
                        else
                        {
                            firstDict.Add(date.ToString(dateFormat), 0.0);
                        }
                    }
                    break;
                case "daily":
                case "default":
                default:
                    int days = Convert.ToInt16((endDate - startDate).TotalDays);
                    for (int i = 0; i < days; i++)
                    {
                        DateTime date = startDate.AddDays(i);
                        if (secondDict.ContainsKey(date.ToString(dateFormat)))
                        {
                            firstDict.Add(date.ToString(dateFormat), secondDict[date.ToString(dateFormat)]);
                        }
                        else
                        {
                            firstDict.Add(date.ToString(dateFormat), 0.0);
                        }
                    }
                    break;
                case "weekly":
                    int weeks = Convert.ToInt16(((endDate - startDate).TotalDays)/7);
                    for (int i = 0; i < weeks; i++)
                    {
                        DateTime date = startDate.AddDays(i * 7);
                        if (secondDict.ContainsKey(date.ToString(dateFormat)))
                        {
                            firstDict.Add(date.ToString(dateFormat), secondDict[date.ToString(dateFormat)]);
                        }
                        else
                        {
                            firstDict.Add(date.ToString(dateFormat), 0.0);
                        }
                    }
                    break;
                case "monthly":
                    int years = Convert.ToInt16(endDate.Year - endDate.Year);
                    int months = Convert.ToInt16(endDate.Month - startDate.Month);
                    int totalMonths = years * 12 + months;
                    for (int i = 0; i < totalMonths; i++)
                    {
                        DateTime date = startDate.AddDays(i * 7);
                        if (secondDict.ContainsKey(date.ToString(dateFormat)))
                        {
                            firstDict.Add(date.ToString(dateFormat), secondDict[date.ToString(dateFormat)]);
                        }
                        else
                        {
                            firstDict.Add(date.ToString(dateFormat), 0.0);
                        }
                    }
                    break;
            }             
        }

        /// <summary>
        /// Gets NCDC station details.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="stationID"></param>
        /// <param name="token">Token required for accessing ncdc services.</param>
        /// <returns></returns>
        public Dictionary<string, string> GetStationDetails(out string errorMsg, string url, string stationID, string token)
        {
            errorMsg = "";
            Dictionary<string, string> stationDetails = new Dictionary<string, string>();
            url = url + stationID;
            if (String.IsNullOrWhiteSpace(token))
            {
                errorMsg = "ERROR: No token provided for retrieving ncdc station details.";
                return new Dictionary<string, string>();
            }
            try
            {
                int retries = 5;                                        // Max number of request retries
                string status = "";                                     // response status code

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
                    //stationDetails = JsonConvert.DeserializeObject<Dictionary<string, string>>(dataBuffer);

                    MemoryStream mStream = new MemoryStream(Encoding.UTF8.GetBytes(dataBuffer));
                    DataContractJsonSerializer ser2 = new DataContractJsonSerializer(typeof(Dictionary<string, string>));
                    stationDetails = ser2.ReadObject(mStream) as Dictionary<string, string>;
                    mStream.Close();

                    reader.Close();
                    response.Close();
                    retries -= 1;
                }
            }
            catch (Exception ex)
            {
                errorMsg = "ERROR: Unable to download ncdc station details. " + ex.Message;
                return new Dictionary<string, string>();
            }
            return stationDetails;
        }

        /// <summary>
        /// Directly downloads from the source using the testInput object. Used for checking the status of the NCDC endpoints.
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="testInput"></param>
        /// <returns></returns>
        public static Dictionary<string, string> CheckStatus(string dataset, ITimeSeriesInput testInput)
        {
            string station_url = "https://www.ncdc.noaa.gov/cdo-web/api/v2/stations/";
            try
            {
                WebRequest wr = WebRequest.Create(station_url);
                wr.Headers.Add("token", testInput.Geometry.GeometryMetadata["token"]);
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
