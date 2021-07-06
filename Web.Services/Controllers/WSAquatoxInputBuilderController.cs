using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;
using System;
using Data;
using Web.Services.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Web.Services.Controllers
{

    /// <summary>
    /// AQUATOX workflow get base jsons input example. 
    /// </summary>
    public class WSAquatoxInputBuilderControllerGetBaseJsonInputExample : IExamplesProvider<Dictionary<string, bool>>
    {
        /// <summary>
        /// Get Example.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, bool> GetExamples()
        {
            Dictionary<string, bool> example = new Dictionary<string, bool>()
            {
                ["Nitrogen"] = true,
                ["Phosphorus"] = false,
                ["Organic Matter"] = false
            };
            return example;
        }
    }

    /// <summary>
    /// API controller for handling requests for Aquatox inputs. 
    /// </summary>
    [ApiVersion("0.1")]
    [Route("api/aquatox/input-builder")]
    [Produces("application/json")]
    public class WSAquatoxInputBuilderController : Controller
    {
        /// <summary>
        /// GET method for returning the AQUATOX base json flags.
        /// </summary>
        /// <returns>List of Aquatox base json flag names.</returns>
        [HttpGet]
        [ProducesResponseType(200)]
        [Route("base-json/flags")]
        public async Task<IActionResult> GetBaseJsonFlags()
        {
            return Ok(await WSAquatoxInputBuilder.GetBaseJsonFlags());
        }

        /// <summary>
        /// POST method for returning the AQUATOX workflow base json based on set flags.
        /// </summary>
        /// <param name="flags">A dictionary of [flag_name : bool]</param>
        /// <returns>Base json for aquatox simulation.</returns>
        [HttpPost]
        [ProducesResponseType(200)]
        [Route("base-json/flags")]
        [SwaggerRequestExample(typeof(Dictionary<string, bool> ), typeof(WSAquatoxInputBuilderControllerGetBaseJsonInputExample))]
        public async Task<IActionResult> PostBaseJsonFlags([FromBody] Dictionary<string, bool> flags)
        {
            try
            {
                string json = await WSAquatoxInputBuilder.GetBaseJson(flags);
                return Ok(JsonConvert.DeserializeObject(json));
            }
            catch (Exception ex)
            {
                return Utilities.Logger.LogAPIException(ex, flags);
            }
        }

        /// <summary>
        /// POST method for returning the AQUATOX workflow base json based on set flags and updating base json 
        /// with contaminant matrix values.
        /// </summary>
        /// <param name="input">Dictionary of flags and contanimants</param>
        /// <returns>Base json for aquatox simulation with SV contaminants updated.</returns>
        /*
        [HttpPost]
        [ProducesResponseType(200)]
        [Route("loadings")]
        public async Task<IActionResult> PostLoadings([FromBody] ContaminantMatrix input)
        {
            try 
            {
                // Get template from flags
                WSAquatoxWorkflow aqt = new WSAquatoxWorkflow();
                string json = await aqt.GetBaseJson(input.Flags);
                return Ok(input);
            }
            catch (Exception ex)
            {
                return Utilities.Logger.LogAPIException(ex, input);
            }
        }
        */
    }
}