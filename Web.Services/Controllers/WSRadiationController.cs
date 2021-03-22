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
    /// Label: Solar Radiation;
    /// Description: High and short wave radiation that reaches the ground.
    /// </summary>
    public class RadiationInput : TimeSeriesInput
    {
        // Add extra radiation specific variables here
        /// <summary>
        /// Description: Radiation data source;
        /// Default: "nldas";
        /// Options: ["nldas", "gldas", "daymet"];
        /// Required: True;
        /// </summary>
        public new string Source { get; set; }
    }

    // --------------- Swashbuckle Examples --------------- //

    /// <summary>
    /// Swashbuckle Radiation POST request example
    /// </summary>
    public class RadiationInputExample : IExamplesProvider<RadiationInput>
    {
        /// <summary>
        /// Get example function.
        /// </summary>
        /// <returns></returns>
        public RadiationInput GetExamples()
        {
            RadiationInput example = new RadiationInput()
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

    // --------------- Radiation Controller --------------- //

    /// <summary>
    /// Radiation controller for HMS.
    /// </summary>
    [ApiVersion("0.1")]             // Version 0.1 endpoint
    [Route("api/meteorology/radiation")]
    [Produces("application/json")]
    public class WSRadiationController : Controller
    {
        /// <summary>
        /// POST method for submitting a request for radiation data.
        /// </summary>
        /// <param name="tempInput">Parameters for retrieving radiation data. Required fields: DateTimeSpan.StartDate, DateTimeSpan.EndDate, Geometry.Point.Latitude, Geometry.Point.Longitude, Source</param>
        /// <returns>ITimeSeries</returns>
        [HttpPost]
        [ProducesResponseType(200)]
        public async Task<IActionResult> POST([FromBody]RadiationInput tempInput)
        {
            try
            {
                ((Data.TimeSeriesInput)tempInput).Source = tempInput.Source;
                WSRadiation rad = new WSRadiation();
                ITimeSeriesOutput results = await rad.GetRadiation(tempInput);
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
                exceptionLog.Fatal(System.Text.Json.JsonSerializer.Serialize(tempInput, options));

                Utilities.ErrorOutput err = new Utilities.ErrorOutput();
                return new ObjectResult(err.ReturnError("Unable to complete request due to invalid request or unknown error."));
            }
        }
    }
}
