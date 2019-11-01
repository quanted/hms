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
    public class TotalFlowInputExample : IExamplesProvider<TotalFlowInput>
    {
        /// <summary>
        /// Get example function.
        /// </summary>
        /// <returns></returns>
        public TotalFlowInput GetExamples()
        {
            TotalFlowInput example = new TotalFlowInput()
            {
                Source = "nldas",
                DateTimeSpan = new DateTimeSpan()
                {
                    StartDate = new DateTime(2015, 01, 01),
                    EndDate = new DateTime(2015, 12, 31)
                },
                GeometryType = "comid",
                GeometryInput = "1049831",
            };
            return example;
        }
    }

    /// <summary>
    /// Total Flow controller for HMS
    /// </summary>
    [ApiVersion("0.1")]             // Version 0.1 endpoint
    [Route("api/hydrology/totalflow")]
    [Produces("application/json")]
    public class WSTotalFlowController : Controller
    {
        /// <summary>
        /// POST method for submitting a request for total flow data.
        /// </summary>
        /// <param name="tfInput"></param>
        /// <returns>ITimeSeriesOutput</returns>
        [HttpPost]
        [ProducesResponseType(200)]
        public async Task<IActionResult> POST([FromBody]TotalFlowInput tfInput)
        {
            WSTotalFlow tFlow = new WSTotalFlow();
            ITimeSeriesOutput results = await tFlow.GetTotalFlowData(tfInput);
            return new ObjectResult(results);
        }


    }
}