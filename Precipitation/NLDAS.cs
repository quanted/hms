using Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Precipitation
{
    /// <summary>
    /// Precipitation NLDAS class
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
            string data = nldas.GetData(out errorMsg, "PRECIP", input);
            if (errorMsg.Contains("ERROR")) { return null; }
            ITimeSeriesOutput setOutput = nldas.SetDataToOutput(out errorMsg, "Precipitation", data, output, input);
            if (errorMsg.Contains("ERROR")) { return null; }
            return setOutput;
        }
    }
}
