using Data;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Examples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Web.Services.Models;

namespace Web.Services.Controllers
{

    /// <summary>
    /// Evapotranspiration Input that implements TimeSeriesInput object
    /// </summary>
    public class EvapotranspirationInput : TimeSeriesInput
    {
        // Add extra evapotranspiration specific variables here
    }


    // --------------- Swashbuckle Examples --------------- //

    /// <summary>
    /// Swashbuckle Evapotranspiration POST request example
    /// </summary>
    public class EvapotranspirationInputExample : IExamplesProvider
    {
        /// <summary>
        /// Get example function.
        /// </summary>
        /// <returns></returns>
        public object GetExamples()
        {
            EvapotranspirationInput example = new EvapotranspirationInput()
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
    /// Swashbuckle Evapotranspiration POST request example
    /// </summary>
    public class EvapotranspirationInputExampleFull : IExamplesProvider
    {
        /// <summary>
        /// Get example function.
        /// </summary>
        /// <returns></returns>
        public object GetExamples()
        {
            EvapotranspirationInput example = new EvapotranspirationInput()
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
    /// Swashbucle Evapotranspiration Output example
    /// </summary>
    public class EvapotranspirationOutputExample : IExamplesProvider
    {
        /// <summary>
        /// Get example function.
        /// </summary>
        /// <returns></returns>
        public object GetExamples()
        {
            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput output = oFactory.Initialize();
            output.Dataset = "Evapotranspiration";
            output.DataSource = "nldas";
            output.Metadata = new Dictionary<string, string>()
            {
                { "nldas_prod_name", "NLDAS_FORA0125_H.002" },
                { "nldas_param_short_name", "EVPsfc" },
                { "nldas_param_name", "Total evapotranspiration" },
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
                { "2015-01-01 00Z", new List<string>() { "-1.400E-003" } },
                { "2015-01-01 01Z", new List<string>() { "-1.500E-003" } },
                { "2015-01-01 02Z", new List<string>() { "-1.300E-003" } },
                { "2015-01-01 03Z", new List<string>() { "-1.000E-003" } },
                { "2015-01-01 04Z", new List<string>() { "-9.000E-004" } },
                { "2015-01-01 05Z", new List<string>() { "-4.000E-004" } },
            };
            return output;
        }
    }


    // --------------- Evapotranspiration Controller --------------- //

    /// <summary>
    /// Evapotranspiration controller for HMS.
    /// </summary>
    public class WSEvapotranspirationController : Controller
    {
        /// <summary>
        /// POST Method for getting evapotranspiration data.
        /// </summary>
        /// <param name="evapoInput">Parameters for retrieving evapotranspiration data. Required fields: DateTimeSpan.StartDate, DateTimeSpan.EndDate, Geometry.Point.Latitude, Geometry.Point.Longitude, Source</param>
        /// <returns>ITimeSeries</returns>
        [HttpPost]
        [Route("api/evapotranspiration/")]
        //[SwaggerRequestExample(typeof(EvapotranspirationInput), typeof(EvapotranspirationInputExample))]
        [SwaggerResponseExample(200, typeof(EvapotranspirationOutputExample))]
        [SwaggerRequestExample(typeof(EvapotranspirationInput), typeof(EvapotranspirationInputExampleFull))]
        public ITimeSeriesOutput POST([FromBody]EvapotranspirationInput evapoInput)
        {
            WSEvapotranspiration evapo = new WSEvapotranspiration();
            ITimeSeriesOutput results = evapo.GetEvapotranspiration(evapoInput);
            return results;
        }
    }
}
