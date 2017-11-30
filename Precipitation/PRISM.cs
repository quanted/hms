using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Precipitation
{
    public class PRISM
    {

        public ITimeSeriesOutput GetData(out string errorMsg, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";
            Data.Source.PRISM prism = new Data.Source.PRISM();
            string data = prism.GetData(out errorMsg, "ppt", input);

            ITimeSeriesOutput prismOutput = output;
            if (errorMsg.Contains("ERROR"))
            {
                Utilities.ErrorOutput err = new Utilities.ErrorOutput();
                output = err.ReturnError("Precipitation", "PRISM", errorMsg);
                errorMsg = "";
                return output;
            }
            else
            {
                prismOutput = prism.SetDataToOutput(out errorMsg, "Precipitation", data, output, input);
            }

            prismOutput = TemporalAggregation(out errorMsg, output, input);
            if (errorMsg.Contains("ERROR")) { return null; }

            return prismOutput;
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
            output.Metadata.Add("prism_temporalresolution", input.TemporalResolution);
            output.Metadata.Add("column_1", "Date");
            if (input.Units.Contains("imperial")) { output.Metadata["prism_unit"] = "in"; }
            switch (input.TemporalResolution)
            {
                case "weekly":
                    output.Data = WeeklyAggregatedSum(out errorMsg, 1.0, output, input);
                    output.Metadata.Add("column_2", "Weekly Total");
                    return output;
                case "monthly":
                    output.Data = MonthlyAggregatedSum(out errorMsg, 1.0, output, input);
                    output.Metadata.Add("column_2", "Monthly Total");
                    return output;
                default:
                    output.Data = (input.Units.Contains("imperial")) ? UnitConversion(out errorMsg, 1.0, output, input) : output.Data;
                    output.Metadata.Add("column_2", "Daily Total");
                    return output;
            }
        }

        /// <summary>
        /// Converts metric ldas kg/m**2 (mm) units to imperial inches units.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Dictionary<string, List<string>> UnitConversion(out string errorMsg, double modifier, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";

            // Unit conversion coefficient
            double unit = 0.0393701;
            Dictionary<string, List<string>> tempData = new Dictionary<string, List<string>>();
            for (int i = 0; i < output.Data.Count; i++)
            {
                tempData.Add(output.Data.Keys.ElementAt(i).ToString(), new List<string>()
                {
                    ( modifier * unit * Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0])).ToString(input.DataValueFormat)
                });
            }
            return tempData;
        }

        /// <summary>
        /// Weekly aggregated sums for precipitation data.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public static Dictionary<string, List<string>> WeeklyAggregatedSum(out string errorMsg, double modifier, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";

            DateTime iDate = new DateTime();
            double sum = 0.0;

            // Unit conversion coefficient
            double unit = (input.Units.Contains("imperial")) ? 0.0393701 : 1.0;

            string dateString0 = output.Data.Keys.ElementAt(0).ToString().Substring(0, output.Data.Keys.ElementAt(0).ToString().Length - 1) + ":00:00";
            DateTime.TryParse(dateString0, out iDate);
            Dictionary<string, List<string>> tempData = new Dictionary<string, List<string>>();
            for (int i = 0; i < output.Data.Count; i++)
            {
                DateTime date = new DateTime();
                string dateString = output.Data.Keys.ElementAt(i).ToString().Substring(0, output.Data.Keys.ElementAt(i).ToString().Length) + ":00:00";
                DateTime.TryParse(dateString, out date);
                int dayDif = (int)(date - iDate).TotalDays;
                if (dayDif >= 7)
                {
                    tempData.Add(iDate.ToString(input.DateTimeSpan.DateTimeFormat), new List<string>() { (modifier * unit * sum).ToString(input.DataValueFormat) });
                    iDate = date;
                    sum = Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0]);
                }
                else
                {
                    sum += Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0]);
                }
            }
            return tempData;
        }

        /// <summary>
        /// Monthly aggregated sums for precipitation data.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public static Dictionary<string, List<string>> MonthlyAggregatedSum(out string errorMsg, double modifier, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";

            DateTime iDate = new DateTime();
            double sum = 0.0;

            // Unit conversion coefficient
            double unit = (input.Units.Contains("imperial")) ? 0.0393701 : 1.0;

            string dateString0 = output.Data.Keys.ElementAt(0).ToString().Substring(0, output.Data.Keys.ElementAt(0).ToString().Length - 1) + ":00:00";
            DateTime.TryParse(dateString0, out iDate);

            Dictionary<string, List<string>> tempData = new Dictionary<string, List<string>>();
            for (int i = 0; i < output.Data.Count; i++)
            {
                DateTime date = new DateTime();
                string dateString = output.Data.Keys.ElementAt(i).ToString().Substring(0, output.Data.Keys.ElementAt(i).ToString().Length - 1) + ":00:00";
                DateTime.TryParse(dateString, out date);
                if (date.Month != iDate.Month)
                {
                    tempData.Add(iDate.ToString(input.DateTimeSpan.DateTimeFormat), new List<string>() { (modifier * unit * sum).ToString(input.DataValueFormat) });
                    iDate = date;
                    sum = Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0]);
                }
                else
                {
                    sum += Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0]);
                }
            }
            return tempData;
        }
    }
}
