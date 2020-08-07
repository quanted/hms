using Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Precipitation
{
    /// <summary>
    /// Precipitation Daymet class.
    /// </summary>
    public class Daymet
    {
        /// <summary>
        /// Makes the GetData call to the base Daymet class.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public ITimeSeriesOutput GetData(out string errorMsg, ITimeSeriesOutput output, ITimeSeriesInput input, int retries = 0)
        {
            errorMsg = "";
            bool validInputs = ValidateInputs(input, out errorMsg);
            if (!validInputs) { return null; }

            Data.Source.Daymet daymet = new Data.Source.Daymet();
            string data = daymet.GetData(out errorMsg, "Precip", input, retries);
            if (errorMsg.Contains("ERROR")) { return null; }

            ITimeSeriesOutput daymetOutput = output;
            daymetOutput = SetDataToOutput(out errorMsg, "Precipitation", data, output, input);
            daymetOutput.Metadata["daymet_unit"] = (input.Units.Contains("imperial")) ? "in" : "mm";

            daymetOutput.Dataset = "Precipitation";
            daymetOutput.DataSource = "daymet";

            // Temporal aggregation
            daymetOutput = TemporalAggregation(out errorMsg, output, input);
            if (errorMsg.Contains("ERROR")) { return null; }

            return daymetOutput;
        }

        /// <summary>
        /// Checks temporal resolution and runs appropriate aggregation function.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public ITimeSeriesOutput TemporalAggregation(out string errorMsg, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";
            output.Metadata.Add("daymet_temporalresolution", input.TemporalResolution);
            output.Metadata.Add("column_1", "Date");
                        
            switch (input.TemporalResolution)
            {
                case "monthly":
                    output.Data = NLDAS.MonthlyAggregatedSum(out errorMsg, 1.0, output, input);
                    output.Metadata.Add("column_2", "Monthly Total");
                    return output;
                case "yearly":
                    output.Data = NLDAS.YearlyAggregatedSum(out errorMsg, 0, 1.0, output, input);
                    output.Metadata.Add("column_2", "Yearly Total");
                    return output;
                case "daily":
                default:
                    output.Metadata.Add("column_2", "Daily Total");
                    return output;
            }
        }

        /// <summary>
        /// Constructs the ITimeSeriesOutput Data and MetaData object from the data string.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="dataSet"></param>
        /// <param name="data"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public ITimeSeriesOutput SetDataToOutput(out string errorMsg, string dataSet, string data, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";
            string[] splitData = data.Split(new string[] { "year,yday,prcp (mm/day)" }, StringSplitOptions.RemoveEmptyEntries);
            output.Metadata = SetMetaData(out errorMsg, splitData[0]);
            if (errorMsg.Contains("ERROR")) { return null; }

            double modifier = (input.Units.Contains("imperial")) ? 0.0393701 : 1.0;
            Dictionary<DateTime, List<string>> outputTemp = SetData(out errorMsg, splitData[1], input.DataValueFormat, input.DateTimeSpan.DateTimeFormat, input.Geometry.GeometryMetadata, modifier, input.DateTimeSpan);
            if (errorMsg.Contains("ERROR")) { return null; }

            SortedDictionary<DateTime, List<string>> sortedData = new SortedDictionary<DateTime, List<string>>(outputTemp);
            if (input.Geometry.GeometryMetadata.ContainsKey("leapYear"))
            {
                // Daymet Leap Year MESS!
                // Inserts Dec 31st for leap years.
                for (int i = 0; i <= (outputTemp.Keys.Last().Year - outputTemp.Keys.First().Year); i++)
                {
                    DateTime date = sortedData.Keys.First().AddYears(i);

                    if (DateTime.IsLeapYear(date.Year) && sortedData.ContainsKey(new DateTime(date.Year, 12, 30)))
                    {
                        sortedData.Add(new DateTime(date.Year, 12, 31), new List<string>() { (0).ToString(input.DataValueFormat) });
                    }
                    if (i == (outputTemp.Keys.Last().Year - outputTemp.Keys.First().Year) - 1) { break; }
                }
            }
            Dictionary<string, List<string>> outputFinal = new Dictionary<string, List<string>>();
            foreach(DateTime key in sortedData.Keys)
            {
                outputFinal.Add(key.ToString(input.DateTimeSpan.DateTimeFormat), sortedData[key]);
            }

            output.Data = outputFinal;
            return output;
        }

        /// <summary>
        /// Creates the metaData dictionary for Daymet data.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="newMetaData"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        private Dictionary<string, string> SetMetaData(out string errorMsg, string metadata)
        {
            errorMsg = "";
            Dictionary<string, string> daymetMetadata = new Dictionary<string, string>();
            string[] metaLines = metadata.Split(new string[] { "\n", "  " }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < metaLines.Length; i++)
            {
                if (metaLines[i].Contains("http"))
                {
                    daymetMetadata.Add("daymet_url_reference:", metaLines[i].Trim());
                }
                else if (metaLines[i].Contains(':'))
                {
                    string[] lineData = metaLines[i].Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);
                    daymetMetadata.Add("daymet_" + lineData[0].Trim(), lineData[1].Trim());
                }
            }
            return daymetMetadata;
        }

        /// <summary>
        /// Creates the timeseries dictionary for the daymet data.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="timeseries"></param>
        /// <param name="dataFormat"></param>
        /// <param name="modifier"></param>
        /// <returns></returns>
        private Dictionary<DateTime, List<string>> SetData(out string errorMsg, string timeseries, string dataFormat, string dateFormat, Dictionary<string, string> geoMeta, double modifier, IDateTimeSpan dateSpan)
        {
            errorMsg = "";
            Dictionary<DateTime, List<string>> data = new Dictionary<DateTime, List<string>>();
            string[] tsLines = timeseries.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (geoMeta.ContainsKey("leapYear"))
            {
                for (int i = 0; i < tsLines.Length; i++)
                {
                    string[] lineData = tsLines[i].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    DateTime date = new DateTime(Convert.ToInt16(Convert.ToDouble(lineData[0])), 1, 1);

                    // Leap year dates have to be shifted by -1 day after Feb 28, due to Daymet not including Feb 29th
                    if (DateTime.IsLeapYear(date.Year) && date > new DateTime(date.Year, 2, 28))
                    {
                        date = date.AddDays(-1.0);
                    };

                    DateTime date2 = new DateTime();
                    date2 = date;
                    if (i > 0) { date2 = date.AddDays(Convert.ToDouble(lineData[1]) - 1); }
                    else { date2 = date; }
                    if (date2.Date >= dateSpan.StartDate.Date && date2.Date <= dateSpan.EndDate.Date)
                    {
                        data.Add(date2, new List<string> { (modifier * Convert.ToDouble(lineData[2])).ToString(dataFormat) });
                    }
                }
            }
            else
            {
                for (int i = 0; i < tsLines.Length; i++)
                {
                    string[] lineData = tsLines[i].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    DateTime date = new DateTime(Convert.ToInt16(Convert.ToDouble(lineData[0])), 1, 1);
                    DateTime date2;
                    if (i > 0) { date2 = date.AddDays(Convert.ToDouble(lineData[1]) - 1); }
                    else { date2 = date; }
                    if (date2 >= dateSpan.StartDate && date2 <= dateSpan.EndDate)
                    {
                        data.Add(date2, new List<string> { (modifier * Convert.ToDouble(lineData[2])).ToString(dataFormat) });
                    }
                }
            }
            return data;
        }

        /// <summary>
        /// Calls the function in Data.Source.Daymet that will perform the status check.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Dictionary<string, string> CheckStatus(ITimeSeriesInput input)
        {
            return Data.Source.Daymet.CheckStatus("Precip", input);
        }

        /// <summary>
        /// Validate input dates and coordinates for precipitation daymet data.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        private Boolean ValidateInputs(ITimeSeriesInput input, out string errorMsg)
        {
            errorMsg = "";
            List<string> errors = new List<string>();
            bool valid = true;
            // Validate Date range
            // Daymet date range 1980 - Present(- 1 year)
            DateTime date0 = new DateTime(1980, 1, 1);
            DateTime yearMax = DateTime.Now;
            DateTime date1 = new DateTime(yearMax.Year - 1, 12, 31);
            string dateFormat = "yyyy-MM-dd";
            if (DateTime.Compare(input.DateTimeSpan.StartDate, date0) < 0 || (DateTime.Compare(input.DateTimeSpan.StartDate, date1) > 0))
            {
                errors.Add("ERROR: Start date is not valid. Date must be between " + date0.ToString(dateFormat) + " and " + date1.ToString(dateFormat) + ". Start date provided: " + input.DateTimeSpan.StartDate.ToString(dateFormat));
            }
            if (DateTime.Compare(input.DateTimeSpan.EndDate, date0) < 0 || DateTime.Compare(input.DateTimeSpan.EndDate, date1) > 0)
            {
                errors.Add("ERROR: End date is not valid. Date must be between " + date0.ToString(dateFormat) + " and " + date1.ToString(dateFormat) + ". End date provided: " + input.DateTimeSpan.EndDate.ToString(dateFormat));
            }

            // Validate Spatial range
            // Daymet spatial range 125W ~ 63E, 25S ~ 53N
            if (input.Geometry.Point.Latitude < -25 || input.Geometry.Point.Latitude > 53)
            {
                errors.Add("ERROR: Latitude is not valid. Latitude must be between -25 and 53. Latitude provided: " + input.Geometry.Point.Latitude.ToString());
            }
            if (input.Geometry.Point.Longitude < -125 || input.Geometry.Point.Longitude > 63)
            {
                errors.Add("ERROR: Longitude is not valid. Longitude must be between -125 and 63. Longitude provided: " + input.Geometry.Point.Longitude.ToString());
            }

            if (errors.Count > 0)
            {
                valid = false;
                errorMsg = String.Join(", ", errors.ToArray());
            }

            return valid;
        }
    }
}
