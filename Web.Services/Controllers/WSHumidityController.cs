using Data;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Threading.Tasks;
using Web.Services.Models;

namespace Web.Services.Controllers
{
    /// <summary>
    /// Humidity Input that implements TimeSeriesInput object
    /// </summary>
    public class HumidityInput : TimeSeriesInput
    {
        // Add extra Humidity specific variables here
        /// <summary>
        /// Relative or Specific Humidity
        /// </summary>
        public bool Relative { get; set; }
    }

    // --------------- Swashbuckle Examples --------------- //

    /// <summary>
    /// Swashbuckle Humidity POST request example
    /// </summary>
    public class HumidityInputExample : IExamplesProvider<HumidityInput>
    {
        /// <summary>
        /// Get example function.
        /// </summary>
        /// <returns></returns>
        public HumidityInput GetExamples()
        {
            HumidityInput example = new HumidityInput()
            {
                Source = "prism",
                Relative = true,
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

    // --------------- Humidity Controller --------------- //

    /// <summary>
    /// Humidity controller for HMS.
    /// </summary>
    [ApiVersion("0.1")]             // Version 0.1 endpoint
    [Route("api/meteorology/humidity")]
    [Produces("application/json")]
    public class WSHumidityController : Controller
    {
        /// <summary>
        /// POST method for submitting a request for humidity data.
        /// </summary>
        /// <param name="tempInput">Parameters for retrieving humidity data. Required fields: DateTimeSpan.StartDate, DateTimeSpan.EndDate, Geometry.Point.Latitude, Geometry.Point.Longitude, Source</param>
        /// <returns>ITimeSeries</returns>
        [HttpPost]
        [ProducesResponseType(200)]
        public async Task<IActionResult> POST([FromBody]HumidityInput tempInput)
        {
            try
            {
                WSHumidity humid = new WSHumidity();
                ITimeSeriesOutput results = await humid.GetHumidity(tempInput);
                results.Metadata = Utilities.Metadata.AddToMetadata("request_url", this.Request.Path, results.Metadata);
                return new ObjectResult(results);
            }
            catch (Exception ex)
            {
                var exceptionLog = Log.ForContext("Type", "exception");
                exceptionLog.Fatal(ex.Message);
                exceptionLog.Fatal(ex.StackTrace);

                Utilities.ErrorOutput err = new Utilities.ErrorOutput();
                return new ObjectResult(err.ReturnError("Unable to complete request due to invalid request or unknown error."));
            }
        }
    }
}
