using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Utilities;

namespace Web.Services.Models
{
    /// <summary>
    /// HMS Web Service Dew Point Model
    /// </summary>
    public class WSDewPoint
    {

        private enum DPointSources { prism }

        /// <summary>
        /// Gets dew point temperature data using the given TimeSeriesInput parameters.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ITimeSeriesOutput> GetDewPoint(ITimeSeriesInput input)
        {
            string errorMsg = "";

            // Constructs default error output object containing error message.
            Utilities.ErrorOutput err = new Utilities.ErrorOutput();

            // Validate Dew Point sources.
            errorMsg = (!Enum.TryParse(input.Source, true, out DPointSources pSource)) ? "ERROR: 'Source' was not found or is invalid. Source provided: " + input.Source : "";
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            // Dew Point object
            DewPoint.DewPoint dPoint = new DewPoint.DewPoint();

            // ITimeSeriesInputFactory object used to validate and initialize all variables of the input object.
            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            dPoint.Input = iFactory.SetTimeSeriesInput(input, new List<string>() { "dewpoint" }, out errorMsg);

            // If error occurs in input validation and setup, errorMsg is added to metadata of an empty object.
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            // Gets the dew point temperature data.
            ITimeSeriesOutput result = dPoint.GetData(out errorMsg);
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            // Get generic statistics
            result = Utilities.Statistics.GetStatistics(out errorMsg, dPoint.Input, result);

            return result;
        }
    }
}