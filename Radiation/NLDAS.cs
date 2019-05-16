﻿using Data;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radiation
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
        public ITimeSeriesOutput GetData(out string errorMsg, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";
            bool validInputs = ValidateInputs(input, out errorMsg);
            if (errorMsg.Contains("ERROR")) { return null; }

            this.timeseriesData = new Dictionary<string, ITimeSeriesOutput>();
            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput output1 = oFactory.Initialize();
            ITimeSeriesOutput output2 = oFactory.Initialize();
            this.GetLongwaveComponent(out errorMsg, input, output1);
            this.GetShortwaveComponent(out errorMsg, input, output2);
            output = Utilities.Merger.MergeTimeSeries(this.timeseriesData["longwave"], this.timeseriesData["shortwave"]);

            output.Dataset = "DW Radiation";
            output.DataSource = "nldas";

            switch (input.TemporalResolution)
            {
                case "daily":
                    output.Data = DailyAverage(out errorMsg, 23, 1.0, output, input);
                    break;
                case "hourly":
                case "default":
                default:
                    break;
            }
            output.Metadata["column_1"] = "date";
            output.Metadata["column_2"] = "longwave";
            output.Metadata["column_3"] = "shortwave";
            output.Metadata["column_2_units"] = "W/m^2";
            output.Metadata["column_3_units"] = "W/m^2";

            return output;

        }

        private void GetLongwaveComponent(out string errorMsg, ITimeSeriesInput input, ITimeSeriesOutput output)
        {
            string title = "DW Longwave";
            input.BaseURL = new List<string>() { Data.TimeSeriesInputFactory.GetBaseURL(input.Source, "longwave_radiation") };
            input.Source = title;
            Data.Source.NLDAS nldas = new Data.Source.NLDAS();
            string data = nldas.GetData(out errorMsg, title, input);
            if (errorMsg.Contains("ERROR")) { return; }

            ITimeSeriesOutput nldasOutput = output.Clone();
            nldasOutput = nldas.SetDataToOutput(out errorMsg, title, data, output, input);
            if (errorMsg.Contains("ERROR")) { return; }

            this.timeseriesData.Add("longwave", nldasOutput);
            if (errorMsg.Contains("ERROR")) { return; }
        }

        private void GetShortwaveComponent(out string errorMsg, ITimeSeriesInput input, ITimeSeriesOutput output)
        {
            string title = "DW Shortwave";
            ITimeSeriesInput tempInput = input.Clone(new List<string>() { "radiation" });
            tempInput.BaseURL = new List<string>() { Data.TimeSeriesInputFactory.GetBaseURL("nldas", "shortwave_radiation") };
            tempInput.Source = title;
            Data.Source.NLDAS nldas = new Data.Source.NLDAS();
            string data = nldas.GetData(out errorMsg, title, tempInput);
            if (errorMsg.Contains("ERROR")) { return; }

            ITimeSeriesOutput nldasOutput = output.Clone();
            nldasOutput = nldas.SetDataToOutput(out errorMsg, title, data, output, tempInput);
            if (errorMsg.Contains("ERROR")) { return; }

            this.timeseriesData.Add("shortwave", nldasOutput);
            if (errorMsg.Contains("ERROR")) { return; }
        }


        /// <summary>
        /// Daily average for radiation data.
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
            if (input.Geometry.Point.Latitude < -25 && input.Geometry.Point.Latitude > 53)
            {
                errors.Add("ERROR: Latitude is not valid. Latitude must be between -25 and 53. Latitude provided: " + input.Geometry.Point.Latitude.ToString());
            }
            if (input.Geometry.Point.Longitude < -125 && input.Geometry.Point.Longitude > 63)
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