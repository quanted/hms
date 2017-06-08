using Data;
using System;
using System.Collections.Generic;
using System.Text;

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
            Data.Simulate.CurveNumber cn = new Data.Simulate.CurveNumber();
            string data = cn.Simulate(out errorMsg, input);
            if (errorMsg.Contains("ERROR")) { return null; }

            ITimeSeriesOutput cnOutput = output;
            cnOutput = cn.SetDataToOutput(out errorMsg, "SurfaceRunoff", data, output, input);
            if (errorMsg.Contains("ERROR")) { return null; }

            //TODO: add temporal resolution function

            return cnOutput;
        }
    }
}
