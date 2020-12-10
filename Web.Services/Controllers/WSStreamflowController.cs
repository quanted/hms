using Data;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Services.Models;
using System.Text.Json;

namespace Web.Services.Controllers
{

    /// <summary>
    /// Streamflow Input that implements TimeSeriesInput object.
    /// </summary>
    public class StreamflowInput : TimeSeriesInput
    {
        // Add extra Dataset specific variables here.
    }

    // --------------- Swashbuckle Examples --------------- //

    /// <summary>
    /// Swashbuckle Streamflow POST request example
    /// </summary>
    public class StreamflowInputExample : IExamplesProvider<StreamflowInput>
    {
        /// <summary>
        /// Get example function.
        /// </summary>
        /// <returns></returns>
        public StreamflowInput GetExamples()
        {
            StreamflowInput example = new StreamflowInput()
            {
                Source = "nwis",
                DateTimeSpan = new DateTimeSpan()
                {
                    StartDate = new DateTime(2015, 01, 01),
                    EndDate = new DateTime(2015, 12, 31),
                    DateTimeFormat = "yyyy-MM-dd HH"
                },
                Geometry = new TimeSeriesGeometry()
                {
                    GeometryMetadata = new Dictionary<string, string>()
                    {
                        { "gaugestation", "02191300"}
                    }
                },
                DataValueFormat = "E3",
                TemporalResolution = "hourly",
                Units = "metric",
                OutputFormat = "json"
            };
            return example;
        }
    }

    // --------------- Streamflow Controller --------------- //

    /// <summary>
    /// Streamflow controller for HMS.
    /// </summary>
    [ApiVersion("0.1")]             // Version 0.1 endpoint
    [Route("api/hydrology/streamflow")]
    [Produces("application/json")]
    public class WSStreamflowController : Controller
    {
        readonly IDiagnosticContext _diagnosticContext;

        public WSStreamflowController(IDiagnosticContext diagnosticContext)
        {
            _diagnosticContext = diagnosticContext;
        }

        /// <summary>
        /// POST method for submitting a request for precipitation data.
        /// </summary>
        /// <param name="sfInput">Parameters for retrieving streamflow data. Required fields: DateTimeSpan.StartDate, DateTimeSpan.EndDate, Geometry.GeometryMetadata.gaugestation, Source</param>
        /// <returns>ITimeSeries</returns>
        [HttpPost]
        [ProducesResponseType(200)]  
        public async Task<IActionResult> POST([FromBody]StreamflowInput sfInput)
        {
            try
            {
                Console.WriteLine("INPUT" + sfInput.ToString());
                WSStreamflow sf = new WSStreamflow();
                var stpWatch = System.Diagnostics.Stopwatch.StartNew();
                ITimeSeriesOutput results = await sf.GetStreamflow(sfInput);
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
                exceptionLog.Fatal(System.Text.Json.JsonSerializer.Serialize(sfInput, options));

                Utilities.ErrorOutput err = new Utilities.ErrorOutput();
                return new ObjectResult(err.ReturnError("Unable to complete request due to invalid request or unknown error."));
            }
        }
    }
}
