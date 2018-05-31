using Data;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Examples;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Web.Services.Models;

namespace Web.Services.Controllers
{

    /// <summary>
    /// Evapotranspiration Input that implements TimeSeriesInput object
    /// </summary>
    public class EvapotranspirationInput : TimeSeriesInput
    {
        // Add extra evapotranspiration specific variables here
        /// <summary>
        /// REQUIRED: Algorithm used for Evapotranspiration.
        /// </summary>
        public string Algorithm { get; set; }

        /// <summary>
        /// REQUIRED: Albedo coefficient.
        /// </summary>
        public double Albedo { get; set; }

        /// <summary>
        /// REQUIRED: Central Longitude of Time Zone in degrees.
        /// </summary>
        public double CentralLongitude { get; set; }

        /// <summary>
        /// REQUIRED: Angle of the sun in degrees.
        /// </summary>
        public double SunAngle { get; set; }

        /// <summary>
        /// REQUIRED: The ability of a surface to emit radiant energy.
        /// </summary>
        public double Emissivity { get; set; }

        /// <summary>
        /// REQUIRED: Specifies if potential, actual, or wet environment evaporation are used.
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// REQUIRED: Zenith Albedo coefficient.
        /// </summary>
        public double Zenith { get; set; }

        /// <summary>
        /// REQUIRED: Surface area of lake in square kilometers.
        /// </summary>
        public double LakeSurfaceArea { get; set; }

        /// <summary>
        /// REQUIRED: Average depth of lake in meters.
        /// </summary>
        public double LakeDepth { get; set; }

        /// <summary>
        /// REQUIRED: Subsurface Resistance.
        /// </summary>
        public double SubsurfaceResistance { get; set; }

        /// <summary>
        /// REQUIRED: Stomatal Resistance.
        /// </summary>
        public double StomatalResistance { get; set; }

        /// <summary>
        /// REQUIRED: Leaf Width in meters.
        /// </summary>
        public double LeafWidth { get; set; }

        /// <summary>
        /// REQUIRED: Roughness Length in meters.
        /// </summary>
        public double RoughnessLength { get; set; }

        /// <summary>
        /// REQUIRED: Vegetation Height in meters.
        /// </summary>
        public double VegetationHeight { get; set; }

        /// <summary>
        /// REQUIRED: Monthly leaf area indices.
        /// </summary>
        public Hashtable LeafAreaIndices { get; set; }

        /// <summary>
        /// REQUIRED: Monthly air temperature coefficients.
        /// </summary>
        public Hashtable AirTemperature { get; set; }
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
                },
                Algorithm = "nldas",
                DataValueFormat = "E3",
                TemporalResolution = "default",
                TimeLocalized = true,
                Units = "default",
                OutputFormat = "json",
                Albedo = 0.23,
                CentralLongitude = 75.0,
                SunAngle = 17.2,
                Emissivity = 0.92,
                Model = "ETP",
                Zenith = 0.05,
                LakeSurfaceArea = 0.005,
                LakeDepth = 0.2,
                SubsurfaceResistance = 500.0,
                StomatalResistance = 400.0,
                LeafWidth = 0.02,
                RoughnessLength = 0.02,
                VegetationHeight = 0.12,
                LeafAreaIndices = new Hashtable { { 1, 2.51 }, { 2, 2.51 }, { 3, 2.51 }, { 4, 2.51 }, { 5, 2.51 }, { 6, 2.51 }, { 7, 2.51 }, { 8, 2.51 }, { 9, 2.51 }, { 10, 2.51 }, { 11, 2.51 }, { 12, 2.51 } },
                AirTemperature = new Hashtable { { 1, 1.0 }, { 2, 1.0 }, { 3, 1.0 }, { 4, 1.0 }, { 5, 1.0 }, { 6, 1.0 }, { 7, 1.0 }, { 8, 1.0 }, { 9, 1.0 }, { 10, 1.0 }, { 11, 1.0 }, { 12, 1.0 } }
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
                Algorithm = "nldas",
                DataValueFormat = "E3",
                TemporalResolution = "default",
                TimeLocalized = true,
                Units = "default",
                OutputFormat = "json",
                Albedo = 0.23,
                CentralLongitude = 75.0,
                SunAngle = 17.2,
                Emissivity = 0.92,
                Model = "ETP",
                Zenith = 0.05,
                LakeSurfaceArea = 0.005,
                LakeDepth = 0.2,
                SubsurfaceResistance = 500.0,
                StomatalResistance = 400.0,
                LeafWidth = 0.02,
                RoughnessLength = 0.02,
                VegetationHeight = 0.12,
                LeafAreaIndices = new Hashtable { { 1, 2.51 }, { 2, 2.51 }, { 3, 2.51 }, { 4, 2.51 }, { 5, 2.51 }, { 6, 2.51 }, { 7, 2.51 }, { 8, 2.51 }, { 9, 2.51 }, { 10, 2.51 }, { 11, 2.51 }, { 12, 2.51 } },
                AirTemperature = new Hashtable { { 1, 1.0 }, { 2, 1.0 }, { 3, 1.0 }, { 4, 1.0 }, { 5, 1.0 }, { 6, 1.0 }, { 7, 1.0 }, { 8, 1.0 }, { 9, 1.0 }, { 10, 1.0 }, { 11, 1.0 }, { 12, 1.0 } }
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
    [Route("api/hydrology/evapotranspiration")]
    public class WSEvapotranspirationController : Controller
    {
        /// <summary>
        /// POST Method for getting evapotranspiration data.
        /// </summary>
        /// <param name="evapoInput">Parameters for retrieving evapotranspiration data. Required fields: DateTimeSpan.StartDate, DateTimeSpan.EndDate, Geometry.Point.Latitude, Geometry.Point.Longitude, Source</param>
        /// <returns>ITimeSeries</returns>
        [HttpPost]
        [Route("")]                 // Default endpoint
        [Route("v1.0")]             // Version 1.0 endpoint 
        [SwaggerResponseExample(200, typeof(EvapotranspirationOutputExample))]
        [SwaggerRequestExample(typeof(EvapotranspirationInput), typeof(EvapotranspirationInputExampleFull))]
        public async Task<IActionResult> POST([FromBody]EvapotranspirationInput evapoInput)
        {
            WSEvapotranspiration evapo = new WSEvapotranspiration();
            ITimeSeriesOutput results = await evapo.GetEvapotranspiration(evapoInput);
            results.Metadata = Utilities.Metadata.AddToMetadata("request_url", this.Request.Path, results.Metadata);
            return new ObjectResult(results);
        }
    }
}