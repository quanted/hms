using Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Temperature
{
    /// <summary>
    /// Temperature NLDAS class.
    /// </summary>
    public class NLDAS
    {
        /// <summary>
        /// Makes the GetData call to the base NLDAS class.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public ITimeSeriesOutput GetData(out string errorMsg, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";
            Data.Source.NLDAS nldas = new Data.Source.NLDAS();
            string data = nldas.GetData(out errorMsg, "Temp", input);
            if (errorMsg.Contains("ERROR")) { return null; }

            ITimeSeriesOutput nldasOutput = output;
            nldasOutput = nldas.SetDataToOutput(out errorMsg, "Temperature", data, output, input);
            if (errorMsg.Contains("ERROR")) { return null; }

            nldasOutput = TemporalAggregation(out errorMsg, output, input);
            if (errorMsg.Contains("ERROR")) { return null; }

            return nldasOutput;
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
            output.Metadata.Add("nldas_temporalresolution", input.TemporalResolution);

            if (input.Units.Contains("imperial")) { output.Metadata["nldas_unit"] = "F"; }
            output.Data = (input.Units.Contains("imperial")) ? UnitConversion(out errorMsg, output, input) : output.Data;

            output.Metadata.Add("column_1", "Date");
            
            switch (input.TemporalResolution)
            {
                case "daily":
                case "default":
                    // Combined max/min/average
                    output.Data = DailyValues(out errorMsg, output, input, "all");
                    output.Metadata.Add("column_2", "Max Temperature");
                    output.Metadata.Add("column_3", "Min Temperature");
                    output.Metadata.Add("column_4", "Average Temperature");
                    return output;
                case "daily-avg":
                    output.Data = DailyValues(out errorMsg, output, input, "avg");
                    output.Metadata.Add("column_2", "Average Temperature");
                    return output;
                case "daily-high":
                    output.Data = DailyValues(out errorMsg, output, input, "high");
                    output.Metadata.Add("column_2", "Max Temperature");
                    return output;
                case "daily-low":
                    output.Data = DailyValues(out errorMsg, output, input, "low");
                    output.Metadata.Add("column_2", "Min Temperature");
                    return output;
                case "weekly":
                    // Combined max/min/average
                    output.Data = WeeklyValues(out errorMsg, output, input, "all");
                    output.Metadata.Add("column_2", "Max Temperature");
                    output.Metadata.Add("column_3", "Min Temperature");
                    output.Metadata.Add("column_4", "Average Temperature");
                    return output;
                case "weekly-avg":
                    output.Data = WeeklyValues(out errorMsg, output, input, "avg");
                    output.Metadata.Add("column_2", "Average Temperature");
                    return output;
                case "weekly-high":
                    output.Data = WeeklyValues(out errorMsg, output, input, "high");
                    output.Metadata.Add("column_2", "Max Temperature");
                    return output;
                case "weekly-low":
                    output.Data = WeeklyValues(out errorMsg, output, input, "low");
                    output.Metadata.Add("column_2", "Min Temperature");
                    return output;
                case "monthly":
                    // Combined max/min/average
                    output.Data = MonthlyValues(out errorMsg, output, input, "all");
                    output.Metadata.Add("column_2", "Max Temperature");
                    output.Metadata.Add("column_3", "Low Temperature");
                    output.Metadata.Add("column_4", "Average Temperature");
                    return output;
                case "monthly-avg":
                    output.Data = MonthlyValues(out errorMsg, output, input, "avg");
                    output.Metadata.Add("column_2", "Average Temperature");
                    return output;
                case "monthly-high":
                    output.Data = MonthlyValues(out errorMsg, output, input, "high");
                    output.Metadata.Add("column_2", "Max Temperature");
                    return output;
                case "monthly-low":
                    output.Data = MonthlyValues(out errorMsg, output, input, "low");
                    output.Metadata.Add("column_2", "Min Temperature");
                    return output;
                case "hourly":
                    output.Metadata.Add("column_2", "Hourly Average Temperature");
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
            Dictionary<string, List< string>> tempData = new Dictionary<string, List<string>>();
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

            double sum = 0.0;
            double high = 0.0;
            double low = 5000;

            double allSum = 0.0;
            double allHigh = 0.0;
            double allLow = 5000;

            int dayIndex = 0;

            Dictionary<string, List<string>> tempData = new Dictionary<string, List<string>>();

            for (int i = 0; i < output.Data.Count; i++)
            {
                DateTime date = new DateTime();
                string dateString = output.Data.Keys.ElementAt(i).ToString().Substring(0, output.Data.Keys.ElementAt(i).ToString().Length - 1) + ":00:00";
                DateTime.TryParse(dateString, out date);
                if (date.Day != iDate.Day)
                {
                    double average = 0.0;
                    switch (type)
                    {
                        case "all":
                        default:
                            average = sum / (dayIndex);
                            tempData.Add(iDate.ToString(input.DateTimeSpan.DateTimeFormat), new List<string>()
                                {
                                    (high).ToString(input.DataValueFormat),
                                    (low).ToString(input.DataValueFormat),
                                    (average).ToString(input.DataValueFormat)
                                }
                            );
                            break;
                        case "avg":
                            average = sum / (dayIndex);
                            tempData.Add(iDate.ToString(input.DateTimeSpan.DateTimeFormat), new List<string>()
                                {
                                    (average).ToString(input.DataValueFormat)
                                }
                            );
                            break;
                        case "high":
                            average = sum / (dayIndex);
                            tempData.Add(iDate.ToString(input.DateTimeSpan.DateTimeFormat), new List<string>()
                                {
                                    (high).ToString(input.DataValueFormat)
                                }
                            );
                            break;
                        case "low":
                            average = sum / (dayIndex);
                            tempData.Add(iDate.ToString(input.DateTimeSpan.DateTimeFormat), new List<string>()
                                {
                                    (low).ToString(input.DataValueFormat)
                                }
                            );
                            break;
                    }
                    double value = Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0]);
                    high = low = sum = value;
                    allSum += value;
                    iDate = date;
                    dayIndex = 1;
                }
                else
                {
                    double value = Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0]);

                    high = (value > high) ? value : high;
                    allHigh = (value > allHigh) ? value : allHigh;
                    low = (value < low) ? value : low;
                    allLow = (value < allLow) ? value : allLow;
                    allSum += value;
                    sum += value;
                    dayIndex++;
                }
            }
            tempData.Add("Total Average", new List<string>() { (allSum / output.Data.Count).ToString(input.DataValueFormat) });
            tempData.Add("Max Temp", new List<string>() { allHigh.ToString(input.DataValueFormat) });
            tempData.Add("Min Temp", new List<string>() { allLow.ToString(input.DataValueFormat) });
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

            double sum = 0.0;
            double high = 0.0;
            double low = 5000;

            double allSum = 0.0;
            double allHigh = 0.0;
            double allLow = 5000;

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
                    switch (type)
                    {
                        case "all":
                        default:
                            average = sum / dayIndex;
                            tempData.Add(iDate.ToString(input.DateTimeSpan.DateTimeFormat), new List<string>()
                                {
                                    (high).ToString(input.DataValueFormat),
                                    (low).ToString(input.DataValueFormat),
                                    (average).ToString(input.DataValueFormat)
                                }
                            );
                            break;
                        case "avg":
                            average = sum / dayIndex;
                            tempData.Add(iDate.ToString(input.DateTimeSpan.DateTimeFormat), new List<string>()
                                {
                                    (average).ToString(input.DataValueFormat)
                                }
                            );
                            break;
                        case "high":
                            tempData.Add(iDate.ToString(input.DateTimeSpan.DateTimeFormat), new List<string>()
                                {
                                    (high).ToString(input.DataValueFormat)
                                }
                            );
                            break;
                        case "low":
                            tempData.Add(iDate.ToString(input.DateTimeSpan.DateTimeFormat), new List<string>()
                                {
                                    (low).ToString(input.DataValueFormat)
                                }
                            );
                            break;
                    }
                    double value = Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0]); ;
                    sum = high = low = value;
                    allSum += value;
                    iDate = date;
                    dayIndex = 0;
                }
                else
                {
                    double value = Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0]);

                    high = (value > high) ? value : high;
                    allHigh = (value > allHigh) ? value : allHigh;
                    low = (value < low) ? value : low;
                    allLow = (value < allLow) ? value : allLow;
                    allSum += value;
                    sum += value;
                    dayIndex++;
                }
            }
            tempData.Add("Total Average", new List<string>() { (allSum / output.Data.Count).ToString(input.DataValueFormat) });
            tempData.Add("Max Temp", new List<string>() { allHigh.ToString(input.DataValueFormat) });
            tempData.Add("Min Temp", new List<string>() { allLow.ToString(input.DataValueFormat) });
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

            double sum = 0.0;
            double high = 0.0;
            double low = 5000;

            double allSum = 0.0;
            double allHigh = 0.0;
            double allLow = 5000;

            int dayIndex = 0;

            Dictionary<string, List<string>> tempData = new Dictionary<string, List<string>>();

            for (int i = 0; i < output.Data.Count; i++)
            {
                DateTime date = new DateTime();
                string dateString = output.Data.Keys.ElementAt(i).ToString().Substring(0, output.Data.Keys.ElementAt(i).ToString().Length - 1) + ":00:00";
                DateTime.TryParse(dateString, out date);
                if (date.Month != iDate.Month || i == output.Data.Count - 1)
                {
                    double average = 0.0;
                    switch (type)
                    {
                        case "all":
                        default:
                            average = sum / dayIndex;
                            tempData.Add(iDate.ToString(input.DateTimeSpan.DateTimeFormat), new List<string>()
                                {
                                    (high).ToString(input.DataValueFormat),
                                    (low).ToString(input.DataValueFormat),
                                    (average).ToString(input.DataValueFormat)
                                }
                            );
                            break;
                        case "avg":
                            average = sum / dayIndex;
                            tempData.Add(iDate.ToString(input.DateTimeSpan.DateTimeFormat), new List<string>()
                                {
                                    (average).ToString(input.DataValueFormat)
                                }
                            );
                            break;
                        case "high":
                            tempData.Add(iDate.ToString(input.DateTimeSpan.DateTimeFormat), new List<string>()
                                {
                                    (high).ToString(input.DataValueFormat)
                                }
                            );
                            break;
                        case "low":
                            tempData.Add(iDate.ToString(input.DateTimeSpan.DateTimeFormat), new List<string>()
                                {
                                    (low).ToString(input.DataValueFormat)
                                }
                            );
                            break;
                    }
                    double value = Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0]);
                    sum = high = low = value;
                    allSum += value;
                    iDate = date;
                    dayIndex = 0;
                }
                else
                {
                    double value = Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0]);

                    high = (value > high) ? value : high;
                    allHigh = (value > allHigh) ? value : allHigh;
                    low = (value < low) ? value : low;
                    allLow = (value < allLow) ? value : allLow;
                    allSum += value;
                    sum += value;
                    dayIndex++;
                }
            }
            tempData.Add("Total Average", new List<string>() { (allSum / output.Data.Count).ToString(input.DataValueFormat) });
            tempData.Add("Max Temp", new List<string>() { allHigh.ToString(input.DataValueFormat) });
            tempData.Add("Min Temp", new List<string>() { allLow.ToString(input.DataValueFormat) });
            return tempData;
        }

        /// <summary>
        /// Calls the function in Data.Source.NLDAS that will perform the status check.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Dictionary<string, string> CheckStatus(ITimeSeriesInput input)
        {
            return Data.Source.NLDAS.CheckStatus("Temperature", input);
        }
    }
}
