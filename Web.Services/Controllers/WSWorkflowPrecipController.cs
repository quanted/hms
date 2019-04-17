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
        [Required]
        public string Dataset { get; set; }

        /// <summary>
        /// List of sources for the workflow.
        /// </summary>
        [Required]
        public List<string> SourceList { get; set; }
    }

    // --------------- Swashbuckle Examples --------------- //

    // --------------- WorkflowPrecip Controller --------------- //
    /// <summary>
    /// WorkflowPrecip controller for HMS.
    /// </summary>
    [ApiVersion("0.1")]             // Version 0.1 endpoint
    [Route("api/workflow/")]
    public class WSWorkflowPrecipController : Controller
    {
        /// <summary>
        /// POST Method for getting PrecipComparison data.
        /// Source parameter must contain a value, but value is not used.
        /// </summary>
        /// <param name="precipInput">Parameters for retrieving PrecipComparison data. Required fields: Dataset, SourceList, Weighted, Extreme params</param>
        /// <returns>ITimeSeries</returns>
        [HttpPost]
        [Route("precip_compare")]             // Default endpoint
        //[SwaggerRequestExample(typeof(PrecipitationExtractionInput), typeof(PrecipitationExtractionInputExample))]
        //[SwaggerResponseExample(200, typeof(PrecipitationExtractionOutputExample))]
        public async Task<IActionResult> POSTComparison([FromBody]PrecipitationCompareInput precipCompareInput)
        {
            WSPrecipCompare precipCompare = new WSPrecipCompare();
            var stpWatch = System.Diagnostics.Stopwatch.StartNew();
            ITimeSeriesOutput results = await precipCompare.GetPrecipCompareData(precipCompareInput);
            stpWatch.Stop();
            results.Metadata = Utilities.Metadata.AddToMetadata("retrievalTime", stpWatch.ElapsedMilliseconds.ToString(), results.Metadata);
            results.Metadata = Utilities.Metadata.AddToMetadata("request_url", this.Request.Path, results.Metadata);
            return new ObjectResult(results);
        }

        /// <summary>
        /// POST Method for getting PrecipExtraction data.
        /// Source parameter must contain a value, but value is not used.
        /// </summary>
        /// <param name="precipInput">Parameters for retrieving PrecipExtraction data. Required fields: Dataset, SourceList</param>
        /// <returns>ITimeSeries</returns>
        [HttpPost]
        [Route("precip_data_extraction")]             // Default endpoint
        //[SwaggerRequestExample(typeof(PrecipitationExtractionInput), typeof(PrecipitationExtractionInputExample))]
        //[SwaggerResponseExample(200, typeof(PrecipitationExtractionOutputExample))]
        public async Task<IActionResult> POSTExtraction([FromBody]PrecipitationExtractionInput precipExtractInput)
        {
            WSPrecipExtraction precipExtract = new WSPrecipExtraction();
            var stpWatch = System.Diagnostics.Stopwatch.StartNew();
            ITimeSeriesOutput results = await precipExtract.GetWorkFlowData(precipExtractInput);
            stpWatch.Stop();
            results.Metadata = Utilities.Metadata.AddToMetadata("retrievalTime", stpWatch.ElapsedMilliseconds.ToString(), results.Metadata);
            results.Metadata = Utilities.Metadata.AddToMetadata("request_url", this.Request.Path, results.Metadata);
            return new ObjectResult(results);
        }
    }
}
