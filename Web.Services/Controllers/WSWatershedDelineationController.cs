using Data;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Web.Services.Models;

namespace Web.Services.Controllers
{
    /// <summary>
    /// Delineation Input that implements TimeSeriesInput object.
    /// </summary>
    public class WatershedDelineationInput : TimeSeriesInput
    {

    }

    // --------------- Swashbuckle Examples --------------- //

    /// <summary>
    /// Swashbuckle DelineationCompare POST request example
    /// </summary>
    public class WatershedDelineationInputExample : IExamplesProvider<WatershedDelineationInput>
    {
        /// <summary>
        /// Get example function.
        /// </summary>
        /// <returns></returns>
        public WatershedDelineationInput GetExamples()
        {
            WatershedDelineationInput example = new WatershedDelineationInput()
            {
                Source = "nldas",
                DateTimeSpan = new DateTimeSpan()
                {
                    StartDate = new DateTime(2015, 01, 01),
                    EndDate = new DateTime(2015, 01, 08),
                    DateTimeFormat = "yyyy-MM-dd HH"
                },
                Geometry = new TimeSeriesGeometry()
                {
                    Description = "EPA Athens Office",
                    Point = new PointCoordinate()
                    {
                        Latitude = 33.925673,
                        Longitude = -83.355723
                    },
                    GeometryMetadata = new Dictionary<string, string>()
                    {
                        { "City", "Athens" },
                        { "State", "Georgia"},
                        { "Country", "United States" },
                        { "huc_12_num", "030502040102" }
                    },
                    Timezone = new Timezone()
                    {
                        Name = "EST",
                        Offset = -5,
                        DLS = false
                    }
                },
                DataValueFormat = "E3",
                TemporalResolution = "default",
                TimeLocalized = true,
                Units = "default",
                OutputFormat = "json"
            };
            return example;
        }
    }

    // --------------- DelineationCompare Controller --------------- //

    /// <summary>
    /// DelineationCompare controller for HMS.
    /// </summary>
    [ApiVersion("0.1")]             // Version 0.1 endpoint
    [Route("api/hydrology/delineation/")]
    [Produces("application/json")]
    public class WSWatershedDelineationController : Controller
    {
        /// <summary>
        /// POST method for submitting a request for delineation compare data.
        /// Source parameter must contain a value, but value is not used.
        /// </summary>
        /// <param name="watershedInput">Parameters for retrieving DelineationCompare data. Required fields: Dataset, SourceList</param>
        /// <returns>ITimeSeries</returns>
        [HttpPost]
        [ProducesResponseType(200)]
        public async Task<IActionResult> POST([FromBody]WatershedDelineationInput watershedInput)
        {
            WSWatershedDelineation watershed = new WSWatershedDelineation();
            ITimeSeriesOutput results = await watershed.GetDelineationData(watershedInput);
            results.Metadata = Utilities.Metadata.AddToMetadata("request_url", this.Request.Path, results.Metadata);
            return new ObjectResult(results);
        }
    }
}
