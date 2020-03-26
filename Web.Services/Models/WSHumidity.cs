using Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Services.Controllers;

namespace Web.Services.Models
{
    /// <summary>
    /// HMS Web Service Humidity Model
    /// </summary>
    public class WSHumidity
    {

        private enum HumidSource { prism }

        /// <summary>
        /// Gets humidty data using the given TimeSeriesInput parameters.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ITimeSeriesOutput> GetHumidity(HumidityInput input)
        {
            string errorMsg = "";

            // Constructs default error output object containing error message.
            Utilities.ErrorOutput err = new Utilities.ErrorOutput();

            // Validate humidity sources.
            errorMsg = (!Enum.TryParse(input.Source, true, out HumidSource pSource)) ? "ERROR: 'Source' was not found or is invalid. Source provided: " + input.Source : "";
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            // Humidity object
            Humidity.Humidity humidity = new Humidity.Humidity(input.Relative);

            // ITimeSeriesInputFactory object used to validate and initialize all variables of the input object.
            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            humidity.Input = iFactory.SetTimeSeriesInput(input, new List<string>() { "humidity" }, out errorMsg);

            // If error occurs in input validation and setup, errorMsg is added to metadata of an empty object.
            if (errorMsg.Contains("ERROR") && !errorMsg.Contains("base url")){ return err.ReturnError(errorMsg); }

            // Gets the Humidity data.
            ITimeSeriesOutput result = humidity.GetData(out errorMsg);
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            // Get generic statistics
            result = Utilities.Statistics.GetStatistics(out errorMsg, humidity.Input, result);

            return result;
        }
    }
}