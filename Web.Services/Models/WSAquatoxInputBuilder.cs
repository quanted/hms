using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using AQUATOX.AQSim_2D;
using AQUATOX.AQTSegment;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using Web.Services.Controllers;
using System.Globalization;

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
            foreach (string item in AQSim_2D.MultiSegSimFlags())
            {
                if (flags.ContainsKey(item))
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
            foreach (DictionaryEntry item in flagDict)
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
            else if (File.Exists(Path.Combine("app", "GUI",
                "GUI.AQUATOX", "2D_Inputs", "BaseJSON", AQSim_2D.MultiSegSimName(flagOptions))))
            {
                return File.ReadAllText(Path.Combine("app", "GUI",
                "GUI.AQUATOX", "2D_Inputs", "BaseJSON", AQSim_2D.MultiSegSimName(flagOptions)));
            }
            // Check for docker file path - 2
            else if (File.Exists(Path.Combine("src", "GUI",
                "GUI.AQUATOX", "2D_Inputs", "BaseJSON", AQSim_2D.MultiSegSimName(flagOptions))))
            {
                return File.ReadAllText(Path.Combine("src", "GUI",
                "GUI.AQUATOX", "2D_Inputs", "BaseJSON", AQSim_2D.MultiSegSimName(flagOptions)));
            }
            // Check for local testing file path 
            else if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "GUI",
                "GUI.AQUATOX", "2D_Inputs", "BaseJSON", AQSim_2D.MultiSegSimName(flagOptions))))
            {
                return File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "GUI",
                "GUI.AQUATOX", "2D_Inputs", "BaseJSON", AQSim_2D.MultiSegSimName(flagOptions)));
            }
            // Error 
            else
            {
                return "{\"Error:\" : \"Base json file could not be found.\"}";
            }
        }

        /// <summary>
        /// Method to insert constant loadings into an Aquatox simulation. 
        /// </summary>
        public static Task<string> InsertConstantLoadings(string json, LoadingsInput input)
        {
            AQTSim sim = new AQTSim();
            foreach (KeyValuePair<string, List<string>> item in input.Loadings.Data)
            {
                try
                {
                    // Insert loading and assign result over original input
                    json = sim.InsertLoadings(json, item.Key, int.Parse(item.Value[0]),
                        double.Parse(item.Value[2]), double.Parse(item.Value[1]));
                }
                catch (Exception ex)
                {
                    return Task.FromResult("Invalid loadings input.");
                }
            }
            return Task.FromResult(json);
        }

        /// <summary>
        /// Method to insert time series loadings into an Aquatox simulation. 
        /// </summary>
        public static Task<string> InsertTimeSeriesLoadings(string json, LoadingsInput input)
        {
            AQTSim sim = new AQTSim();
            try
            {
                // Get length of data in a single timeseries unit
                int length = input.Loadings.Data.First().Value.Count;

                // Split TimeSeriesOutput into array of SortedLists with length
                SortedList<DateTime, double>[] data = new SortedList<DateTime, double>[length];
                for (int i = 0; i < length; i++)
                {
                    data[i] = new SortedList<DateTime, double>();
                }
                foreach (KeyValuePair<string, List<string>> item in input.Loadings.Data)
                {
                    for (int i = 0; i < length; i++)
                    {
                        // Example: 
                        // [
                        //     [ "2013-01-01 00" : 0.3242],
                        // ]
                        data[i].Add(DateTime.ParseExact(item.Key, "yyyy-MM-dd HH", CultureInfo.InvariantCulture), double.Parse(item.Value[i]));
                    }
                }

                // Iterate over length of list array, get loading types, and call insert method
                for (int i = 0; i < length; i++)
                {
                    // Get loading column info
                    string name = input.Loadings.Metadata[$"column_{i + 1}"];
                    int type = int.Parse(input.Loadings.Metadata[$"column_{i + 1}_type"]);
                    double multldg = double.Parse(input.Loadings.Metadata[$"column_{i + 1}_multldg"]);

                    // Insert loading and assign result over original input
                    json = sim.InsertLoadings(json, name, type, data[i], multldg);
                }
            }
            catch (Exception ex)
            {
                return Task.FromResult("Invalid time series.");
            }
            return Task.FromResult(json);
        }
    }
}