﻿using Data;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections;
using System.Threading.Tasks;
using Web.Services.Models;
using System.Text.Json;

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

        /// <summary>
        /// OPTIONAL: Data file provided by the user.
        /// </summary>
        public string UserData { get; set; }
    }

    // --------------- Swashbuckle Examples --------------- //

    /// <summary>
    /// Swashbuckle Evapotranspiration POST request example
    /// </summary>
    public class EvapotranspirationInputExample : IExamplesProvider<EvapotranspirationInput>
    {
        /// <summary>
        /// Get example function.
        /// </summary>
        /// <returns></returns>
        public EvapotranspirationInput GetExamples()
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
                    Timezone = null
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
                AirTemperature = new Hashtable { { 1, 1.0 }, { 2, 1.0 }, { 3, 1.0 }, { 4, 1.0 }, { 5, 1.0 }, { 6, 1.0 }, { 7, 1.0 }, { 8, 1.0 }, { 9, 1.0 }, { 10, 1.0 }, { 11, 1.0 }, { 12, 1.0 } },
                UserData = "2015-01-01 00Z    -2.7000E-03\n2015 - 01 - 01 01Z - 1.9000E-03\n2015 - 01 - 01 02Z - 1.3000E-03\n2015 - 01 - 01 03Z - 9.0000E-04\n2015 - 01 - 01 04Z - 3.0000E-04"
            };
            return example;
        }
    }

    // --------------- Evapotranspiration Controller --------------- //

    /// <summary>
    /// Evapotranspiration controller for HMS.
    /// </summary>
    [ApiVersion("0.1")]             // Version 0.1 endpoint
    [Route("api/hydrology/evapotranspiration")]
    [Produces("application/json")]
    public class WSEvapotranspirationController : Microsoft.AspNetCore.Mvc.Controller
    {
        /// <summary>
        /// POST method for submitting a request for evapotranspiration data.
        /// </summary>
        /// <param name="evapoInput">Parameters for retrieving evapotranspiration data. Required fields: DateTimeSpan.StartDate, DateTimeSpan.EndDate, Geometry.Point.Latitude, Geometry.Point.Longitude, Source</param>
        /// <returns>ITimeSeries</returns>
        [HttpPost]
        [ProducesResponseType(200)]
        public async Task<IActionResult> POST([FromBody]EvapotranspirationInput evapoInput)
        {
            try
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();   //For Debugging
                WSEvapotranspiration evapo = new WSEvapotranspiration();
                ITimeSeriesOutput results = await evapo.GetEvapotranspiration(evapoInput);
                results.Metadata = Utilities.Metadata.AddToMetadata("request_url", this.Request.Path, results.Metadata);
                watch.Stop();
                string elapsed = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalMinutes.ToString();
                results.Metadata.Add("Time Elapsed", elapsed);
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