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

    /***************** Aquatox Workflow Contaminant Matrix Input Class **********************/
    /// <summary>
    /// 
    /// </summary>
    public class ContaminantMatrix
    {
        Dictionary<string, List<string>> matrix { get; set; }
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
        /// GET method for returning the AQUATOX workflow options/flags/modules.
        /// </summary>
        /// <returns>[ { "option" : "option_1" }, { "option" : "option_2" } ... ]</returns>
        [HttpGet]
        [ProducesResponseType(200)]
        [Route("options")]
        public IActionResult GetOptions()
        {
            WSAquatoxWorkflow aqt = new WSAquatoxWorkflow();
            JArray options = new JArray();
            int index = 0;
            foreach(string option in aqt.GetOptions()) 
            {
                options.Add(
                    new JObject
                    {
                        new JProperty("option", option),
                    }
                );
                index++;
            }
            return Ok(options);
        }

        /// <summary>
        /// POST method for returning the AQUATOX workflow base json based on set flags.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200)]
        [Route("options")]
        public async Task<IActionResult> PostOptionsBase([FromBody] List<bool> flags)
        {
            // Return base Json from flags
            try
            {
                WSAquatoxWorkflow aqt = new WSAquatoxWorkflow();
                string json = "";
                await Task.Run(() =>
                {
                    json = aqt.GetBaseJson(flags);
                });
                return Ok(JsonConvert.DeserializeObject(json));
            }
            catch (Exception ex)
            {
                Utilities.Logger.LogAPIException(ex, flags);
            }
        }

        /// <summary>
        /// POST method for returning the AQUATOX workflow base json based on set flags and updating base json 
        /// from contaminant matrix.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200)]
        [Route("options")]
        public async Task<IActionResult> PostOptions([FromBody] ContaminantMatrix input, List<int> flags)
        {
            // [
            //    comid_1 : SV[],
            //    comid_2 : SV[]
            // ]

            // 1. Get template from flags
            WSAquatoxWorkflow aqt = new WSAquatoxWorkflow();
            // JObject json = aqt.GetBaseJson(flags);

            // 2. Itterate over comids in CM

            // 3. Flush template, call JC function to put contaminant into SV for current comids
            // await

            // Return SV block for eaach catchment
            // Dictionary<comid, SV>
            //
            return Ok(input);
        }

        ///  public IActionResult PostOptions([FromBody] string input, List<int> flags)
        /// 

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
                
            }
        }
    }
}
