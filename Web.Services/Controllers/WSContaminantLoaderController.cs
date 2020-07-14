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
    /// ContaminantLoader Input that implements TimeSeriesInput object
    /// </summary>
    public class ContaminantLoaderInput
    {
        // Add extra ContaminantLoader specific variables here

        public string ContaminantType { get; set; }
        public string ContaminantInputType { get; set; }
        public string ContaminantInput { get; set; }

    }

    // --------------- Swashbuckle Examples --------------- //

    /// <summary>
    /// Swashbuckle ContaminantLoader POST request example
    /// </summary>
    public class ContaminantLoaderInputExample : IExamplesProvider<ContaminantLoaderInput>
    {
        /// <summary>
        /// Get example function.
        /// </summary>
        /// <returns></returns>
        public ContaminantLoaderInput GetExamples()
        {
            ContaminantLoaderInput example = new ContaminantLoaderInput();
            example.ContaminantInput = "Date-Time, TestValues\n2010-01-01 00, 1.0\n2010-01-01 01, 1.5\n2010-01-01 02, 2.0\n2010-01-01 03, 2.5\n2010-01-01 04, 3.0\n2010-01-01 05, 3.5\n2010-01-01 06, 4.0\n2010-01-01 07, 4.5\n2010-01-01 08, 5.0\n2010-01-01 09, 5.5\n2010-01-01 10, 6.0\n2010-01-01 11, 6.5\n2010-01-01 12, 6.0\n2010-01-01 13, 5.5\n2010-01-01 14, 5.0\n2010-01-01 15, 4.5\n2010-01-01 16, 4.0\n2010-01-01 17, 3.5\n2010-01-01 18, 3.0\n2010-01-01 19, 2.5\n2010-01-01 20, 2.0\n2010-01-01 21, 1.5\n2010-01-01 22, 1.0\n2010-01-01 23, 1.0";
            example.ContaminantInputType = "csv";
            example.ContaminantType = "generic";
            return example;
        }
    }

    // --------------- ContaminantLoader Controller --------------- //

    /// <summary>
    /// ContaminantLoader controller for HMS.
    /// </summary>
    [ApiController]
    [ApiVersion("0.1")]             // Version 0.1 endpoint
    [Route("api/contaminantloader/")]
    //[Produces("application/json")]
    public class WSContaminantLoaderController : ControllerBase
    {
        /// <summary>
        /// POST Method for getting ContaminantLoader data.
        /// </summary>
        /// <param name="tempInput">Parameters for loading up a compatible contaminant timeseries.
        /// /// <returns>ITimeSeries</returns>
        [HttpPost]
        [ProducesResponseType(200)]
        public async Task<IActionResult> POST([FromBody]ContaminantLoaderInput tempInput)
        {
            try { 
                WSContaminantLoader contam = new WSContaminantLoader();
                ITimeSeriesOutput results = await contam.LoadContaminant(tempInput);
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
