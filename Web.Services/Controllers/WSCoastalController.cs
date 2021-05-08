using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using Web.Services.Models;

namespace Web.Services.Controllers
{
    // --------------- Swashbuckle Examples --------------- //

    /// <summary>
    /// Swashbuckle Coastal POST request example
    /// </summary>
    public class CoastalInputExample : IExamplesProvider<TimeSeriesInput>
    {
        /// <summary>
        /// Get example function.
        /// </summary>
        /// <returns></returns>
        public TimeSeriesInput GetExamples()
        {
            TimeSeriesInput example = new TimeSeriesInput()
            {
                Source = "noaa_coastal",
                DateTimeSpan = new DateTimeSpan()
                {
                    StartDate = new DateTime(2013, 01, 01),
                    EndDate = new DateTime(2013, 01, 02),
                    DateTimeFormat = "yyyy-MM-dd HH"
                },
                Geometry = new TimeSeriesGeometry()
                {
                    StationID = "8454000",
                    Timezone = new Timezone()
                    {
                        Name = "GMT"
                    },
                    GeometryMetadata = new Dictionary<string, string>()
                    {
                        ["product"] = "water_level",
                        ["datum"] = "mllw",
                        ["application"] = "web_services",
                    }
                },
                DataValueFormat = "E3",
                TemporalResolution = "default",
                Units = "metric",
                OutputFormat = "json",
            };
            return example;
        }
    }

    /// <summary>
    /// API controller for handling requests for coastal data.
    /// </summary>
    [Route("api/coastal")]
    [Produces("application/json")]
    public class WSCoastalController : Controller
    {
        /// <summary>
        /// POST method for submitting a request for coastal data.
        /// </summary>
        /// <param name="input">
        /// <returns>ITimeSeries</returns>
        [HttpPost]
        public async Task<IActionResult> GetCoastalData([FromBody] TimeSeriesInput coastalInput)
        {
            try
            {
                WSCoastal coastal = new WSCoastal();
                var stpWatch = System.Diagnostics.Stopwatch.StartNew();
                ITimeSeriesOutput results = await coastal.GetCoastal(coastalInput);
                stpWatch.Stop();
                results.Metadata = Utilities.Metadata.AddToMetadata("request_url", this.Request.Path, results.Metadata);
                results.Metadata = Utilities.Metadata.AddToMetadata("retrievalTime", stpWatch.ElapsedMilliseconds.ToString() + " ms", results.Metadata);
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
