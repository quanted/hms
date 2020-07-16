using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Web.Services.Models;
using Data;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Filters;
using Serilog;

namespace Web.Services.Controllers
{
    /// <summary>
    /// Solar Data input object
    /// </summary>
    public class SolarInput
    {
        /// <summary>
        /// Input Dictionary, containing unknown values.
        /// </summary>
        public Dictionary<string, object> input { get; set; }
    }

    /// <summary>
    /// Input example taken from the default input values call.
    /// </summary>
    public class SolarInputExample : IExamplesProvider<SolarInput>
    {
        /// <summary>
        /// Gets example object.
        /// </summary>
        /// <returns></returns>
        public SolarInput GetExamples()
        {
            SolarInput sInput = new SolarInput();
            sInput.input = new Dictionary<string, object>()
                    {
                        { "contaminant name", "Methoxyclor" },
                        { "contaminant type", "Chemical" },
                        { "water type name", "Pure Water" },
                        { "min wavelength", 297.5 },
                        { "max wavelength", 330 },
                        { "longitude", "83.2" },
                        { "latitude(s)", new int[] { 40, -99, -99, -99, -99, -99, -99, -99, -99, -99 }},
                        { "season(s)", new string[]{ "Spring", "  ", "  ", "  "}},
                        { "atmospheric ozone layer", 0.3 },
                        { "initial depth (cm)", "0.001"},
                        { "final depth (cm)", "5"},
                        { "depth increment (cm)", "10"},
                        { "quantum yield", "0.32"},
                        { "refractive index", "1.34"},
                        { "elevation", "0"},
                        { "wavelength table", new Dictionary<string, object>()
                        {
                            { "297.50", new Dictionary<string, string>()
                            {
                                { "water attenuation coefficients (m**-1)", "0.069000" },
                                { "chemical absorption coefficients (L/(mole cm))", "11.100000" }
                            } },
                            { "300.00", new Dictionary<string, string>()
                            {
                                { "water attenuation coefficients (m**-1)", "0.061000" },
                                { "chemical absorption coefficients (L/(mole cm))", "4.6700000" }
                            } },
                            { "302.50", new Dictionary<string, string>()
                            {
                                { "water attenuation coefficients (m**-1)", "0.057000" },
                                { "chemical absorption coefficients (L/(mole cm))", "1.900000" }
                            } },
                            { "305.00", new Dictionary<string, string>()
                            {
                                { "water attenuation coefficients (m**-1)", "0.053000" },
                                { "chemical absorption coefficients (L/(mole cm))", "1.100000" }
                            } },
                            { "307.50", new Dictionary<string, string>()
                            {
                                { "water attenuation coefficients (m**-1)", "0.049000" },
                                { "chemical absorption coefficients (L/(mole cm))", "0.800000" }
                            } },
                            { "310.00", new Dictionary<string, string>()
                            {
                                { "water attenuation coefficients (m**-1)", "0.045000" },
                                { "chemical absorption coefficients (L/(mole cm))", "0.5300000" }
                            } },
                            { "312.50", new Dictionary<string, string>()
                            {
                                { "water attenuation coefficients (m**-1)", "0.043000" },
                                { "chemical absorption coefficients (L/(mole cm))", "0.330000" }
                            } },
                            { "315.00", new Dictionary<string, string>()
                            {
                                { "water attenuation coefficients (m**-1)", "0.041000" },
                                { "chemical absorption coefficients (L/(mole cm))", "0.270000" }
                            } },
                            { "317.50", new Dictionary<string, string>()
                            {
                                { "water attenuation coefficients (m**-1)", "0.039000" },
                                { "chemical absorption coefficients (L/(mole cm))", "0.1600000" }
                            } },
                            { "320.00", new Dictionary<string, string>()
                            {
                                { "water attenuation coefficients (m**-1)", "0.037000" },
                                { "chemical absorption coefficients (L/(mole cm))", "0.100000" }
                            } },
                            { "323.10", new Dictionary<string, string>()
                            {
                                { "water attenuation coefficients (m**-1)", "0.035000" },
                                { "chemical absorption coefficients (L/(mole cm))", "0.060000" }
                            } },
                            { "330.00", new Dictionary<string, string>()
                            {
                                { "water attenuation coefficients (m**-1)", "0.029000" },
                                { "chemical absorption coefficients (L/(mole cm))", "0.020000" }
                            } }
                }
            }
            };
            return sInput;
        }
    }

    /// <summary>
    /// HMS API controller for GC Solar data.
    /// </summary>
    [Route("api/water-quality/solar")]                      // Default endpoint
    [Produces("application/json")]
    public class WSSolarController : Controller
    {
        /// <summary>
        /// GET request for retrieving the default output values for the GCSolar module, 
        /// this is equivalent to selecting the third option from the start menu of the desktop application.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("run/")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GETDefaultOutput()
        {
            try
            {
                WSSolar solar = new WSSolar();
                Dictionary<string, object> result = await solar.GetGCSolarOutput();
                return new ObjectResult(result);
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

        /// <summary>
        /// GET request for retrieving the default input values for the GCSolar module,
        /// this is equivalent to selecting the first option from the start menu of the desktop application.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("inputs/")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GETDefaultInput()
        {
            try
            {
                WSSolar solar = new WSSolar();
                Dictionary<string, object> result = await solar.GetGCSolarDefaultInput();
                return new ObjectResult(result);
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

        /// <summary>
        /// POST request for retrieving solar data using custom values from the GCSolar module,
        /// this is equivalent to selecting the second option from the start menu of the desktop application.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("run/")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> POSTCustomInput([FromBody]SolarInput input)
        {
            try
            {
                WSSolar solar = new WSSolar();
                if (input.input is null)
                {
                    Dictionary<string, object> errorMsg = new Dictionary<string, object>()
                {
                    { "Input Error:", "No inputs found in the request or inputs contain invalid formatting." }
                };
                    return new ObjectResult(errorMsg);
                }
                else
                {
                    Dictionary<string, object> result = await solar.GetGCSolarOutput(input.input);
                    return new ObjectResult(result);
                }
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

        /// <summary>
        /// GET request for metadata on the inputs for the GCSolar module.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("inputs/metadata")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetInputMetadata()
        {
            try
            {
                WSSolar solar = new WSSolar();
                Dictionary<string, object> metadata = new Dictionary<string, object>();
                metadata["Input Variables"] = await solar.GetMetadata();
                return new ObjectResult(metadata);
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

    /// <summary>
    /// Input example for NOAA Solar Calculator POST
    /// </summary>
    public class SolarCalcInputExample :IExamplesProvider<SolarCalculatorInput>
    {
        /// <summary>
        /// Get example function
        /// </summary>  
        /// <returns></returns>
        public SolarCalculatorInput GetExamples()
        {
            SolarCalculatorInput example = new SolarCalculatorInput()
            {

                Model = "year",
                LocalTime = "12:00:00",
                DateTimeSpan = new DateTimeSpan()
                {
                    StartDate = new DateTime(2010, 01, 01),
                    EndDate = new DateTime(2010, 12, 31)
                },
                Geometry = new TimeSeriesGeometry()
                {
                    Point = new PointCoordinate()
                    {
                        Latitude = 40,
                        Longitude = -105
                    },
                    Timezone = new Timezone()
                    {
                        Offset = -7,
                    }
                }
            };
            return example;
        }
    }

    /// <summary>
    /// Meterology Solar HMS endpoint class
    /// </summary>
    [ApiVersion("0.1")]             // Version 0.1 endpoint
    [Route("api/meteorology/solar")]
    [Produces("application/json")]
    public class WSMeteorologySolarController : Controller
    {
        /// <summary>
        /// NOAA Solar Calculator 
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> POSTSolarCalculator([FromBody]SolarCalculatorInput i)
        {
            try
            {
                WSSolar solar = new WSSolar();
                Utilities.ErrorOutput error = new Utilities.ErrorOutput();
                ITimeSeriesOutput result;
                if (i is null)
                {
                    result = error.ReturnError("Input Error: No inputs found in the request or inputs contain invalid formatting.");
                }
                else
                {
                    result = await solar.RunSolarCalculator(i);

                }
                return new ObjectResult(result);
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