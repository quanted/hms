using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mime;
using Serilog;

using Swashbuckle.AspNetCore.Filters;

using Web.Services.Models;
using Hawqs;

namespace Web.Services.Controllers
{
    // --------------- Swashbuckle Examples --------------- //
    /// <summary>
    /// Swashbuckle project/inputs GET response example
    /// </summary>
    public class InputsResponseExample : IExamplesProvider<HawqsExampleInputsResponse>
    {
        /// <summary></summary>
        /// <returns></returns>
        public HawqsExampleInputsResponse GetExamples()
        {
            return new HawqsExampleInputsResponse();
        }
    }
    // --------------- Swashbuckle Examples End --------------- //

    /// <summary>
    /// HMS API controller for retrieving HAWQS project inputs.
    /// </summary>
    [Route("api/hawqs/project/inputs")]
    [Produces(MediaTypeNames.Application.Json)]
    public class WSHawqsProjectInputsController : Microsoft.AspNetCore.Mvc.Controller
    {
        /// <summary>
        /// Returns HAWQS project input parameters as a JSON string
        /// </summary>
        /// <returns>
        /// A JSON string
        /// </returns>
        [HttpGet]
        [ProducesResponseType(typeof(HawqsExampleInputsResponse), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetProjectInputDefinitions()
        {
            try
            {
                WSHawqs hawqs = new WSHawqs();
                string result = await hawqs.GetProjectInputDefinitions();
                return Ok(result);
            }
            catch (Exception ex)
            {
                var exceptionlog = Log.ForContext("type", "exception");
                exceptionlog.Fatal(ex.Message);
                exceptionlog.Fatal(ex.StackTrace);

                Utilities.ErrorOutput err = new Utilities.ErrorOutput();
                ObjectResult response = new ObjectResult(err.ReturnError("Unable to retireive HAWQS input definitions."));
                response.StatusCode = 500;
                return response;
            }
        }
    }
}