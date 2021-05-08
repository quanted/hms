using AQUATOXEcotoxicology;
using Data;
using System;
using System.IO;

namespace Web.Services.Models
{
    /// <summary>
    /// Pass through model object for calling AQTEcotoxicologyModel.
    /// </summary>
    public class WSAquatoxEcotoxicology
    {
        /// <summary>
        /// Calls AQTAnimals and runs simulation.
        /// </summary>
        public void RunAQTEcotoxicology(ref string json, out string errormsg)
        {
            new AQTEcotoxicologyModel(ref json, out errormsg, true);
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
        /// Returns the json example file for AQUATOX Ecotoxicology.
        /// </summary>
        /// <returns>byte[]</returns>
        public byte[] GetValidJsonExampleFile()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "..", "Ecotoxicology",
                "DOCS", "AQUATOX_Ecotoxicology_Valid.JSON");


            if (File.Exists(path))
            {
                return File.ReadAllBytes(path);
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
            string path = Path.Combine(Directory.GetCurrentDirectory(), "..", "Ecotoxicology",
                "DOCS", "AQUATOX_Ecotoxicology_Valid.JSON");


            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }
            else
            {
                return @"{Error: 'Example json file could not be found.'}";
            }
        }
    }
}
