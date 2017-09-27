using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web.Services.Models;
using Newtonsoft.Json;

namespace Web.Services.Controllers
{
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
        [Route("DefaultOutput")]
        public string GETDefaultOutput()
        {
            WSSolar solar = new WSSolar();
            string result = solar.GetGCSolarDefaultOutput();
            return result;
        }

        /// <summary>
        /// GET request for retrieving the default input values for teh GCSolar module,
        /// this is equivalent to selecting the first option from the start menu of the desktop application.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("DefaultInput")]
        public string GETDefaultInput()
        {
            WSSolar solar = new WSSolar();
            string result = solar.GetGCSolarDefaultInput();
            return result;
        }
    }
}