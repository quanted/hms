using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Solar;
using GCSOLAR;

namespace Web.Services.Models
{
    /// <summary>
    /// Model for the WSSolar controller
    /// </summary>
    public class WSSolar
    {

        /// <summary>
        /// Calls into the Solar module and gets the default input data,
        /// equivlanet to selectin the first option from the windows start form.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> GetGCSolarDefaultInput()
        {
            GCSolar gcS = new GCSolar();
            return gcS.GetDefaultInputs();
        }

        /// <summary>
        /// Calls into the Solar module and gets the default data, 
        /// equivalent to selecting the third option from the windows start form.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> GetGCSolarOutput()
        {
            GCSolar gcS = new GCSolar();
            return gcS.GetOutput();
        }

        /// <summary>
        /// Calls into the Solar module and sets the Common variables using the input object and returns the data based on those inputs.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetGCSolarOutput(Dictionary<string, object> input)
        {
            GCSolar gcS = new GCSolar();
            gcS.SetCommonVariables(input);
            return gcS.GetOutput();
        }
    }
}