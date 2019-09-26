using Data;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        public string ContaminantType;
        public string ContaminantInputType;
        public string ContaminantInput;

    }


    // --------------- Swashbuckle Examples --------------- //

    /// <summary>
    /// Swashbuckle ContaminantLoader POST request example
    /// </summary>
    public class ContaminantLoaderInputExample : IExamplesProvider
    {
        /// <summary>
        /// Get example function.
        /// </summary>
        /// <returns></returns>
        public object GetExamples()
        {
            ContaminantLoaderInput example = new ContaminantLoaderInput();
            example.ContaminantInput = "Date-Time, TestValues\n2010-01-01 00, 1.0\n2010-01-01 01, 1.5\n2010-01-01 02, 2.0\n2010-01-01 03, 2.5\n2010-01-01 04, 3.0\n2010-01-01 05, 3.5\n2010-01-01 06, 4.0\n2010-01-01 07, 4.5\n2010-01-01 08, 5.0\n2010-01-01 09, 5.5\n2010-01-01 10, 6.0\n2010-01-01 11, 6.5\n2010-01-01 12, 6.0\n2010-01-01 13, 5.5\n2010-01-01 14, 5.0\n2010-01-01 15, 4.5\n2010-01-01 16, 4.0\n2010-01-01 17, 3.5\n2010-01-01 18, 3.0\n2010-01-01 19, 2.5\n2010-01-01 20, 2.0\n2010-01-01 21, 1.5\n2010-01-01 22, 1.0\n2010-01-01 23, 1.0";
            example.ContaminantInputType = "csv";
            example.ContaminantType = "generic";
            return example;
        }
    }

    /// <summary>
    /// Swashbucle ContaminantLoader Output example
    /// </summary>
    public class ContaminantLoaderOutputExample : IExamplesProvider
    {
        /// <summary>
        /// Get example function.
        /// </summary>
        /// <returns></returns>
        public object GetExamples()
        {
            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput output = oFactory.Initialize();
            output.Dataset = "Contaminant";
            output.DataSource = "User Input";
            output.Metadata = new Dictionary<string, string>()
            {
            };
            output.Data = new Dictionary<string, List<string>>()
            {
                { "2015-01-01 00Z", new List<string>() { "2.764E+002" } },
                { "2015-01-01 01Z", new List<string>() { "2.754E+002" } },
                { "2015-01-01 02Z", new List<string>() { "2.747E+002" } },
                { "2015-01-01 03Z", new List<string>() { "2.741E+002" } },
                { "2015-01-01 04Z", new List<string>() { "2.735E+002" } },
                { "2015-01-01 05Z", new List<string>() { "2.731E+002" } },
            };
            return output;
        }
    }


    // --------------- ContaminantLoader Controller --------------- //

    /// <summary>
    /// ContaminantLoader controller for HMS.
    /// </summary>
    [ApiVersion("0.1")]             // Version 0.1 endpoint
    [Route("api/contaminantloader/")]
    public class WSContaminantLoaderController : Controller
    {
        /// <summary>
        /// POST Method for getting ContaminantLoader data.
        /// </summary>
        /// <param name="tempInput">Parameters for loading up a compatible contaminant timeseries.
        /// /// <returns>ITimeSeries</returns>
        [HttpPost]
        [SwaggerResponseExample(200, typeof(ContaminantLoaderOutputExample))]
        [SwaggerRequestExample(typeof(ContaminantLoaderInput), typeof(ContaminantLoaderInputExample))]
        public async Task<IActionResult> POST([FromBody]ContaminantLoaderInput tempInput)
        {
            WSContaminantLoader contam = new WSContaminantLoader();
            ITimeSeriesOutput results = await contam.LoadContaminant(tempInput);
            results.Metadata = Utilities.Metadata.AddToMetadata("request_url", this.Request.Path, results.Metadata);
            return new ObjectResult(results);
        }
    }
}
