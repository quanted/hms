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
    /// Precipitation Input that implements TimeSeriesInput object.
    /// </summary>
    public class PrecipitationInput : TimeSeriesInput
    {
        // Add extra Dataset specific variables here.
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
                    }
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
    public class WSPrecipitationController : Controller
    {
        readonly IDiagnosticContext _diagnosticContext;

        public WSPrecipitationController(IDiagnosticContext diagnosticContext)
        {
            _diagnosticContext = diagnosticContext;
        }

        public IActionResult Index()
        {
            _diagnosticContext.Set("CatalogLoadTime", 1423);
            return View();
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
    }
}
