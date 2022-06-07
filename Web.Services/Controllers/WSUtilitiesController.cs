using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Web.Services.Controllers
{

    /// <summary>
    /// Utility REST endpoints for HMS components.
    /// </summary>
    [ApiVersion("0.1")]             // Version 0.1 endpoint
    [Route("api")]                  // Default endpoints
    [Produces("application/json")]
    public class WSUtilitiesController : Microsoft.AspNetCore.Mvc.Controller
    {

        /// <summary>
        /// Checks endpoints for all datasets.
        /// </summary>
        /// <returns></returns>
        [Route("utilities/status")]
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<IActionResult> AllDatasetEndpointsCheck()
        {
            Task<Dictionary<string, bool>> endpointStatus = Models.WSUtilities.ServiceCheck();
            return new ObjectResult(endpointStatus.Result);
        }
    }
}
