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
    interface WorkflowSources
    {
        string RunoffSource { get; set; }
        string StreamHydrology { get; set; }
    }


    /// <summary>
    /// WorkFlow Input that implements TimeSeriesInput object.
    /// </summary>
    public class WatershedWorkflowInput : TimeSeriesInput, WorkflowSources
    {
        /// <summary>
        /// OPTIONAL: Specifies the requested source for Runoff Data
        /// </summary>
        public string RunoffSource { get; set; }

        /// <summary>
        /// OPTIONAL: Specifies the requested Stream Hydrology Algorithm to use
        /// </summary>
        public string StreamHydrology { get; set; }

        /// <summary>
        /// OPTIONAL: States whether or not runoff should be aggregated by catchments
        /// </summary>
        public Boolean Aggregation { get; set; }
        // Add extra SurfaceRunoff specific variables here
    }

    /// <summary>
    /// WorkFlow Output object
    /// </summary>
    public class WatershedWorkflowOutput : ITimeSeriesOutput<Dictionary<string, ITimeSeriesOutput>>
    {
        public string Dataset { get; set; }
        public string DataSource { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
        public Dictionary<string, Dictionary<string, ITimeSeriesOutput>> Data { get; set; }
        public Dictionary<string, Dictionary<string, string>> Table { get; set; }
        public ITimeSeriesOutput<Dictionary<string, ITimeSeriesOutput>> Clone()
        {
            return this;
        }
    }

    // --------------- Swashbuckle Examples --------------- //

    /// <summary>
    /// Swashbuckle WorkFlowCompare POST request example
    /// </summary>
    public class WatershedWorkflowInputExample : IExamplesProvider<WatershedWorkflowInput>
    {
        /// <summary>
        /// Get example function.
        /// </summary>
        /// <returns></returns>
        public WatershedWorkflowInput GetExamples()
        {
            WatershedWorkflowInput example = new WatershedWorkflowInput()
            {
                Source = "nldas",
                DateTimeSpan = new DateTimeSpan()
                {
                    StartDate = new DateTime(2014, 07, 01),
                    EndDate = new DateTime(2014, 07, 31),
                    DateTimeFormat = "yyyy-MM-dd HH"
                },
                Geometry = new TimeSeriesGeometry()
                {
                    HucID = "030502040102",
                    GeometryMetadata = new Dictionary<string, string>()
                    {
                        { "precipSource", "daymet"}
                    },
                    Timezone = null
                },
                DataValueFormat = "E3",
                TemporalResolution = "default",
                TimeLocalized = true,
                Units = "default",
                OutputFormat = "json",
                Aggregation = false,
                RunoffSource = "curvenumber",
                StreamHydrology = "constant"
            };
            return example;
        }
    }

    // --------------- WorkFlowCompare Controller --------------- //

    /// <summary>
    /// WorkFlowCompare controller for HMS.
    /// </summary>
    [ApiVersion("0.1")]             // Version 0.1 endpoint
    [Route("api/workflow/watershed")]
    [Produces("application/json")]
    public class WSWatershedWorkflowController : Controller
    {
        /// <summary>
        /// POST method for submitting a request for getting workflow compare data.
        /// Source parameter must contain a value, but value is not used.
        /// </summary>
        /// <param name="workflowInput">Parameters for retrieving WorkFlowCompare data. Required fields: Dataset, SourceList</param>
        /// <returns>ITimeSeries</returns>
        [HttpPost]
        [ProducesResponseType(200)]
        public async Task<IActionResult> POST([FromBody]WatershedWorkflowInput workflowInput)
        {
            try
            {
                WSWatershedWorkFlow workFlow = new WSWatershedWorkFlow();
                WatershedWorkflowOutput results = await workFlow.GetWorkFlowData(workflowInput);
                results.Metadata = Utilities.Metadata.AddToMetadata("request_url", this.Request.Path, results.Metadata);
                return new ObjectResult(results);
            }
            catch (Exception ex)
            {


                Log.Information("Exception caught processing request to /workflow/watershed/, review exception log file for details.");
                var exceptionLog = Log.ForContext("Type", "exception");
                exceptionLog.Fatal(workflowInput.ToString());
                exceptionLog.Fatal(ex.Message); 
                exceptionLog.Fatal(ex.StackTrace);
                JsonSerializerOptions options = new JsonSerializerOptions()
                {
                    AllowTrailingCommas = true,
                    PropertyNameCaseInsensitive = true
                };
                exceptionLog.Fatal(System.Text.Json.JsonSerializer.Serialize(workflowInput, options));

                Utilities.ErrorOutput err = new Utilities.ErrorOutput();
                return new ObjectResult(err.ReturnError("Unable to complete request due to invalid request or unknown error."));
            }
        }
    }
}