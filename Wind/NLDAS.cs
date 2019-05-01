using Data;
using System;
using System.Collections.Generic;
using System.Text;

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
            //TODO: Add spatial aggregation options

            return output;

        }


        private void GetUComponent(out string errorMsg, ITimeSeriesInput input, ITimeSeriesOutput output)
        {
            input.BaseURL = new List<string>() { Data.TimeSeriesInputFactory.GetBaseURL(input.Source, "u_wind") };
            input.Source = "U Wind";
            Data.Source.NLDAS nldas = new Data.Source.NLDAS();
            string data = nldas.GetData(out errorMsg, "U Wind", input);
            if (errorMsg.Contains("ERROR")) { return; }

            ITimeSeriesOutput nldasOutput = output.Clone();
            nldasOutput = nldas.SetDataToOutput(out errorMsg, "U Wind", data, output, input);
            if (errorMsg.Contains("ERROR")) { return; }

            this.timeseriesData.Add("u", nldasOutput);
            if (errorMsg.Contains("ERROR")) { return; }
        }

        private void GetVComponent(out string errorMsg, ITimeSeriesInput input, ITimeSeriesOutput output)
        {
            ITimeSeriesInput tempInput = input.Clone(new List<string>() { "wind" });
            tempInput.BaseURL = new List<string>() { Data.TimeSeriesInputFactory.GetBaseURL("nldas", "v_wind") };
            tempInput.Source = "V Wind";
            Data.Source.NLDAS nldas = new Data.Source.NLDAS();
            string data = nldas.GetData(out errorMsg, "V Wind", tempInput);
            if (errorMsg.Contains("ERROR")) { return; }

            ITimeSeriesOutput nldasOutput = output.Clone();
            nldasOutput = nldas.SetDataToOutput(out errorMsg, "V Wind", data, output, tempInput);
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
                    double deg = 180 + (180 / Math.PI) * Math.Atan2(u, v);
                    timeseries.Add(date, new List<string>() { vel.ToString("E3"), deg.ToString("N3") });
                }
                output.Metadata["column_1"] = "date";
                output.Metadata["column_2"] = "velocity";
                output.Metadata["column_3"] = "direction";
                output.Metadata["column_2_units"] = "m/s";
                output.Metadata["column_3_units"] = "deg";
            }
            else
            {
                foreach (string date in this.timeseriesData["u"].Data.Keys)
                {
                    double u = Double.Parse(this.timeseriesData["u"].Data[date][0]);
                    double v = Double.Parse(this.timeseriesData["v"].Data[date][0]);
                    double vel = Math.Sqrt(Math.Pow(u, 2) + Math.Pow(v, 2));
                    double deg = 180 + (180 / Math.PI) * Math.Atan2(u, v);
                    timeseries.Add(date, new List<string>() { v.ToString("E3"), u.ToString("E3"), vel.ToString("E3"), deg.ToString("N3") });
                }
                output.Metadata["column_1"] = "date";
                output.Metadata["column_2"] = "v";
                output.Metadata["column_3"] = "u";
                output.Metadata["column_4"] = "velocity";
                output.Metadata["column_5"] = "direction";
                output.Metadata["column_2_units"] = "m/s";
                output.Metadata["column_3_units"] = "m/s";
                output.Metadata["column_4_units"] = "m/s";
                output.Metadata["column_5_units"] = "deg";
            }
            output.Data = timeseries;
        }

    }
}
