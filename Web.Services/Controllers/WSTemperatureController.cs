using Data;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Threading.Tasks;
using Web.Services.Models;

namespace Web.Services.Controllers
{

    /// <summary>
    /// Temperature Input that implements TimeSeriesInput object
    /// </summary>
    public class TemperatureInput : TimeSeriesInput
    {
        // Add extra evapotranspiration specific variables here
    }

    // --------------- Swashbuckle Examples --------------- //

    /// <summary>
    /// Swashbuckle Temperature POST request example
    /// </summary>
    public class TemperatureInputExample : IExamplesProvider<TemperatureInput>
    {
        /// <summary>
        /// Get example function.
        /// </summary>
        /// <returns></returns>
        public TemperatureInput GetExamples()
        {
            TemperatureInput example = new TemperatureInput()
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

    // --------------- Temperature Controller --------------- //

    /// <summary>
    /// Temperature controller for HMS.
    /// </summary>
    [ApiVersion("0.1")]             // Version 0.1 endpoint
    [Route("api/hydrology/temperature")]
    [Produces("application/json")]
    public class WSTemperatureController : Controller
    {
        /// <summary>
        /// POST method for submitting a request for evapotranspiration data.
        /// </summary>
        /// <param name="tempInput">Parameters for retrieving evapotranspiration data. Required fields: DateTimeSpan.StartDate, DateTimeSpan.EndDate, Geometry.Point.Latitude, Geometry.Point.Longitude, Source</param>
        /// <returns>ITimeSeries</returns>
        [HttpPost]
        [ProducesResponseType(200)]
        public async Task<IActionResult> POST([FromBody]TemperatureInput tempInput)
        {
            try
            {
                WSTemperature temp = new WSTemperature();
                ITimeSeriesOutput results = await temp.GetTemperature(tempInput);
                results.Metadata = Utilities.Metadata.AddToMetadata("request_url", this.Request.Path, results.Metadata);
                return new ObjectResult(results);
            }
            catch (Exception ex)
            {
                var exceptionLog = Log.ForContext("Type", "exception");
                exceptionLog.Fatal(ex.Message);
                exceptionLog.Fatal(ex.StackTrace);

                Utilities.ErrorOutput err = new Utilities.ErrorOutput();
                return new ObjectResult(err.ReturnError("Unable to complete request due to invalid request or unknown error."));
            }
        }
    }
}
