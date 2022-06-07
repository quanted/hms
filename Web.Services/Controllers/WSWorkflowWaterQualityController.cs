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
    /// WaterQuality Input that implements TimeSeriesInput object.
    /// </summary>
    public class WaterQualityInput
    {
        /// <summary>
        /// Specified dataset for the workflow
        /// </summary>
        //[Required]
        //public List<List<string>> ConnectivityTable { get; set; }     //To load from file, will be added at a later date.

        /// <summary>
        /// TaskID required for data storage in mongodb
        /// </summary>
        public string TaskID { get; set; }

        /// <summary>
        /// Data source for data retrieval
        /// If value is 'nldas': surface runoff and subsurface flow will be from nldas (no precip will be downloaded); 
        /// If value is 'ncei', precip data will be downloaded from the closest station to the catchment and curvenumber will be used for surface runoff/subsurface flow.
        /// </summary>
        public string DataSource { get; set; }

        public int MinNitrate { get; set; }
        public int MaxNitrate { get; set; }
        public int MinAmmonia { get; set; }
        public int MaxAmmonia { get; set; }

    }

    // --------------- Swashbuckle Examples --------------- //
    /// <summary>
    /// Swashbuckle Water Quality POST request example
    /// </summary>
    public class WaterQualityInputExample : IExamplesProvider<WaterQualityInput>
    {
        /// <summary>
        /// Get example function.
        /// </summary>
        /// <returns></returns>
        public WaterQualityInput GetExamples()
        {
            WaterQualityInput example = new WaterQualityInput()
            {
                //ConnectivityTable = new List<List<string>>(){}
                DataSource = "nldas",
                MinNitrate = 1000,
                MaxNitrate = 10000,
                MinAmmonia = 100000,
                MaxAmmonia = 750000
            };

            WSWorkflowWaterQualityController WQC = new WSWorkflowWaterQualityController();


            return example;
        }
    }

    // --------------- Water Quality Controller --------------- //
    /// <summary>
    /// Workflow Water Quality controller for HMS.
    /// </summary>
    [ApiVersion("0.1")]             // Version 0.1 endpoint
    [Route("api/workflow/waterquality")]
    [Produces("application/json")]
    public class WSWorkflowWaterQualityController : Microsoft.AspNetCore.Mvc.Controller
    {
        /// <summary>
        /// POST method to submit a request for water quality data.
        /// dataSource can be 'nldas' or 'ncei', which will pull data from GHCND:US1NCCM0006
        /// </summary>
        /// <param name="waterqualityInput"></param>
        /// <returns>ITimeSeries</returns>
        [HttpPost]
        [ProducesResponseType(200)]
        public async Task<IActionResult> POSTComparison([FromBody]WaterQualityInput waterqualityInput)
        {
            try
            {
                if (waterqualityInput.TaskID == null)
                {
                    waterqualityInput.TaskID = Guid.NewGuid().ToString();
                }
                WSWaterQuality wq = new WSWaterQuality();
                var stpWatch = System.Diagnostics.Stopwatch.StartNew();
                ITimeSeriesOutput results = await wq.GetWaterQualityData(waterqualityInput);
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
