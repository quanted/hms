using AQUATOXChemicals;
using Data;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Web.Services.Models
{
    /// <summary>
    /// Pass through model object for calling AQTChemicalModel.
    /// </summary>
    public class WSAquatoxChemicals
    {
        /// <summary>
        /// Calls AQTChemicalModel and runs simulation.
        /// </summary>
        public void RunAQTChemicals(ref string json, out string errormsg)
        {
            new AQTChemicalModel(ref json, out errormsg, true);
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
        /// Returns the json example file for AQUATOX Chemicals.
        /// </summary>
        /// <returns>byte[]</returns>
        public byte[] GetValidJsonExampleFile()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "..", "Chemicals",
                "DOCS", "AQUATOX_Chemical_Model_Valid.JSON");

            if (File.Exists(path))
            {
                return File.ReadAllBytes(path);
            }
            else if (File.Exists("/app/Chemicals/DOCS/AQUATOX_Chemical_Model_Valid.JSON"))
            {
                return File.ReadAllBytes("/app/Chemicals/DOCS/AQUATOX_Chemical_Model_Valid.JSON");
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
            string path = Path.Combine(Directory.GetCurrentDirectory(), "..", "Chemicals",
                "DOCS", "AQUATOX_Chemical_Model_Valid.JSON");

            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }
            else if (File.Exists("/app/Chemicals/DOCS/AQUATOX_Chemical_Model_Valid.JSON"))
            {
                return File.ReadAllText("/app/Chemicals/DOCS/AQUATOX_Chemical_Model_Valid.JSON");
            }
            else
            {
                return @"{Error: 'Example json file could not be found.'}";
            }
        }
    }
}
