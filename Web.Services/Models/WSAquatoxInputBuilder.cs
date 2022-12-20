using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using Globals;
using AQUATOX.AQSim_2D;
using AQUATOX.AQTSegment;
using AQUATOX.OrgMatter;
using AQUATOX.Nutrients;
using System.Threading.Tasks;
using System.IO;
using Web.Services.Controllers;
using System.Linq;

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
            else if (File.Exists(Path.Combine("/app", "GUI",
                "GUI.AQUATOX", "2D_Inputs", "BaseJSON", AQSim_2D.MultiSegSimName(flagOptions))))
            {
                return File.ReadAllText(Path.Combine("/app", "GUI",
                "GUI.AQUATOX", "2D_Inputs", "BaseJSON", AQSim_2D.MultiSegSimName(flagOptions)));
            }
            // Check for docker file path - 2
            else if (File.Exists(Path.Combine("..", "src", "GUI",
                "GUI.AQUATOX", "2D_Inputs", "BaseJSON", AQSim_2D.MultiSegSimName(flagOptions))))
            {
                return File.ReadAllText(Path.Combine("..", "src", "GUI",
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
                return "{\"Error\" : \"Base json file could not be found. File: " + AQSim_2D.MultiSegSimName(flagOptions) + "\"}";
            }
        }

        /// <summary>
        /// Method to insert loadings into an Aquatox simulation. 
        /// </summary>
        public static Task<string> InsertLoadings(string json, LoadingsInput input)
        {
            try
            {
                // Iterate and insert all loadings
                AQTSim sim = new AQTSim();
                foreach (LoadingsObject loading in input.Loadings)
                {
                    string sv = loading.Param;
                    // Get loading metadata to assign for sv
                    if (loading.Metadata != null && loading.Metadata.Count > 0)
                    {
                        switch (loading.Metadata.Keys.First())
                        {
                            case "DataType":
                                // Can't assign DataType property inside AQTSegment.InsertLoadings
                                // without breaking desktop version.
                                sim.Instantiate(json);
                                TStateVariable TSV =
                                    sim.FirstSeg().GetStatePointer(AllVariables.DissRefrDetr, T_SVType.StV, T_SVLayer.WaterCol);
                                ((TDissRefrDetr)TSV).InputRecord.DataType = (DetrDataType)int.Parse(loading.Metadata["DataType"]);
                                sim.SaveJSON(ref json);
                                break;
                            case "TP_NPS":
                                sv = bool.Parse(loading.Metadata["TP_NPS"]) ? "TP" : loading.Param;
                                break;
                            case "TN_NPS":
                                sv = bool.Parse(loading.Metadata["TN_NPS"]) ? "TN" : loading.Param;
                                break;
                            default:
                                break;
                        }
                    }
                    if (loading.UseConstant)
                    {
                        json = sim.InsertLoadings(json, sv, loading.LoadingType,
                        loading.Constant, loading.multiplier);
                    }
                    else
                    {
                        json = sim.InsertLoadings(json, sv, loading.LoadingType,
                        loading.TimeSeries, loading.multiplier);
                    }
                }
            }
            catch (Exception ex)
            {
                return Task.FromResult("Invalid loadings input.");
            }
            return Task.FromResult(json);
        }
    }
}