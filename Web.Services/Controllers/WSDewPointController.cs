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
    /// Dew Point Input that implements TimeSeriesInput object
    /// </summary>
    public class DewPointInput : TimeSeriesInput
    {
        // Add extra Dew Point specific variables here
        /// <summary>
        /// Description: DewPoint data source;
        /// Default: "prism";
        /// Options: ["prism"];
        /// Required: True;
        /// </summary>
        public new string Source { get; set; }
    }

    // --------------- Swashbuckle Examples --------------- //

    /// <summary>
    /// Swashbuckle Dew Point POST request example
    /// </summary>
    public class DewPointInputExample : IExamplesProvider<DewPointInput>
    {
        /// <summary>
        /// Get example function.
        /// </summary>
        /// <returns></returns>
        public DewPointInput GetExamples()
        {
            DewPointInput example = new DewPointInput()
            {
                Source = "prism",
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

    // --------------- DewPoint Controller --------------- //

    /// <summary>
    /// DewPoint controller for HMS.
    /// </summary>
    [ApiVersion("0.1")]             // Version 0.1 endpoint
    [Route("api/meteorology/dewpoint")]
    [Produces("application/json")]
    public class WSDewPointController : Controller
    {
        /// <summary>
        /// POST method for submitting a request for dew point temperature data.
        /// </summary>
        /// <param name="tempInput">Parameters for retrieving dew point data. Required fields: DateTimeSpan.StartDate, DateTimeSpan.EndDate, Geometry.Point.Latitude, Geometry.Point.Longitude, Source</param>
        /// <returns>ITimeSeries</returns>
        [HttpPost]
        [ProducesResponseType(200)]
        public async Task<IActionResult> POST([FromBody]DewPointInput dInput)
        {
            try
            {
                ((Data.TimeSeriesInput)dInput).Source = dInput.Source;
                WSDewPoint dPoint = new WSDewPoint();
                ITimeSeriesOutput results = await dPoint.GetDewPoint(dInput);
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
                exceptionLog.Fatal(System.Text.Json.JsonSerializer.Serialize(dInput, options));

                Utilities.ErrorOutput err = new Utilities.ErrorOutput();
                return new ObjectResult(err.ReturnError("Unable to complete request due to invalid request or unknown error."));
            }
        }
    }
}
