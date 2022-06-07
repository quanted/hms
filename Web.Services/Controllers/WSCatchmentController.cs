using Data;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Threading.Tasks;
using Web.Services.Models;
using System.Collections.Generic;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Web.Services.Controllers
{

    /// <summary>
    /// HMS API controller for retrieving catchment data.
    /// </summary>
    [Route("api/info/catchment")]
    [Produces("application/json")]
    public class WSCatchmentController : Microsoft.AspNetCore.Mvc.Controller
    {
        /// <summary>
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GETDefaultOutput([FromQuery] string comid, bool streamcat=false, bool geometry=false, bool nwis=false, bool streamGeometry=false, bool cn=false, bool network=false)
        {
            try
            {
                WSCatchment catchment = new WSCatchment();
                Dictionary<string, object> result = await catchment.Get(comid, streamcat, geometry, nwis, streamGeometry, cn, network);
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
