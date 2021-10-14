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
        public List<LoadingsObject> Loadings { get; set; }
    }

    public class LoadingsObject
    {
        public string Param { get; set; }
        public int LoadingType { get; set; }
        public bool UseConstant { get; set; }
        public double Constant { get; set; }
        public SortedList<DateTime, double> TimeSeries { get; set; }
        public double multiplier { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }

    /// <summary>
    /// Aquatox workflow get base jsons input example. 
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
    /// Aquatox insert loadings with constant input example. 
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
                    ["Phosphorus"] = true,
                    ["Organic Matter"] = true
                },
                Loadings = new List<LoadingsObject>()
                {
                    new LoadingsObject()
                    {
                        Param = "TNO3Obj",
                        LoadingType = -1,
                        UseConstant = true,
                        Constant = 0.1,
                        TimeSeries = new SortedList<DateTime, double>()
                        {
                            [new DateTime(2019, 1, 1)] = 0.1,
                        },
                        multiplier = 1.0,
                        Metadata = new Dictionary<string, string>()
                        {
                            ["TN_NPS"] = "true",
                        }
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
        /// GET method for returning the Aquatox base json flags.
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
        /// POST method for returning the Aquatox workflow base json based on set flags.
        /// </summary>
        /// <param name="flags">A dictionary of [flag_name : bool]</param>
        /// <returns>Base json for aquatox simulation.</returns>
        [HttpPost]
        [ProducesResponseType(200)]
        [Route("base-json/flags")]
        [SwaggerRequestExample(typeof(Dictionary<string, bool>), typeof(WSAquatoxInputBuilderControllerGetBaseJsonInputExample))]
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
        /// GET method that returns dictionary of loading types and values
        /// </summary>
        /// -1 = inflow load,  0 = point-source, 1 = direct precipitation (maybe irrelevant), 2 = nonpoint-source
        [HttpGet]
        [ProducesResponseType(200)]
        [Route("loadings/types")]
        public IActionResult GetLoadings()
        {
            return Ok(new Dictionary<string, int>()
            {
                ["Inflow Load"] = -1,
                ["Point-source"] = 0,
                ["Direct Precipitation"] = 1,
                ["Nonpoint-source"] = 2
            });
        }

        /// <summary>
        /// POST method for inserting state variable loadings into Aquatox base json.
        /// </summary>
        /// <param name="input">Object of flags and list of LoadingsObjects</param>
        /// <returns>Updated "SV" contaminants.</returns>
        [HttpPost]
        [ProducesResponseType(200)]
        [Route("loadings/insert")]
        [SwaggerRequestExample(typeof(LoadingsInput), typeof(WSAquatoxInputBuilderControllerLoadingsInputExample))]
        public async Task<IActionResult> PostLoadings([FromBody] LoadingsInput input)
        {
            try
            {
                // Get template from flags
                string json = await WSAquatoxInputBuilder.GetBaseJson(input.Flags);
                // Insert loadings into template and return 
                json = await WSAquatoxInputBuilder.InsertLoadings(json, input);
                return Ok(JsonConvert.DeserializeObject<JObject>(json)["AQTSeg"]["SV"]);
            }
            catch (Exception ex)
            {
                return Utilities.Logger.LogAPIException(ex, input);
            }
        }
    }
}