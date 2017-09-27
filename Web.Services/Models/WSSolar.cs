using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Solar;

namespace Web.Services.Models
{
    /// <summary>
    /// Model for the WSSolar controller
    /// </summary>
    public class WSSolar
    {
        /// <summary>
        /// Calls into the Solar module and gets the default data, 
        /// equivalent to selecting the third option from the windows start form.
        /// </summary>
        /// <returns></returns>
        public string GetGCSolarDefaultOutput()
        {
            GCSolar gcS = new GCSolar();
            string output = gcS.GetDefaultData();
            return output;
        }

        /// <summary>
        /// Calls into the Solar module and gets the default input data,
        /// equivlanet to selectin the first option from the windows start form.
        /// </summary>
        /// <returns></returns>
        public string GetGCSolarDefaultInput()
        {
            GCSolar gcS = new GCSolar();
            string output = gcS.GetDefaultInputs();
            return output;
        }
    }
}