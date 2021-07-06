using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using AQUATOX.AQSim_2D;
using System.Threading.Tasks;
using System.IO;

namespace Web.Services.Models
{
    /// <summary>
    ///
    /// </summary>
    public static class WSAquatoxInputBuilder
    {
        /// <summary>
        /// Returns the types of flags for getting a base simulation json.
        /// </summary>
        /// <returns>List of flag options</returns>
        public static Task<List<string>> GetBaseJsonFlags()
        {
            return Task.FromResult(AQSim_2D.MultiSegSimFlags());
        }

        /// <summary>
        /// Returns a base simulation json from file based on set flags.
        /// Defaults to whatever file is returned as if all parameters are false.
        /// </summary>
        /// <param name="flags">Dictionary of flag names and values</param>
        /// <returns>Base json string from file</returns>
        public static Task<string> GetBaseJson(Dictionary<string, bool> flags)
        {
            // Create ordered dictionary to guarantee flag order and populate.
            OrderedDictionary flagDict = new OrderedDictionary();
            foreach(string item in AQSim_2D.MultiSegSimFlags()) 
            {
                if(flags.ContainsKey(item)) 
                {
                    flagDict.Add(item, flags[item]);
                } 
                else 
                {
                    flagDict.Add(item, false);
                }
            }

            // Construct the flag list of bools
            List<bool> flagOptions = new List<bool>();
            foreach(DictionaryEntry item in flagDict) 
            {
                flagOptions.Add(Convert.ToBoolean(item.Value));
            }

            return Task.FromResult(GetBaseJsonHelper(flagOptions));
        }

        /// <summary>
        /// Helper for returning file from GetBaseJson. Made in an attempt to make code 
        /// modular and more readable.
        /// </summary>
        /// <param name="flagOptions">List of flags</param>
        /// <returns>Base json string from file</returns>
        public static string GetBaseJsonHelper(List<bool> flagOptions)
        {
            // Check local file path
            if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "..", "GUI", 
                "GUI.AQUATOX", "2D_Inputs", "BaseJSON", AQSim_2D.MultiSegSimName(flagOptions))))
            {
                return File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "..", "GUI", 
                "GUI.AQUATOX", "2D_Inputs", "BaseJSON", AQSim_2D.MultiSegSimName(flagOptions)));
            }
            // Check for docker file path 
            else if(File.Exists("/app/GUI/GUI.AQUATOX/2D_Inputs/BaseJSON/" + AQSim_2D.MultiSegSimName(flagOptions)))
            {
                return File.ReadAllText("/app/GUI/GUI.AQUATOX/2D_Inputs/BaseJSON/" + AQSim_2D.MultiSegSimName(flagOptions));
            }
            // Check for local testing file path 
            else if(File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "GUI", 
                "GUI.AQUATOX", "2D_Inputs", "BaseJSON", AQSim_2D.MultiSegSimName(flagOptions))))
            {
                return File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "GUI", 
                "GUI.AQUATOX", "2D_Inputs", "BaseJSON", AQSim_2D.MultiSegSimName(flagOptions)));
            }
            // Error 
            else
            {
                return "Base json file could not be found.";
            }
        }
    }
}