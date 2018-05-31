using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Web.Services.Models
{
    /// <summary>
    /// HMS Web Service SubSurfaceFlow Model
    /// </summary>
    public class WSSubSurfaceFlow
    {

        private enum SSFlowSources { nldas, gldas }

        /// <summary>
        /// Gets subsurfaceflow data using the given TimeSeriesInput parameters.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ITimeSeriesOutput> GetSubSurfaceFlow(ITimeSeriesInput input)
        {
            string errorMsg = "";

            // Constructs default error output object containing error message.
            Utilities.ErrorOutput err = new Utilities.ErrorOutput();

            // Validate subsurfaceflow sources.
            errorMsg = (!Enum.TryParse(input.Source, true, out SSFlowSources pSource)) ? "ERROR: 'Source' was not found or is invalid." : "";
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            // SubSurfaceFlow object
            SubSurfaceFlow.SubSurfaceFlow evapo = new SubSurfaceFlow.SubSurfaceFlow();

            // ITimeSeriesInputFactory object used to validate and initialize all variables of the input object.
            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            evapo.Input = iFactory.SetTimeSeriesInput(input, new List<string>() { "BASEFLOW" }, out errorMsg);

            // If error occurs in input validation and setup, errorMsg is added to metadata of an empty object.
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            // Gets the SubSurfaceFlow data.
            ITimeSeriesOutput result = evapo.GetData(out errorMsg);
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            return result;
        }
    }
}