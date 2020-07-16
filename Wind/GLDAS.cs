using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using MathNet.Numerics.LinearAlgebra;

namespace Wind
{
    /// <summary>
    /// Wind GLDAS class
    /// </summary>
    public class GLDAS
    {
        /// <summary>
        /// Makes the GetData call to the base GLDAS class.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public ITimeSeriesOutput GetData(out string errorMsg, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";
            Data.Source.GLDAS gldas = new Data.Source.GLDAS();
            List<string> data = gldas.GetData(out errorMsg, "Wind", input);
            if (errorMsg.Contains("ERROR")) { return null; }

            ITimeSeriesOutput gldasOutput = output;
            gldasOutput = gldas.SetDataToOutput(out errorMsg, "Wind", data, output, input);
            if (errorMsg.Contains("ERROR")) { return null; }

            switch (input.TemporalResolution)
            {
                case "monthly":
                    output.Data = DailyAverage(out errorMsg, 7, 1.0, output, input);
                    output.Data = MonthlyAverage(out errorMsg, 7, 1.0, output, input);
                    break;
                case "daily":
                    output.Data = DailyAverage(out errorMsg, 7, 1.0, output, input);
                    break;
                case "default":
                default:
                    break;
            }
            output.Metadata["column_1"] = "date";
            output.Metadata["column_2"] = "speed";
            output.Metadata["column_2_units"] = "m/s";

            return gldasOutput;
        }

        /// <summary>
        /// Daily average for wind data.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public static Dictionary<string, List<string>> DailyAverage(out string errorMsg, int hours, double modifier, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";

            // Unit conversion coefficient
            double unit = (input.Units.Contains("imperial")) ? 0.0393701 : 1.0;
            if (input.Units.Contains("imperial")) { output.Metadata["nldas_unit"] = "in"; }

            if (output.Data.Keys.Last().Contains(" 00"))
            {
                output.Data.Remove(output.Data.Keys.Last());
            }
            // Daily aggregation using MathNet Matrix multiplication (results in ~25% speedup)
            hours += 1;
            int totalDays = (output.Data.Keys.Count / hours) + 1;
            Dictionary<string, List<string>> tempData = new Dictionary<string, List<string>>();
            for (int index = 0; index < output.Data.ElementAt(0).Value.Count; index++)
            {
                double[][] dValues = new double[totalDays][];
                for (int i = 0; i < totalDays; i++)
                {
                    dValues[i] = new double[hours];
                }
                int hour = 0;
                int day = 0;
                //int i0 = (output.DataSource.Equals("nldas")) ? 1 : 0;
                for (int i = 0; i < output.Data.Keys.Count; i++)
                {
                    hour = i % (hours);
                    day = i / (hours);
                    dValues[day][hour] = modifier * Double.Parse(output.Data.ElementAt(i).Value[index]);
                }

                var m = Matrix<double>.Build;
                Matrix<double> matrix = m.DenseOfRowArrays(dValues);
                Vector<double> precipRowValues = matrix.RowSums();
                var tempKeys = output.Data.Keys;
                int j = 0;
                if (index == 0)
                {
                    for (int i = 0; i < output.Data.Count; i += hours)
                    {
                        tempData.Add(tempKeys.ElementAt(i).Replace(" 01", " 00"), new List<string> { (unit * precipRowValues.ElementAt(j) / matrix.Row(j).Count).ToString(input.DataValueFormat) });
                        j++;
                    }
                }
                else
                {
                    for (int i = 0; i < output.Data.Count; i += hours)
                    {
                        tempData[tempKeys.ElementAt(i).Replace(" 01", " 00")].Add((unit * precipRowValues.ElementAt(j) / matrix.Row(j).Count).ToString(input.DataValueFormat));
                        j++;
                    }
                }
            }
            return tempData;
        }

        /// <summary>
        /// Monthly average for wind data.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public static Dictionary<string, List<string>> MonthlyAverage(out string errorMsg, int hours, double modifier, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";
            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput output1 = oFactory.Initialize();
            Dictionary<string, double> dict = new Dictionary<string, double>();
            DateTime iDate;
            DateTime newDate;
            string dateString0 = output.Data.Keys.ElementAt(0).ToString().Substring(0, output.Data.Keys.ElementAt(0).ToString().Length - 1) + ":00:00";
            DateTime.TryParse(dateString0, out newDate);
            double sum = 0.0;
            int days = -1;
            for (int i = 0; i < output.Data.Count; i++)
            {
                days += 1;
                string dateString = output.Data.Keys.ElementAt(i).ToString().Substring(0, output.Data.Keys.ElementAt(0).ToString().Length - 1) + ":00:00";
                DateTime.TryParse(dateString, out iDate);
                if (iDate.Month != newDate.Month || i == output.Data.Count - 1)
                {
                    sum = sum / days;
                    output1.Data.Add(newDate.ToString("yyyy-MM-dd HH"), new List<string>() { sum.ToString() });
                    newDate = iDate;
                    sum = Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0]);
                    days = 0;
                    if (errorMsg.Contains("ERROR")) { return null; }
                }
                else
                {
                    sum += Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0]);
                    //days += 1;
                    if (errorMsg.Contains("ERROR")) { return null; }
                }
            }
            return output1.Data;
        }
    }
}
