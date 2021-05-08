using System;
using Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Web.Services.Models
{
    /// <summary>
    /// Web Services Model object class for all Coastal requests.
    /// </summary>
    public class WSCoastal
    {
        private enum CoastalSources { noaa_coastal };

        /// <summary>
        /// Gets coastal data using the given TimeSeriesInput parameters.
        /// </summary>
        /// <param name="input">ITimeSeriesInput</param>
        /// <returns></returns>
        public async Task<ITimeSeriesOutput> GetCoastal(TimeSeriesInput input)
        {
            string errorMsg;

            // Constructs default error output object containing error message.
            Utilities.ErrorOutput err = new Utilities.ErrorOutput();

            // Validate coastal sources.
            errorMsg = (!Enum.TryParse(input.Source, true, out CoastalSources cSource)) ? "ERROR: 'Source' was not found or is invalid." : "";
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            // Coastal object
            Coastal.Coastal coast = new Coastal.Coastal();

            // ITimeSeriesInputFactory object used to validate and initialize all variables of the input object.
            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            coast.Input = iFactory.SetTimeSeriesInput(input, new List<string>() { "coastal" }, out errorMsg);
            // If error occurs in input validation and setup, errorMsg is added to metadata of an empty object.
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            // Gets the coastal data.
            ITimeSeriesOutput result = coast.GetData(out errorMsg);
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            // Get generic statistics
            result = Utilities.Statistics.GetStatistics(out errorMsg, coast.Input, result);
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            return result;
        }
    }
}
