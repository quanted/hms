using Data;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Services.Models;
using System.Text.Json;

namespace Web.Services.Controllers
{
    /// <summary>
    /// Label: Soil Moiture;
    /// Description: Soil moisture content for specified layer depths;
    /// </summary>
    public class SoilMoistureInput : TimeSeriesInput
    {
        // Add extra SoilMoisture specific variables here
        /// <summary>
        /// Description: List of requested soil moisture layers;
        /// Default: None;
        /// Options: ["0-10", "10-40", "40-100", "100-200", "0-100", "0-200"];
        /// Required: True;
        /// </summary>
        public List<string> Layers { get; set; }

        /// <summary>
        /// Description: Soil moisture data source;
        /// Default: "nldas";
        /// Options: ["nldas", "gldas"];
        /// Required: True;
        /// </summary>
        public new string Source { get; set; }
    }

    // --------------- Swashbuckle Examples --------------- //

    /// <summary>
    /// Swashbuckle SoilMoisture POST request example
    /// </summary>
    public class SoilMoistureInputExample : IExamplesProvider<SoilMoistureInput>
    {
        /// <summary>
        /// Get example function.
        /// </summary>
        /// <returns></returns>
        public SoilMoistureInput GetExamples()
        {
            SoilMoistureInput example = new SoilMoistureInput()
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
                },
                Layers = new List<string>()
                {
                    { "0-10" },
                    { "10-40" }
                }
            };
            return example;
        }
    }

    // --------------- SoilMoisture Controller --------------- //

    /// <summary>
    /// SoilMoisture controller for HMS.
    /// </summary>
    [ApiVersion("0.1")]             // Version 0.1 endpoint
    [Route("api/hydrology/soilmoisture")]    
    public class WSSoilMoistureController : Microsoft.AspNetCore.Mvc.Controller
    {
        /// <summary>
        /// POST method for submitting a request for soil moisture data.
        /// </summary>
        /// <param name="smInput">Parameters for retrieving SoilMoisture data. Required fields: DateTimeSpan.StartDate, DateTimeSpan.EndDate, Geometry.Point.Latitude, Geometry.Point.Longitude, Source</param>
        /// <returns>ITimeSeries</returns>
        [HttpPost]
        [ProducesResponseType(200)]
        public async Task<IActionResult> POST([FromBody]SoilMoistureInput smInput)
        {
            try
            {
                ((Data.TimeSeriesInput)smInput).Source = smInput.Source;
                WSSoilMoisture evapo = new WSSoilMoisture();
                ITimeSeriesOutput results = await evapo.GetSoilMoisture(smInput);
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
                exceptionLog.Fatal(System.Text.Json.JsonSerializer.Serialize(smInput, options));

                Utilities.ErrorOutput err = new Utilities.ErrorOutput();
                return new ObjectResult(err.ReturnError("Unable to complete request due to invalid request or unknown error."));
            }
        }
    }
}
