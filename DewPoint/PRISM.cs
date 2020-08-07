using Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DewPoint
{
    public class PRISM
    {
        /// <summary>
        /// Makes the GetData call to the base PRISM class.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public ITimeSeriesOutput GetData(out string errorMsg, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";
            Data.Source.PRISM prism = new Data.Source.PRISM();
            string data = prism.GetData(out errorMsg, "'tdmean'", input);
            if (errorMsg.Contains("ERROR")) { return null; }

            ITimeSeriesOutput prismOutput = output;
            prismOutput = prism.SetDataToOutput(out errorMsg, "Dew Point", data, output, input);
            if (errorMsg.Contains("ERROR")) { return null; }

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

            if (input.Units.Contains("imperial")) { output.Metadata["prism_unit"] = "F"; }
            output.Data = (input.Units.Contains("imperial")) ? UnitConversion(out errorMsg, output, input) : output.Data;

            output.Metadata.Add("column_1", "Date");

            switch (input.TemporalResolution.ToLower())
            {
                case "daily":
                case "default":
                    output.Metadata.Add("column_2", "Mean Dew Point Temperature");
                    return output;
                case "monthly":
                    output.Data = MonthlyValues(out errorMsg, output, input);
                    output.Metadata.Add("column_2", "Mean Dew Point Temperature");
                    return output;
                default:
                    return output;
            }
        }

        /// <summary>
        /// Converts Kelvin to Fahrenheit
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Dictionary<string, List<string>> UnitConversion(out string errorMsg, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";
            Dictionary<string, List<string>> tempData = new Dictionary<string, List<string>>();
            for (int i = 0; i < output.Data.Count; i++)
            {
                tempData.Add(output.Data.Keys.ElementAt(i).ToString(), new List<string>()
                {
                    ((Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0]) * (9.0 / 5.0)) - 459.67).ToString(input.DataValueFormat)
                });
            }
            return tempData;
        }

        /// <summary>
        /// Gets weekly temperature values, calculating and setting to Data average, high and low, depending on request.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <param name="type">"all", "avg", "high", "low"</param>
        /// <returns></returns>
        public static Dictionary<string, List<string>> WeeklyValues(out string errorMsg, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";
            DateTime iDate = new DateTime();
            string dateString0 = output.Data.Keys.ElementAt(0).ToString().Substring(0, output.Data.Keys.ElementAt(0).ToString().Length - 1) + ":00:00";
            DateTime.TryParse(dateString0, out iDate);

            double sum = 0.0;
            int dayIndex = 0;

            Dictionary<string, List<string>> tempData = new Dictionary<string, List<string>>();

            for (int i = 0; i < output.Data.Count; i++)
            {
                DateTime date = new DateTime();
                string dateString = output.Data.Keys.ElementAt(i).ToString().Substring(0, output.Data.Keys.ElementAt(i).ToString().Length - 1) + ":00:00";
                DateTime.TryParse(dateString, out date);
                int dayDif = (int)(date - iDate).TotalDays;
                if (dayDif >= 7)
                {
                    double average = 0.0;
                    average = sum / dayIndex;
                    tempData.Add(iDate.ToString(input.DateTimeSpan.DateTimeFormat), new List<string>()
                                {
                                    (average).ToString(input.DataValueFormat)
                                }
                    );
                    double value = Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0]); ;
                    sum = value;
                    iDate = date;
                    dayIndex = 0;
                }
                else
                {
                    double value = Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0]);
                    sum += value;
                    dayIndex++;
                }
            }
            return tempData;
        }

        /// <summary>
        /// Calculates monthly aggregated values for dew point temperature
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Dictionary<string, List<string>> MonthlyValues(out string errorMsg, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";
            DateTime iDate = new DateTime();
            string dateString0 = output.Data.Keys.ElementAt(0).ToString().Substring(0, output.Data.Keys.ElementAt(0).ToString().Length - 1) + ":00:00";
            DateTime.TryParse(dateString0, out iDate);

            double sum = 0.0;
            int dayIndex = 0;

            Dictionary<string, List<string>> tempData = new Dictionary<string, List<string>>();

            for (int i = 0; i < output.Data.Count; i++)
            {
                DateTime date = new DateTime();
                string dateString = output.Data.Keys.ElementAt(i).ToString().Substring(0, output.Data.Keys.ElementAt(i).ToString().Length - 1) + ":00:00";
                DateTime.TryParse(dateString, out date);
                if (date.Month != iDate.Month || output.Data.Count - 1 == i)
                {
                    double average = 0.0;
                    average = sum / dayIndex;
                    tempData.Add(iDate.ToString(input.DateTimeSpan.DateTimeFormat), new List<string>()
                                {
                                    (average).ToString(input.DataValueFormat)
                                }
                    );
                    double value = Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0]);
                    sum = value;
                    iDate = date;
                    dayIndex = 0;
                }
                else
                {
                    double value = Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0]);
                    sum += value;
                    dayIndex++;
                }
            }
            return tempData;
        }
    }
}
