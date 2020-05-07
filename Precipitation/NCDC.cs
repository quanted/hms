using Data;
using Data.Source;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Precipitation
{

    public class NCEIPrecipitation
    {
        public string DATE { get; set; }
        public string STATION { get; set; }
        public double PRCP { get; set; }
        public string PRCP_ATTRIBUTES { get; set; }
    }

    /// <summary>
    /// Base precipitation ncdc class.
    /// </summary>
    public class NCDC
    {
        /// <summary>
        /// Makes the GetData call to the base NCDC class.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public ITimeSeriesOutput GetData(out string errorMsg, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";

            Data.Source.NCDC ncdc = new Data.Source.NCDC();
            ITimeSeriesOutput ncdcOutput = output;

            // Make call to get station metadata and add to output.Metadata
            if (!input.Geometry.GeometryMetadata.ContainsKey("token"))
            {
                input.Geometry.GeometryMetadata["token"] = "RUYNSTvfSvtosAoakBSpgxcHASBxazzP";
            }
            //    errorMsg = "ERROR: No NCEI token provided. Please provide a valid NCEI token.";
            //    return null;
            //}
            string station_url = "https://www.ncdc.noaa.gov/cdo-web/api/v2/stations/";
            if (input.Geometry.GeometryMetadata.ContainsKey("stationID"))
            {
                input.Geometry.StationID = input.Geometry.GeometryMetadata["stationID"];
            }
            else if(input.Geometry.StationID != null && !input.Geometry.GeometryMetadata.ContainsKey("stationID"))
            {
                input.Geometry.GeometryMetadata.Add("stationID", input.Geometry.StationID);
            }
            if (!input.Geometry.GeometryMetadata.ContainsKey("stationID"))
            {
                errorMsg = "ERROR: No NCEI stationID provided. Please provide a valid NCEI stationID.";
                return null;
            }
            ncdcOutput.Metadata = SetMetadata(out errorMsg, "ncei", NCEI<NCEIPrecipitation>.GetStationDetails(out errorMsg, station_url, input.Geometry.GeometryMetadata["stationID"], input.Geometry.GeometryMetadata["token"]));
            ncdcOutput.Metadata.Add("ncei", input.TemporalResolution);
            ncdcOutput.Metadata.Add("ncei_units", "mm");

            // Data aggregation takes place within ncdc.GetData

            Dictionary<string, double> data = new Dictionary<string, double>();

            if (input.Geometry.StationID.Contains("GHCND"))
            {
                NCEI<NCEIPrecipitation> ncei = new NCEI<NCEIPrecipitation>();
                List<NCEIPrecipitation> preData = ncei.GetData(out errorMsg, "PRCP", input);
                data = this.ParseData(out errorMsg, preData, input.DateTimeSpan.DateTimeFormat, input.TemporalResolution, input.DateTimeSpan.StartDate, input.DateTimeSpan.EndDate);
            }
            else
            {
                data = ncdc.GetData(out errorMsg, "PRCP", input);
            }
            if (errorMsg.Contains("ERROR")) { return null; }

            // Set resulting data to output.Data
            ncdcOutput.Data = ConvertDict(out errorMsg, input.DataValueFormat, data);
            if (errorMsg.Contains("ERROR")) { return null; }

            ncdcOutput.DataSource = "ncei";
            ncdcOutput.Dataset = "Precipitation";
            ncdcOutput.Metadata.Add("column_1", "Date");
            ncdcOutput.Metadata.Add("column_2", "NCEI Total");
            return ncdcOutput;
        }

        /// <summary>
        /// Converts the returned Dictionary into the appropriate format for ITimeSeriesOutput.Data
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="dataFormat"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private Dictionary<string, List<string>> ConvertDict(out string errorMsg, string dataFormat, Dictionary<string, double> data)
        {
            errorMsg = "";
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            foreach (var key in data)
            {
                result.Add(key.Key.ToString(), new List<string>() { data[key.Key.ToString()].ToString(dataFormat) });
            }
            return result;
        }

        /// <summary>
        /// Adds source_ to the metadata keys.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="source"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        private Dictionary<string, string> SetMetadata(out string errorMsg, string source, Dictionary<string, string> metadata)
        {
            errorMsg = "";
            Dictionary<string, string> newMeta = new Dictionary<string, string>();
            foreach (var ele in metadata)
            {
                newMeta.Add(source + "_" + ele.Key, ele.Value);
            }
            return newMeta;
        }

        /// <summary>
        /// Calls the function in Data.Source.NCDC that will perform the status check.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Dictionary<string, string> CheckStatus(ITimeSeriesInput input)
        {
            return Data.Source.NCDC.CheckStatus("Precipitation", input);
        }

        /// <summary>
        /// Parse the resulting NCEIPrecipitation list into a dictionary of timestamps and double 
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="rawdata"></param>
        /// <param name="dateFormat"></param>
        /// <returns></returns>
        protected Dictionary<string, double> ParseData(out string errorMsg, List<NCEIPrecipitation> rawdata, string dateFormat, string aggregation, DateTime tempStartDate, DateTime tempEndDate)
        {
            errorMsg = "";
            Dictionary<string, double> data = new Dictionary<string, double>();
            DateTime iDate;
            DateTime newDate;
            DateTime.TryParse(rawdata[0].DATE, out newDate);
            double sum = 0.0;
            for (int i = 0; i <= rawdata.Count - 1; i++)
            {
                DateTime.TryParse(rawdata[i].DATE, out iDate);
                if (iDate.Date == newDate.Date)
                {
                    if (sum < 0)
                    {
                        sum = 0;
                    }
                    double addition = NCEI<NCEIPrecipitation>.AttributeCheck(out errorMsg, rawdata[i].PRCP, rawdata[i].PRCP_ATTRIBUTES);
                    if (addition < 0)
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
                    data.Add(newDate.ToString(dateFormat), sum);
                    newDate = iDate;
                    sum = NCEI<NCEIPrecipitation>.AttributeCheck(out errorMsg, rawdata[i].PRCP, rawdata[i].PRCP_ATTRIBUTES);
                    if (i == rawdata.Count - 1)
                    {
                        iDate = iDate.AddHours(-iDate.Hour);
                        data.Add(iDate.ToString(dateFormat), sum);
                    }
                    if (errorMsg.Contains("ERROR")) { return null; }
                }
            }
            data = this.AggregateData(out errorMsg, aggregation, data, tempStartDate, tempEndDate, dateFormat);
            return data;
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
        private Dictionary<string, double> AggregateData(out string errorMsg, string aggregation, Dictionary<string, double> data, DateTime tempStartDate, DateTime tempEndDate, string dateFormat)
        {
            errorMsg = "";
            Dictionary<string, double> aggregatedValues = new Dictionary<string, double>();
            switch (aggregation)
            {               
                case "weekly":
                    // Weekly aggregation of ncdc data requires daily summed values.
                    aggregatedValues = SumWeeklyValues(out errorMsg, dateFormat, data);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                case "monthly":
                    // Monthly aggregation of ncdc data requires daily summed values.
                    aggregatedValues = SumMonthlyValues(out errorMsg, dateFormat, data);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                case "yearly":
                case "annual":
                    // Yearly aggregation of ncdc data requires daily summed values.
                    aggregatedValues = SumYearlyValues(out errorMsg, dateFormat, data);
                    if (errorMsg.Contains("ERROR")) { return null; }
                    break;
                default:
                case "daily":
                    aggregatedValues = data;
                    break;
            }
            return aggregatedValues;
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
                if (iDate.Month != newDate.Month || i == dailyData.Count - 1)
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
                if (dayDif >= 7)
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

                    sum += addition;
                    if (errorMsg.Contains("ERROR")) { return null; }
                }
            }
            return dict;
        }


    }
}
