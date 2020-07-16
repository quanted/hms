using Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Temperature
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
            string data = prism.GetData(out errorMsg, "'tmax', 'tmin'", input);
            if (errorMsg.Contains("ERROR")) { return null; }

            ITimeSeriesOutput prismOutput = output;
            prismOutput = prism.SetDataToOutput(out errorMsg, "Temperature", data, output, input);
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
                    // Combined max/min/mean
                    output.Data = DailyValues(out errorMsg, output, input, "all");
                    output.Metadata.Add("column_2", "Max Temp");
                    output.Metadata.Add("column_3", "Min Temp");
                    output.Metadata.Add("column_4", "Avg Temp");
                    return output;
                case "monthly":
                    // Combined max/min/mean
                    output.Data = DailyValues(out errorMsg, output, input, "all");
                    output.Data = MonthlyValues(out errorMsg, output, input, "all");
                    output.Metadata.Add("column_2", "Avg Max Temp");
                    output.Metadata.Add("column_3", "Avg Low Temp");
                    output.Metadata.Add("column_4", "Avg Temp");
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
        /// Gets daily temperature values, calculating and setting to Data average, high and low, depending on request.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <param name="type">"all", "avg", "high", "low"</param>
        /// <returns></returns>
        public static Dictionary<string, List<string>> DailyValues(out string errorMsg, ITimeSeriesOutput output, ITimeSeriesInput input, string type)
        {
            errorMsg = "";
            DateTime iDate = new DateTime();
            string dateString0 = output.Data.Keys.ElementAt(0).ToString().Substring(0, output.Data.Keys.ElementAt(0).ToString().Length - 1) + ":00:00";
            DateTime.TryParse(dateString0, out iDate);

            Dictionary<string, List<string>> tempData = new Dictionary<string, List<string>>();

            for (int i = 0; i < output.Data.Count; i++)
            {
                DateTime date = new DateTime();
                string dateString = output.Data.Keys.ElementAt(i).ToString().Substring(0, output.Data.Keys.ElementAt(i).ToString().Length - 1) + ":00:00";
                DateTime.TryParse(dateString, out date);
                double maxValue = Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0]);
                double minValue = Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][1]);
                double average = (maxValue + minValue) / 2;
                tempData.Add(date.ToString(input.DateTimeSpan.DateTimeFormat), new List<string>()
                                {
                                    (maxValue).ToString(input.DataValueFormat),
                                    (minValue).ToString(input.DataValueFormat),
                                    (average).ToString(input.DataValueFormat)
                                }
                );
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
        public static Dictionary<string, List<string>> WeeklyValues(out string errorMsg, ITimeSeriesOutput output, ITimeSeriesInput input, string type)
        {
            errorMsg = "";
            DateTime iDate = new DateTime();
            string dateString0 = output.Data.Keys.ElementAt(0).ToString().Substring(0, output.Data.Keys.ElementAt(0).ToString().Length - 1) + ":00:00";
            DateTime.TryParse(dateString0, out iDate);

            Dictionary<string, List<string>> tempData = new Dictionary<string, List<string>>();
            double average = 0.0;
            double max = -9999.9;
            double min = 9999.9;
            double maxValue = 0.0;
            double minValue = 0.0;
            for (int i = 0; i < output.Data.Count; i++)
            {
                DateTime date = new DateTime();
                string dateString = output.Data.Keys.ElementAt(i).ToString().Substring(0, output.Data.Keys.ElementAt(i).ToString().Length - 1) + ":00:00";
                DateTime.TryParse(dateString, out date);
                maxValue = Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0]);
                minValue = Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][1]);
                max = (maxValue > max) ? maxValue : max;
                min = (minValue < min) ? minValue : min;
                average = (average + (maxValue + minValue) / 2) / 2;
                int dayDif = (int)(date - iDate).TotalDays;
                if (dayDif >= 7)
                {
                    tempData.Add(iDate.ToString(input.DateTimeSpan.DateTimeFormat), new List<string>()
                                {
                                    (max).ToString(input.DataValueFormat),
                                    (min).ToString(input.DataValueFormat),
                                    (average).ToString(input.DataValueFormat)
                                }
                    );
                    max = -9999.9;
                    min = 9999.9;
                    iDate = date;
                }
            }
            return tempData;
        }

        /// <summary>
        /// Gets monthly temperature values, calculating and setting to Data average, high and low, depending on request.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <param name="type">"all", "avg", "high", "low"</param>
        /// <returns></returns>
        public static Dictionary<string, List<string>> MonthlyValues(out string errorMsg, ITimeSeriesOutput output, ITimeSeriesInput input, string type)
        {
            errorMsg = "";
            DateTime iDate = new DateTime();
            string dateString0 = output.Data.Keys.ElementAt(0).ToString().Substring(0, output.Data.Keys.ElementAt(0).ToString().Length - 1) + ":00:00";
            DateTime.TryParse(dateString0, out iDate);

            double avgSum = 0.0;
            double lowSum = 0.0;
            double hiSum = 0.0;

            int dayIndex = 0;

            Dictionary<string, List<string>> tempData = new Dictionary<string, List<string>>();

            for (int i = 0; i < output.Data.Count; i++)
            {
                DateTime date = new DateTime();
                string dateString = output.Data.Keys.ElementAt(i).ToString().Substring(0, output.Data.Keys.ElementAt(i).ToString().Length - 1) + ":00:00";
                DateTime.TryParse(dateString, out date);
                if (date.Month != iDate.Month || i == output.Data.Count - 1)
                {
                    double average = avgSum / dayIndex;
                    double lAverage = lowSum / dayIndex;
                    double hAverage = hiSum / dayIndex;

                    tempData.Add(iDate.ToString(input.DateTimeSpan.DateTimeFormat), new List<string>()
                                {
                                    (hAverage).ToString(input.DataValueFormat),
                                    (lAverage).ToString(input.DataValueFormat),
                                    (average).ToString(input.DataValueFormat)
                                }
                    );

                    avgSum = Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][2]);
                    hiSum = Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0]);
                    lowSum = Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][1]);
                    iDate = date;
                    dayIndex = 1;
                }
                else
                {
                    avgSum += Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][2]);
                    hiSum += Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0]);
                    lowSum += Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][1]);

                    dayIndex++;
                }
            }
            return tempData;
        }
    }
}
