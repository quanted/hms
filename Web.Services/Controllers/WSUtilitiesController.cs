using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Web.Services.Controllers
{

    /// <summary>
    /// Utility REST endpoints for HMS components.
    /// </summary>
    /*[ApiVersion("0.1")]             // Version 0.1 endpoint
    [Route("api")]                  // Default endpoints
    [Produces("application/json")]*/
    public class WSUtilitiesController : Controller
    {

        /// <summary>
        /// Checks endpoints for all datasets.
        /// </summary>
        /// <returns></returns>
/*        [Route("utilities/status")]
        [HttpGet]
        [ProducesResponseType(200)]*/
        public async Task<IActionResult> AllDatasetEndpointsCheck()
        {
            Task<Dictionary<string, Dictionary<string, string>>> evapo = Models.WSUtilities.CheckEvapoEndpoints();
            Dictionary<string, Dictionary<string, string>> evapoResults = await evapo;

            Task<Dictionary<string, Dictionary<string, string>>> precip = Models.WSUtilities.CheckPrecipEndpoints();
            Dictionary<string, Dictionary<string, string>> precipResults = await precip;

            Task<Dictionary<string, Dictionary<string, string>>> soilM = Models.WSUtilities.CheckSoilMEndpoints();
            Dictionary<string, Dictionary<string, string>> soilMResults = await soilM;

            Task<Dictionary<string, Dictionary<string, string>>> subsurface = Models.WSUtilities.CheckSubsurfaceEndpoints();
            Dictionary<string, Dictionary<string, string>> subsurfaceResults = await subsurface;

            Task<Dictionary<string, Dictionary<string, string>>> runoff = Models.WSUtilities.CheckRunoffEndpoints();
            Dictionary<string, Dictionary<string, string>> runoffResults = await runoff;

            Task<Dictionary<string, Dictionary<string, string>>> temp = Models.WSUtilities.CheckTempEndpoints();
            Dictionary<string, Dictionary<string, string>> tempResults = await temp;

            Dictionary<string, Dictionary<string, Dictionary<string, string>>> endpointStatus = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>
            {
                { "Evapotranspiration", evapoResults },
                { "Precipitation", precipResults },
                { "Soil Moisture", soilMResults },
                { "Subsurface Flow", subsurfaceResults },
                { "Surface Runoff", runoffResults },
                { "Temperature", tempResults }
            };
            return new ObjectResult(endpointStatus);
        }

        /// <summary>
        /// Checks endpoints for a specified dataset.
        /// Valid datasets: "evapo", "evapotranspiration", "precip", "precipitation", "soilm", "soilmoisture", "subsurface", "subsurfaceflow", "runoff", "surfacerunoff", "temp", "temperature"
        /// </summary>
        /// <param name="dataset"></param>
        /// <returns></returns>
/*        [Route("utilities/status/{dataset}")]
        [HttpGet]
        [ProducesResponseType(200)]*/
        public async Task<IActionResult> DatasetEndpointsCheck(string dataset)
        {
            List<string> validEndpoints = new List<string>() { "evapo", "evapotranspiration", "precip", "precipitation", "soilm", "soilmoisture", "subsurface", "subsurfaceflow", "runoff", "surfacerunoff", "temp", "temperature" };
            List<Dictionary<string, Dictionary<string, string>>> endpointStatus = new List<Dictionary<string, Dictionary<string, string>>>();
            switch (dataset)
            {
                case "evapo":
                case "evapotranspiration":
                    Task<Dictionary<string, Dictionary<string, string>>> evapo = Models.WSUtilities.CheckEvapoEndpoints();
                    Dictionary<string, Dictionary<string, string>> evapoResults = await evapo;
                    endpointStatus.Add(evapoResults);
                    break;
                case "precip":
                case "precipitation":
                    Task<Dictionary<string, Dictionary<string, string>>> precip = Models.WSUtilities.CheckPrecipEndpoints();
                    Dictionary<string, Dictionary<string, string>> precipResults = await precip;
                    endpointStatus.Add(precipResults);  
                    break;
                case "soilm":
                case "soilmoisture":
                    Task<Dictionary<string, Dictionary<string, string>>> soilM = Models.WSUtilities.CheckSoilMEndpoints();
                    Dictionary<string, Dictionary<string, string>> soilMResults = await soilM;
                    endpointStatus.Add(soilMResults);
                    break;
                case "subsurface":
                case "subsurfaceflow":
                    Task<Dictionary<string, Dictionary<string, string>>> subsurface = Models.WSUtilities.CheckSubsurfaceEndpoints();
                    Dictionary<string, Dictionary<string, string>> subsurfaceResults = await subsurface;
                    endpointStatus.Add(subsurfaceResults);
                    break;
                case "runoff":
                case "surfacerunoff":
                    Task<Dictionary<string, Dictionary<string, string>>> runoff = Models.WSUtilities.CheckRunoffEndpoints();
                    Dictionary<string, Dictionary<string, string>> runoffResults = await runoff;
                    endpointStatus.Add(runoffResults);
                    break;
                case "temp":
                case "temperature":
                    Task<Dictionary<string, Dictionary<string, string>>> temp = Models.WSUtilities.CheckTempEndpoints();
                    Dictionary<string, Dictionary<string, string>> tempResults = await temp;
                    endpointStatus.Add(tempResults);
                    break;
                default:
                    endpointStatus.Add(new Dictionary<string, Dictionary<string, string>>() { { "UNKNOWN DATASET", new Dictionary<string, string>() {
                        { "status", "dataset provided is not valid." },
                        { "provided dataset", dataset },
                        { "valid dataset values", String.Join(',', validEndpoints) }
                    } } });
                    break;
            }
            return new ObjectResult(endpointStatus);
        }
    }
}
