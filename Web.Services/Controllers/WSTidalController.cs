using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using Web.Services.Models;

namespace Web.Services.Controllers
{
    // --------------- Swashbuckle Examples --------------- //

    /// <summary>
    /// Swashbuckle Precipitation POST request example
    /// </summary>
    public class TidalInputExample : IExamplesProvider<WSTidal>
    {
        /// <summary>
        /// Get example function.
        /// </summary>
        /// <returns></returns>
        public WSTidal GetExamples()
        {

            WSTidal example = new WSTidal()
            {
                begin_date = "20130101 10:00",
                end_date = "20130101 10:24",
                station = "8454000",
                product = "water_level",
                datum = "mllw",
                units = "metric",
                time_zone = "gmt",
                application = "web_services",
            };
            return example;
        }
    }

    /// <summary>
    /// API controller for handling requests for Tidal data.
    /// </summary>
    [Route("api/tidal")]
    [Produces("application/json")]
    public class WSTidalController : Controller
    {
        /// <summary>
        /// GET request that will return all of the parameters that can be used
        /// to make a tidal request.
        /// </summary>
        /// <returns>json of all tidal paramters</returns>
        [HttpGet]
        [ProducesResponseType(200)]
        [Route("info")]
        public IActionResult GetTidalInfo()
        {
            return Ok();
        }

        /// <summary>
        /// POST request that will return results from a tidal request.
        /// </summary>
        /// <returns>json of all tidal paramters</returns>
        [HttpPost]
        public async Task<IActionResult> GetTidalData([FromBody] WSTidal tidalRequest)
        {
            try
            {
                JObject result = new JObject();
                await Task.Run(() => {
                    result = tidalRequest.GetTidalData();
                });
                return Ok(result);
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
