using Data;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Web.Services.Models;

namespace Web.Services.Controllers
{
    /// <summary>
    /// PrecipitationCompare Input that implements TimeSeriesInput object.
    /// </summary>
    public class PrecipitationCompareInput : TimeSeriesInput
    {
        /// <summary>
        /// Specified dataset for the workflow
        /// </summary>
        [Required]
        public string Dataset { get; set; }

        /// <summary>
        /// List of sources for the workflow.
        /// </summary>
        [Required]
        public List<string> SourceList { get; set; }

        /// <summary>
        /// States whether or not precip should be aggregated by grid cells (weighted spatial average).
        /// </summary>
        [Required]
        public Boolean Weighted { get; set; }

        /// <summary>
        /// Daily precip threshold in mm.
        /// </summary>
        public double ExtremeDaily { get; set; }

        /// <summary>
        /// Five day total precip threshold in mm.
        /// </summary>
        public double ExtremeTotal { get; set; }
    }

    /// <summary>
    /// PrecipitationExtraction Input that implements TimeSeriesInput object.
    /// </summary>
    public class PrecipitationExtractionInput : TimeSeriesInput
    {
        /// <summary>
        /// Specified dataset for the workflow
        /// </summary>
        public string Dataset { get; set; }

        /// <summary>
        /// List of sources for the workflow.
        /// </summary>
        public List<string> SourceList { get; set; }
    }

    // --------------- Swashbuckle Examples --------------- //
    /// <summary>
    /// Swashbuckle Precipitation POST request example
    /// </summary>
    public class PrecipitationCompareInputExample : IExamplesProvider<PrecipitationCompareInput>
    {
        /// <summary>
        /// Get example function.
        /// </summary>
        /// <returns></returns>
        public PrecipitationCompareInput GetExamples()
        {
            PrecipitationCompareInput example = new PrecipitationCompareInput()
            {
                Dataset = "Precipitation",
                SourceList = new List<String>() { "nldas", "gldas" },
                Weighted = true,
                DateTimeSpan = new DateTimeSpan()
                {
                    StartDate = new DateTime(2015, 01, 01),
                    EndDate = new DateTime(2015, 01, 08),
                    DateTimeFormat = "yyyy-MM-dd HH"
                },
                Geometry = new TimeSeriesGeometry()
                {
                    ComID = 1053791
                },
                DataValueFormat = "E3",
                TemporalResolution = "default",
                Units = "metric",
                OutputFormat = "json"
            };
            return example;
        }
    }

    /// <summary>
    /// Swashbuckle Precipitation Data Extraction POST request example
    /// </summary>
    public class PrecipitationExtractionInputExample : IExamplesProvider<PrecipitationExtractionInput>
    {
        /// <summary>
        /// Get example function.
        /// </summary>
        /// <returns></returns>
        public PrecipitationExtractionInput GetExamples()
        {
            PrecipitationExtractionInput example = new PrecipitationExtractionInput()
            {
                Dataset = "Precipitation",
                SourceList = new List<String>() { "ncei", "nldas" },
                DateTimeSpan = new DateTimeSpan()
                {
                    StartDate = new DateTime(2015, 01, 01),
                    EndDate = new DateTime(2015, 01, 08),
                    DateTimeFormat = "yyyy-MM-dd HH"
                },
                Geometry = new TimeSeriesGeometry()
                {
                    ComID = 1053791
                },
                DataValueFormat = "E3",
                TemporalResolution = "default",
                Units = "metric",
                OutputFormat = "json"
            };
            return example;
        }
    }

    // --------------- WorkflowPrecip Controller --------------- //
    /// <summary>
    /// WorkflowPrecip controller for HMS.
    /// </summary>
    [ApiVersion("0.1")]             // Version 0.1 endpoint
    [Route("api/workflow/")]
    [Produces("application/json")]
    public class WSWorkflowPrecipController : Controller
    {
        /// <summary>
        /// POST method for submitting a request for precip comparison data.
        /// Source parameter must contain a value, but value is not used.
        /// </summary>
        /// <param name="precipInput">Parameters for retrieving PrecipComparison data. Required fields: Dataset, SourceList, Weighted, Extreme params</param>
        /// <returns>ITimeSeries</returns>
        [HttpPost]
        [Route("precip_compare")]             // Default endpoint
        [ProducesResponseType(200)]
        public async Task<IActionResult> POSTComparison([FromBody]PrecipitationCompareInput precipCompareInput)
        {
            try
            {
                WSPrecipCompare precipCompare = new WSPrecipCompare();
                var stpWatch = System.Diagnostics.Stopwatch.StartNew();
                ITimeSeriesOutput results = await precipCompare.GetPrecipCompareData(precipCompareInput);
                stpWatch.Stop();
                results.Metadata = Utilities.Metadata.AddToMetadata("retrievalTime", stpWatch.ElapsedMilliseconds.ToString(), results.Metadata);
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

        /// <summary>
        /// POST method for submitting a request for precip extraction data.
        /// Source parameter must contain a value, but value is not used.
        /// </summary>
        /// <param name="precipInput">Parameters for retrieving PrecipExtraction data. Required fields: Dataset, SourceList</param>
        /// <returns>ITimeSeries</returns>
        [HttpPost]
        [Route("precip_data_extraction")]             // Default endpoint
        [ProducesResponseType(200)]
        public async Task<IActionResult> POSTExtraction([FromBody]PrecipitationExtractionInput precipExtractInput)
        {
            try
            {
                WSPrecipExtraction precipExtract = new WSPrecipExtraction();
                var stpWatch = System.Diagnostics.Stopwatch.StartNew();
                ITimeSeriesOutput results = await precipExtract.GetWorkFlowData(precipExtractInput);
                stpWatch.Stop();
                results.Metadata = Utilities.Metadata.AddToMetadata("retrievalTime", stpWatch.ElapsedMilliseconds.ToString(), results.Metadata);
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
