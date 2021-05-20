using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using AQUATOX.AQTSegment;
using System.Collections.Generic;
using System;
using Data;
using Web.Services.Models;
using System.Text.Json;
using Serilog;

namespace Web.Services.Controllers
{
    /***************** Aquatox Workflow Input Class **********************/
    /// <summary>
    /// 
    /// </summary>
    public class WSAquatoxWorkflowInput
    {
        public AQTSim Sim_Input { get; set; }
        public Dictionary<string, string> Upstream { get; set; }
        public Dictionary<string, string> Data_Sources { get; set; }
    }

    /***************** Swagger Example JSON **********************/
    /// <summary>
    /// AQUATOX workflow input example. 
    /// </summary>
    public class WSAquatoxWorkflowControllerInputExample : IExamplesProvider<WSAquatoxWorkflowInput>
    {
        /// <summary>
        /// Get Example.
        /// </summary>
        /// <returns></returns>
        public WSAquatoxWorkflowInput GetExamples()
        {
            WSAquatoxWorkflowInput example = new WSAquatoxWorkflowInput()
            {
                Sim_Input = new AQTSim(),
                Upstream = new Dictionary<string, string>()
                {
                    ["comid1"] = "taskid_comid1",
                    ["comid2"] = "taskid_comid2",
                },
                Data_Sources = null
            };
            return example;
        }
    }

    /// <summary>
    /// AQUATOX workflow controller class.
    /// </summary>
    [ApiVersion("0.1")]
    [Route("api/aquatox/workflow")]
    [Produces("application/json")]
    public class WSAquatoxWorkflowController : Controller
    {
        /// <summary>
        /// POST method for calling the AQUATOX workflow.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> POST([FromBody] WSAquatoxWorkflowInput input)
        {
            try
            {
                WSAquatoxWorkflow aqt = new WSAquatoxWorkflow();
                string errormsg = "";
                string output = "";
                await Task.Run(() =>
                {
                    // Start workflow
                    output = aqt.Run(input, out errormsg);
                });
                ITimeSeriesOutput err = aqt.CheckForErrors(errormsg);
                if (err == null)
                {
                    return Ok(output);
                }
                return Ok(err);
            }
            catch (Exception ex)
            {
                var exceptionLog = Log.ForContext("Type", "exception");
                exceptionLog.Fatal(ex.Message);
                exceptionLog.Fatal(ex.StackTrace);
                JsonSerializerOptions options = new JsonSerializerOptions()
                {
                    AllowTrailingCommas = true,
                    PropertyNameCaseInsensitive = true
                };
                exceptionLog.Fatal(JsonSerializer.Serialize(input, options));
                Utilities.ErrorOutput err = new Utilities.ErrorOutput();
                return new ObjectResult(err.ReturnError("Unable to complete request due to invalid request or unknown error."));
            }
        }
    }
}
