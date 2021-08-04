using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System;
using Data;
using Web.Services.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Web.Services.Controllers
{
    /************************** Controller Class *****************************/
    /// <summary>
    /// AQUATOX workflow controller class.
    /// </summary>
    [ApiVersion("0.1")]
    [Route("api/aquatox/workflow")]
    [Produces("application/json")]
    public class WSAquatoxWorkflowController : Controller
    {
        /// <summary>
        /// GET method for calling the AQUATOX workflow.
        /// </summary>
        /// <param name="task_id"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GET([FromQuery] string task_id)
        {
            try
            {
                WSAquatoxWorkflow aqt = new WSAquatoxWorkflow();
                string output = "";
                string errormsg = "";
                // Start workflow
                await Task.Run(() =>
                {
                    aqt.Run(task_id, ref output, out errormsg);
                });
                ITimeSeriesOutput err = aqt.CheckForErrors(errormsg);
                if (err == null)
                {
                    return Ok(JsonConvert.DeserializeObject<JObject>(output));
                }
                return Ok(err);
            }
            catch (Exception ex)
            {
                return Utilities.Logger.LogAPIException(ex, task_id);
            }
        }

        /// <summary>
        /// Given the task_id for a single comid, returns the archived results of 
        /// the state variables outputs.
        /// </summary>
        /// <param name="task_id"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200)]
        [Route("archive-results")]
        public async Task<IActionResult> GetArchivedOutput([FromQuery] string task_id)
        {
            try
            {
                WSAquatoxWorkflow aqt = new WSAquatoxWorkflow();
                WSAquatoxWorkflow.ConvertedArchive CA =
                    await aqt.ArchiveOutput(task_id, out string errormsg);
                ITimeSeriesOutput err = aqt.CheckForErrors(errormsg);
                if (err == null)
                {
                    return Ok(CA);
                }
                return Ok(err);
            }
            catch (Exception ex)
            {
                return Utilities.Logger.LogAPIException(ex, task_id);
            }
        }
    }
}
