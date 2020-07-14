using Data;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Wind
{
    public class NLDAS
    {

        private Dictionary<string, ITimeSeriesOutput> timeseriesData;


        /// <summary>
        /// Makes the GetData call to the base NLDAS class.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public ITimeSeriesOutput GetData(out string errorMsg, string product, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";
            bool validInputs = ValidateInputs(input, out errorMsg);
            if (!validInputs) { return null; }

            this.timeseriesData = new Dictionary<string, ITimeSeriesOutput>();
            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput output1 = oFactory.Initialize();
            ITimeSeriesOutput output2 = oFactory.Initialize();
            this.GetUComponent(out errorMsg, input, output1);
            this.GetVComponent(out errorMsg, input, output2);

            switch (product.ToUpper())
            {
                case "U/V":
                case "V/U":
                    output = Utilities.Merger.MergeTimeSeries(this.timeseriesData["u"], this.timeseriesData["v"]);
                    break;
                case "S/D":
                case "SPEED/DIR":
                case "SPEED/DIRECTION":
                    CalculateWindspeedDir(false, output);
                    break;
                case "ALL":
                default:
                    CalculateWindspeedDir(true, output);
                    break;
            }
            output.Dataset = "Wind";
            output.DataSource = "nldas";

            switch (input.TemporalResolution)
            {
                case "daily":
                    output.Data = DailyAverage(out errorMsg, 23, 1.0, output, input);
                    break;
                case "weekly":
                    //output.Data = WeeklyAverage(out errorMsg, 23, 1.0, output, input);
                    return output;
                case "monthly":
                    output.Data = DailyAverage(out errorMsg, 23, 1.0, output, input);
                    output.Data = MonthlyAverage(out errorMsg, 23, 1.0, output, input);
                    return output;
                case "yearly":
                    //output.Data = YearlyAverage(out errorMsg, 23, 1.0, output, input);
                    return output;
                case "hourly":
                default:
                    break;
            }

            return output;

        }

        private Boolean ValidateInputs(ITimeSeriesInput input, out string errorMsg)
        {
            errorMsg = "";
            List<string> errors = new List<string>();
            bool valid = true;
            // Validate Date range
            // NLDAS 2.0 date range 1948-01-01 - 2010-12-31
            DateTime date0 = new DateTime(1948, 1, 1);
            DateTime tempDate = DateTime.Now;
            DateTime date1 = new DateTime(tempDate.Year, tempDate.Month, tempDate.Day).AddDays(-6);
            string dateFormat = "yyyy-MM-dd";
            if (DateTime.Compare(input.DateTimeSpan.StartDate, date0) < 0 || (DateTime.Compare(input.DateTimeSpan.StartDate, date1) > 0))
            {
                errors.Add("ERROR: Start date is not valid. Date must be between " + date0.ToString(dateFormat) + " and " + date1.ToString(dateFormat) + ". Start date provided: " + input.DateTimeSpan.StartDate.ToString(dateFormat));
            }
            if (DateTime.Compare(input.DateTimeSpan.EndDate, date0) < 0 || DateTime.Compare(input.DateTimeSpan.EndDate, date1) > 0)
            {
                errors.Add("ERROR: End date is not valid. Date must be between " + date0.ToString(dateFormat) + " and " + date1.ToString(dateFormat) + ". End date provided: " + input.DateTimeSpan.EndDate.ToString(dateFormat));
            }

            if (errors.Count > 0)
            {
                valid = false;
                errorMsg = String.Join(", ", errors.ToArray());
            }

            return valid;
        }

        private void GetUComponent(out string errorMsg, ITimeSeriesInput input, ITimeSeriesOutput output)
        {
            input.BaseURL = new List<string>() { Data.TimeSeriesInputFactory.GetBaseURL(input.Source, "u_wind") };
            input.Source = "Zonal Wind";
            Data.Source.NLDAS nldas = new Data.Source.NLDAS();
            string data = nldas.GetData(out errorMsg, "Zonal Wind", input);
            if (errorMsg.Contains("ERROR")) { return; }

            ITimeSeriesOutput nldasOutput = output.Clone();
            nldasOutput = nldas.SetDataToOutput(out errorMsg, "Zonal Wind", data, output, input);
            if (errorMsg.Contains("ERROR")) { return; }

            this.timeseriesData.Add("u", nldasOutput);
            if (errorMsg.Contains("ERROR")) { return; }
        }

        private void GetVComponent(out string errorMsg, ITimeSeriesInput input, ITimeSeriesOutput output)
        {
            ITimeSeriesInput tempInput = input.Clone(new List<string>() { "wind" });
            tempInput.BaseURL = new List<string>() { Data.TimeSeriesInputFactory.GetBaseURL("nldas", "v_wind") };
            tempInput.Source = "Meridional Wind";
            Data.Source.NLDAS nldas = new Data.Source.NLDAS();
            string data = nldas.GetData(out errorMsg, "Meridional Wind", tempInput);
            if (errorMsg.Contains("ERROR")) { return; }

            ITimeSeriesOutput nldasOutput = output.Clone();
            nldasOutput = nldas.SetDataToOutput(out errorMsg, "Meridional Wind", data, output, tempInput);
            if (errorMsg.Contains("ERROR")) { return; }

            this.timeseriesData.Add("v", nldasOutput);
            if (errorMsg.Contains("ERROR")) { return; }
        }

        private void CalculateWindspeedDir(bool all, ITimeSeriesOutput output)
        {

            Dictionary<string, List<string>> timeseries = new Dictionary<string, List<string>>();
            if (!all)
            {
                foreach (string date in this.timeseriesData["u"].Data.Keys)
                {
                    double u = Double.Parse(this.timeseriesData["u"].Data[date][0]);
                    double v = Double.Parse(this.timeseriesData["v"].Data[date][0]);
                    double vel = Math.Sqrt(Math.Pow(u, 2) + Math.Pow(v, 2));
                    //double deg = 180 + (180 / Math.PI) * Math.Atan2(u, v);
                    //timeseries.Add(date, new List<string>() { vel.ToString("E3"), deg.ToString("N3") });
                    timeseries.Add(date, new List<string>() { vel.ToString("E3") });
                }
                output.Metadata["column_1"] = "date";
                output.Metadata["column_2"] = "speed";
                //output.Metadata["column_3"] = "direction";
                output.Metadata["column_2_units"] = "m/s";
                //output.Metadata["column_3_units"] = "deg";
            }
            else
            {
                foreach (string date in this.timeseriesData["u"].Data.Keys)
                {
                    double u = Double.Parse(this.timeseriesData["u"].Data[date][0]);
                    double v = Double.Parse(this.timeseriesData["v"].Data[date][0]);
                    double vel = Math.Sqrt(Math.Pow(u, 2) + Math.Pow(v, 2));
                    //double deg = 180 + (180 / Math.PI) * Math.Atan2(u, v);
                    //timeseries.Add(date, new List<string>() { v.ToString("E3"), u.ToString("E3"), vel.ToString("E3"), deg.ToString("N3") });
                    timeseries.Add(date, new List<string>() { v.ToString("E3"), u.ToString("E3"), vel.ToString("E3"), });
                }
                output.Metadata["column_1"] = "date";
                output.Metadata["column_2"] = "meridional_wind";
                output.Metadata["column_3"] = "zonal_wind";
                output.Metadata["column_4"] = "speed";
                //output.Metadata["column_5"] = "direction";
                output.Metadata["column_2_units"] = "m/s";
                output.Metadata["column_3_units"] = "m/s";
                output.Metadata["column_4_units"] = "m/s";
                //output.Metadata["column_5_units"] = "deg";
            }
            output.Data = timeseries;
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
            double usum = 0.0;
            double vsum = 0.0;
            double velsum = 0.0;
            int days = -1;
            for (int i = 0; i < output.Data.Count; i++)
            {
                days += 1;
                string dateString = output.Data.Keys.ElementAt(i).ToString().Substring(0, output.Data.Keys.ElementAt(0).ToString().Length - 1) + ":00:00";
                DateTime.TryParse(dateString, out iDate);
                if (iDate.Month != newDate.Month || i == output.Data.Count - 1)
                {
                    usum = usum / days;
                    vsum = vsum / days;
                    velsum = velsum / days;
                    output1.Data.Add(newDate.ToString("yyyy-MM-dd HH"), new List<string>() { usum.ToString(), vsum.ToString(), velsum.ToString() } );
                    newDate = iDate;
                    usum = Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0]);
                    vsum = Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][1]);
                    velsum = Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][2]);
                    days = 0;
                    if (errorMsg.Contains("ERROR")) { return null; }
                }
                else
                {
                    usum += Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0]);
                    vsum += Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][1]);
                    velsum += Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][2]);
                    //days += 1;
                    if (errorMsg.Contains("ERROR")) { return null; }
                }
            }
            return output1.Data;
        }
    }
}
