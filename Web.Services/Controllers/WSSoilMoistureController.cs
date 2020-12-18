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
    /// SoilMoisture Input that implements TimeSeriesInput object
    /// </summary>
    public class SoilMoistureInput : TimeSeriesInput
    {
        // Add extra SoilMoisture specific variables here
        /// <summary>
        /// List of requested soil moisture layers
        /// </summary>
        public List<string> Layers { get; set; }
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
    public class WSSoilMoistureController : Controller
    {
        /// <summary>
        /// POST method for submitting a request for soil moisture data.
        /// </summary>
        /// <param name="evapoInput">Parameters for retrieving SoilMoisture data. Required fields: DateTimeSpan.StartDate, DateTimeSpan.EndDate, Geometry.Point.Latitude, Geometry.Point.Longitude, Source</param>
        /// <returns>ITimeSeries</returns>
        [HttpPost]
        [ProducesResponseType(200)]
        public async Task<IActionResult> POST([FromBody]SoilMoistureInput evapoInput)
        {
            try
            {
                WSSoilMoisture evapo = new WSSoilMoisture();
                ITimeSeriesOutput results = await evapo.GetSoilMoisture(evapoInput);
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
                exceptionLog.Fatal(System.Text.Json.JsonSerializer.Serialize(evapoInput, options));

                Utilities.ErrorOutput err = new Utilities.ErrorOutput();
                return new ObjectResult(err.ReturnError("Unable to complete request due to invalid request or unknown error."));
            }
        }
    }
}
