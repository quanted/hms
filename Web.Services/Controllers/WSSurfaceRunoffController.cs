using Data;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Threading.Tasks;
using Web.Services.Models;
using System.Text.Json;

namespace Web.Services.Controllers
{
    /// <summary>
    /// SurfaceRunoff Input that implements TimeSeriesInput object
    /// </summary>
    public class SurfaceRunoffInput : TimeSeriesInput
    {

        /// <summary>
        /// Description: Surface runoff data source;
        /// Default: "nldas";
        /// Options: ["nldas", "gldas", "curvenumber"];
        /// Required: True;
        /// </summary>
        public new string Source { get; set; }

        /// <summary>
        /// Description: Precipitation data source for Curve Number.;
        /// Default: "nldas";
        /// Options: ["nldas", "gldas", "ncei", "daymet", "prism", "trmm"];
        /// Required: False;
        /// </summary>
        public string PrecipSource { get; set; }

        // Add extra SurfaceRunoff specific variables here
    }

    // --------------- Swashbuckle Examples --------------- //

    /// <summary>
    /// Swashbuckle SurfaceRunoff POST request example
    /// </summary>
    public class SurfaceRunoffInputExample : IExamplesProvider<SurfaceRunoffInput>
    {
        /// <summary>
        /// Get example function.
        /// </summary>
        /// <returns></returns>
        public SurfaceRunoffInput GetExamples()
        {
            SurfaceRunoffInput example = new SurfaceRunoffInput()
            {
                Source = "nldas",
                DateTimeSpan = new DateTimeSpan()
                {
                    StartDate = new DateTime(2015, 01, 01),
                    EndDate = new DateTime(2015, 01, 08)
                },
                Geometry = new TimeSeriesGeometry()
                {
                    Point = new PointCoordinate()
                    {
                        Latitude = 33.925673,
                        Longitude = -83.355723
                    },
                    Timezone = null
                }
            };
            return example;
        }
    }

    // --------------- SurfaceRunoff Controller --------------- //

    /// <summary>
    /// SurfaceRunoff controller for HMS.
    /// </summary>
    [ApiVersion("0.1")]
    [Route("api/hydrology/surfacerunoff/")]
    [Produces("application/json")]
    public class WSSurfaceRunoffController : Controller
    {
        /// <summary>
        /// POST method for submitting a request for surface runoff data.
        /// </summary>
        /// <param name="runoffInput">Parameters for retrieving SurfaceRunoff data. Required fields: DateTimeSpan.StartDate, DateTimeSpan.EndDate, Geometry.Point.Latitude, Geometry.Point.Longitude, Source</param>
        /// <returns>ITimeSeries</returns>
        [HttpPost]
        [ProducesResponseType(200)]
        public async Task<IActionResult> POST([FromBody]SurfaceRunoffInput runoffInput)
        {
            try
            {
                ((Data.TimeSeriesInput)runoffInput).Source = runoffInput.Source;
                if(runoffInput.Geometry.GeometryMetadata == null && runoffInput.PrecipSource != null)
                {
                    runoffInput.Geometry.GeometryMetadata = new System.Collections.Generic.Dictionary<string, string>()
                    {
                        { "precipSource", runoffInput.PrecipSource }
                    };
                }
                WSSurfaceRunoff runoff = new WSSurfaceRunoff();
                ITimeSeriesOutput results = await runoff.GetSurfaceRunoff(runoffInput);
                results.Metadata = Utilities.Metadata.AddToMetadata("request_url", this.Request.Path, results.Metadata);
                return new ObjectResult(results);
            }
            catch (Exception ex)
            {
                var exceptionLog = Log.ForContext("Type", "exception");
                exceptionLog.Fatal(ex.Message);
                exceptionLog.Fatal(ex.StackTrace);
                JsonSerializerOptions options = new JsonSerializerOptions()
                {
                    AllowTrailingCommas = true,
                    PropertyNameCaseInsensitive = true
                };
                exceptionLog.Fatal(System.Text.Json.JsonSerializer.Serialize(runoffInput, options));

                Utilities.ErrorOutput err = new Utilities.ErrorOutput();
                return new ObjectResult(err.ReturnError("Unable to complete request due to invalid request or unknown error."));
            }
        }
    }
}
