using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Web.Services.Controllers
{

    /// <summary>
    /// Utility REST endpoints for HMS components.
    /// </summary>
    public class WSUtilitiesController : Controller
    {


        /// <summary>
        /// Checks endpoints for a specified dataset.
        /// </summary>
        /// <param name="dataset"></param>
        /// <returns></returns>
        [Route("utilities/status/{dataset}")]
        [HttpGet]
        public Dictionary<string, Dictionary<string, string>> DatasetEndpointsCheck(string dataset)
        {
            Dictionary<string, Dictionary<string, string>> endpointStatus = new Dictionary<string, Dictionary<string, string>>();
            switch (dataset)
            {
                case "precipitation":
                    endpointStatus = Models.WSUtilities.CheckPrecipEndpoints();  
                    break;
                default:
                    endpointStatus = new Dictionary<string, Dictionary<string, string>>() { { "UNNOWN DATASET", new Dictionary<string, string>() { { "status", "dataset provided is not valid." } } } };
                    break;
            }
            return endpointStatus;
        }
    }
}
