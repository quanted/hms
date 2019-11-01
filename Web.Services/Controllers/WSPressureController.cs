using Data;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Web.Services.Models;

namespace Web.Services.Controllers
{
    /// <summary>
    /// Pressure Input that implements TimeSeriesInput object
    /// </summary>
    public class PressureInput : TimeSeriesInput
    {
        // Add extra pressure specific variables here

    }

    // --------------- Swashbuckle Examples --------------- //

    /// <summary>
    /// Swashbuckle Pressure POST request example
    /// </summary>
    public class PressureInputExample : IExamplesProvider<PressureInput>
    {
        /// <summary>
        /// Get example function.
        /// </summary>
        /// <returns></returns>
        public PressureInput GetExamples()
        {
            PressureInput example = new PressureInput()
            {
                Source = "gldas",
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

    // --------------- Pressure Controller --------------- //

    /// <summary>
    /// Pressure controller for HMS.
    /// </summary>
    [ApiVersion("0.1")]             // Version 0.1 endpoint
    [Route("api/meteorology/pressure")]
    [Produces("application/json")]

    public class WSPressureController : Controller
    {
        /// <summary>
        /// POST method for submitting a request for pressure data.
        /// </summary>
        /// <param name="tempInput">Parameters for retrieving pressure data. Required fields: DateTimeSpan.StartDate, DateTimeSpan.EndDate, Geometry.Point.Latitude, Geometry.Point.Longitude, Source</param>
        /// <returns>ITimeSeries</returns>
        [HttpPost]
        [ProducesResponseType(200)]
        public async Task<IActionResult> POST([FromBody]PressureInput tempInput)
        {
            WSPressure press = new WSPressure();
            ITimeSeriesOutput results = await press.GetPressure(tempInput);
            results.Metadata = Utilities.Metadata.AddToMetadata("request_url", this.Request.Path, results.Metadata);
            return new ObjectResult(results);
        }
    }
}
