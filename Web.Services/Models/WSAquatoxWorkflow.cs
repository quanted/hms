using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using Data;
using Web.Services.Controllers;
using AQUATOX.AQSim_2D;
using AQUATOX.AQTSegment;
using AQUATOX.Volume;
using Globals;
using MongoDB.Bson;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using Newtonsoft.Json;

namespace Web.Services.Models
{
    /// <summary>
    /// Web.Services model class for running an Aquatox simulation.
    /// </summary>
    public class WSAquatoxWorkflow : AQSim_2D
    {
        private Dictionary<string, string> upstream;
        private Dictionary<string, string> dependencies;


        /// <summary>
        /// Gets output from upstream segments from mongodb and merges them to
        /// the input of the current segment simulation.
        /// </summary>
        /// <param name="task_id">Task ID of current simulation in database.</param>
        /// <param name="json">Stringified sim output.</param>
        /// <param name="errormsg">Error message or empty string.</param>
        /// <returns>Serialized json string of the AQUATOX simulation output.</returns>
        public void Run(string task_id, ref string json, out string errormsg)
        {
            // Get simulation from database
            errormsg = GetCurrentSim(task_id);
            if (errormsg != "") { return; }

            // Instantiate new simulation
            AQTSim sim = new AQTSim();
            try { errormsg = sim.Instantiate(baseSimJSON); }
            catch (NullReferenceException ex)
            {
                // Can't instantiate from empty sim or incorrect sim format given
                errormsg = "Error starting simulation. Incorrect simulation input given.";
            }
            if (errormsg != "") { return; }

            // Check for dependencies in input
            errormsg = CheckDependencies(sim);
            if (errormsg != "") { return; }

            // Convert upstream comids to ints to run with AQSim_2D
            errormsg = ConvertUpstreamToInt(upstream.Keys.ToList(), out List<int> comids);
            if (errormsg != "") { return; }

            // Get upstream outputs and archive them
            errormsg = ArchiveUpstreamOutputs(comids);
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
        /// Using the given task ID, get the simulation json, upstream dictionary, and dependencies dictionary
        /// from the mongodb. 
        /// </summary>
        /// <param name="task_id">Task ID for current simulation in database.</param>
        /// <returns>Error message or empty string</returns>
        private string GetCurrentSim(string task_id)
        {
            // Get from database
            try
            {
                Task<BsonDocument> output = Utilities.MongoDB.FindByTaskIDAsync("hms_workflows", "data", task_id);
                output.Wait();
                BsonObjectId key;
                var outputKey = output.Result.GetValue("input");
                if (outputKey.GetType() == typeof(BsonObjectId))
                {
                    key = (BsonObjectId)outputKey;
                }
                else
                {
                    key = new BsonObjectId(new ObjectId(outputKey.AsString));
                }
                baseSimJSON = Utilities.MongoDB.FindInGridFS("hms_workflows",key);
                var tempUp = output.Result.GetValue("upstream");
                if (tempUp is BsonNull)
                {
                    upstream = new Dictionary<string, string>();
                }
                else
                {
                    upstream = JsonConvert.DeserializeObject<Dictionary<string, string>>(tempUp.AsBsonDocument.ToJson());
                }
                var tempDep = output.Result.GetValue("dependencies");
                if (tempDep is BsonNull)
                {
                    dependencies = new Dictionary<string, string>();
                }
                else
                {
                    dependencies = JsonConvert.DeserializeObject<Dictionary<string, string>>(tempDep.AsBsonDocument.ToJson());
                }
            }
            catch (Exception ex)
            {
                return "Error getting simulation from database. " + ex.Message;
            }
            return "";
        }

        /// <summary>
        ///  Iterates over dictionary of dependencies and performs the appropriate action.
        /// </summary>
        /// <param name="sim">Current segment simulation</param>
        /// <returns>Error message or empty string</returns>
        public string CheckDependencies(AQTSim sim)
        {
            // Iterate over dependencies dict
            foreach (KeyValuePair<string, string> item in dependencies)
            {
                // Switch based on dependency key, eg: streamflow, etc..
                switch (item.Key)
                {
                    case "streamflow":
                        // Get streamflow from database
                        Task<BsonDocument> output = Utilities.MongoDB.FindByTaskIDAsync("hms_workflows", "data", item.Value);
                        output.Wait();
                        // Get result as string and deserialize to time series output 
                        TimeSeriesOutput TSO = new TimeSeriesOutput();
                        try
                        {
                            BsonObjectId key;
                            var outputKey = output.Result.GetValue("output");
                            if (outputKey.GetType() == typeof(BsonObjectId))
                            {
                                key = (BsonObjectId)outputKey;
                            }
                            else
                            {
                                key = new BsonObjectId(new ObjectId(outputKey.AsString));
                            }
                            string outputJson = Utilities.MongoDB.FindInGridFS("hms_workflows", key);
                            TSO = JsonConvert.DeserializeObject<TimeSeriesOutput>(outputJson);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                            return "Invalid Time Series Output.";
                        }
                        // Update stream discharge for current segment simulation
                        FlowsFromNWM(sim.AQTSeg.GetStatePointer(AllVariables.Volume, T_SVType.StV, T_SVLayer.WaterCol) as TVolume, TSO, true);  //JSC 7/1/2021
                        break;
                    default:
                        return "Unrecognized dependency: " + item.Key;
                }
            }
            return "";
        }

        /// <summary>
        /// Iterate over each comid and convert to int.
        /// </summary>
        /// <param name="upstream">List of comids as strings</param>
        /// <param name="comids">List of comids converted to ints</param>
        /// <returns>Error message or empty string</returns>
        private string ConvertUpstreamToInt(List<string> upstream, out List<int> comids)
        {
            comids = new List<int>();
            foreach (string item in upstream)
            {
                if (int.TryParse(item, out int value))
                {
                    comids.Add(value);
                }
                else
                {
                    // Try parse failed
                    return $"Invalid Comid: '{item}' in upstream segments.";
                }
            }
            return "";
        }

        /// <summary>
        /// Auxiliary function to mimic how data is passed in AQSim_2D.executeModel()
        /// </summary>
        /// <param name="sim">The current simulation to pass data to.</param>
        /// <param name="comids">List of comids</param>
        private void Pass_Data(AQTSim sim, List<int> comids)
        {
            // Pass the archived data to the current simulation
            int nSources = 0;
            foreach (int SrcId in comids)
            {
                nSources++;
                Pass_Data(sim, SrcId, nSources, this.archive[SrcId]);
            };
        }

        /// <summary>
        /// Class that has the archived state variable data converted to a dictionary
        /// of strings and values.
        /// </summary>
        public class ConvertedArchive
        {
            public DateTime[] Dates;
            public double[] Washout;  // m3/d

            /// Dictionary of State Variables and associated values.
            /// g/m3 or mg/m3 depending on state var
            /// ex: { "Total Ammonia as N (mg/L)" : [0.08408, 0.0480355, ...] }
            public Dictionary<string, double[]> Data;
        }

        /// <summary>
        /// Gets the output from database given a task ID and returns the archived results.
        /// </summary>
        /// <param name="task_id">Task ID in database.</param>
        public Task<ConvertedArchive> ArchiveOutput(string task_id, out string errormsg)
        {
            try
            {
                Task<BsonDocument> output = Utilities.MongoDB.FindByTaskIDAsync("hms_workflows", "data", task_id);
                output.Wait();

                // Convert to string and instantiate a new simulation
                BsonObjectId key;
                var outputKey = output.Result.GetValue("output");
                if (outputKey.GetType() == typeof(BsonObjectId))
                {
                    key = (BsonObjectId)outputKey;
                }
                else
                {
                    key = new BsonObjectId(new ObjectId(outputKey.AsString));
                }
                string json = Utilities.MongoDB.FindInGridFS("hms_workflows", key);
                // string json = output.Result.GetValue("output").AsString;
                AQTSim sim = new AQTSim();
                errormsg = sim.Instantiate(json);
                if (errormsg != "")
                {
                    //errormsg = $"Invalid simulation output.";
                    return null;
                }

                // Archive the simulation to the inherited property: archive
                archiveOutput(0, sim);

                // Convert the archived results to a dictionary
                return Task.FromResult(ConvertArchive(sim));
            }
            catch (Exception ex)
            {
                errormsg = "Error retrieving task_id from database.";
                return null;
            }
        }

        /// <summary>
        /// Iterate over each comid and generate the archive results for each.
        /// </summary>
        /// <param name="comids">List of comids</param>
        /// <returns>Error message or empty string</returns>
        public string ArchiveUpstreamOutputs(List<int> comids)
        {
            foreach (int item in comids)
            {
                try
                {
                    // Get the sim output from database for current comid_taskid
                    upstream.TryGetValue(item.ToString(), out string value);
                    Task<BsonDocument> output = Utilities.MongoDB.FindByTaskIDAsync("hms_workflows", "data", value);
                    output.Wait();

                    // Convert to string and instantiate a new simulation
                    BsonObjectId key;
                    var outputKey = output.Result.GetValue("output");
                    if (outputKey.GetType() == typeof(BsonObjectId))
                    {
                        key = (BsonObjectId)outputKey;
                    }
                    else
                    {
                        key = new BsonObjectId(new ObjectId(outputKey.AsString));
                    }
                    string json = Utilities.MongoDB.FindInGridFS("hms_workflows", key);
                    //string json = output.Result.GetValue("output").AsString;
                    AQTSim sim = new AQTSim();
                    string error = sim.Instantiate(json);
                    if (error != "")
                    {
                        return $"Invalid simulation output. Error: " + error;
                    }

                    // Archive the simulation to the inherited property: archive
                    archiveOutput(item, sim);
                }
                catch (Exception ex)
                {
                    return "Invalid simulation output, or unknown error.";
                }
            }
            // No errors return empty string
            return "";
        }

        /// <summary>   
        /// Convert the archived results to a dictionary
        /// </summary>
        /// <param name="sim">Current segment simulation</param>
        /// <returns>Converted archive</returns>
        private ConvertedArchive ConvertArchive(AQTSim sim)
        {
            // Convert the archived results
            ConvertedArchive CA = new ConvertedArchive();
            CA.Dates = archive[0].dates;
            CA.Washout = archive[0].washout;
            CA.Data = new Dictionary<string, double[]>();

            for (int i = 0; i < sim.AQTSeg.SV.Count; i++)
            {
                // Get the state variable ITimeSeriesOutput and pull units from metadata
                ITimeSeriesOutput TSO = sim.AQTSeg.SV[i].SVoutput;
                // Get the state variable name and the current list of values
                string name = sim.AQTSeg.SV[i].PName + $" ({TSO.Metadata["Unit_1"]})";
                double[] values = archive[0].concs[i];
                // Add to dictionary
                CA.Data.Add(name, values);
            }

            return CA;
        }

        /// <summary>
        /// Utility function to check for error by API controller.
        /// </summary>
        /// <param name="errorMsg">Error message</param>
        /// <returns>ITimeSeriesOutput with error</returns>
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
