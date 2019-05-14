using Data;
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

    }
}
