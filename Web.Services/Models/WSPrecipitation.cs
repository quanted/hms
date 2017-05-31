using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Services.Models
{
    public class WSPrecipitation
    {

        public ITimeSeries GetPrecipitation(out string errorMsg, ITimeSeriesInput input)
        {

            errorMsg = "";
            Utilities.ErrorOutput err = new Utilities.ErrorOutput();

            Precipitation.Precipitation precip = new Precipitation.Precipitation(input);
            ITimeSeries result = precip.GetData(out errorMsg);
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }
            return result;
        }
    }
}