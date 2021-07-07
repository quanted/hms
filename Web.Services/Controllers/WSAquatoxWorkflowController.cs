using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;
using System;
using Data;
using Web.Services.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using MongoDB.Bson;

namespace Web.Services.Controllers
{
    /*********************** Swagger Example JSONs ***************************/
    /// <summary>
    /// AQUATOX workflow input example. 
    /// </summary>
    public class WSAquatoxWorkflowControllerInputExample : IExamplesProvider<string>
    {
        /// <summary>
        /// Get Example.
        /// </summary>
        /// <returns></returns>
        public string GetExamples()
        {
            return "task_id";
        }
    }

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
        /// POST method for calling the AQUATOX workflow.
        /// </summary>
        /// <param name="task_id"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200)]
        [SwaggerRequestExample(typeof(string), typeof(WSAquatoxWorkflowControllerInputExample))]
        public async Task<IActionResult> POST([FromQuery] string task_id)
        {
            try
            {
                WSAquatoxWorkflow aqt = new WSAquatoxWorkflow();
                string output = "";
                string errormsg = "";
                // Start workflow
                await Task.Run(() => {
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

        ///
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<IActionResult> TEST()
        {
            BsonDocument data = new BsonDocument()
            {
                { "_id", "60bfbb301036b32c88d23235324"},
                { "input", await WSAquatoxInputBuilder.GetBaseJson(new Dictionary<string, bool>())},
                { "upstream", new BsonDocument() 
                    {
                        {"comid_1", "425436"},
                        {"comid_2", "355i587"}
                    }
                },
                { "dependencies", new BsonDocument() 
                    {
                        {"streamflow", ""}
                    }
                }
            };

            Utilities.MongoDB.InsertOne("hms_workflows", "data", data);

            return Ok();
        }
    }
}
