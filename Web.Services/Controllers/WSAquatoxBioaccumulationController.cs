using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Web.Services.Models;
using Swashbuckle.AspNetCore.Filters;
using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Serilog;
using Data;
using System.Text.Json;

namespace Web.Services.Controllers
{ 
    /***************** Swagger Example JSON **********************/
    /// <summary>
    /// AQUATOX Bioaccumulation input example.
    /// </summary>
    public class AQTBioaccumulationInputExample : IExamplesProvider<JObject>
    {
        /// <summary>
        /// Get Example.
        /// </summary>
        /// <returns></returns>
        public JObject GetExamples()
        {
            WSAquatoxBioaccumulation b = new WSAquatoxBioaccumulation();
            string text = b.GetValidJsonExampleString();
            try
            {
                JObject example = JObject.Parse(text);
                return example;
            }
            catch (JsonReaderException e)
            {
                Console.WriteLine(e.Message);
            }

            return JObject.Parse(@"{Error: 'Could not parse text.'}");
        }
    }


    /******************* AQUATOX Bioaccumulation Controller *************************/

    /// <summary>
    /// AQUATOX Bioaccumulation controller for HMS.
    /// </summary>
    [Obsolete]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiVersion("0.1")]
    [Route("api/aquatox/bioaccumulation")]
    [Produces("application/json")]
    public class WSAQTBioaccumulationController : Microsoft.AspNetCore.Mvc.Controller
    {
        /// <summary>
        /// POST method for running AQUATOX Bioaccumulation.
        /// </summary>
        /// <param name="json"></param>
        /// <returns>IActionResult</returns>
        /// 
        /// Note: The POST method attempts to map the body of the request to an object.
        ///       Without an object it can map to, the value obtained will be null. Newtonsoft
        ///       provides a generic JObject class that this function is able to map a generic JSON
        ///       object to. 
        ///       
        [HttpPost]
        [ProducesResponseType(200)]
        [SwaggerRequestExample(typeof(JObject), typeof(AQTBioaccumulationInputExample))]
        public async Task<IActionResult> POST([FromBody] JObject json)
        {
            try
            {
                WSAquatoxBioaccumulation b = new WSAquatoxBioaccumulation();
                string serializedJson = JsonConvert.SerializeObject(json);
                string errormsg = "";
                await Task.Run(() =>
                {
                    b.RunAQTBioaccumulation(ref serializedJson, out errormsg);
                });
                ITimeSeriesOutput err = b.CheckForErrors(errormsg);
                if (err == null)
                {
                    return Ok(serializedJson);
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
                exceptionLog.Fatal(System.Text.Json.JsonSerializer.Serialize(json, options));
                Utilities.ErrorOutput err = new Utilities.ErrorOutput();
                return new ObjectResult(err.ReturnError("Unable to complete request due to invalid request or unknown error."));
            }
        }

        /// <summary>
        /// GET request of an example JSON string for AQUATOX Bioaccumulation.
        /// </summary>
        /// <returns>IActionResult</returns>
        [HttpGet]
        [ProducesResponseType(200)]
        [Route("input/example")]
        public async Task<IActionResult> GetExampleString()
        {
            WSAquatoxBioaccumulation b = new WSAquatoxBioaccumulation();
            string text = "";
            await Task.Run(() => {
                text = b.GetValidJsonExampleString();
            });
            JObject json = JObject.Parse(text);
            return Ok(json);
        }

        /// <summary>
        /// GET request of example JSON file for AQUATOX Bioaccumulation.
        /// </summary>
        /// <returns>FileResult</returns>
        [HttpGet]
        [ProducesResponseType(200)]
        [Route("input/example/file")]
        public async Task<Microsoft.AspNetCore.Mvc.FileResult> GetExampleFile()
        {
            WSAquatoxBioaccumulation b = new WSAquatoxBioaccumulation();
            byte[] bytes = null;
            await Task.Run(() => {
                bytes = b.GetValidJsonExampleFile();
            });
            return File(bytes, System.Net.Mime.MediaTypeNames.Application.Octet, "AQUATOX_Bioaccumulation_JSON_Example.txt");
        }
    }
}
