﻿using Data;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Threading.Tasks;
using Web.Services.Models;
using System.Text.Json;

namespace Web.Services.Controllers
{
    /// <summary>
    /// SubSurfaceFlow Input that implements TimeSeriesInput object
    /// </summary>
    public class SubSurfaceFlowInput : TimeSeriesInput
    {
        /// <summary>
        /// OPTIONAL: Precipitation data source for Curve Number (NLDAS, GLDAS, NCDC, DAYMET, PRISM, WGEN)
        /// </summary>
        public string PrecipSource { get; set; }
    }

    // --------------- Swashbuckle Examples --------------- //

    /// <summary>
    /// Swashbuckle SubSurfaceFlow POST request example
    /// </summary>
    public class SubSurfaceFlowInputExample : IExamplesProvider<SubSurfaceFlowInput>
    {
        /// <summary>
        /// Get example function.
        /// </summary>
        /// <returns></returns>
        public SubSurfaceFlowInput GetExamples()
        {
            SubSurfaceFlowInput example = new SubSurfaceFlowInput()
            {
                Source = "nldas",
                DateTimeSpan = new DateTimeSpan()
                {
                    StartDate = new DateTime(2015, 01, 01),
                    EndDate = new DateTime(2015, 01, 08)
                },
                Geometry = new TimeSeriesGeometry()
                {
                    Point = new PointCoordinate()
                    {
                        Latitude = 33.925673,
                        Longitude = -83.355723
                    },
                    Timezone = null
                }
            };
            return example;
        }
    }

    // --------------- SubSurfaceFlow Controller --------------- //

    /// <summary>
    /// SubSurfaceFlow controller for HMS.
    /// </summary>
    [ApiVersion("0.1")]             // Version 0.1 endpoint
    [Route("api/hydrology/subsurfaceflow")]
    [Produces("application/json")]
    public class WSSubSurfaceFlowController : Controller
    {
        /// <summary>
        /// POST method for submitting a request for subsurface flow data.
        /// </summary>
        /// <param name="ssFlowInput">Parameters for retrieving SubSurfaceFlow data. Required fields: DateTimeSpan.StartDate, DateTimeSpan.EndDate, Geometry.Point.Latitude, Geometry.Point.Longitude, Source</param>
        /// <returns>ITimeSeries</returns>
        [HttpPost]
        [ProducesResponseType(200)]
        public async Task<IActionResult> POST([FromBody]SubSurfaceFlowInput ssFlowInput)
        {
            try
            {
                if (ssFlowInput.Geometry.GeometryMetadata == null && ssFlowInput.PrecipSource != null)
                {
                    ssFlowInput.Geometry.GeometryMetadata = new System.Collections.Generic.Dictionary<string, string>()
                    {
                        { "precipSource", ssFlowInput.PrecipSource }
                    };
                }
                WSSubSurfaceFlow ssFlow = new WSSubSurfaceFlow();
                ITimeSeriesOutput results = await ssFlow.GetSubSurfaceFlow(ssFlowInput);
                results.Metadata = Utilities.Metadata.AddToMetadata("request_url", this.Request.Path, results.Metadata);
                return new ObjectResult(results);
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
                exceptionLog.Fatal(System.Text.Json.JsonSerializer.Serialize(ssFlowInput, options));

                Utilities.ErrorOutput err = new Utilities.ErrorOutput();
                return new ObjectResult(err.ReturnError("Unable to complete request due to invalid request or unknown error."));
            }
        }
    }
}
