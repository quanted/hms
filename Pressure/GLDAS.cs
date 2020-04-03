using Data;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pressure
{
    public class GLDAS
    {

        /// <summary>
        /// Makes the GetData call to the base GLDAS class.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public ITimeSeriesOutput GetData(out string errorMsg, ITimeSeriesOutput output0, ITimeSeriesInput input)
        {
            errorMsg = "";
            bool validInputs = ValidateInputs(input, out errorMsg);
            //if (!validInputs) { return null; }

            Data.Source.GLDAS gldas = new Data.Source.GLDAS();
            List<string> data = gldas.GetData(out errorMsg, "PSURF", input);
            if(data == null)
            {
                data = new List<string>();
            }

            ITimeSeriesOutput output = gldas.SetDataToOutput(out errorMsg, "Surface Air Pressure", data, output0, input);

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
            output.Metadata["column_2"] = "psurf";
            output.Metadata["column_2_units"] = "Pa";

            return output;
        }

        private Boolean ValidateInputs(ITimeSeriesInput input, out string errorMsg)
        {
            errorMsg = "";
            List<string> errors = new List<string>();
            bool valid = true;
            // Validate Date range
            // GLDAS 2.0 date range 1948-01-01 - 2010-12-31
            DateTime date0 = new DateTime(1948, 1, 1);
            DateTime tempDate = DateTime.Now;
            DateTime date1 = new DateTime(tempDate.Year, tempDate.Month, tempDate.Day).AddDays(-5);
            //DateTime date1 = new DateTime(2010, 12, 31);
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
            // GLDAS 2.0 spatial range 180W ~ 180E, 60S ~ 90N
            if (input.Geometry.Point.Latitude < -60 && input.Geometry.Point.Latitude > 90)
            {
                errors.Add("ERROR: Latitude is not valid. Latitude must be between -60 and 90. Latitude provided: " + input.Geometry.Point.Latitude.ToString());
            }
            if (input.Geometry.Point.Longitude < -180 && input.Geometry.Point.Longitude > 180)
            {
                errors.Add("ERROR: Longitude is not valid. Longitude must be between -180 and 180. Longitude provided: " + input.Geometry.Point.Longitude.ToString());
            }

            if (errors.Count > 0)
            {
                valid = false;
                errorMsg = String.Join(", ", errors.ToArray());
            }

            return valid;
        }


        /// <summary>
        /// Daily average for pressure data.
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
        /// Monthly average for pressure data.
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
            double press = 0.0;
            int days = -1;
            for (int i = 0; i < output.Data.Count; i++)
            {
                days += 1;
                string dateString = output.Data.Keys.ElementAt(i).ToString().Substring(0, output.Data.Keys.ElementAt(0).ToString().Length - 1) + ":00:00";
                DateTime.TryParse(dateString, out iDate);
                if (iDate.Month != newDate.Month || i == output.Data.Count - 1)
                {
                    output1.Data.Add(newDate.ToString("yyyy-MM-dd HH"), new List<string>() { (press / days).ToString(input.DataValueFormat) });
                    newDate = iDate;
                    press = Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0]);
                    days = 0;
                    if (errorMsg.Contains("ERROR")) { return null; }
                }
                else
                {
                    press += Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0]);
                    if (errorMsg.Contains("ERROR")) { return null; }
                }
            }
            return output1.Data;
        }
    }
}
