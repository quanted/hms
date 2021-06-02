using System;
using System.Collections.Generic;
using Data;
using Web.Services.Controllers;
using AQUATOX.AQSim_2D;
using AQUATOX.AQTSegment;
using Newtonsoft.Json;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Web.Services.Models
{
    public class WSAquatoxWorkflow : AQSim_2D
    {
        /// <summary>
        /// Gets output from upstream segments from mongodb and merges them to
        /// the input of the current segment in input.sim_input. 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="errormsg"></param>
        /// <returns>Serialized json string of the AQUATOX simulation output.</returns>
        public void Run(WSAquatoxWorkflowInput input, ref string json, out string errormsg)
        {
            // Instantiate new simulation
            AQTSim sim = new AQTSim();
            errormsg = sim.Instantiate(json);
            if(errormsg != "") { return; }

            // Get upstream outputs and archive them
            ArchiveUpstreamOutputs(input.Upstream);

            // Pass data from upstream to current simulation
            // passData(sim, input);

            // Execute simulation
            errormsg = sim.Integrate();
            if (errormsg != "") { return; }

            // Save sim to json
            errormsg = sim.SaveJSON(ref json);
        }

        /// <summary>
        /// Itterate over each comid and generate the archive results for each
        /// one.
        /// </summary>
        private void ArchiveUpstreamOutputs(Dictionary<string, string> upstream)
        {
            foreach(KeyValuePair<string, string> item in upstream)
            {
                // Get the sim output from database for current comid/taskid
                Task<BsonDocument> output = Utilities.MongoDB.FindByTaskID(item.Value);

                // TODO: May need to remove extra database keys from output.

                // Convert to string and instaniate a new simulation
                string json = JsonConvert.SerializeObject(output);
                AQTSim sim = new AQTSim();
                sim.Instantiate(json);

                // Archive the simulation to the inherited property: archive
                archiveOutput(int.Parse(item.Key), sim);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sim"></param>
        /// <param name="sim2D"></param>
        /// <param name="input"></param>
        private void PassData(AQTSim sim, WSAquatoxWorkflowInput input)
        {
            // Pass the archived data to the current simulation
            int comid = 0;
            int nSources = 0;
            foreach (string SrcID in input.Upstream.Keys)
            {
                int id = int.Parse(SrcID);
                if (id != comid)  // set to itself in boundaries 
                {
                    nSources++;
                    Pass_Data(sim, id, nSources);
                }
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
