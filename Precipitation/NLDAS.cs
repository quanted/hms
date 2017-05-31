using Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Precipitation
{
    class NLDAS
    {

        public ITimeSeries GetData(out string errorMsg, ITimeSeries output, ITimeSeriesInput input)
        {
            errorMsg = "";
            Data.Source.NLDAS nldas = new Data.Source.NLDAS();
            string data = nldas.GetData(out errorMsg, input);
            if (errorMsg.Contains("ERROR")) { return null; }
            ITimeSeries setOutput = nldas.SetDataToOutput(out errorMsg, "Precipitation", data, output, input);
            if (errorMsg.Contains("ERROR")) { return null; }
            return setOutput;
        }
    }
}
