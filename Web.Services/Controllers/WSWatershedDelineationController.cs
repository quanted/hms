using Data;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Services.Models;

namespace Web.Services.Controllers
{
    /// <summary>
    /// Delineation Input that implements TimeSeriesInput object.
    /// </summary>
    public class WatershedDelineationInput : TimeSeriesInput
    {
        public List<List<object>> contaminantInflow { get; set; }

        public string inflowSource { get; set; }
    }

    /// <summary>
    /// WorkFlow Output object
    /// </summary>
    public class WatershedDelineationOutput
    { 
        public string Dataset { get; set; }
        public string DataSource { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
        public Dictionary<string, Dictionary<string, List<string>>> Data { get; set; }
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
                    HucID = "030502040102",
                    StationID = null,
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
                    Timezone = null
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
    [Route("api/workflow/timeoftravel")]//[Route("api/hydrology/delineation/")]
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
            try
            {
                var stpWatch = System.Diagnostics.Stopwatch.StartNew();

                WSWatershedDelineation watershed = new WSWatershedDelineation();
                WatershedDelineationOutput results = await watershed.GetDelineationData(watershedInput);
                results.Metadata = Utilities.Metadata.AddToMetadata("request_url", this.Request.Path, results.Metadata);

                stpWatch.Stop();
                long time = stpWatch.ElapsedMilliseconds / 1000;
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
