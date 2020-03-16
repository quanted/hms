using Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Services.Controllers;

namespace Web.Services.Models
{
    /// <summary>
    /// HMS Web Service Wind Model
    /// </summary>
    public class WSWind
    {

        //private enum WindSources { nldas }

        /// <summary>
        /// Gets wind data using the given TimeSeriesInput parameters.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ITimeSeriesOutput> GetWind(WindInput input)
        {
            string errorMsg = "";

            // Constructs default error output object containing error message.
            Utilities.ErrorOutput err = new Utilities.ErrorOutput();

            // Validate wind sources.
            //errorMsg = (!Enum.TryParse(input.Source, true, out WindSources pSource)) ? "ERROR: 'Source' was not found or is invalid. Source provided: " + input.Source : "";
            //if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            // Wind object
            Wind.Wind wind = new Wind.Wind();

            // ITimeSeriesInputFactory object used to validate and initialize all variables of the input object.
            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            wind.Input = iFactory.SetTimeSeriesInput(input, new List<string>() { "wind" }, out errorMsg);
            wind.component = input.Component;

            // If error occurs in input validation and setup, errorMsg is added to metadata of an empty object.
            if (errorMsg.Contains("ERROR")) {
                if (!errorMsg.Contains("base url") && wind.Input.Source != "nldas")
                {
                    return err.ReturnError(errorMsg);
                } 
            }

            if (wind.Input.Source.Contains("ncei"))
            {
                wind.Input.Geometry.GeometryMetadata["token"] = (wind.Input.Geometry.GeometryMetadata.ContainsKey("token")) ? wind.Input.Geometry.GeometryMetadata["token"] : "RUYNSTvfSvtosAoakBSpgxcHASBxazzP";
            }

            // Gets the Wind data.
            ITimeSeriesOutput result = wind.GetData(out errorMsg);
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            // Get generic statistics
            result = Utilities.Statistics.GetStatistics(out errorMsg, wind.Input, result);

            return result;
        }
    }
}