using Data;
using System;
using System.Collections.Generic;
using System.Text;
using Precipitation;

namespace SurfaceRunoff
{
    /// <summary>
    /// SurfaceRunoff curve number class.
    /// </summary>
    class CurveNumber
    {

        /// <summary>
        /// GetData function for curvenumber.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public ITimeSeriesOutput GetData(out string errorMsg, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";

            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            // TODO: Add options for different precip inputs
            ITimeSeriesInput precipInput = iFactory.SetTimeSeriesInput(input, new List<string>() { "NLDAS" }, out errorMsg);
            ITimeSeriesOutput precipData = GetPrecipData(out errorMsg, precipInput, output);
            if (errorMsg.Contains("ERROR")) { return null; }

            Data.Simulate.CurveNumber cn = new Data.Simulate.CurveNumber();
            string data = cn.Simulate(out errorMsg, input);
            if (errorMsg.Contains("ERROR")) { return null; }

            ITimeSeriesOutput cnOutput = output;
            cnOutput = cn.SetDataToOutput(out errorMsg, "SurfaceRunoff", data, output, input);
            if (errorMsg.Contains("ERROR")) { return null; }

            //TODO: add temporal resolution function

            return cnOutput;
        }

        /// <summary>
        /// Gets precipitation data.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        private ITimeSeriesOutput GetPrecipData(out string errorMsg, ITimeSeriesInput input, ITimeSeriesOutput output)
        {
            errorMsg = "";

            Precipitation.Precipitation precip = new Precipitation.Precipitation();
            precip.Input = input;
            precip.Output = output;

            if (input.Geometry.GeometryMetadata.ContainsKey("precipSource"))
            {
                switch (input.Geometry.GeometryMetadata["precipSource"])
                {
                    case "nldas":
                        input.Source = "nldas";
                        input.TemporalResolution = "daily";
                        break;
                    case "gldas":
                        input.Source = "gldas";
                        input.TemporalResolution = "daily";
                        break;
                    case "daymet":
                    default:
                        input.Source = "daymet";
                        break;
                }
            }
            else
            {
                input.Source = "daymet";
            }
            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            ITimeSeriesInput tempInput = iFactory.SetTimeSeriesInput(input, new List<string>() { "precip" }, out errorMsg);
            precip.Input = tempInput;
            precip.Output = precip.GetData(out errorMsg);
            if (errorMsg.Contains("ERROR")) { return null; }
            return precip.Output;
        }

    }
}
