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
    /// Wind Input that implements TimeSeriesInput object
    /// </summary>
    public class WindInput : TimeSeriesInput
    {
        // Add extra wind specific variables here

        /// Wind component
        /// User request for U, V values and/or speed, direction. All input is made upper case for comparison.
        /// Valid values: U/V, SPEED/DIR (SPEED/DIRECTION), ALL
        /// Defaults: ALL 
        public string Component = "ALL";
    }

    // --------------- Swashbuckle Examples --------------- //

    /// <summary>
    /// Swashbuckle Wind POST request example
    /// </summary>
    public class WindInputExample : IExamplesProvider<WindInput>
    {
        /// <summary>
        /// Get example function.
        /// </summary>
        /// <returns></returns>
        public WindInput GetExamples()
        {
            WindInput example = new WindInput()
            {
                Source = "nldas",
                Component = "all",
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
                    Timezone = new Timezone()
                    {
                        Name = "EST",
                        Offset = -5,
                        DLS = false
                    }
                }
            };
            return example;
        }
    }

    // --------------- Wind Controller --------------- //

    /// <summary>
    /// Wind controller for HMS.
    /// </summary>
    [ApiVersion("0.1")]             // Version 0.1 endpoint
    [Route("api/meteorology/wind")]
    [Produces("application/json")]
    public class WSWindController : Controller
    {
        /// <summary>
        /// POST method for submitting a request for wind data.
        /// </summary>
        /// <param name="tempInput">Parameters for retrieving wind data. Required fields: DateTimeSpan.StartDate, DateTimeSpan.EndDate, Geometry.Point.Latitude, Geometry.Point.Longitude, Source</param>
        /// <returns>ITimeSeries</returns>
        [HttpPost]
        [ProducesResponseType(200)]
        public async Task<IActionResult> POST([FromBody]WindInput tempInput)
        {
            try
            {
                WSWind wind = new WSWind();
                ITimeSeriesOutput results = await wind.GetWind(tempInput);
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
