using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web.Services.Models;
using Swashbuckle.AspNetCore.Examples;

namespace Web.Services.Controllers
{
    /// <summary>
    /// Input example taken from the default input values call.
    /// </summary>
    public class GCSolarInputExample : IExamplesProvider
    {
        /// <summary>
        /// Gets example object.
        /// </summary>
        /// <returns></returns>
        public object GetExamples()
        {
            return new Dictionary<string, object>()
            {
                { "Containment Name", "Methoxyclor" },
                { "Water Type Name", "Pure Water" },
                { "Type of Atmosphere", "Terrestrial" },
                { "Longitude", "83.2" },
                { "Elevation", "0"},
                { "Quantum Yield", "0.32"},
                { "Initial Depth", "0.001"},
                { "Depth Increment", "10"},
                { "Final Depth", "5"},
                { "Refractive Index", "1.34"},
                { "Depth Point", "None"},
                { "Season(s)", new string[]{ "Spring", "  ", "  ", "  "}},
                { "Latitude(s)", new int[] { 40, -99, -99, -99, -99, -99, -99, -99, -99, -99 }},
                { "Input Table", new Dictionary<string, object>()
                {
                    { "0", new Dictionary<string, object>() {
                        { "Wavelength", "297.50" },
                        { "Water Attenuation Coefficients", "0.061000" },
                        { "Chemical Absorption Coefficients", "11.100000" }
                    }},
                    { "1", new Dictionary<string, object>() {
                        { "Wavelength", "300.00" },
                        { "Water Attenuation Coefficients", "0.069000" },
                        { "Chemical Absorption Coefficients", "4.670000" }
                    }},
                    { "2", new Dictionary<string, object>() {
                        { "Wavelength", "302.50" },
                        { "Water Attenuation Coefficients", "0.057000" },
                        { "Chemical Absorption Coefficients", "1.900000" }
                    }},
                    { "3", new Dictionary<string, object>() {
                        { "Wavelength", "305.00" },
                        { "Water Attenuation Coefficients", "0.053000" },
                        { "Chemical Absorption Coefficients", "1.100000" }
                    }},
                    { "4", new Dictionary<string, object>() {
                        { "Wavelength", "307.50" },
                        { "Water Attenuation Coefficients", "0.049000" },
                        { "Chemical Absorption Coefficients", "0.800000" }
                    }},
                    { "5", new Dictionary<string, object>() {
                        { "Wavelength", "310.00" },
                        { "Water Attenuation Coefficients", "0.045000" },
                        { "Chemical Absorption Coefficients", "0.530000" }
                    }},
                    { "6", new Dictionary<string, object>() {
                        { "Wavelength", "312.50" },
                        { "Water Attenuation Coefficients", "0.043000" },
                        { "Chemical Absorption Coefficients", "0.330000" }
                    }},
                    { "7", new Dictionary<string, object>() {
                        { "Wavelength", "315.00" },
                        { "Water Attenuation Coefficients", "0.041000" },
                        { "Chemical Absorption Coefficients", "0.270000" }
                    }},
                    { "8", new Dictionary<string, object>() {
                        { "Wavelength", "317.50" },
                        { "Water Attenuation Coefficients", "0.039000" },
                        { "Chemical Absorption Coefficients", "0.160000" }
                    }},
                    { "9", new Dictionary<string, object>() {
                        { "Wavelength", "320.00" },
                        { "Water Attenuation Coefficients", "0.037000" },
                        { "Chemical Absorption Coefficients", "0.100000" }
                    }},
                    { "10", new Dictionary<string, object>() {
                        { "Wavelength", "323.10" },
                        { "Water Attenuation Coefficients", "0.035000" },
                        { "Chemical Absorption Coefficients", "0.060000" }
                    }},
                    { "11", new Dictionary<string, object>() {
                        { "Wavelength", "333.00" },
                        { "Water Attenuation Coefficients", "0.029000" },
                        { "Chemical Absorption Coefficients", "0.020000" }
                    }},
                }
                }

            };
        }
    }

    /// <summary>
    /// HMS API controller for solar data.
    /// </summary>
    [Produces("application/json")]
    [Route("api/WSSolar")]
    public class WSSolarController : Controller
    {
        /// <summary>
        /// GET request for retrieving the default output values for the GCSolar module, 
        /// this is equivalent to selecting the third option from the start menu of the desktop application.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("run")]
        public Dictionary<string, object> GETDefaultOutput()
        {
            WSSolar solar = new WSSolar();
            Dictionary<string, object> result = solar.GetGCSolarOutput();
            return result;
        }

        /// <summary>
        /// GET request for retrieving the default input values for the GCSolar module,
        /// this is equivalent to selecting the first option from the start menu of the desktop application.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("inputs")]
        public Dictionary<string, object> GETDefaultInput()
        {
            WSSolar solar = new WSSolar();
            Dictionary<string, object> result = solar.GetGCSolarDefaultInput();
            return result;
        }

        /// <summary>
        /// POST request for retrieving solar data using custom values from the GCSolar module,
        /// this is equivalent to selecting the second option from the start menu of the desktop application.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("run")]
        [SwaggerRequestExample(typeof(Dictionary<string, object>), typeof(GCSolarInputExample))]
        public Dictionary<string, object> POSTCustomInput([FromBody]Dictionary<string, object> input)
        {
            WSSolar solar = new WSSolar();
            Dictionary<string, object> result = solar.GetGCSolarOutput(input);
            return result;
        }
    }
}