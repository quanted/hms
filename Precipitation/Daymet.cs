using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public ITimeSeriesOutput GetData(out string errorMsg, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";

            Data.Source.Daymet daymet = new Data.Source.Daymet();
            string data = daymet.GetData(out errorMsg, "Precip", input);
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
        private ITimeSeriesOutput TemporalAggregation(out string errorMsg, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";
            output.Metadata.Add("daymet_temporalresolution", input.TemporalResolution);
            output.Metadata.Add("column_1", "Date");

            switch (input.TemporalResolution)
            {
                case "weekly":
                    output.Data = NLDAS.WeeklyAggregatedSum(out errorMsg, 1.0, output, input);
                    output.Metadata.Add("column_2", "Weekly Total");
                    return output;
                case "monthly":
                    output.Data = NLDAS.MonthlyAggregatedSum(out errorMsg, 1.0, output, input);
                    output.Metadata.Add("column_2", "Monthly Total");
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
            Dictionary<DateTime, List<string>> outputTemp = SetData(out errorMsg, splitData[1], input.DataValueFormat, input.DateTimeSpan.DateTimeFormat, modifier);
            if (errorMsg.Contains("ERROR")) { return null; }

            SortedDictionary<DateTime, List<string>> sortedData = new SortedDictionary<DateTime, List<string>>(outputTemp);

            // Daymet Leap Year MESS!
            // Inserts Dec 31st for leap years.
            // Daymet leap year black magic (DON'T TOUCH)
            for(int i = 0; i <= (outputTemp.Keys.Last().Year - outputTemp.Keys.First().Year); i++)
            {
                DateTime date = sortedData.Keys.First().AddYears(i);

                if(DateTime.IsLeapYear(date.Year) && sortedData.ContainsKey(new DateTime(date.Year, 12, 30)))
                {
                    sortedData.Add(new DateTime(date.Year, 12, 31), new List<string>() { (0).ToString(input.DataValueFormat) });
                }
                if(i == (outputTemp.Keys.Last().Year - outputTemp.Keys.First().Year) - 1) { break; }
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
        private Dictionary<DateTime, List<string>> SetData(out string errorMsg, string timeseries, string dataFormat, string dateFormat, double modifier)
        {
            errorMsg = "";
            Dictionary<DateTime, List<string>> data = new Dictionary<DateTime, List<string>>();
            string[] tsLines = timeseries.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < tsLines.Length; i++)
            {
                string[] lineData = tsLines[i].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                DateTime date = new DateTime(Convert.ToInt16(lineData[0]), 1, 1);

                // Leap year dates have to be shifted by -1 day after Feb 28, due to Daymet not including Feb 29th
                if (DateTime.IsLeapYear(date.Year) && date > new DateTime(date.Year, 2, 28))
                {
                    date = date.AddDays(-1.0);
                };

                DateTime date2;
                if (i > 0) { date2 = date.AddDays(Convert.ToInt16(lineData[1]) - 1); }
                else { date2 = date; }
                data.Add(date2, new List<string> { ( modifier * Convert.ToDouble(lineData[2])).ToString(dataFormat) });
            }
            return data;
        }
    }
}
