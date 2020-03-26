using Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Services.Controllers;

namespace Web.Services.Models
{
    /// <summary>
    /// HMS Web Service Radiation Model
    /// </summary>
    public class WSRadiation
    {

        /// <summary>
        /// Gets radiation data using the given TimeSeriesInput parameters.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ITimeSeriesOutput> GetRadiation(RadiationInput input)
        {
            string errorMsg = "";

            // Constructs default error output object containing error message.
            Utilities.ErrorOutput err = new Utilities.ErrorOutput();

            // Radiation object
            Radiation.Radiation rad = new Radiation.Radiation();

            // ITimeSeriesInputFactory object used to validate and initialize all variables of the input object.
            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            rad.Input = iFactory.SetTimeSeriesInput(input, new List<string>() { "radiation" }, out errorMsg);

            // If error occurs in input validation and setup, errorMsg is added to metadata of an empty object.
            if (errorMsg.Contains("ERROR")) {
                if (!errorMsg.Contains("base url") && (rad.Input.Source != "nldas" || rad.Input.Source != "gldas"))
                {
                    return err.ReturnError(errorMsg);
                } 
            }

            // Gets the Radiation data.
            ITimeSeriesOutput result = rad.GetData(out errorMsg);
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            // Get generic statistics
            result = Utilities.Statistics.GetStatistics(out errorMsg, rad.Input, result);

            return result;
        }
    }
}