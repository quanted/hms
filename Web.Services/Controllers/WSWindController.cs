﻿using Data;
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
    /// Wind Input that implements TimeSeriesInput object
    /// </summary>
    public class WindInput : TimeSeriesInput
    {
        // Add extra wind specific variables here

        /// Wind component
        /// User request for U, V values and/or speed, direction. All input is made upper case for comparison.
        /// Valid values: U/V, SPEED/DIR (SPEED/DIRECTION), ALL
        /// Defaults: ALL 
        public string Component = "ALL";
    }


    // --------------- Swashbuckle Examples --------------- //

    /// <summary>
    /// Swashbuckle Wind POST request example
    /// </summary>
    public class WindInputExample : IExamplesProvider
    {
        /// <summary>
        /// Get example function.
        /// </summary>
        /// <returns></returns>
        public object GetExamples()
        {
            WindInput example = new WindInput()
            {
                Source = "nldas",
                Component = "all",
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
    /// Swashbuckle Wind POST request example
    /// </summary>
    public class WindInputExampleFull : IExamplesProvider
    {
        /// <summary>
        /// Get example function.
        /// </summary>
        /// <returns></returns>
        public object GetExamples()
        {
            WindInput example = new WindInput()
            {
                Source = "nldas",
                Component = "all",
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
    /// Swashbucle Wind Output example
    /// </summary>
    public class WindOutputExample : IExamplesProvider
    {
        /// <summary>
        /// Get example function.
        /// </summary>
        /// <returns></returns>
        public object GetExamples()
        {
            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput output = oFactory.Initialize();
            output.Dataset = "Wind";
            output.DataSource = "nldas";
            output.Metadata = new Dictionary<string, string>()
            {
                { "nldas_prod_name", "NLDAS_FORA0125_H.002" },
                { "nldas_param_short_name", "TMP2m" },
                { "nldas_param_name", "2-m above ground temperature" },
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


    // --------------- Wind Controller --------------- //

    /// <summary>
    /// Wind controller for HMS.
    /// </summary>
    [ApiVersion("0.1")]             // Version 0.1 endpoint
    [Route("api/meteorology/wind")]
    public class WSWindController : Controller
    {
        /// <summary>
        /// POST Method for getting wind data.
        /// </summary>
        /// <param name="tempInput">Parameters for retrieving wind data. Required fields: DateTimeSpan.StartDate, DateTimeSpan.EndDate, Geometry.Point.Latitude, Geometry.Point.Longitude, Source</param>
        /// <returns>ITimeSeries</returns>
        [HttpPost]
        [SwaggerResponseExample(200, typeof(WindOutputExample))]
        [SwaggerRequestExample(typeof(WindInput), typeof(WindInputExample))]
        public async Task<IActionResult> POST([FromBody]WindInput tempInput)
        {
            WSWind wind = new WSWind();
            ITimeSeriesOutput results = await wind.GetWind(tempInput);
            results.Metadata = Utilities.Metadata.AddToMetadata("request_url", this.Request.Path, results.Metadata);
            return new ObjectResult(results);
        }
    }
}