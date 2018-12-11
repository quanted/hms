using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Web.Services.Models
{
    /// <summary>
    /// HMS Web Service Precipitation Model
    /// </summary>
    public class WSPrecipitation
    {

        private enum PrecipSources{ nldas, gldas, ncei, daymet, wgen, prism };

        /// <summary>
        /// Gets precipitation data using the given TimeSeriesInput parameters.
        /// </summary>
        /// <param name="input">ITimeSeriesInput</param>
        /// <returns></returns>
        public async Task<ITimeSeriesOutput> GetPrecipitation(ITimeSeriesInput input)
        {
            string errorMsg = "";
            
            // Constructs default error output object containing error message.
            Utilities.ErrorOutput err = new Utilities.ErrorOutput();

            // Validate precipitation sources.
            if(input.Source == "ncdc") { input.Source = "ncei"; }
            errorMsg = (!Enum.TryParse(input.Source, true, out PrecipSources pSource)) ? "ERROR: 'Source' was not found or is invalid.": "";
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            // Precipitation object
            Precipitation.Precipitation precip = new Precipitation.Precipitation();
            
            // ITimeSeriesInputFactory object used to validate and initialize all variables of the input object.
            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            precip.Input = iFactory.SetTimeSeriesInput(input, new List<string>() { "precipitation" }, out errorMsg);

            // If error occurs in input validation and setup, errorMsg is added to metadata of an empty object.
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }           

            if (precip.Input.Source.Contains("ncei"))
            {
                precip.Input.Geometry.GeometryMetadata["token"] = (precip.Input.Geometry.GeometryMetadata.ContainsKey("token")) ? precip.Input.Geometry.GeometryMetadata["token"] : "RUYNSTvfSvtosAoakBSpgxcHASBxazzP";
            }

            // Gets the Precipitation data.
            ITimeSeriesOutput result = precip.GetData(out errorMsg);
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            return result;
        }
    }
}