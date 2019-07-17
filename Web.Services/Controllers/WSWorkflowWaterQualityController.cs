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
        [Required]
        public string DataSource { get; set; }

    }


    // --------------- Swashbuckle Examples --------------- //
    /// <summary>
    /// Swashbuckle Water Quality POST request example
    /// </summary>
    public class WaterQualityInputExample : IExamplesProvider
    {
        /// <summary>
        /// Get example function.
        /// </summary>
        /// <returns></returns>
        public object GetExamples()
        {
            WaterQualityInput example = new WaterQualityInput()
            {
                //ConnectivityTable = new List<List<string>>(){}
                DataSource = "nldas"
            };
            return example;
        }
    }



    // --------------- Water Quality Controller --------------- //
    /// <summary>
    /// Workflow Water Quality controller for HMS.
    /// </summary>
    [ApiVersion("0.1")]             // Version 0.1 endpoint
    [Route("api/workflow/waterquality")]
    public class WSWorkflowWaterQualityController : Controller
    {
        /// <summary>
        /// POST Method for getting Water Quality data.
        /// dataSource can be 'nldas' or 'ncei', which will pull data from GHCND:US1NCCM0006
        /// </summary>
        /// <param name="waterqualityInput"></param>
        /// <returns>ITimeSeries</returns>
        [HttpPost]
        [SwaggerRequestExample(typeof(WaterQualityInput), typeof(WaterQualityInputExample))]
        public async Task<IActionResult> POSTComparison([FromBody]WaterQualityInput waterqualityInput)
        {
            if(waterqualityInput.TaskID == null)
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
    }
}
