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
    /// HMS Web Service Temperature Model
    /// </summary>
    public class WSTemperature
    {

        private enum TempSources { nldas, gldas, daymet, prism }

        /// <summary>
        /// Gets temperature data using the given TimeSeriesInput parameters.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ITimeSeriesOutput> GetTemperature(ITimeSeriesInput input)
        {
            string errorMsg = "";

            // Constructs default error output object containing error message.
            Utilities.ErrorOutput err = new Utilities.ErrorOutput();

            // Validate evapotranspiration sources.
            errorMsg = (!Enum.TryParse(input.Source, true, out TempSources pSource)) ? "ERROR: 'Source' was not found or is invalid. Source provided: " + input.Source : "";
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            // Temperature object
            Temperature.Temperature temp = new Temperature.Temperature();

            // ITimeSeriesInputFactory object used to validate and initialize all variables of the input object.
            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            temp.Input = iFactory.SetTimeSeriesInput(input, new List<string>() { "temperature" }, out errorMsg);

            // If error occurs in input validation and setup, errorMsg is added to metadata of an empty object.
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            // Gets the Temperature data.
            ITimeSeriesOutput result = temp.GetData(out errorMsg);
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            // Get generic statistics
            result = Utilities.Statistics.GetStatistics(out errorMsg, temp.Input, result);

            return result;
        }
    }
}