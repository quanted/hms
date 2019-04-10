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
    /// SurfaceRunoff Input that implements TimeSeriesInput object
    /// </summary>
    public class SurfaceRunoffInput : TimeSeriesInput
    {
        /// <summary>
        /// OPTIONAL: Precipitation data source for Curve Number (NLDAS, GLDAS, NCDC, DAYMET, PRISM, WGEN)
        /// </summary>
        public string PrecipSource { get; set; }

        /// <summary>
        /// Determines whether to use point-based runoff or area-based runoff
        /// </summary>
        public string RunoffType { get; set; }
        // Add extra SurfaceRunoff specific variables here
    }


    // --------------- Swashbuckle Examples --------------- //

    /// <summary>
    /// Swashbuckle SurfaceRunoff POST request example
    /// </summary>
    public class SurfaceRunoffInputExample : IExamplesProvider
    {
        /// <summary>
        /// Get example function.
        /// </summary>
        /// <returns></returns>
        public object GetExamples()
        {
            SurfaceRunoffInput example = new SurfaceRunoffInput()
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
                }
            };
            return example;
        }
    }

    /// <summary>
    /// Swashbuckle SurfaceRunoff POST request example
    /// </summary>
    public class SurfaceRunoffInputExampleFull : IExamplesProvider
    {
        /// <summary>
        /// Get example function.
        /// </summary>
        /// <returns></returns>
        public object GetExamples()
        {
            SurfaceRunoffInput example = new SurfaceRunoffInput()
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
    /// Swashbucle SurfaceRunoff Output example
    /// </summary>
    public class SurfaceRunoffOutputExample : IExamplesProvider
    {
        /// <summary>
        /// Get example function.
        /// </summary>
        /// <returns></returns>
        public object GetExamples()
        {
            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput output = oFactory.Initialize();
            output.Dataset = "SurfaceRunoff";
            output.DataSource = "nldas";
            output.Metadata = new Dictionary<string, string>()
            {
                { "nldas_prod_name", "NLDAS_FORA0125_H.002" },
                { "nldas_param_short_name", "SSRUNsfc" },
                { "nldas_param_name", "Surface runoff (non-infiltrating)" },
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
                { "nldas_Request_time", "Fri Jun  2 20:00:24 2017" }
            };
            output.Data = new Dictionary<string, List<string>>()
            {
                { "2015-01-01 00Z", new List<string>() { "0.000E+000" } },
                { "2015-01-01 01Z", new List<string>() { "0.000E+000" } },
                { "2015-01-01 02Z", new List<string>() { "0.000E+000" } },
                { "2015-01-01 03Z", new List<string>() { "0.000E+000" } },
                { "2015-01-01 04Z", new List<string>() { "0.000E+000" } },
                { "2015-01-01 05Z", new List<string>() { "0.000E+000" } },
            };
            return output;
        }
    }


    // --------------- SurfaceRunoff Controller --------------- //

    /// <summary>
    /// SurfaceRunoff controller for HMS.
    /// </summary>
    [ApiVersion("0.1")]
    [Route("api/hydrology/surfacerunoff/")]
    public class WSSurfaceRunoffController : Controller
    {
        /// <summary>
        /// POST Method for getting SurfaceRunoff data.
        /// </summary>
        /// <param name="runoffInput">Parameters for retrieving SurfaceRunoff data. Required fields: DateTimeSpan.StartDate, DateTimeSpan.EndDate, Geometry.Point.Latitude, Geometry.Point.Longitude, Source</param>
        /// <returns>ITimeSeries</returns>
        [HttpPost]
        [SwaggerResponseExample(200, typeof(SurfaceRunoffOutputExample))]
        [SwaggerRequestExample(typeof(SurfaceRunoffInput), typeof(SurfaceRunoffInputExampleFull))]
        public async Task<IActionResult> POST([FromBody]SurfaceRunoffInput runoffInput)
        {
            WSSurfaceRunoff runoff = new WSSurfaceRunoff();
            ITimeSeriesOutput results = await runoff.GetSurfaceRunoff(runoffInput);
            results.Metadata = Utilities.Metadata.AddToMetadata("request_url", this.Request.Path, results.Metadata);
            return new ObjectResult(results);
        }
    }
}
