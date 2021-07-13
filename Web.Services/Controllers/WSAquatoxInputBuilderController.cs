using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;
using System;
using Web.Services.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Web.Services.Controllers
{
    public class LoadingsInput 
    {
        public Dictionary<string, bool> Flags { get; set; }
        public Dictionary<string, Loading> Loadings { get; set; }
    }

    public class Loading
    {
        public int Type { get; set; }
        public double Constant { get; set; }
        public SortedList<DateTime, double> TimeSeries { get; set; }
        public double MultLdg { get; set; }
    }

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
    /// AQUATOX workflow get base jsons input example. 
    /// </summary>
    public class WSAquatoxInputBuilderControllerLoadingsInputExample : IExamplesProvider<LoadingsInput>
    {
        /// <summary>
        /// Get Example.
        /// </summary>
        /// <returns></returns>
        public LoadingsInput GetExamples()
        {
            LoadingsInput example = new LoadingsInput()
            {
                Flags = new Dictionary<string, bool>()
                {
                    ["Nitrogen"] = true,
                    ["Phosphorus"] = false,
                    ["Organic Matter"] = false
                },
                Loadings = new Dictionary<string, Loading>()
                {
                    ["TNH4Obj"] = new Loading()
                    {
                        Type = -1,
                        Constant = 0.0853,
                        MultLdg = 1
                    }
                }
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
        /// <param name="input">Object of flags and dictionary of loadings</param>
        /// <returns>Base json for aquatox simulation with SV contaminants updated.</returns>
        [HttpPost]
        [ProducesResponseType(200)]
        [Route("insert-loadings")]
        public async Task<IActionResult> PostLoadingsConstant([FromBody] LoadingsInput input)
        {
            try 
            {
                // Get template from flags
                string json = await WSAquatoxInputBuilder.GetBaseJson(input.Flags);
                // Insert loadings into template and return 
                json = await WSAquatoxInputBuilder.InsertLoadings(json, input);
                return Ok(JsonConvert.DeserializeObject<JObject>(json));
            }
            catch (Exception ex)
            {
                return Utilities.Logger.LogAPIException(ex, input);
            }
        }
    }
}