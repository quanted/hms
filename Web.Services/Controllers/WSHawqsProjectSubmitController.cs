using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mime;
using Serilog;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Swashbuckle.AspNetCore.Filters;

using Web.Services.Models;
using Hawqs;
using System.Collections.Generic;

namespace Web.Services.Controllers
{
    // --------------- Swashbuckle Examples --------------- //
    /// <summary>
    /// Swashbuckle project/submit POST request example
    /// </summary>
    public class SubmitInputExample : IExamplesProvider<HawqsDefaultSubmitRequest>
    {
        /// <summary></summary>
        /// <returns></returns>
        public HawqsDefaultSubmitRequest GetExamples()
        {
            return new HawqsDefaultSubmitRequest();
        }
    }
    /// <summary>
    /// Swashbuckle project/submit POST response example
    /// </summary>
    public class SubmitResponseExample : IExamplesProvider<HawqsExampleSubmitResponse>
    {
        /// <summary></summary>
        /// <returns></returns>
        public HawqsExampleSubmitResponse GetExamples()
        {
            return new HawqsExampleSubmitResponse();
        }
    }

    /// <summary>
    /// Swashbuckle project/status POST request example
    /// </summary>
    public class StatusInputExample : IExamplesProvider<HawqsDefaultStatusRequest>
    {
        /// <summary></summary>
        /// <returns></returns>
        public HawqsDefaultStatusRequest GetExamples()
        {
            return new HawqsDefaultStatusRequest();
        }
    }
    /// <summary>
    /// Swashbuckle project/status POST response example
    /// </summary>
    public class StatusResponseExample : IExamplesProvider<HawqsExampleStatusResponse>
    {
        /// <summary></summary>
        /// <returns></returns>
        public HawqsExampleStatusResponse GetExamples()
        {
            return new HawqsExampleStatusResponse();
        }
    }

    /// <summary>
    /// Swashbuckle project/data POST request example
    /// </summary>
    public class DataInputExample : IExamplesProvider<HawqsDefaultDataRequest>
    {
        /// <summary></summary>
        /// <returns></returns>
        public HawqsDefaultDataRequest GetExamples()
        {
            return new HawqsDefaultDataRequest();
        }
    }
    /// <summary>
    /// Swashbuckle project/data POST response example
    /// </summary>
    public class DataResponseExample : IExamplesProvider<HawqsExampleDataResponse>
    {
        /// <summary></summary>
        /// <returns></returns>
        public HawqsExampleDataResponse GetExamples()
        {
            return new HawqsExampleDataResponse();
        }
    }

    /// <summary>
    /// Swashbuckle project/cancel POST request example
    /// </summary>
    public class CancelInputExample : IExamplesProvider<HawqsDefaultCancelRequest>
    {
        /// <summary></summary>
        /// <returns></returns>
        public HawqsDefaultCancelRequest GetExamples()
        {
            return new HawqsDefaultCancelRequest();
        }
    }
    /// <summary>
    /// Swashbuckle project/cancel POST response example
    /// </summary>
    public class CancelResponseExample : IExamplesProvider<HawqsExampleCancelResponse>
    {
        /// <summary></summary>
        /// <returns></returns>
        public HawqsExampleCancelResponse GetExamples()
        {
            return new HawqsExampleCancelResponse();
        }
    }
    // --------------- Swashbuckle Examples End --------------- //

    /// <summary>
    /// HMS API controller for hawqs project submission, status, and data.
    /// </summary>
    [Route("api/hawqs/project")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public class WSHawqsProjectSubmitController : Microsoft.AspNetCore.Mvc.Controller
    {
        /// <summary>
        /// Submits and executes a HAWQS project. Returns project id and status url as a JSON string
        /// </summary>
        /// <returns>
        /// job id and data url or error
        /// </returns>
        [HttpPost]
        [Route("submit")]
        [SwaggerRequestExample(typeof(HawqsSubmitRequest), typeof(SubmitInputExample))]
        [ProducesResponseType(typeof(HawqsExampleSubmitResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> SubmitProject([FromBody] HawqsSubmitRequest request)
        {
            WSHawqs hawqs = new WSHawqs();
            try
            {
                string result = await hawqs.SubmitProject(request.apiKey, request.inputData);
                JObject jResult = JsonConvert.DeserializeObject<JObject>(result);
                return new ObjectResult(jResult);
            }
            catch (Exception ex)
            {
                var exceptionLog = Log.ForContext("Type", "exception");
                exceptionLog.Fatal(ex.Message);
                exceptionLog.Fatal(ex.StackTrace);

                Utilities.ErrorOutput err = new Utilities.ErrorOutput();
                ObjectResult response = new ObjectResult(err.ReturnError("Unable to complete Hawqs project submission."));
                response.StatusCode = 500;
                return response;
            }
        }

        /// <summary>
        /// Returns project progress percentage as a JSON string
        /// </summary>
        /// <returns>
        /// Returns JSON string
        /// </returns>
        [HttpPost]
        [Route("status/{projectId}")]
        [SwaggerRequestExample(typeof(HawqsStatusRequest), typeof(StatusInputExample))]
        [ProducesResponseType(typeof(HawqsExampleStatusResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetProjectStatus([FromBody] HawqsStatusRequest request, string projectId)
        {
            WSHawqs hawqs = new WSHawqs();
            try
            {
                HawqsStatus result = await hawqs.GetProjectStatus(request.apiKey, projectId);
                return new ObjectResult(result);
            }
            catch (Exception ex)
            {
                var exceptionLog = Log.ForContext("Type", "exception");
                exceptionLog.Fatal(ex.Message);
                exceptionLog.Fatal(ex.StackTrace);

                Utilities.ErrorOutput err = new Utilities.ErrorOutput();
                ObjectResult response = new ObjectResult(err.ReturnError("Unable to retreive HAWQS project status."));
                response.StatusCode = 500;
                return response;
            }
        }

        /// <summary>
        /// Returns list of web URLs for data retrevial as a JSON string
        /// </summary>
        /// <returns>
        /// Returns JSON string
        /// </returns>
        [HttpPost]
        [Route("data/{projectId}")]
        [SwaggerRequestExample(typeof(HawqsDataRequest), typeof(DataInputExample))]
        [ProducesResponseType(typeof(HawqsExampleDataResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetProjectData([FromBody] HawqsDataRequest request, string projectId)
        {
            WSHawqs hawqs = new WSHawqs();
            try
            {
                List<HawqsOutput> result = await hawqs.GetProjectData(request.apiKey, projectId, request.process);
                return new ObjectResult(result);
            }
            catch (Exception ex)
            {
                var exceptionLog = Log.ForContext("Type", "exception");
                exceptionLog.Fatal(ex.Message);
                exceptionLog.Fatal(ex.StackTrace);

                Utilities.ErrorOutput err = new Utilities.ErrorOutput();
                ObjectResult response = new ObjectResult(err.ReturnError("Unable to retieive HAWQS project data."));
                response.StatusCode = 500;
                return response;
            }
        }

        /// <summary>
        /// Cancels project execution
        /// </summary>
        /// <returns>
        /// </returns>
        [HttpPost]
        [Route("cancel/{projectId}")]
        [SwaggerRequestExample(typeof(HawqsCancelRequest), typeof(CancelInputExample))]
        [ProducesResponseType(typeof(HawqsExampleCancelResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> CancelProjectExecution([FromBody] HawqsCancelRequest request, string projectId)
        {
            WSHawqs hawqs = new WSHawqs();
            try
            {
                string result = await hawqs.CancelProjectExecution(request.apiKey, projectId);
                JObject jResult = JsonConvert.DeserializeObject<JObject>(result);
                return new ObjectResult(jResult);
            }
            catch (Exception ex)
            {
                var exceptionLog = Log.ForContext("Type", "exception");
                exceptionLog.Fatal(ex.Message);
                exceptionLog.Fatal(ex.StackTrace);

                Utilities.ErrorOutput err = new Utilities.ErrorOutput();
                ObjectResult response = new ObjectResult(err.ReturnError("Unable to cancel HAWQS project."));
                response.StatusCode = 500;
                return response;
            }
        }
    }
}