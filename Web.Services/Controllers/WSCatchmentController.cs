using Data;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Threading.Tasks;
using Web.Services.Models;
using System.Text.Json;
using System.Collections.Generic;

namespace Web.Services.Controllers
{


    /// <summary>
    /// HMS API controller for retrieving catchment data.
    /// </summary>
    [Route("api/info/catchment")]
    [Produces("application/json")]
    public class WSCatchmentController : Controller
    {
        /// <summary>
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GETDefaultOutput([FromQuery] string comid, bool streamcat=true, bool geometry=true, bool nwis=true, bool streamGeometry=false, bool cn=false)
        {
            try
            {
                WSCatchment catchment = new WSCatchment();
                Dictionary<string, object> result = await catchment.Get(comid, streamcat, geometry, nwis, streamGeometry, cn);
                return new ObjectResult(result);
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
