using Data;
using Plants.AQUATOX;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Web.Services.Models
{
    /// <summary>
    /// Pass through model object for calling AQTPlants Model.
    /// </summary>
    public class AQTPlant
    {
        /// <summary>
        /// Calls AQTPlants Model and runs simulation.
        /// </summary>
        public void RunAQTPlants(ref string json, out string errormsg)
        {
            new AQTPlants(ref json, out errormsg, true);
        }

        /// <summary>
        /// Utility function to check for error after running a simulation.
        /// </summary>
        /// <returns>ITimeSeriesOutput</returns>
        public ITimeSeriesOutput CheckForErrors(string errorMsg)
        {
            // Constructs default error output object containing error message.
            Utilities.ErrorOutput err = new Utilities.ErrorOutput();

            if (!String.IsNullOrEmpty(errorMsg) || errorMsg.ToUpper().Contains("ERROR"))
            {
                return err.ReturnError(errorMsg);
            }

            return null;
        }

        /// <summary>
        /// Returns the json example file for AQUATOX Plants.
        /// </summary>
        /// <returns>byte[]</returns>
        public byte[] GetValidJsonExampleFile()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "..",
            "Plants", "DOCS",
            "AQUATOX_Plant_Model_Complex_Valid.JSON");


            if (File.Exists(path))
            {
                return File.ReadAllBytes(path);
            }
            else if (File.Exists("/app/Plants/DOCS/AQUATOX_Plant_Model_Complex_Valid.JSON"))
            {
                return File.ReadAllBytes("/app/Plants/DOCS/AQUATOX_Plant_Model_Complex_Valid.JSON");
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the json as a string from valid text file.
        /// </summary>
        /// <returns>string</returns>
        public string GetValidJsonExampleString()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "..",
            "Plants", "DOCS",
            "AQUATOX_Plant_Model_Complex_Valid.JSON");


            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }
            else if (File.Exists("/app/Plants/DOCS/AQUATOX_Plant_Model_Complex_Valid.JSON"))
            {
                return File.ReadAllText("/app/Plants/DOCS/AQUATOX_Plant_Model_Complex_Valid.JSON");
            }
            else
            {
                return @"{Error: 'Example json file could not be found.'}";
            }
        }
    }
}
