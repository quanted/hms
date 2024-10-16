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
    /// Label: Humidity;
    /// Description: Humidity input;
    /// </summary>
    public class HumidityInput : TimeSeriesInput
    {
        // Add extra Humidity specific variables here
        /// <summary>
        /// Description: Relative or Specific Humidity. Only relative humidity implemented.;
        /// Default: True;
        /// Options: True;
        /// Required: True;
        /// </summary>
        public bool Relative { get; set; }

        /// <summary>
        /// Description: Humidity data source;
        /// Default: "prism";
        /// Options: ["prism"];
        /// Required: True;
        /// </summary>
        public new string Source { get; set; }
    }

    // --------------- Swashbuckle Examples --------------- //

    /// <summary>
    /// Swashbuckle Humidity POST request example
    /// </summary>
    public class HumidityInputExample : IExamplesProvider<HumidityInput>
    {
        /// <summary>
        /// Get example function.
        /// </summary>
        /// <returns></returns>
        public HumidityInput GetExamples()
        {
            HumidityInput example = new HumidityInput()
            {
                Source = "prism",
                Relative = true,
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

    // --------------- Humidity Controller --------------- //

    /// <summary>
    /// Humidity controller for HMS.
    /// </summary>
    [ApiVersion("0.1")]             // Version 0.1 endpoint
    [Route("api/meteorology/humidity")]
    [Produces("application/json")]
    public class WSHumidityController : Microsoft.AspNetCore.Mvc.Controller
    {
        /// <summary>
        /// POST method for submitting a request for humidity data.
        /// </summary>
        /// <param name="tempInput">Parameters for retrieving humidity data. Required fields: DateTimeSpan.StartDate, DateTimeSpan.EndDate, Geometry.Point.Latitude, Geometry.Point.Longitude, Source</param>
        /// <returns>ITimeSeries</returns>
        [HttpPost]
        [ProducesResponseType(200)]
        public async Task<IActionResult> POST([FromBody]HumidityInput hInput)
        {
            try
            {
                ((Data.TimeSeriesInput)hInput).Source = hInput.Source;
                WSHumidity humid = new WSHumidity();
                ITimeSeriesOutput results = await humid.GetHumidity(hInput);
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
                exceptionLog.Fatal(System.Text.Json.JsonSerializer.Serialize(hInput, options));

                Utilities.ErrorOutput err = new Utilities.ErrorOutput();
                return new ObjectResult(err.ReturnError("Unable to complete request due to invalid request or unknown error."));
            }
        }
    }
}
