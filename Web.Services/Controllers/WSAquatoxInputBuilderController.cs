using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;
using System;
using Web.Services.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Data;

namespace Web.Services.Controllers
{
    ///
    public class LoadingsInput 
    {
        public Dictionary<string, bool> Flags { get; set; }
        public TimeSeriesOutput Loadings { get; set; }
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
    public class WSAquatoxInputBuilderControllerLoadingsConstantsInputExample : IExamplesProvider<LoadingsInput>
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
                Loadings = new TimeSeriesOutput()
                {
                    Data = new Dictionary<string, List<string>>()
                    {
                        ["TNH4Obj"] = new List<string>()
                        {
                            "-1",
                            "1.0",
                            "0.791"
                        }
                    }
                }
            };
            return example;
        }
    }

    /// <summary>
    /// Aquatox insert loadings with time series input example. 
    /// </summary>
    public class WSAquatoxInputBuilderControllerLoadingsTimeSeriesInputExample : IExamplesProvider<LoadingsInput>
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
                Loadings = new TimeSeriesOutput()
                {
                    Metadata = new Dictionary<string, string>()
                    {
                        ["column_1"] = "TNH4Obj",
                        ["column_1_type"] = "0",
                        ["column_1_multldg"] = "1.0",
                        ["column_2"] = "TNO3Obj",
                        ["column_2_type"] = "0",
                        ["column_2_multldg"] = "1.0"
                    },
                    Data = new Dictionary<string, List<string>>()
                    {
                        ["2013-01-01 00"] = new List<string>()
                        {
                            "0.791",
                            "0.549"
                        },
                        ["2013-01-01 01"] = new List<string>()
                        {
                            "1.101",
                            "0.856"
                        },
                        ["2013-01-01 02"] = new List<string>()
                        {
                            "1.265",
                            "1.159"
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
        /// <param name="input">Object of flags and TimeSeriesOutput of loadings</param>
        /// <returns>Base json for aquatox simulation with SV contaminants updated.</returns>
        [HttpPost]
        [ProducesResponseType(200)]
        [Route("loadings/insert-constants")]
        [SwaggerRequestExample(typeof(LoadingsInput), typeof(WSAquatoxInputBuilderControllerLoadingsConstantsInputExample))]
        public async Task<IActionResult> PostConstantLoadings([FromBody] LoadingsInput input)
        {
            try 
            {
                // Get template from flags
                string json = await WSAquatoxInputBuilder.GetBaseJson(input.Flags);
                // Insert loadings into template and return 
                json = await WSAquatoxInputBuilder.InsertConstantLoadings(json, input);
                return Ok(JsonConvert.DeserializeObject<JObject>(json));
            }
            catch (Exception ex)
            {
                return Utilities.Logger.LogAPIException(ex, input);
            }
        }

        /// <summary>
        /// POST method for inserting state variable loadings into Aquatox base json.
        /// </summary>
        /// <param name="input">Object of flags and dictionary of loadings</param>
        /// <returns>Base json for aquatox simulation with SV contaminants updated.</returns>
        [HttpPost]
        [ProducesResponseType(200)]
        [Route("loadings/insert-time-series")]
        [SwaggerRequestExample(typeof(LoadingsInput), typeof(WSAquatoxInputBuilderControllerLoadingsTimeSeriesInputExample))]
        public async Task<IActionResult> PostTimeSeriesLoadings([FromBody] LoadingsInput input)
        {
            try 
            {
                // Get template from flags
                string json = await WSAquatoxInputBuilder.GetBaseJson(input.Flags);
                // Insert loadings into template and return 
                json = await WSAquatoxInputBuilder.InsertTimeSeriesLoadings(json, input);
                return Ok(JsonConvert.DeserializeObject<JObject>(json));
            }
            catch (Exception ex)
            {
                return Utilities.Logger.LogAPIException(ex, input);
            }
        }
    }
}