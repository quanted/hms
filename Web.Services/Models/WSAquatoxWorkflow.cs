using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using Data;
using Web.Services.Controllers;
using AQUATOX.AQSim_2D;
using AQUATOX.AQTSegment;
using Newtonsoft.Json;
using MongoDB.Bson;
using System.Threading.Tasks;
using System.Linq;
using System.IO;

namespace Web.Services.Models
{
    public class WSAquatoxWorkflow : AQSim_2D
    {
        /// <summary>
        /// Returns the types of flags for getting a base simulation json.
        /// </summary>
        /// <returns>List of flag options</returns>
        public List<string> GetOptions()
        {
            return MultiSegSimFlags();
        }

        /// <summary>
        /// Returns a base simulation json from file based on set flags.
        /// </summary>
        /// <param name="flags">Dictionary of flag names and values</param>
        /// <returns>Base json string from file</returns>
        public string GetBaseJson(Dictionary<string, bool> flags)
        {
            // Create ordered dictionary to gaurauntee flag order and populate.
            OrderedDictionary flagDict = new OrderedDictionary();
            foreach(string item in MultiSegSimFlags()) 
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

            // Construct path and return base json
            string path = Path.Combine(Directory.GetCurrentDirectory(), "..", "GUI", 
                "GUI.AQUATOX", "2D_Inputs", "BaseJSON", MultiSegSimName(flagOptions));

            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }
            else if(File.Exists("/app/GUI/GUI.AQUATOX/2D_Inputs/BaseJSON" + MultiSegSimName(flagOptions)))
            {
                return File.ReadAllText("/app/GUI/GUI.AQUATOX/2D_Inputs/BaseJSON" + MultiSegSimName(flagOptions));
            }
            else
            {
                return @"{Error: 'Base json file could not be found.'}";
            }
        }

        /// <summary>
        /// Gets output from upstream segments from mongodb and merges them to
        /// the input of the current segment in input.sim_input. 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="json"></param>
        /// <param name="errormsg"></param>
        /// <returns>Serialized json string of the AQUATOX simulation output.</returns>
        public void Run(WSAquatoxWorkflowInput input, ref string json, out string errormsg)
        {
            // CheckDependencies()
            // 
            //     foreach => switch streamflow => AQSim_2D.UpdateDischarge()

            // Instantiate new simulation
            AQTSim sim = new AQTSim();
            errormsg = sim.Instantiate(json);
            if(errormsg != "") { return; }

            // Convert upstream comids to ints to run with AQSim_2D
            errormsg = convertUpstreamToInt(input.Upstream.Keys.ToList(), out List<int> comids);
            if (errormsg != "") { return; }

            // Get upstream outputs and archive them
            errormsg = ArchiveUpstreamOutputs(comids, input.Upstream);
            if (errormsg != "") { return; }

            // Pass data from upstream to current simulation
            Pass_Data(sim, comids);

            // Execute simulation
            errormsg = sim.Integrate();
            if (errormsg != "") { return; }

            // Save sim to json
            errormsg = sim.SaveJSON(ref json);
        }

        /// <summary>
        /// Itterate over each comid and convert to int.
        /// </summary>
        private string convertUpstreamToInt(List<string> upstream, out List<int> comids)
        {
            comids = new List<int>();
            foreach(string item in upstream)
            {
                if(int.TryParse(item, out int value)) 
                {
                    comids.Add(value);
                } 
                else 
                {
                    // Try parse failed
                    return $"Invalid Comid: {item}";
                }
            }
            return "";
        }

        /// <summary>
        /// Itterate over each comid and generate the archive results for each.
        /// </summary>
        /// <param name="comids">List of comids</param>
        /// <param name="upstream">Dictionary of comids and taskids</param>
        private string ArchiveUpstreamOutputs(List<int> comids, Dictionary<string, string> upstream)
        {
            foreach(int item in comids)
            {
                try
                {
                    // Get the sim output from database for current comid_taskid
                    upstream.TryGetValue(item.ToString(), out string value);
                    Task<BsonDocument> output = Utilities.MongoDB.FindByTaskID(value);

                    // Convert to string and instantiate a new simulation
                    string json = JsonConvert.SerializeObject(output.Result.GetValue("input"));
                    AQTSim sim = new AQTSim();
                    string error = sim.Instantiate(json);
                    if(error != "") 
                    {
                        return $"Invalid simulation input.";
                    }

                    // Archive the simulation to the inherited property: archive
                    archiveOutput(item, sim);
                }
                catch(Exception ex) 
                {
                    return "Invalid input, or unknown eror.";
                }
            }
            // No errors return empty string
            return "";
        }

        /// <summary>
        /// Auxiliary function to mimic how data is passed in AQSim_2D.executeModel() : Line 363
        /// </summary>
        /// <param name="sim">The currrent simulation to pass data to.</param>
        /// <param name="comids">List of comids</param>
        private void Pass_Data(AQTSim sim, List<int> comids)
        {
            // Pass the archived data to the current simulation
            int nSources = 0;
            foreach (int SrcId in comids)
            {
                nSources++;
                Pass_Data(sim, SrcId, nSources);
            };
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
    }
}
