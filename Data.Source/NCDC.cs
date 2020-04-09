using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Runtime.Serialization;
using System.Text.Json;
using Serilog;
using System.Threading.Tasks;
using System.Net.Http;

namespace Data.Source
{

    // ---------------- NCEI return object ---------------- //

    /// <summary>
    /// Metadata of json string retrieved from ncei.
    /// </summary>
    public class MetaData
    {
        public Resultset resultset { get; set; }
    }

    /// <summary>
    /// Result structure of the json string retrieved from ncei.
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
    /// Complete CSV object returned from NCEI.
    /// </summary>
    [DataContract]
    public class NCDCCSV
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
        public Dictionary<string, double> GetData(out string errorMsg, string dataTypeID, ITimeSeriesInput input)
        {
            errorMsg = "";

            // Add one day to the end of the requested time span, to include the end date. NCDC cuts off at the requested end date.
            //input.DateTimeSpan.EndDate = input.DateTimeSpan.EndDate.AddDays(1);

            // Begins sequence to download ncdc data.
            Dictionary<string, double> data = BeginDataDownload(out errorMsg, dataTypeID, input);
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
        private Dictionary<string, double> BeginDataDownload(out string errorMsg, string dataTypeID, ITimeSeriesInput input)
        {
            errorMsg = "";

            Dictionary<string, double> data = new Dictionary<string, double>();
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

            //string token = (input.Geometry.GeometryMetadata.ContainsKey("token")) ? input.Geometry.GeometryMetadata["token"] : (string)HttpContext.Current.Application["ncdc_token"];
            string url = ConstructURL(out errorMsg, dataTypeID, stationID, input.BaseURL.First(), tempStartDate, tempEndDate);
            if (errorMsg.Contains("ERROR")) { return null; }

            string csv = DownloadData(token, url, 0).Result;
            if (errorMsg.Contains("ERROR")) { return null; }
            if (!csv.Equals("{}"))
            {

                NCDCCSV results;
                if (stationID.Contains("COOP"))
                {
                    results = ParseJson(out errorMsg, csv, tempStartDate, tempEndDate);
                }
                else
                {
                    results = ReadData(out errorMsg, csv);
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
                sb.Append("dataset=daily-summaries" + "&dataTypes=" + dataTypeID + "&stations=" + station + "&startDate=" + startDate.ToString("yyyy-MM-dd") + "&endDate=" + endDate.ToString("yyyy-MM-dd") + "&format=csv" + "&includeAttributes=true" + "&units=metric");
            }
            else if(station.Contains("COOP"))
            {
                sb.Append("https://www.ncdc.noaa.gov/cdo-web/api/v2/data?");
                sb.Append("datasetid=PRECIP_HLY" + "&stationid=" + station + "&units=metric" + "&startdate=" + startDate.ToString("yyyy-MM-dd") + "&enddate=" + endDate.ToString("yyyy-MM-dd") + "&limit=1000");
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
        private async Task<string> DownloadData(string token, string url, int retries)
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
                    return this.DownloadData(token, url, retries).Result;
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

            //Explanation of quality flags: https://www1.ncdc.noaa.gov/pub/data/cdo/documentation/GHCND_documentation.pdf
            char[] qualityFlags = new char[] { 'D', 'G', 'I', 'K', 'L', 'M', 'N', 'O', 'R', 'S', 'T', 'W', 'X', 'Z' };
            string[] tables = attribute.Split(',');

            errorMsg = "";
            if (attribute.Contains("[") || attribute.Contains("]"))             // Begin and end of deleted period, during the given hour
            {
                return -9999;
            }
            else if (attribute.Contains("{") || attribute.Contains("}"))        // Begin and end of missing period, during the given hour
            {
                return -9999;
            }
            else if (tables[0].Contains("M") || tables[1].IndexOfAny(qualityFlags) != -1)        // One period of missing data or failed QA check
            {
                return -9999;
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
        private Dictionary<string, double> AggregateData(out string errorMsg, ITimeSeriesInput inputData, NCDCCSV results, DateTime tempStartDate, DateTime tempEndDate)
        {
            Dictionary<string, double> sumValues = new Dictionary<string, double>();
            switch (inputData.TemporalResolution)
            {
                case "hourly":
                    sumValues = SumHourlyValues(out errorMsg, inputData.DateTimeSpan.DateTimeFormat, results);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                case "extreme_5":
                case "daily":
                case "default":
                default:
                    sumValues = SumDailyValues(out errorMsg, inputData.DateTimeSpan.DateTimeFormat, results);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                case "weekly":
                    sumValues = SumDailyValues(out errorMsg, inputData.DateTimeSpan.DateTimeFormat, results);
                    Dictionary<string, double> tempValues = new Dictionary<string, double>();
                    AppendData(ref tempValues, sumValues, inputData.DateTimeSpan.DateTimeFormat, tempStartDate, tempEndDate, "daily");////
                    if (errorMsg.Contains("ERROR")) { return null; }
                    // Weekly aggregation of ncdc data requires daily summed values.
                    sumValues = SumWeeklyValues(out errorMsg, inputData.DateTimeSpan.DateTimeFormat, tempValues);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                case "monthly":
                    sumValues = SumDailyValues(out errorMsg, inputData.DateTimeSpan.DateTimeFormat, results);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    // Monthly aggregation of ncdc data requires daily summed values.
                    sumValues = SumMonthlyValues(out errorMsg, inputData.DateTimeSpan.DateTimeFormat, sumValues);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                case "annual":
                    sumValues = SumDailyValues(out errorMsg, inputData.DateTimeSpan.DateTimeFormat, results);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    // Yearly aggregation of ncdc data requires daily summed values.
                    sumValues = SumYearlyValues(out errorMsg, inputData.DateTimeSpan.DateTimeFormat, sumValues);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
            }
            return sumValues;
        }

        /// <summary>
        /// Sums the values for each recorded value to return a dictionary of yearly summed values.
        /// Requires summing of daily values first.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private Dictionary<string, double> SumYearlyValues(out string errorMsg, string dateFormat, Dictionary<string, double> dailyData)
        {
            errorMsg = "";
            Dictionary<string, double> dict = new Dictionary<string, double>();
            DateTime iDate;
            DateTime newDate;
            string dateString0 = dailyData.Keys.ElementAt(0).ToString().Substring(0, dailyData.Keys.ElementAt(0).ToString().Length - 1) + ":00:00";
            DateTime.TryParse(dateString0, out newDate);
            double sum = 0.0;
            for (int i = 0; i < dailyData.Count; i++)
            {
                string dateString = dailyData.Keys.ElementAt(i).ToString().Substring(0, dailyData.Keys.ElementAt(0).ToString().Length - 1) + ":00:00";
                DateTime.TryParse(dateString, out iDate);
                if (iDate.Year != newDate.Year || i == dailyData.Count - 1)
                {
                    dict.Add(newDate.ToString(dateFormat), sum);
                    newDate = iDate;
                    sum = dailyData[dailyData.Keys.ElementAt(i)];
                    if (errorMsg.Contains("ERROR")) { return null; }
                }
                else
                {
                    sum += dailyData[dailyData.Keys.ElementAt(i)];
                    if (errorMsg.Contains("ERROR")) { return null; }
                }
            }
            return dict;
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
            string dateString0 = dailyData.Keys.ElementAt(0).ToString().Substring(0, dailyData.Keys.ElementAt(0).ToString().Length - 1) + ":00:00";
            DateTime.TryParse(dateString0, out newDate);
            double sum = 0.0;
            for (int i = 0; i < dailyData.Count; i++)
            {
                string dateString = dailyData.Keys.ElementAt(i).ToString().Substring(0, dailyData.Keys.ElementAt(0).ToString().Length - 1) + ":00:00";
                DateTime.TryParse(dateString, out iDate);
                if (iDate.Month != newDate.Month || i == dailyData.Count-1)
                {
                    dict.Add(newDate.ToString(dateFormat), sum);
                    newDate = iDate;
                    sum = dailyData[dailyData.Keys.ElementAt(i)];
                    if (errorMsg.Contains("ERROR")) { return null; }
                }
                else
                {
                    sum += dailyData[dailyData.Keys.ElementAt(i)];
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
            DateTime.TryParseExact(dailyData.Keys.ElementAt(0), new string[] { "yyyy-MM-dd HH" }, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out newDate);
            double sum = 0.0;
            int week = 0;
            for (int i = 0; i < dailyData.Count; i++)
            {
                DateTime.TryParseExact(dailyData.Keys.ElementAt(i), new string[] { "yyyy-MM-dd HH" }, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out iDate);
                int dayDif = (int)(iDate - newDate).TotalDays;
                if(dayDif >= 7)
                {
                    dict.Add(newDate.ToString(dateFormat), sum);
                    newDate = iDate;
                    sum = dailyData[dailyData.Keys.ElementAt(i)];
                    if (errorMsg.Contains("ERROR")) { return null; }
                }
                else
                {
                    week++;
                    double addition = dailyData[dailyData.Keys.ElementAt(i)];
                    if (sum < 0)
                    {
                        sum = 0;
                    }
                    if (addition < 0)
                    {
                        sum = addition;
                    }
                    else
                    {
                        sum += addition;
                    }

                    sum += addition;//dailyData[dailyData.Keys.ElementAt(i)];
                    if (errorMsg.Contains("ERROR")) { return null; }
                }
                /*
                if (week < 7)
                {
                    week++;
                    sum += dailyData[dailyData.Keys.ElementAt(i)];
                    if (errorMsg.Contains("ERROR")) { return null; }
                }
                else
                {
                    week = 1;
                    dict.Add(newDate.ToString(dateFormat), sum);
                    newDate = iDate;
                    sum = dailyData[dailyData.Keys.ElementAt(i)];
                    if (errorMsg.Contains("ERROR")) { return null; }
                }
                if (i == dailyData.Count - 1)
                {
                    dict.Add(newDate.ToString(dateFormat), sum);
                    newDate = iDate;
                    sum = dailyData[dailyData.Keys.ElementAt(i)];
                    if (errorMsg.Contains("ERROR")) { return null; }
                }*/
            }
            return dict;
        }

        /// <summary>
        /// Sums the values for each recorded value to return a dictionary of daily summed values.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private Dictionary<string, double> SumDailyValues(out string errorMsg, string dateFormat, NCDCCSV data)
        {
            errorMsg = "";
            Dictionary<string, double> dict = new Dictionary<string, double>();
            DateTime iDate;
            DateTime newDate;
            DateTime.TryParse(data.results[0].date, out newDate);
            double sum = 0.0;
            for (int i = 0; i <= data.results.Count - 1; i++)
            {
                DateTime.TryParse(data.results[i].date, out iDate);
                if (iDate.Date == newDate.Date)
                {
                    if (sum < 0)
                    {
                        sum = 0;
                    }
                    double addition = NCDCAttributeCheck(out errorMsg, data.results[i].value, data.results[i].attributes);
                    if(addition < 0)
                    {
                        sum = addition;
                    }
                    else
                    {
                        sum += addition;
                    }
                    if (errorMsg.Contains("ERROR")) { return null; }
                }
                else
                {
                    newDate = newDate.AddHours(-newDate.Hour);
                    dict.Add(newDate.ToString(dateFormat), sum);
                    newDate = iDate;
                    sum = NCDCAttributeCheck(out errorMsg, data.results[i].value, data.results[i].attributes);
                    if (i == data.results.Count - 1)
                    {
                        iDate = iDate.AddHours(-iDate.Hour);
                        dict.Add(iDate.ToString(dateFormat), sum);
                    }
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
        private Dictionary<string, double> SumHourlyValues(out string errorMsg, string dateFormat, NCDCCSV data)
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
                            firstDict.Add(date.ToString(dateFormat), -9999);
                        }
                    }
                    break;
                case "extreme_5":
                case "daily":
                case "default":
                default:
                    int days = Convert.ToInt16((endDate - startDate).TotalDays);
                    for (int i = 0; i <= days; i++)
                    {
                        DateTime date = startDate.AddDays(i);
                        if (firstDict.ContainsKey(date.ToString(dateFormat)))
                        {
                            continue;
                        }
                        else if (secondDict.ContainsKey(date.ToString(dateFormat)))
                        {
                            firstDict.Add(date.ToString(dateFormat), secondDict[date.ToString(dateFormat)]);
                        }
                        else
                        {
                            firstDict.Add(date.ToString(dateFormat), -9999);
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
                            firstDict.Add(date.ToString(dateFormat), -9999);
                        }
                    }
                    break;
                case "monthly":
                    int years = Convert.ToInt16(endDate.Year - startDate.Year);
                    int months = Convert.ToInt16(endDate.Month - startDate.Month);
                    int totalMonths = years * 12 + months;
                    //if (totalMonths >= 12) { totalMonths -= 1; } //Remove extra month caused by adding extra day
                    if (totalMonths == 0) { break; }
                    for (int i = 0; i <= totalMonths; i++)
                    {
                        DateTime date = startDate.AddMonths(i);//.AddDays(i * 7);
                        if (secondDict.ContainsKey(date.ToString(dateFormat)))
                        {
                            firstDict.Add(date.ToString(dateFormat), secondDict[date.ToString(dateFormat)]);
                        }
                        else
                        {
                            firstDict.Add(date.ToString(dateFormat), -9999);
                        }
                    }
                    break;
                case "annual":
                    int yrs = Convert.ToInt16(endDate.Year - startDate.Year);
                    //if (yrs >= 1) { yrs -= 1; } //Remove extra year caused by adding extra day
                    if (Convert.ToInt16((endDate - startDate).TotalDays) < 364) { break; } //Check for full years
                    for (int i = 0; i <= yrs; i++)
                    {
                        DateTime date = startDate.AddYears(i);
                        if (secondDict.ContainsKey(date.ToString(dateFormat)))
                        {
                            firstDict.Add(date.ToString(dateFormat), secondDict[date.ToString(dateFormat)]);
                        }
                        else
                        {
                            firstDict.Add(date.ToString(dateFormat), -9999);
                        }
                    }
                    break;
            }             
        }

        /// <summary>
        /// Gets NCEI station details.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="stationID"></param>
        /// <param name="token">Token required for accessing NCEI services.</param>
        /// <returns></returns>
        public Dictionary<string, string> GetStationDetails(out string errorMsg, string url, string stationID, string token)
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
                foreach(KeyValuePair<string, object> kv in details)
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

        public NCDCCSV ReadData(out string errorMsg, string data)
        {
            NCDCCSV output = new NCDCCSV();
            output.results = new List<Result>();
            errorMsg = "";
            string[] tsLines = data.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 1; i < tsLines.Length; i++)
            {
                string[] lineData = tsLines[i].Split(new string[] { "\",\"", "\"" }, StringSplitOptions.RemoveEmptyEntries);
                if(lineData.Length != 4)
                {
                    //Some data skipped over, so mark as missing with null attributes
                    lineData = new string[]{ lineData[0], lineData[1], "-9999", ",,,"};
                }
                Result result = new Result();
                result.station = lineData[0];
                result.date = lineData[1];
                result.value = Convert.ToDouble(lineData[2]);
                result.attributes = lineData[3];
                output.results.Add(result);

            }
            return output;
        }

        public NCDCCSV ParseJson(out string errorMsg, string data, DateTime startDate, DateTime endDate)
        {
            errorMsg = "";
            NCDCCSV results;
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    IgnoreNullValues = true
                };
                results = JsonSerializer.Deserialize<NCDCCSV>(data, options);
            }
            catch(Exception ex)
            {
                errorMsg = ex.Message;
                return null;
            }
            string station = (results.results.Count > 0) ? results.results[0].station : "Unknown";
            string dtype = (results.results.Count > 0) ? results.results[0].datatype : "Unknown";
            List<Result> correctedTS = new List<Result>();
            DateTime currentTimeStep = new DateTime(startDate.Year, startDate.Month, startDate.Day, startDate.Hour, 0, 0);
            while (currentTimeStep <= endDate)
            {
                string dt = currentTimeStep.ToString("yyyy-MM-ddTHH:mm:ss");
                if (results.results.Count > 0) {
                    if (results.results[0].date == dt)
                    {
                        Result r = results.results[0];
                        results.results.RemoveAt(0);
                        correctedTS.Add(r);
                    }
                    else
                    {
                        Result r = new Result()
                        {
                            date = dt,
                            attributes = ",",
                            station = station,
                            datatype = dtype,
                            value = 0.0
                        };
                        correctedTS.Add(r);
                    }
                }
                else
                {
                    Result r = new Result()
                    {
                        date = dt,
                        attributes = ",",
                        station = station,
                        datatype = dtype,
                        value = 0.0
                    };
                    correctedTS.Add(r);
                }
                currentTimeStep = currentTimeStep.AddHours(1);
            }
            results.results = correctedTS;
            return results;           
        }
    }
}