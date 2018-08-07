using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Web.Services.Controllers;

namespace Web.Services.Models
{
    /// <summary>
    /// HMS Web Serivce WorkFlow Model
    /// </summary>
    public class WSWatershedWorkFlow
    {
        private enum RunoffSources { nldas, gldas, curvenumber }

        /// <summary>
        /// Gets workflow data.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ITimeSeriesOutput> GetWorkFlowData(WatershedWorkflowInput input)
        {
            string errorMsg = "";
            // Constructs default error output object containing error message.
            Utilities.ErrorOutput err = new Utilities.ErrorOutput();

            // Validate SurfaceRunoff sources.
            errorMsg = (!Enum.TryParse(input.Source, true, out RunoffSources pSource)) ? "ERROR: 'Source' was not found or is invalid." : "";
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            // SurfaceRunoff object
            SurfaceRunoff.SurfaceRunoff runoff = new SurfaceRunoff.SurfaceRunoff()
            {
                CurveSource = input.CurveSource
            };

            // ITimeSeriesInputFactory object used to validate and initialize all variables of the input object.
            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            runoff.Input = iFactory.SetTimeSeriesInput(input, new List<string>() { "SURFFLOW" }, out errorMsg);

            // If error occurs in input validation and setup, errorMsg is added to metadata of an empty object.
            if (errorMsg.Contains("ERROR") && input.Source != "curvenumber") { return err.ReturnError(errorMsg); }

            // Gets the SurfaceRunoff data.
            ITimeSeriesOutput result = runoff.GetData(out errorMsg);
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            if (input.Aggregation)
            {
                // SubSurfaceFlow object
                SubSurfaceFlow.SubSurfaceFlow sub = new SubSurfaceFlow.SubSurfaceFlow();
                input.Source = input.Source == "curvenumber" ? input.CurveSource : input.Source;
                if(input.Source != "nldas" || input.Source != "gldas")
                {
                    input.Source = "nldas";//Subflow source should be same as surface. If surface uses curve number (not available to subflow), default to nldas for sub.
                }
                // ITimeSeriesInputFactory object used to validate and initialize all variables of the input object.
                ITimeSeriesInputFactory subiFactory = new TimeSeriesInputFactory();
                sub.Input = subiFactory.SetTimeSeriesInput(input, new List<string>() { "BASEFLOW" }, out errorMsg);
                // If error occurs in input validation and setup, errorMsg is added to metadata of an empty object.
                if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }
                // Gets the SubSurfaceFlow data.
                ITimeSeriesOutput subResult = sub.GetData(out errorMsg);
                if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

                Utilities.CatchmentAggregation cd = new Utilities.CatchmentAggregation();
                Utilities.GeometryData gd = cd.getData(input, result);
                foreach (var entry in result.Data)
                {
                    string key = entry.Key.ToString();
                    string newval = (Convert.ToDouble(entry.Value[0]) + Convert.ToDouble(subResult.Data[key][0])).ToString();
                    entry.Value[0] = newval;
                }
                ITimeSeriesOutput outs = cd.getCatchmentAggregation(input, result, gd);
                return outs;
            }
            else
            {
                return result;
            }
        }
    }
}