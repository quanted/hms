using Data;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Threading.Tasks;
using Web.Services.Models;

namespace Web.Services.Controllers
{
    /// <summary>
    /// Radiation Input that implements TimeSeriesInput object
    /// </summary>
    public class RadiationInput : TimeSeriesInput
    {
        // Add extra radiation specific variables here

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
            WSRadiation rad = new WSRadiation();
            ITimeSeriesOutput results = await rad.GetRadiation(tempInput);
            results.Metadata = Utilities.Metadata.AddToMetadata("request_url", this.Request.Path, results.Metadata);
            return new ObjectResult(results);
        }
    }
}
