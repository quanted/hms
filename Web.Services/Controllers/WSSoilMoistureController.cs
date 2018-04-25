using Data;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Examples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Web.Services.Models;

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
    public class SoilMoistureInputExample : IExamplesProvider
    {
        /// <summary>
        /// Get example function.
        /// </summary>
        /// <returns></returns>
        public object GetExamples()
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
                    Timezone = new Timezone()
                    {
                        Name = "EST",
                        Offset = -5,
                        DLS = false
                    }
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

    /// <summary>
    /// Swashbuckle SoilMoisture POST request example
    /// </summary>
    public class SoilMoistureInputExampleFull : IExamplesProvider
    {
        /// <summary>
        /// Get example function.
        /// </summary>
        /// <returns></returns>
        public object GetExamples()
        {
            SoilMoistureInput example = new SoilMoistureInput()
            {
                Source = "nldas",
                DateTimeSpan = new DateTimeSpan()
                {
                    StartDate = new DateTime(2015, 01, 01),
                    EndDate = new DateTime(2015, 01, 08),
                    DateTimeFormat = "yyyy-MM-dd HH"
                },
                Geometry = new TimeSeriesGeometry()
                {
                    Description = "EPA Athens Office",
                    Point = new PointCoordinate()
                    {
                        Latitude = 33.925673,
                        Longitude = -83.355723
                    },
                    GeometryMetadata = new Dictionary<string, string>()
                    {
                        { "City", "Athens" },
                        { "State", "Georgia"},
                        { "Country", "United States" }
                    },
                    Timezone = new Timezone()
                    {
                        Name = "EST",
                        Offset = -5,
                        DLS = false
                    }
                },
                Layers = new List<string>()
                {
                    { "0-10" },
                    { "10-40" }
                },
                DataValueFormat = "E3",
                TemporalResolution = "default",
                TimeLocalized = true,
                Units = "default",
                OutputFormat = "json"
            };
            return example;
        }
    }

    /// <summary>
    /// Swashbucle SoilMoisture Output example
    /// </summary>
    public class SoilMoistureOutputExample : IExamplesProvider
    {
        /// <summary>
        /// Get example function.
        /// </summary>
        /// <returns></returns>
        public object GetExamples()
        {
            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput output = oFactory.Initialize();
            output.Dataset = "SoilMoisture";
            output.DataSource = "nldas";
            output.Metadata = new Dictionary<string, string>()
            {
                { "nldas_prod_name", "NLDAS_FORA0125_H.002" },
                { "nldas_param_short_name", "SOILM0-10cm" },
                { "nldas_param_name", "0-10 cm soil moisture content" },
                { "nldas_unit", "kg/m^2" },
                { "nldas_undef", "  9.9990e+20" },
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
                { "nldas_Request_time", "Fri Jun  2 20:00:24 2017" },
                { "nldas_column_1", "Date" },
                { "nldas_column_2", "0-10cm" }
            };
            output.Data = new Dictionary<string, List<string>>()
            {
                { "2015-01-01 00Z", new List<string>() { "2.325E+001" } },
                { "2015-01-01 01Z", new List<string>() { "2.327E+001" } },
                { "2015-01-01 02Z", new List<string>() { "2.330E+001" } },
                { "2015-01-01 03Z", new List<string>() { "2.333E+001" } },
                { "2015-01-01 04Z", new List<string>() { "2.337E+001" } },
                { "2015-01-01 05Z", new List<string>() { "2.341E+001" } },
            };
            return output;
        }
    }


    // --------------- SoilMoisture Controller --------------- //

    /// <summary>
    /// SoilMoisture controller for HMS.
    /// </summary>
    [Route("api/hydrology/soilmoisture")]
    public class WSSoilMoistureController : Controller
    {
        /// <summary>
        /// POST Method for getting SoilMoisture data.
        /// </summary>
        /// <param name="evapoInput">Parameters for retrieving SoilMoisture data. Required fields: DateTimeSpan.StartDate, DateTimeSpan.EndDate, Geometry.Point.Latitude, Geometry.Point.Longitude, Source</param>
        /// <returns>ITimeSeries</returns>
        [HttpPost]
        [Route("")]             // Default endpoint
        [Route("v1.0")]         // Version 1.0 endpoint
        //[SwaggerRequestExample(typeof(SoilMoistureInput), typeof(SoilMoistureInputExample))]
        [SwaggerResponseExample(200, typeof(SoilMoistureOutputExample))]
        [SwaggerRequestExample(typeof(SoilMoistureInput), typeof(SoilMoistureInputExampleFull))]
        public async Task<IActionResult> POST([FromBody]SoilMoistureInput evapoInput)
        {
            WSSoilMoisture evapo = new WSSoilMoisture();
            ITimeSeriesOutput results = await evapo.GetSoilMoisture(evapoInput);
            results.Metadata = Utilities.Metadata.AddToMetadata("request_url", this.Request.Path, results.Metadata);
            return new ObjectResult(results);
        }
    }
}
