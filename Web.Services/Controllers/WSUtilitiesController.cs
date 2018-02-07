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
    [Route("api")]             // Default endpoints
    public class WSUtilitiesController : Controller
    {

        /// <summary>
        /// Checks endpoints for all datasets.
        /// </summary>
        /// <returns></returns>
        [Route("utilities/status")]
        [Route("utilities/status/v1.0")]
        [HttpGet]
        public Dictionary<string, Dictionary<string, Dictionary<string, string>>> AllDatasetEndpointsCheck()
        {
            Dictionary<string, Dictionary<string, Dictionary<string, string>>> endpointStatus = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>
            {
                { "Evapotranspiration", Models.WSUtilities.CheckEvapoEndpoints() },
                { "Precipitation", Models.WSUtilities.CheckPrecipEndpoints() },
                { "Soil Moisture", Models.WSUtilities.CheckSoilMEndpoints() },
                { "Subsurface Flow", Models.WSUtilities.CheckSubsurfaceEndpoints() },
                { "Surface Runoff", Models.WSUtilities.CheckRunoffEndpoints() },
                { "Temperature", Models.WSUtilities.CheckTempEndpoints() }
            };
            return endpointStatus;
        }

        /// <summary>
        /// Checks endpoints for a specified dataset.
        /// </summary>
        /// <param name="dataset"></param>
        /// <returns></returns>
        [Route("utilities/status/{dataset}")]
        [Route("utilities/status/{dataset}/v1.0")]
        [HttpGet]
        public List<Dictionary<string, Dictionary<string, string>>> DatasetEndpointsCheck(string dataset)
        {
            List<string> validEndpoints = new List<string>() { "evapo", "evapotranspiration", "precip", "precipitation", "soilm", "soilmoisture", "subsurface", "subsurfaceflow", "runoff", "surfacerunoff", "temp", "temperature" };
            List<Dictionary<string, Dictionary<string, string>>> endpointStatus = new List<Dictionary<string, Dictionary<string, string>>>();
            switch (dataset)
            {
                case "evapo":
                case "evapotranspiration":
                    endpointStatus.Add(Models.WSUtilities.CheckEvapoEndpoints());
                    break;
                case "precip":
                case "precipitation":
                    endpointStatus.Add(Models.WSUtilities.CheckPrecipEndpoints());  
                    break;
                case "soilm":
                case "soilmoisture":
                    endpointStatus.Add(Models.WSUtilities.CheckSoilMEndpoints());
                    break;
                case "subsurface":
                case "subsurfaceflow":
                    endpointStatus.Add(Models.WSUtilities.CheckSubsurfaceEndpoints());
                    break;
                case "runoff":
                case "surfacerunoff":
                    endpointStatus.Add(Models.WSUtilities.CheckRunoffEndpoints());
                    break;
                case "temp":
                case "temperature":
                    endpointStatus.Add(Models.WSUtilities.CheckTempEndpoints());
                    break;
                default:
                    endpointStatus.Add(new Dictionary<string, Dictionary<string, string>>() { { "UNKNOWN DATASET", new Dictionary<string, string>() {
                        { "status", "dataset provided is not valid." },
                        { "provided dataset", dataset },
                        { "valid dataset values", String.Join(',', validEndpoints) }
                    } } });
                    break;
            }
            return endpointStatus;
        }
    }
}
