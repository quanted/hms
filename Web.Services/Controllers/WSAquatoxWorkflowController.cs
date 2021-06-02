using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;
using System;
using Data;
using Web.Services.Models;
using System.Text.Json;
using Serilog;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using AQUATOX.AQTSegment;

namespace Web.Services.Controllers
{
    /***************** Aquatox Workflow Input Class **********************/
    /// <summary>
    /// 
    /// </summary>
    public class WSAquatoxWorkflowInput
    {
        public JObject Input { get; set; }
        public Dictionary<string, string> Upstream { get; set; }
        public Dictionary<string, string> Data_Sources { get; set; }
        public Dictionary<string, string> Dependencies { get; set; }
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
                Input = new JObject(),
                Upstream = new Dictionary<string, string>()
                {
                    ["comid1"] = "taskid_comid1",
                    ["comid2"] = "taskid_comid2",
                },
                Data_Sources = null,
                Dependencies = null
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
        [ProducesResponseType(200)]
        public async Task<IActionResult> POST([FromBody] WSAquatoxWorkflowInput input)
        {
            try
            {
                WSAquatoxWorkflow aqt = new WSAquatoxWorkflow();
                string json = JsonConvert.SerializeObject(input.Input);
                string errormsg = "";
                // Start workflow
                await Task.Run(() =>
                {
                    aqt.Run(input, ref json, out errormsg);
                });
                ITimeSeriesOutput err = aqt.CheckForErrors(errormsg);
                if (err == null)
                {
                    return Ok(JsonConvert.DeserializeObject<JObject>(json));
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
                exceptionLog.Fatal(System.Text.Json.JsonSerializer.Serialize(input, options));
                Utilities.ErrorOutput err = new Utilities.ErrorOutput();
                return new ObjectResult(err.ReturnError("Unable to complete request due to invalid request or unknown error."));
            }
        }
    }
}
