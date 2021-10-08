using Data;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Threading.Tasks;
using Web.Services.Models;
using System.Collections.Generic;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;


namespace Web.Services.Controllers
{

    /// <summary>
    /// HMS API controller for retrieving stream data.
    /// </summary>
    [Route("api/info/streamnetwork")]
    [Produces("application/json")]
    public class WSStreamController : Controller
    {
        /// <summary>
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GETDefaultOutput([FromQuery] string comid, string endComid=null, double maxDistance = 50.0, string mainstem="false", string huc=null)
        {
            try
            {
                maxDistance = (maxDistance > 100.0) ? 100.0 : maxDistance;      // Max maxDistance of 100km
                var stopwatch = Stopwatch.StartNew();
                WSStream catchment = new WSStream();
                Dictionary<string, object> result = await catchment.Get(comid, endComid, huc, maxDistance, mainstem.ToLower() == "true");
                var runtime = stopwatch.ElapsedMilliseconds;
                result["runtime"] = runtime.ToString() + " ms";
                string jsonResults = System.Text.Json.JsonSerializer.Serialize(result);
                JObject jResult = JsonConvert.DeserializeObject<JObject>(jsonResults);
                return new ObjectResult(jResult); ;
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
