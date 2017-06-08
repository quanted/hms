using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Temperature
{
    /// <summary>
    /// Temperature Daymet class.
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
            string data = daymet.GetData(out errorMsg, "Temp", input);
            if (errorMsg.Contains("ERROR")) { return null; }

            ITimeSeriesOutput daymetOutput = output;
            daymetOutput = SetDataToOutput(out errorMsg, "Temperature", data, output, input);

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

            output.Metadata["daymet_unit"] = (input.Units.Contains("imperial")) ? "F" : "K";
            output.Data = (input.Units.Contains("imperial")) ? UnitConversion(out errorMsg, false, output, input) : UnitConversion(out errorMsg, true, output, input);

            output.Metadata.Add("daymet_column_1", "date");
            output.Metadata.Add("daymet_column_2", "Max Temp");
            output.Metadata.Add("daymet_column_3", "Min Temp");
            switch (input.TemporalResolution)
            {
                case "weekly":
                    output.Data = WeeklyAverage(out errorMsg, output, input);
                    return output;
                case "monthly":
                    output.Data = MonthlyAverage(out errorMsg, output, input);
                    return output;
                case "daily":
                default:
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
            string[] splitData = data.Split(new string[] { "year,yday,tmax (deg c),tmin (deg c)" }, StringSplitOptions.RemoveEmptyEntries);
            output.Metadata = SetMetaData(out errorMsg, splitData[0]);
            if (errorMsg.Contains("ERROR")) { return null; }

            double modifier = (input.Units.Contains("imperial")) ? 0.0393701 : 1.0;
            output.Data = SetData(out errorMsg, splitData[1], input.DataValueFormat, input.DateTimeSpan.DateTimeFormat, modifier);
            if (errorMsg.Contains("ERROR")) { return null; }

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
        private Dictionary<string, List<string>> SetData(out string errorMsg, string timeseries, string dataFormat, string dateFormat, double modifier)
        {
            errorMsg = "";
            Dictionary<string, List<string>> data = new Dictionary<string, List<string>>();
            string[] tsLines = timeseries.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < tsLines.Length; i++)
            {
                string[] lineData = tsLines[i].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                DateTime date = new DateTime(Convert.ToInt16(lineData[0]), 1, 1);
                DateTime date2;
                if (i > 0) { date2 = date.AddDays(Convert.ToInt16(lineData[1]) - 1); }
                else { date2 = date; }
                data.Add(date2.ToString(dateFormat), new List<string>
                {
                    { (modifier * Convert.ToDouble(lineData[2])).ToString(dataFormat) },
                    { (modifier * Convert.ToDouble(lineData[3])).ToString(dataFormat) }
                });
            }
            return data;
        }

        private Dictionary<string, List<string>> UnitConversion(out string errorMsg, bool metric, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";
            Dictionary<string, List<string>> tempData = new Dictionary<string, List<string>>();

            if (metric == true)
            {
                for (int i = 0; i < output.Data.Count; i++)
                {
                    tempData.Add(output.Data.Keys.ElementAt(i).ToString(), new List<string>()
                {
                        { ( 273.15 + Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0])).ToString(input.DataValueFormat) },
                        { ( 273.15 + Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][1])).ToString(input.DataValueFormat) }
                });
                }
            }
            else
            {
                for (int i = 0; i < output.Data.Count; i++)
                {
                    tempData.Add(output.Data.Keys.ElementAt(i).ToString(), new List<string>()
                {
                        { ( 32 + ((9.0/5.0) * Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0]))).ToString(input.DataValueFormat) },
                        { ( 32 + ((9.0/5.0) * Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][1]))).ToString(input.DataValueFormat) }
                });
                }

            }
            return tempData;
        }

        /// <summary>
        /// Weekly averages for daymet temperature data. Calculated from sum of (daily max + daily min)/2 divided by 7.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private Dictionary<string, List<string>> WeeklyAverage(out string errorMsg, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";
            Dictionary<string, List<string>> tempData = new Dictionary<string, List<string>>();
            DateTime iDate = new DateTime();

            for (int i = 0; i < output.Data.Count; i++)
            {
                DateTime date = new DateTime();
                string dateString = output.Data.Keys.ElementAt(i).ToString().Substring(0, output.Data.Keys.ElementAt(i).ToString().Length) + ":00:00";
                DateTime.TryParse(dateString, out date);
                double sum = 0.0;
                int dayDif = (int)(date - iDate).TotalDays;
                if (dayDif >= 7)
                {
                    double wAverage = sum / 7;
                    tempData.Add(iDate.ToString(input.DateTimeSpan.DateTimeFormat), new List<string>()
                    {
                        { (output.Data[output.Data.Keys.ElementAt(i)][0]) },
                        { (output.Data[output.Data.Keys.ElementAt(i)][1]) },
                        { (wAverage).ToString() }
                    });
                    iDate = date;
                    sum = (Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0]) + Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][1])) / 2;
                }
                else
                {
                    sum += (Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0]) + Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][1])) / 2;
                }
            }

            return tempData;
        }

        /// <summary>
        /// Monthly averages for daymet temperature data. Calculated from sum of (daily max + daily min)/2 divided by lenght of Month.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private Dictionary<string, List<string>> MonthlyAverage(out string errorMsg, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";
            Dictionary<string, List<string>> tempData = new Dictionary<string, List<string>>();
            DateTime iDate = new DateTime();
            int mDays = 0;
            for (int i = 0; i < output.Data.Count; i++)
            {
                DateTime date = new DateTime();
                string dateString = output.Data.Keys.ElementAt(i).ToString().Substring(0, output.Data.Keys.ElementAt(i).ToString().Length) + ":00:00";
                DateTime.TryParse(dateString, out date);
                double sum = 0.0;
                if (date.Month != iDate.Month)
                {
                    double wAverage = sum / mDays;
                    tempData.Add(iDate.ToString(input.DateTimeSpan.DateTimeFormat), new List<string>()
                    {
                        { (output.Data[output.Data.Keys.ElementAt(i)][0]) },
                        { (output.Data[output.Data.Keys.ElementAt(i)][1]) },
                        { (wAverage).ToString() }
                    });
                    mDays = 0;
                    iDate = date;
                    sum = (Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0]) + Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][1])) / 2;
                }
                else
                {
                    sum += (Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0]) + Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][1])) / 2;
                    mDays++;
                }
            }

            return tempData;
        }

    }
}
