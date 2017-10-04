using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Solar;
using GCSOLAR;
using Newtonsoft.Json;

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
            List<string> errors = new List<string>();
            gcS.SetCommonVariables(input, out errors);
            if (errors.Count > 0)
            {
                return new Dictionary<string, object>()
                {
                    { "Input Errors", errors }
                };
            }
            return gcS.GetOutput();
        }

        /// <summary>
        /// Constructs input metadata for GCSolar module.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> GetMetadata()
        {
            GCSolar gcS = new GCSolar();
            string waves = JsonConvert.SerializeObject(gcS.common.getWave());
            Dictionary<string, object> metadata = new Dictionary<string, object>();
            metadata.Add("Contaminant Name", new Dictionary<string, string>()
            {
                { "Default Value", "Methoxyclor" },
                { "Possible Values", "null" },
                { "Description", "null" }
            });
            metadata.Add("Contaminant Type", new Dictionary<string, string>()
            {
                { "Default Value", "Chemical" },
                { "Description", "Corresponds to contaminant name." }
            });
            metadata.Add("Water Type Name", new Dictionary<string, string>()
            {
                { "Default Value", "Pure Water" },
                { "Possible Values", "Pure Water, Natural Water" },
                { "Description", "null" }
            });
            metadata.Add("Min Wavelength", new Dictionary<string, string>()
            {
                { "Default Value", "297.5" },
                { "Possible Values", waves },
                { "Description", "Minimum wavelength for calculated solar data. Must be less than maximum wavelength." }
            });
            metadata.Add("Max Wavelength", new Dictionary<string, string>()
            {
                { "Default Value", "330" },
                { "Possible Values", waves },
                { "Description", "Maximum wavelength for calculated solar data. Must be greater than minimum wavelength." }
            });
            metadata.Add("Notice", "Metadata is incomplete.");
            return metadata;
        }
    }
}