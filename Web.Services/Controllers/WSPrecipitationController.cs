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
    /// Label: Precipitation;
    /// Description: Precipitation Input that implements TimeSeriesInput object.;
    /// </summary>
    public class PrecipitationInput : TimeSeriesInput
    {

        // Add extra Dataset specific variables here.
        /// <summary>
        /// Description: Precipitation data source;
        /// Default: "nldas";
        /// Options: ["nldas", "gldas", "ncei", "daymet", "prism", "trmm"];
        /// Required: True;
        /// </summary>
        public new string Source { get; set; }
    }

    // --------------- Swashbuckle Examples --------------- //

    /// <summary>
    /// Swashbuckle Precipitation POST request example
    /// </summary>
    public class PrecipitationInputExample : IExamplesProvider<PrecipitationInput>
    {
        /// <summary>
        /// Get example function.
        /// </summary>
        /// <returns></returns>
        public PrecipitationInput GetExamples()
        {
            PrecipitationInput example = new PrecipitationInput()
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
                    Point = new PointCoordinate()
                    {
                        Latitude = 33.925673,
                        Longitude = -83.355723
                    },
                    Timezone = null
                },
                DataValueFormat = "E3",
                TemporalResolution = "default",
                Units = "metric",
                OutputFormat = "json"
            };
            return example;
        }
    }

    // --------------- Precipitation Controller --------------- //

    /// <summary>
    /// Precipitation controller for HMS.
    /// </summary>
    [ApiVersion("0.1")]             // Version 0.1 endpoint
    [Route("api/meteorology/precipitation")]
    [Produces("application/json")]
    public class WSPrecipitationController : Microsoft.AspNetCore.Mvc.Controller
    {
        readonly IDiagnosticContext _diagnosticContext;

        public WSPrecipitationController(IDiagnosticContext diagnosticContext)
        {
            _diagnosticContext = diagnosticContext;
        }

        /// <summary>
        /// POST method for submitting a request for precipitation data.
        /// </summary>
        /// <param name="precipInput">Parameters for retrieving precipitation data. Required fields: DateTimeSpan.StartDate, DateTimeSpan.EndDate, Geometry.Point.Latitude, Geometry.Point.Longitude, Source</param>
        /// <returns>ITimeSeries</returns>
        [HttpPost]
        [ProducesResponseType(200)]  
        public async Task<IActionResult> POST([FromBody]PrecipitationInput precipInput)
        {
            try
            {
                ((Data.TimeSeriesInput)precipInput).Source = precipInput.Source;
                Console.WriteLine("INPUT" + precipInput.ToString());
                WSPrecipitation precip = new WSPrecipitation();
                var stpWatch = System.Diagnostics.Stopwatch.StartNew();
                ITimeSeriesOutput results = await precip.GetPrecipitation(precipInput);
                results.Metadata = Utilities.Metadata.AddToMetadata("request_url", this.Request.Path, results.Metadata);
                stpWatch.Stop();
                results.Metadata = Utilities.Metadata.AddToMetadata("retrievalTime", stpWatch.ElapsedMilliseconds.ToString() + " ms", results.Metadata);
                results.Metadata = Utilities.Metadata.AddToMetadata("request_url", this.Request.Path, results.Metadata);
                return new ObjectResult(results);
            }
            catch(Exception ex)
            {
                var exceptionLog = Log.ForContext("Type", "exception");
                exceptionLog.Fatal(ex.Message);
                exceptionLog.Fatal(ex.StackTrace);
                JsonSerializerOptions options = new JsonSerializerOptions()
                {
                    AllowTrailingCommas = true,
                    PropertyNameCaseInsensitive = true
                };
                exceptionLog.Fatal(System.Text.Json.JsonSerializer.Serialize(precipInput, options));

                Utilities.ErrorOutput err = new Utilities.ErrorOutput();
                return new ObjectResult(err.ReturnError("Unable to complete request due to invalid request or unknown error."));
            }
        }
    }
}
