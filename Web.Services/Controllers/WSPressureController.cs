using Data;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Web.Services.Models;

namespace Web.Services.Controllers
{
    /// <summary>
    /// Pressure Input that implements TimeSeriesInput object
    /// </summary>
    public class PressureInput : TimeSeriesInput
    {
        // Add extra pressure specific variables here

    }


    // --------------- Swashbuckle Examples --------------- //

    /// <summary>
    /// Swashbuckle Pressure POST request example
    /// </summary>
    public class PressureInputExample : IExamplesProvider
    {
        /// <summary>
        /// Get example function.
        /// </summary>
        /// <returns></returns>
        public object GetExamples()
        {
            PressureInput example = new PressureInput()
            {
                Source = "gldas",
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
                    Timezone = new Timezone()
                    {
                        Name = "EST",
                        Offset = -5,
                        DLS = false
                    }
                }
            };
            return example;
        }
    }

    /// <summary>
    /// Swashbucle Pressure Output example
    /// </summary>
    public class PressureOutputExample : IExamplesProvider
    {
        /// <summary>
        /// Get example function.
        /// </summary>
        /// <returns></returns>
        public object GetExamples()
        {
            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput output = oFactory.Initialize();
            output.Dataset = "Surface Air Pressure";
            output.DataSource = "gldas";
            output.Metadata = new Dictionary<string, string>()
            {
                { "nldas_begin_time", "2015/01/01/00" },
                { "nldas_end_time", "2015/01/01/05" },
                { "nldas_time_interval[hour]", "1" },
                { "nldas_tot_record", "5" },
                { "nldas_grid_y", "71" },
                { "nldas_grid_x", "333" },
                { "nldas_elevation[m]", "219.065796" },
                { "nldas_dlat", "0.125000" },
                { "nldas_dlon", "0.125000" },
                { "nldas_ydim(original data set)", "224" },
                { "nldas_xdim(original data set)", "464" },
                { "nldas_start_lat(original data set)", "  25.0625" },
                { "nldas_start_lon(original data set)", "-124.9375" },
                { "nldas_Last_update", "Fri Jun  2 15:41:10 2017" },
                { "nldas_begin_time_index", "315563" },
                { "nldas_end_time_index", "315731" },
                { "nldas_lat", "  33.9375" },
                { "nldas_lon", " -83.3125" },
                { "nldas_Request_time", "Fri Jun  2 20:00:24 2017" }
            };
            output.Data = new Dictionary<string, List<string>>()
            {
                { "2015-01-01 00Z", new List<string>() { "2.764E+002" } },
                { "2015-01-01 01Z", new List<string>() { "2.754E+002" } },
                { "2015-01-01 02Z", new List<string>() { "2.747E+002" } },
                { "2015-01-01 03Z", new List<string>() { "2.741E+002" } },
                { "2015-01-01 04Z", new List<string>() { "2.735E+002" } },
                { "2015-01-01 05Z", new List<string>() { "2.731E+002" } },
            };
            return output;
        }
    }


    // --------------- Pressure Controller --------------- //

    /// <summary>
    /// Pressure controller for HMS.
    /// </summary>
    [ApiVersion("0.1")]             // Version 0.1 endpoint
    [Route("api/meteorology/pressure")]
    public class WSPressureController : Controller
    {
        /// <summary>
        /// POST Method for getting pressure data.
        /// </summary>
        /// <param name="tempInput">Parameters for retrieving pressure data. Required fields: DateTimeSpan.StartDate, DateTimeSpan.EndDate, Geometry.Point.Latitude, Geometry.Point.Longitude, Source</param>
        /// <returns>ITimeSeries</returns>
        [HttpPost]
        [SwaggerResponseExample(200, typeof(PressureOutputExample))]
        [SwaggerRequestExample(typeof(PressureInput), typeof(PressureInputExample))]
        public async Task<IActionResult> POST([FromBody]PressureInput tempInput)
        {
            WSPressure press = new WSPressure();
            ITimeSeriesOutput results = await press.GetPressure(tempInput);
            results.Metadata = Utilities.Metadata.AddToMetadata("request_url", this.Request.Path, results.Metadata);
            return new ObjectResult(results);
        }
    }
}
