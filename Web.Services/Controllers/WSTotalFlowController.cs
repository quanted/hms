using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Data;
using Web.Services.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Web.Services.Controllers
{

    // --------------- Swashbuckle Examples --------------- //

    /// <summary>
    /// Swashbuckle TotalFlow POST request example
    /// </summary>
    public class TotalFlowInputExample : IExamplesProvider
    {
        /// <summary>
        /// Get example function.
        /// </summary>
        /// <returns></returns>
        public object GetExamples()
        {
            TotalFlowInput example = new TotalFlowInput()
            {
                Source = "nldas",
                DateTimeSpan = new DateTimeSpan()
                {
                    StartDate = new DateTime(2015, 01, 01),
                    EndDate = new DateTime(2015, 12, 31)
                },
                GeometryType = "huc",
                GeometryInput = "11010001",
            };
            return example;
        }
    }

    /// <summary>
    /// Total Flow controller for HMS
    /// </summary>
    [Produces("application/json")]
    [Route("api/hydrology/totalflow")]
    public class WSTotalFlowController : Controller
    {
        /// <summary>
        /// POST method for getting total flow data.
        /// </summary>
        /// <param name="tfInput"></param>
        /// <returns>ITimeSeriesOutput</returns>
        [HttpPost]
        [Route("")]
        [Route("v1.0")]
        [SwaggerRequestExample(typeof(TotalFlowInput), typeof(TotalFlowInputExample))]
        public async Task<IActionResult> POST([FromBody]TotalFlowInput tfInput)
        {
            WSTotalFlow tFlow = new WSTotalFlow();
            ITimeSeriesOutput results = await tFlow.GetTotalFlowData(tfInput);
            return new ObjectResult(results);
        }


    }
}