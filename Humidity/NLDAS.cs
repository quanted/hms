using Data;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Humidity
{
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
            bool validInputs = ValidateInputs(input, out errorMsg);
            if (!validInputs) { return null; }

            Data.Source.NLDAS nldas = new Data.Source.NLDAS();
            string data = nldas.GetData(out errorMsg, "humidity", input);


            ITimeSeriesOutput nldasOutput = nldas.SetDataToOutput(out errorMsg, "Specific Humidity", data, output, input);

            switch (input.TemporalResolution)
            {
                case "monthly":
                    nldasOutput.Data = DailyValues(out errorMsg, 1.0, output, input);
                    nldasOutput.Data = MonthlyAverage(out errorMsg, 23, 1.0, output, input);
                    break;
                case "daily":
                case "default":
                default:
                    nldasOutput.Data = DailyValues(out errorMsg, 1.0, output, input);
                    break;
            }
            nldasOutput.Metadata["column_1"] = "date";
            nldasOutput.Metadata["column_2"] = "specifichumidity";
            nldasOutput.Metadata["column_2_units"] = "kg/kg";

            return nldasOutput;
        }

        /// <summary>
        /// Daily aggregated sums for precipitation data.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public static Dictionary<string, List<string>> DailyValues(out string errorMsg, double modifier, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";
            int hours = 23;
            // Unit conversion coefficient
            double unit = (input.Units.Contains("imperial")) ? 0.0393701 : 1.0;
            if (input.Units.Contains("imperial")) { output.Metadata["nldas_unit"] = "in"; }

            if (output.Data.Keys.Last().Contains(" 00"))
            {
                output.Data.Remove(output.Data.Keys.Last());
            }
            // Daily aggregation using MathNet Matrix multiplication (results in ~25% speedup)
            hours += 1;
            double minSH = 0.0;
            double maxSH = 0.0;
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
                var x = matrix.ToRowArrays();
                                
                var tempKeys = output.Data.Keys;
                int j = 0;
                if (index == 0)
                {
                    for (int i = 0; i < output.Data.Count; i += hours)
                    {
                        minSH = MathNet.Numerics.Statistics.Statistics.Minimum(x[j]);
                        maxSH = MathNet.Numerics.Statistics.Statistics.Maximum(x[j]);
                        tempData.Add(tempKeys.ElementAt(i).Replace(" 01", " 00"), new List<string> { (unit * minSH).ToString(input.DataValueFormat), (unit * maxSH).ToString(input.DataValueFormat) });
                        j++;
                    }
                }
                else
                {
                    for (int i = 0; i < output.Data.Count; i += hours)
                    {
                        tempData[tempKeys.ElementAt(i).Replace(" 01", " 00")].Add((unit * minSH).ToString(input.DataValueFormat));
                        j++;
                    }
                }
            }
            return tempData;
        }

        /// <summary>
        /// Monthly average for humidity data.
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
            List<double> humids = new List<double>();
            int days = -1;
            for (int i = 0; i < output.Data.Count; i++)
            {
                days += 1;
                string dateString = output.Data.Keys.ElementAt(i).ToString().Substring(0, output.Data.Keys.ElementAt(0).ToString().Length - 1) + ":00:00";
                DateTime.TryParse(dateString, out iDate);
                if (iDate.Month != newDate.Month || i == output.Data.Count - 1)
                {
                    output1.Data.Add(newDate.ToString("yyyy-MM-dd HH"), new List<string>() { humids.Min().ToString(input.DataValueFormat), humids.Max().ToString(input.DataValueFormat) });
                    newDate = iDate;
                    humids.Clear();
                    humids.Add(Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0]));
                    humids.Add(Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][1]));
                    days = 0;
                    if (errorMsg.Contains("ERROR")) { return null; }
                }
                else
                {
                    humids.Add(Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0]));
                    humids.Add(Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][1]));
                    if (errorMsg.Contains("ERROR")) { return null; }
                }
            }
            return output1.Data;
        }

        /// <summary>
        /// Validate input dates and coordinates for precipitation nldas data.
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
            // NDLAS date range 1979-01-01T13 - Present(- 5 days)
            DateTime date0 = new DateTime(1979, 1, 1);
            DateTime date1 = DateTime.Now.AddDays(-5);
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
            // NLDAS spatial range 125W ~ 63E, 25S ~ 53N
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
