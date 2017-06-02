using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Services.Models
{
    /// <summary>
    /// HMS Web Service Precipitation Model
    /// </summary>
    public class WSPrecipitation
    {
        /// <summary>
        /// Gets precipitation data using the give TimeSeriesInput parameters.
        /// </summary>
        /// <param name="input">ITimeSeriesInput</param>
        /// <returns></returns>
        public ITimeSeriesOutput GetPrecipitation(ITimeSeriesInput input)
        {
            string errorMsg = "";
            
            // Constructs default error output object containing error message.
            Utilities.ErrorOutput err = new Utilities.ErrorOutput();

            // Precipitation object
            Precipitation.Precipitation precip = new Precipitation.Precipitation();
            
            // ITimeSeriesInputFactory object used to validate and initialize all variables of the input object.
            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            precip.Input = iFactory.SetTimeSeriesInput(input, out errorMsg);

            // If error occurs in input validation and setup, errorMsg is added to metadata of an empty object.
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }           

            // Gets the Precipitation data.
            ITimeSeriesOutput result = precip.GetData(out errorMsg);
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            return result;
        }
    }
}