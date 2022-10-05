using Accord.Math;
using DnsClient;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatershedDelineation;

namespace Web.Services.Models
{
    /// <summary>
    /// HMS Web Service Stream Model
    /// </summary>
    public class WSStream
    {

        Dictionary<string, object> plusflowlineResults = new Dictionary<string, object>();
        Dictionary<string, List<string>> sourceResults = new Dictionary<string, List<string>>();
        Dictionary<string, Dictionary<string, string>> comidDetails = new Dictionary<string, Dictionary<string, string>>();
        Dictionary<int, string> hydroComid = new Dictionary<int, string>();

        /// <summary>
        /// Gets stream network data for a provided comid
        /// </summary>
        /// <param name="comid"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, object>> Get(string comid, string endComid = null, string huc = null, double maxDistance = 50.0, bool mainstem = false)
        {
            string errorMsg = "";
            Stopwatch stopWatch = new Stopwatch();

            // Constructs default error output object containing error message.
            Utilities.ErrorOutput err = new Utilities.ErrorOutput();

            bool optimized = true;

            // Check comid
            if (comid is null) { return this.Error("ERROR: comid input is not valid."); }

            Dictionary<string, object> result = new Dictionary<string, object>();

            WatershedDelineation.Streams streamN = new WatershedDelineation.Streams(comid, null, null);

            List<List<object>> networkTable = new List<List<object>>();
            int maxTries = 3;
            int iTries = 0;
            bool completed = false;
            while (!completed)
            {
                try
                {
                    stopWatch.Start();
                    var streamNetwork = streamN.GetNetwork(maxDistance, endComid, mainstem);
                    stopWatch.Stop();
                    Log.Information("Stream Network - GetNetwork Attempt: " + (iTries + 1).ToString() + ", Runtime: " + stopWatch.Elapsed.TotalSeconds.ToString() + " sec");
                    stopWatch.Reset();
                    networkTable = StreamNetwork.generateTable(streamNetwork, null);
                }
                catch(Exception ex)
                {
                    
                }
                if (networkTable.Count == 0)
                {
                    iTries += 1;
                }
                else
                {
                    completed = true;
                }
                if (iTries == maxTries)
                {
                    completed = true;
                }
            }
            if (networkTable.Count == 0)
            {
                return this.Error("Unable to obtain network data from EPA Waters.");
            }
            if (optimized)
            {
                stopWatch.Restart();
                Network network = new Network(networkTable, comid);
                stopWatch.Stop();
                Log.Information("Stream Network - Create Network Runtime: " + stopWatch.Elapsed.TotalSeconds.ToString() + " sec");
                stopWatch.Restart();
                network.LoadData();
                stopWatch.Stop();
                Log.Information("Stream Network - Load DB Data Runtime: " + stopWatch.Elapsed.TotalSeconds.ToString() + " sec");
                stopWatch.Restart();
                result = network.ReturnData();
                stopWatch.Stop();
                Log.Information("Stream Network - Generate Output Runtime: " + stopWatch.Elapsed.TotalSeconds.ToString() + " sec");

                return result;
            }

            this.loadSQLData(networkTable);

            stopWatch.Start();
            List<List<int>> segOrder = new List<List<int>>();
            if (mainstem)
            {
                string switchedComid = null;
                if (!String.IsNullOrEmpty(endComid))
                {
                    switchedComid = comid;
                }
                segOrder = this.generateMainstemOrder(networkTable, switchedComid);
                if (!String.IsNullOrEmpty(endComid))
                {
                    networkTable = this.purgeTable(networkTable, segOrder);
                }
                result = this.getSourceComids(segOrder, networkTable);
                result.Add("order", segOrder);

            }
            else if (!String.IsNullOrEmpty(endComid))
            {
                result = this.generateOrderAndSourcesPP(ref networkTable, comid, huc);
            }
            else
            {
                result = this.generateOrderAndSources(ref networkTable, comid, huc);
            }
            stopWatch.Stop();
            Log.Information("Stream Network - GenerateOrder Runtime: " + stopWatch.Elapsed.TotalSeconds.ToString() + " sec");
            stopWatch.Reset();
            result.Add("network", networkTable);
            List<string> sourcesList = (result["sources"] as Dictionary<string, List<string>>).Keys.ToList();
            stopWatch.Start();
            result.Add("waterbodies", this.checkWaterbodies(sourcesList));
            stopWatch.Stop();
            Log.Information("Stream Network - CheckWaterbodies Runtime: " + stopWatch.Elapsed.TotalSeconds.ToString() + " sec");
            return result;
        }

        private void loadSQLData(List<List<object>> networkTable)
        {
            string dbPath = Path.Combine(".", "App_Data", "catchments.sqlite");

            StringBuilder sb = new StringBuilder();
            List<object> fields = networkTable[0];
            fields.Remove("comid");
            for (int i = 1; i < networkTable.Count; i++)
            {
                string comid = networkTable[i][0].ToString();
                int hydro = Int32.Parse(networkTable[i][1].ToString());
                Dictionary<string, string> details = new Dictionary<string, string>();
                for(int j = 1; j < fields.Count; j++)
                {
                    details.Add(fields[j].ToString(), networkTable[i][j].ToString());
                }
                this.comidDetails.Add(comid, details);
                this.hydroComid.Add(hydro, comid);

                sb.Append(comid);
                if (i < networkTable.Count - 1)
                {
                    sb.Append(", ");
                }

                string sourceQuery = "SELECT ComID FROM PlusFlowlineVAA WHERE DnHydroseq=" + hydro.ToString();
                Dictionary<string, object> sourceResults = Utilities.SQLite.GetDataObject(dbPath, sourceQuery) as Dictionary<string, object>;
                List<object> sourceComID = new List<object>();
                if (sourceResults.Count > 0)
                {
                    sourceComID.Add(sourceResults["ComID"].ToString().Split(","));
                }
                this.sourceResults.Add(comid, new List<string>());
                for (int j = 0; j < sourceComID.Count; j++)
                {
                    this.sourceResults[comid].Add(sourceComID[j].ToString());   
                }
            }
            string comidsStr = sb.ToString();

            // Obtain all comid data from PlusFlowlineVAA table
            string divergentQuery = "Select ComID, Divergence, WBAREACOMI  From PlusFlowlineVAA WHERE Comid IN (" + comidsStr + ")";
            Dictionary<string, object> divergentResults = Utilities.SQLite.GetDataObject(dbPath, divergentQuery) as Dictionary<string, object>;
            List<string> resultComids = divergentResults["ComID"].ToString().Split(",").ToList<string>();
            List<string> resultDiv = divergentResults["Divergence"].ToString().Split(",").ToList<string>();
            List<string> resultWB = divergentResults["WBAREACOMI"].ToString().Split(",").ToList<string>();
            for (int i = 0; i < resultComids.Count; i++)
            {
                this.comidDetails[resultComids[i].ToString()].Add("divergence", resultDiv[i].ToString());
                this.comidDetails[resultComids[i].ToString()].Add("WBAREACOMI", resultWB[i].ToString());
            }




            string test = "";

        }


        private List<List<object>> purgeTable(List<List<object>> networkTable, List<List<int>> order)
        {
            List<int> comids = new List<int>();
            foreach(List<int> row in order)
            {
                foreach(int comid in row)
                {
                    comids.Add(comid);
                }
            }
            List<List<object>> purgedTable = new List<List<object>>();
            purgedTable.Add(networkTable[0]);
            for(int i = 1; i <= networkTable.Count - 1; i++)
            {
                int comid = Int32.Parse(networkTable[i][0].ToString());
                if (comids.Contains(comid))
                {
                    purgedTable.Add(networkTable[i]);
                }
            }
            return purgedTable;
        } 

        private Dictionary<string, object> getSourceComids(List<List<int>> order, List<List<object>> table)
        {
            string dbPath = Path.Combine(".", "App_Data", "catchments.sqlite");
            Dictionary<string, List<string>> sourceCOMIDs = new Dictionary<string, List<string>>();
            List<int> hydroList = new List<int>();
            for (int i = 1; i < table.Count; i++)
            {
                string comid = table[i][0].ToString();
                sourceCOMIDs.Add(comid, new List<string>());
                int hydroseq = Int32.Parse(table[i][1].ToString());
                hydroList.Add(hydroseq);
            }
            List<string> boundaryCOMIDS = new List<string>();
            for (int i = 1; i < table.Count; i++)
            {
                string comid = table[i][0].ToString();
                int hydroseq = Int32.Parse(table[i][1].ToString());
                int dnhydroseq = Int32.Parse(table[i][3].ToString());
                int uphydroseq = Int32.Parse(table[i][2].ToString());
                for (int j = 1; j < table.Count; j++)
                {
                    string comid2 = table[j][0].ToString();
                    int hydroseq2 = Int32.Parse(table[j][1].ToString());
                    if (hydroseq2 == dnhydroseq && !comid.Equals(comid2))
                    {
                        sourceCOMIDs[comid2].Add(comid);
                    }
                }
                string query = "SELECT COMID FROM PlusFlowlineVAA WHERE DnHydroseq=" + hydroseq.ToString();
                Dictionary<string, string> sourceComid = Utilities.SQLite.GetData(dbPath, query);
                if (sourceComid.ContainsKey("ComID"))
                {
                    foreach (string c in sourceComid["ComID"].Split(","))
                    {
                        if (!sourceCOMIDs[comid].Contains(c))
                        {
                            sourceCOMIDs[comid].Add(c);
                            boundaryCOMIDS.Add(c);
                        }
                    }
                }
            }
            //sourceCOMIDs.Add("boundaries", boundaryCOMIDS);
            Dictionary<string, object> boundaries = new Dictionary<string, object>()
            {
                { "headwater", new List<string>() },{ "out-of-network", boundaryCOMIDS }
            };
            return new Dictionary<string, object>() { { "sources", sourceCOMIDs }, { "boundary", boundaries } };
        }

        //public List<List<int>> generateOrder(List<List<object>> networkTable)
        //{
        //    string dbPath = Path.Combine(".", "App_Data", "catchments.sqlite");

        //    List<int> dag = new List<int>();
        //    for (int i = networkTable.Count - 1; i > 0; i--)
        //    {
        //        int hydroseq = Int32.Parse(networkTable[i][1].ToString());
        //        dag.Add(hydroseq);
        //    }

        //    List<List<int>> seqOrder = new List<List<int>>();
        //    seqOrder.Add(new List<int>()
        //    {
        //        Int32.Parse(networkTable[1][1].ToString())
        //    });
        //    List<List<int>> comidOrder = new List<List<int>>();
        //    comidOrder.Add(new List<int>()
        //    {
        //        Int32.Parse(networkTable[1][0].ToString())
        //    });
        //    for (int i = 2; i <= dag.Count; i++)
        //    {
        //        seqOrder.Add(new List<int>());
        //        comidOrder.Add(new List<int>());
        //    }
        //    for (int i = 2; i <= dag.Count; i++)
        //    {
        //        int comid = Int32.Parse(networkTable[i][0].ToString());
        //        int hydroseq = Int32.Parse(networkTable[i][1].ToString());
        //        int dnhydroseq = Int32.Parse(networkTable[i][3].ToString());
        //        string query = "SELECT Hydroseq FROM PlusFlowlineVAA WHERE DnHydroseq=" + hydroseq.ToString();
        //        Dictionary<string, string> sourceComid = Utilities.SQLite.GetData(dbPath, query);
        //        List<int> uphydroseq = new List<int>();
        //        if (sourceComid.ContainsKey("Hydroseq"))
        //        {
        //            foreach(string c in sourceComid["Hydroseq"].Split(","))
        //            {
        //                uphydroseq.Add(Int32.Parse(c));
        //            }
        //        }
        //        else
        //        {
        //            uphydroseq.Add(Int32.Parse(networkTable[i][2].ToString()));
        //        }
        //        int seq_j = 0;
        //        for (int j = seqOrder.Count - 1; j >= 0; j--)
        //        {
        //            foreach(int up in uphydroseq)
        //            {
        //                if (seqOrder[j].Contains(up))
        //                {
        //                    seq_j = j - 1;
        //                    break;
        //                }
        //            }             
        //            if (seqOrder[j].Contains(dnhydroseq))
        //            {
        //                seq_j = j + 1;
        //                break;
        //            }
        //        }
        //        if (seq_j == -1)
        //        {
        //            List<int> newOrderLevel = new List<int>();
        //            newOrderLevel.Add(comid);
        //            comidOrder = comidOrder.Prepend(newOrderLevel).ToList();
        //            List<int> newSeqLevel = new List<int>();
        //            newSeqLevel.Add(hydroseq);
        //            seqOrder = seqOrder.Prepend(newSeqLevel).ToList();
        //        }
        //        else
        //        {
        //            comidOrder[seq_j].Add(comid);
        //            seqOrder[seq_j].Add(hydroseq);
        //        }
        //    }
        //    int nOrder = comidOrder.Count;
        //    for (int i = nOrder - 1; i >= 0; i--)
        //    {
        //        if (comidOrder[i].Count == 0)
        //        {
        //            comidOrder.RemoveAt(i);
        //        }
        //    }
        //    comidOrder.Reverse();

        //    return comidOrder;
        //}

        public List<List<int>> generateMainstemOrder(List<List<object>> networkTable, string endComid = null)
        {

            List<int> dag = new List<int>();
            List<List<int>> comidOrder = new List<List<int>>();
            for (int i = 1; i <= networkTable.Count - 1; i++)
            {
                int hydroseq = Int32.Parse(networkTable[i][1].ToString());
                string comidSTR = networkTable[i][0].ToString();
                int comid = Int32.Parse(comidSTR);

                dag.Add(hydroseq);
                List<int> newOrderLevel = new List<int>();
                newOrderLevel.Add(comid);
                comidOrder.Add(newOrderLevel);

                if (!String.IsNullOrEmpty(endComid))
                {
                    if (comidSTR.Equals(endComid))
                    {
                        break;
                    }
                }
            }
            if (String.IsNullOrEmpty(endComid))
            {
                comidOrder.Reverse();
            }
            return comidOrder;
        }

        private Dictionary<string, object> Error(string errorMsg)
        {
            Dictionary<string, object> output = new Dictionary<string, object>();
            output.Add("ERROR", errorMsg);
            return output;
        }

        private void recTraverse(int hydroseq, string upComid, string pourpoint, ref List<string> sequence, ref List<int> traversed, ref Dictionary<string, List<string>> sources, Dictionary<int, string> hydroComid, Dictionary<int, List<object>> mapping)
        {
            if (upComid == pourpoint)
            {
                return;
            }
            if (!hydroComid.ContainsKey(hydroseq))
            {
                // Downstream flows out of the network
                sequence.Add("-1");
                return;
            }
            if (!sources.ContainsKey(hydroComid[hydroseq]))
            {
                sequence.Add("-1");
                return;
            }
            sequence.Add(hydroComid[hydroseq]);
            if (!sources[hydroComid[hydroseq]].Contains(upComid))
            {
                sources[hydroComid[hydroseq]].Add(upComid);
            }
            if (traversed.Contains(hydroseq) || hydroComid[hydroseq].Equals(pourpoint))
            {
                return;
            }

            else
            {
                traversed.Add(hydroseq);
                int dnHydroseq = Int32.Parse(mapping[hydroseq][3].ToString());
                recTraverse(dnHydroseq, hydroComid[hydroseq], pourpoint, ref sequence, ref traversed, ref sources, hydroComid, mapping);
            }
        }

        private void ReorderSequence(ref List<List<string>> order, List<string> sequence)
        {
            int n = 0;
            if (order.Count == 0)
            {
                for (int i = 0; i < sequence.Count; i++)
                {
                    order.Add(new List<string>() { sequence[i] });
                }
            }
            else
            {
                for (int i = 0; i < order.Count; i++)
                {
                    if (order[i].Contains(sequence.Last<string>()))
                    {
                        n = i;
                        break;
                    }
                }
                int j = 0;
                for (int i = sequence.Count - 2; i >= 0; i--)
                {
                    int m = n - j - 1;
                    if( m < 0) { 
                        order.Insert(0, new List<string>() { { sequence[i] } });
                    }
                    else
                    {
                        if (!order[m].Contains(sequence[i])){
                            order[m].Add(sequence[i]);
                        }
                    }
                    j++;
   
                }
            }
        }

        private int divergentCheck(string segmentComid)
        {
            //string dbPath = Path.Combine(".", "App_Data", "catchments.sqlite");
            //string query = "SELECT Divergence FROM PlusFlowlineVAA WHERE ComID=" + segmentComid;
            //Dictionary<string, string> results = Utilities.SQLite.GetData(dbPath, query);
            //if (results.Count > 0)
            //{
            //    // Divergence is defined as a value of 0, the mainstem path of a divergence is 1, and >1 are the divergent paths
            //    return Int32.Parse(results["Divergence"]);
            //}
            string divStr = this.comidDetails[segmentComid]["divergence"];
            return Int32.Parse(divStr);
        }

        private Dictionary<string, string> segmentDetails(string segmentHydroseq)
        {
            int hydro = Int32.Parse(segmentHydroseq);
            if (this.hydroComid.ContainsKey(hydro))
            {
                return this.comidDetails[this.hydroComid[hydro]];
            }
            else
            {
                string dbPath = Path.Combine(".", "App_Data", "catchments.sqlite");
                string query = "SELECT ComID, Hydroseq, UpHydroseq, DnHydroseq, Divergence FROM PlusFlowlineVAA WHERE Hydroseq=" + segmentHydroseq;
                return Utilities.SQLite.GetData(dbPath, query);
            }
        }

        private Dictionary<string, string> divergenceDetails(string segmentHydroseq)
        {
            int hydro = Int32.Parse(segmentHydroseq);
            if (this.hydroComid.ContainsKey(hydro))
            {
                return this.comidDetails[this.hydroComid[hydro]];
            }
            else
            {
                string dbPath = Path.Combine(".", "App_Data", "catchments.sqlite");
                string query = "SELECT ComID, Hydroseq, UpHydroseq, DnHydroseq, Divergence FROM PlusFlowlineVAA WHERE UpHydroseq=" + segmentHydroseq + " AND Divergence=2";
                return Utilities.SQLite.GetData(dbPath, query);
            }
        }

        private Dictionary<string, object> traverseDivergence(string pourpoint)
        {
            Dictionary<string, List<string>> sources = new Dictionary<string, List<string>>();      // Sources dictionary (COMID)
            List<List<string>> order = new List<List<string>>();
            List<string> divComids = new List<string>();

            string iSegment = pourpoint;
            string pseudoPourpoint = "";
            Dictionary<string, string> iDetails = this.segmentDetails(iSegment);
            Dictionary<string, string> jDetails = new Dictionary<string, string>();

            bool divergent = true;
            while(divergent)
            {
                
                string upHydro = iDetails["UpHydroseq"];

                jDetails = this.segmentDetails(upHydro);
                sources.Add(iDetails["ComID"], new List<string>() { jDetails["ComID"] });
                order = order.Prepend(new List<string>() { iDetails["ComID"] }).ToList();

                if(Int32.Parse(jDetails["Divergence"]) == 0)
                {
                    divergent = false;
                    pseudoPourpoint = jDetails["ComID"];
                }
                else
                {
                    iDetails = jDetails;
                }
            }

            Dictionary<string, string> pSegment = this.segmentDetails(jDetails["DnHydroseq"]);
            Dictionary<string, List<string>> divParallel = new Dictionary<string, List<string>>();
            divParallel.Add(iDetails["ComID"], new List<string>()
            {
                pSegment["ComID"]
            });

            return new Dictionary<string, object>()
            {
                { "pseudo-pourpoint",  pseudoPourpoint},
                { "sources", sources },
                { "order", order },
                { "boundary", new Dictionary<string, object>(){{"divergence", divComids } } },
                { "divergent-paths", divParallel}
            };
        }

        private Dictionary<string, string> getSegmentSources(string segmentHydroseq)
        {
            string dbPath = Path.Combine(".", "App_Data", "catchments.sqlite");
            string query = "SELECT ComID FROM PlusFlowlineVAA WHERE DnHydroseq=" + segmentHydroseq;
            return Utilities.SQLite.GetData(dbPath, query);
        }

        private string getParallelSegment(string hydroseq, string uphydroseq)
        {
            string dbPath = Path.Combine(".", "App_Data", "catchments.sqlite");
            //string query = "SELECT ComID FROM PlusFlowlineVAA WHERE Hydroseq=(SELECT DnHydroseq FROM PlusFlowlineVAA WHERE Hydroseq=" + hydroseq + ")";
            string query = "SELECT ComID FROM PlusFlowlineVAA WHERE UpHydroseq=" + uphydroseq + " AND Hydroseq!=" + hydroseq;
            Dictionary<string, string> qResults = Utilities.SQLite.GetData(dbPath, query);
            return qResults["ComID"];
        }

        private Dictionary<string, string> getDivSegment(string hydroseq)
        {
            Dictionary<string, string> iDetails = this.segmentDetails(hydroseq);
            Dictionary<string, string> jDetails = new Dictionary<string, string>();

            string divSource = "";
            bool div1 = true;
            while (div1)
            {
                string upHydro = iDetails["UpHydroseq"];
                int div = Int32.Parse(iDetails["Divergence"]);
                if (div == 0)
                {
                    divSource = iDetails["Hydroseq"];
                    div1 = false;
                }
                else
                {
                    jDetails = iDetails;
                    iDetails = this.segmentDetails(upHydro);
                }
            }
            Dictionary<string, string> divDetails = this.divergenceDetails(divSource);
            return new Dictionary<string, string>(){
                { "divergence", divDetails["ComID"] },
                { "mainstem",jDetails["ComID"]}
            };
        }

        public Dictionary<string, object> generateOrderAndSources(ref List<List<object>> networkTable, string pourpoint, string huc = "")
        {
            string dbPath = Path.Combine(".", "App_Data", "catchments.sqlite");

            // Check if pourpoint is along a divergent path, non-mainstem path
            // If true, traverse upstream of the provided pourpoint until segment is not on divergent path set that segment as pseudopourpoint
            Dictionary<string, object> dResults = new Dictionary<string, object>();
            string dPourpoint = pourpoint;
            bool divergent = false;
            bool divergentUp = false;
            Dictionary<string, string> divSegment = new Dictionary<string, string>();
            int pourDivergence = this.divergentCheck(pourpoint);
            
            if (pourDivergence > 1)                 // Pourpoint is on a divergent path
            {
                dResults = this.traverseDivergence(networkTable[1][1].ToString());
                pourpoint = dResults["pseudo-pourpoint"].ToString();
                divergent = true;
            }
            else if(pourDivergence == 1)            // Pourpoint is on the mainstem where an upstream segment diverges
            {
                divSegment = this.getDivSegment(networkTable[1][1].ToString());
                divergentUp = true;
            }

            // Get the HUC12 of the pourpoint from the NHDPlus V2.1 db
            string query1 = "SELECT HUC12 FROM HUC12_PU_COMIDs_CONUS WHERE COMID=" + pourpoint;
            Dictionary<string, string> aoiHUCq = Utilities.SQLite.GetData(dbPath, query1);
            string aoiHUC = (aoiHUCq.ContainsKey("HUC12")) ? aoiHUCq["HUC12"] : "";
            // Get all comids for the aoiHUC from the NHDPlus V2.1 db
            string query2 = "SELECT COMID FROM HUC12_PU_COMIDs_CONUS WHERE HUC12='" + aoiHUC + "'";
            Dictionary<string, string> aoiCOMIDSq = Utilities.SQLite.GetData(dbPath, query2);
            List<string> aoiCOMIDs = (aoiCOMIDSq.ContainsKey("COMID")) ? aoiCOMIDSq["COMID"].Split(",").ToList<string>() : new List<string>();
            string pourpointHUC = networkTable[1][7].ToString();
            // If the aoiHUC does not match with the EPA Water
            if (pourpointHUC != aoiHUC)
            {
                string fileName = "HUC12_mismatch.txt";
                string header = "COMID, NHDPlusV2.1 HUC12, EPA Waters HUC12";
                string huclog = pourpoint + ", " + aoiHUC + ", " + pourpointHUC;
                Utilities.Logger.WriteToFile(fileName, huclog, false, header);
            }

            huc = (huc == null) ? "" : huc;
            List<int> networkHydro = new List<int>();
            Dictionary<string, List<string>> sources = new Dictionary<string, List<string>>();      // Sources dictionary (COMID)
            Dictionary<int, List<object>> hydroMapping = new Dictionary<int, List<object>>();       // Hydroseq to networkTable info mapping dict
            Dictionary<int, string> hydroComid = new Dictionary<int, string>();                     // Hydroseq to comid mapping
            List<int> outOfNetwork = new List<int>();
            List<List<object>> filteredNetworkTable = new List<List<object>>()
            {
                { networkTable[0] }
            };
            int j = 1;
            // Initialize the outOfNetwork segments, the set of hydroseq (networkHydro), the sources dictionary (sources), and the filteredNetworkTable which replaces the network table 
            for(int i = 1; i < networkTable.Count; i++)
            {
                string comid = networkTable[i][0].ToString();
                int hydro = Int32.Parse(networkTable[i][1].ToString());
                if (huc.Equals("12") && !aoiCOMIDs.Contains(comid))
                {
                    outOfNetwork.Add(hydro);
                }
                else
                {
                    networkHydro.Add(hydro);
                    sources.Add(comid, new List<string>());
                    filteredNetworkTable.Add(networkTable[i]);
                    filteredNetworkTable[j][7] = aoiHUC;
                    j += 1;
                }
                hydroMapping.Add(hydro, networkTable[i]);
                hydroComid.Add(hydro, comid);
            }
            List<int> edges = new List<int>();
            List<string> headwaters = new List<string>();
            List<string> outNetwork = new List<string>();

            for (int i = 1; i < networkTable.Count; i++)
            {
                string comid = networkTable[i][0].ToString();
                int hydro = Int32.Parse(networkTable[i][1].ToString());
                int uphydro = Int32.Parse(networkTable[i][2].ToString());
                if (outOfNetwork.Contains(hydro))
                {
                    int dnhydro = Int32.Parse(networkTable[i][3].ToString());
                    if (!outOfNetwork.Contains(dnhydro) && hydroMapping.ContainsKey(dnhydro))
                    {
                        string dnCOMID = hydroMapping[dnhydro][0].ToString();
                        if (!sources[dnCOMID].Contains(comid))
                        {
                            sources[hydroMapping[dnhydro][0].ToString()].Add(comid);
                        }
                        if (!outNetwork.Contains(comid))
                        {
                            outNetwork.Add(comid);
                        }
                    }
                }
                else if (uphydro == 0)
                {
                    headwaters.Add(comid);
                    edges.Add(hydro);
                }
                else if (!networkHydro.Contains(uphydro) && comid != pourpoint)
                {
                    edges.Add(hydro);
                    bool addToOut = false;
                    if (hydroMapping.ContainsKey(uphydro))
                    {
                        string srcComid = hydroMapping[uphydro][0].ToString();
                        if (!outNetwork.Contains(srcComid))
                        {
                            outNetwork.Add(srcComid);
                        }
                    }
                    else
                    {
                        addToOut = true;
                    }
                    string query = "SELECT COMID FROM PlusFlowlineVAA WHERE Hydroseq=" + uphydro.ToString();
                    Dictionary<string, string> sourceComid = Utilities.SQLite.GetData(dbPath, query);
                    if (sourceComid.ContainsKey("ComID")) {
                        if (!sources[comid].Contains(sourceComid["ComID"]))
                        {
                            sources[comid].Add(sourceComid["ComID"]);
                        }
                        if (addToOut)
                        {
                            outNetwork.Add(sourceComid["ComID"]);
                        }
                    }
                }
            }
            List<int> traversed = new List<int>();
            List<List<string>> order = new List<List<string>>();
            List<int> diffEdges = new List<int>();
            List<string> divComids = new List<string>();
            if (divergent)
            {
                divComids.Add(dPourpoint);
            }
            Dictionary<string, List<string>> divParallel = new Dictionary<string, List<string>>();
            if (networkHydro.Count == 1)
            {
                order.Add(new List<string>() { networkTable[1][0].ToString() });
            }
            else
            {
                for (int i = 0; i < edges.Count; i++)
                {
                    traversed.Add(edges[i]);
                    List<string> sequence = new List<string>();
                    sequence.Add(hydroComid[edges[i]]);
                    int dnHydro = Int32.Parse(hydroMapping[edges[i]][3].ToString());
                    recTraverse(dnHydro, hydroComid[edges[i]], pourpoint, ref sequence, ref traversed, ref sources, hydroComid, hydroMapping);
                    if (!sequence.Contains("-1"))
                    {
                        ReorderSequence(ref order, sequence);
                    }
                    else
                    {
                        foreach(string s in sequence)
                        {
                            if(s != "-1")
                            {
                                sources.Remove(s);
                            }
                        }
                    }
                }
                bool notFound = false;
                int nTraversed = 0;
                while(networkHydro.Count > traversed.Count+1 && !notFound)
                {
                    List<int> diff = new List<int>();
                    diffEdges = new List<int>();
                    foreach(int hydro in networkHydro)
                    {
                        if (!traversed.Contains(hydro) && hydroComid[hydro] != pourpoint)
                        {
                            diff.Add(hydro);
                        }
                    }
                    for(int i = 0; i < diff.Count; i++)
                    {
                        int upHydro = Int32.Parse(hydroMapping[diff[i]][2].ToString());
                        if (traversed.Contains(upHydro))
                        {
                            diffEdges.Add(diff[i]);
                            //string pSeg = this.getParallelSegment(diff[i].ToString());
                            string pSeg = this.getParallelSegment(diff[i].ToString(), upHydro.ToString());
                            divParallel.Add(hydroComid[diff[i]], new List<string>() { pSeg });
                            divParallel.Add(pSeg, new List<string>() { hydroComid[diff[i]] });
                            if (!divComids.Contains(hydroComid[diff[i]]))
                            {
                                divComids.Add(hydroComid[diff[i]]);
                            }
                        }
                        string comid = hydroComid[Int32.Parse(hydroMapping[diff[i]][2].ToString())];
                        if (!sources[hydroComid[diff[i]]].Contains(comid))
                        {
                            sources[hydroComid[diff[i]]].Add(comid);
                        }

                    }
                    for(int i = 0; i < diffEdges.Count; i++)
                    {
                        traversed.Add(diffEdges[i]);
                        List<string> sequence = new List<string>();
                        sequence.Add(hydroComid[diffEdges[i]]);
                        int dnHydro = Int32.Parse(hydroMapping[diffEdges[i]][3].ToString());
                        recTraverse(dnHydro, hydroComid[diffEdges[i]], pourpoint, ref sequence, ref traversed, ref sources, hydroComid, hydroMapping);
                        if (!sequence.Contains("-1"))
                        {
                            ReorderSequence(ref order, sequence);
                        }
                        else
                        {
                            foreach (string s in sequence)
                            {
                                if (s != "-1")
                                {
                                    sources.Remove(s);
                                }
                            }
                        }
                    }
                    if(traversed.Count + 1 == nTraversed)
                    {
                        notFound = true;
                    }
                    nTraversed = traversed.Count + 1;
                }
            }
            networkTable = filteredNetworkTable;

            //if pourpoint is divergent, merge divergent path results with pseudo-pourpoint results
            if (divergent)
            {
                foreach(KeyValuePair<string, List<string>> divSource in dResults["sources"] as Dictionary<string, List<string>>)
                {
                    if (!sources.ContainsKey(divSource.Key))
                    {
                        sources.Add(divSource.Key, divSource.Value);
                    }
                    else
                    {
                        sources[divSource.Key].Union(divSource.Value);
                    }
                }
                foreach(List<string> dOrder in dResults["order"] as List<List<string>>)
                {
                    order.Add(dOrder);
                }
                foreach(KeyValuePair<string, List<string>> divPar in dResults["divergent-paths"] as Dictionary<string, List<string>>)
                {
                    if (!divParallel.ContainsKey(divPar.Key))
                    {
                        divParallel.Add(divPar.Key, divPar.Value);
                    }
                    else
                    {
                        foreach(string pV in divPar.Value)
                        {
                            if (!divParallel[divPar.Key].Contains(pV))
                            {
                                divParallel[divPar.Key].Add(pV);
                            }
                        }
                    }
                }
            }
            else if (divergentUp)
            {
                divParallel[divSegment["mainstem"]] = new List<string>() { divSegment["divergence"] };
            }

            return new Dictionary<string, object>()
            {
                { "sources", sources },
                { "order", order },
                { "boundary", new Dictionary<string, object>(){ {"headwater", headwaters}, {"out-of-network", outNetwork}, {"divergence", divComids } } },
                { "divergent-paths", divParallel}
            };

        }

        public Dictionary<string, object> generateOrderAndSourcesPP(ref List<List<object>> networkTable, string pourpoint, string huc = "")
        {
            string dbPath = Path.Combine(".", "App_Data", "catchments.sqlite");

            // Check if pourpoint is along a divergent path, non-mainstem path
            // If true, traverse upstream of the provided pourpoint until segment is not on divergent path set that segment as pseudopourpoint
            Dictionary<string, object> dResults = new Dictionary<string, object>();
            string dPourpoint = pourpoint;
            bool divergent = false;
            bool divergentUp = false;
            Dictionary<string, string> divSegment = new Dictionary<string, string>();
            int pourDivergence = this.divergentCheck(pourpoint);

            if (pourDivergence > 1)                 // Pourpoint is on a divergent path
            {
                dResults = this.traverseDivergence(networkTable[1][1].ToString());
                pourpoint = dResults["pseudo-pourpoint"].ToString();
                divergent = true;
            }
            else if (pourDivergence == 1)            // Pourpoint is on the mainstem where an upstream segment diverges
            {
                divSegment = this.getDivSegment(networkTable[1][1].ToString());
                divergentUp = true;
            }

            // Get the HUC12 of the pourpoint from the NHDPlus V2.1 db
            string query1 = "SELECT HUC12 FROM HUC12_PU_COMIDs_CONUS WHERE COMID=" + pourpoint;
            Dictionary<string, string> aoiHUCq = Utilities.SQLite.GetData(dbPath, query1);
            string aoiHUC = (aoiHUCq.ContainsKey("HUC12")) ? aoiHUCq["HUC12"] : "";
            // Get all comids for the aoiHUC from the NHDPlus V2.1 db
            string query2 = "SELECT COMID FROM HUC12_PU_COMIDs_CONUS WHERE HUC12='" + aoiHUC + "'";
            Dictionary<string, string> aoiCOMIDSq = Utilities.SQLite.GetData(dbPath, query2);
            List<string> aoiCOMIDs = (aoiCOMIDSq.ContainsKey("COMID")) ? aoiCOMIDSq["COMID"].Split(",").ToList<string>() : new List<string>();
            string pourpointHUC = networkTable[1][7].ToString();
            // If the aoiHUC does not match with the EPA Water
            if (pourpointHUC != aoiHUC)
            {
                string fileName = "HUC12_mismatch.txt";
                string header = "COMID, NHDPlusV2.1 HUC12, EPA Waters HUC12";
                string huclog = pourpoint + ", " + aoiHUC + ", " + pourpointHUC;
                Utilities.Logger.WriteToFile(fileName, huclog, false, header);
            }

            huc = (huc == null) ? "" : huc;
            List<int> networkHydro = new List<int>();
            Dictionary<string, List<string>> sources = new Dictionary<string, List<string>>();      // Sources dictionary (COMID)
            Dictionary<int, List<object>> hydroMapping = new Dictionary<int, List<object>>();       // Hydroseq to networkTable info mapping dict
            Dictionary<int, string> hydroComid = new Dictionary<int, string>();                     // Hydroseq to comid mapping
            List<int> outOfNetwork = new List<int>();
            List<List<object>> filteredNetworkTable = new List<List<object>>()
            {
                { networkTable[0] }
            };
            int j = 1;
            // Initialize the outOfNetwork segments, the set of hydroseq (networkHydro), the sources dictionary (sources), and the filteredNetworkTable which replaces the network table 
            for (int i = 1; i < networkTable.Count; i++)
            {
                string comid = networkTable[i][0].ToString();
                int hydro = Int32.Parse(networkTable[i][1].ToString());
                if (huc.Equals("12") && !aoiCOMIDs.Contains(comid))
                {
                    outOfNetwork.Add(hydro);
                }
                else
                {
                    networkHydro.Add(hydro);
                    sources.Add(comid, new List<string>());
                    filteredNetworkTable.Add(networkTable[i]);
                    filteredNetworkTable[j][7] = aoiHUC;
                    j += 1;
                }
                hydroMapping.Add(hydro, networkTable[i]);
                hydroComid.Add(hydro, comid);
            }
            List<int> edges = new List<int>();
            List<string> headwaters = new List<string>();
            List<string> outNetwork = new List<string>();

            for (int i = 1; i < networkTable.Count; i++)
            {
                string comid = networkTable[i][0].ToString();
                int hydro = Int32.Parse(networkTable[i][1].ToString());
                int uphydro = Int32.Parse(networkTable[i][2].ToString());
                if (outOfNetwork.Contains(hydro))
                {
                    int dnhydro = Int32.Parse(networkTable[i][3].ToString());
                    if (!outOfNetwork.Contains(dnhydro) && hydroMapping.ContainsKey(dnhydro))
                    {
                        string dnCOMID = hydroMapping[dnhydro][0].ToString();
                        if (!sources[dnCOMID].Contains(comid))
                        {
                            sources[hydroMapping[dnhydro][0].ToString()].Add(comid);
                        }
                        if (!outNetwork.Contains(comid))
                        {
                            outNetwork.Add(comid);
                        }
                    }
                }
                else if (uphydro == 0)
                {
                    headwaters.Add(comid);
                    edges.Add(hydro);
                }
                else if (!networkHydro.Contains(uphydro) && comid != pourpoint)
                {
                    edges.Add(hydro);
                    bool addToOut = false;
                    if (hydroMapping.ContainsKey(uphydro))
                    {
                        string srcComid = hydroMapping[uphydro][0].ToString();
                        if (!outNetwork.Contains(srcComid))
                        {
                            outNetwork.Add(srcComid);
                        }
                    }
                    else
                    {
                        addToOut = true;
                    }

                    string query = "SELECT COMID FROM PlusFlowlineVAA WHERE Hydroseq=" + uphydro.ToString();
                    Dictionary<string, string> sourceComid = Utilities.SQLite.GetData(dbPath, query);
                    if (sourceComid.ContainsKey("ComID"))
                    {
                        if (!sources[comid].Contains(sourceComid["ComID"]))
                        {
                            sources[comid].Add(sourceComid["ComID"]);
                        }
                        if (addToOut)
                        {
                            outNetwork.Add(sourceComid["ComID"]);
                        }
                    }
                }
            }
            List<int> traversed = new List<int>();
            List<List<string>> order = new List<List<string>>();
            List<int> diffEdges = new List<int>();
            List<string> divComids = new List<string>();
            if (divergent)
            {
                divComids.Add(dPourpoint);
            }
            Dictionary<string, List<string>> divParallel = new Dictionary<string, List<string>>();
            if (networkHydro.Count == 1)
            {
                order.Add(new List<string>() { networkTable[1][0].ToString() });
            }
            else
            {
                for (int i = 0; i < edges.Count; i++)
                {
                    traversed.Add(edges[i]);
                    List<string> sequence = new List<string>();
                    sequence.Add(hydroComid[edges[i]]);
                    int dnHydro = Int32.Parse(hydroMapping[edges[i]][3].ToString());
                    recTraverse(dnHydro, hydroComid[edges[i]], pourpoint, ref sequence, ref traversed, ref sources, hydroComid, hydroMapping);
                    if (!sequence.Contains("-1"))
                    {
                        ReorderSequence(ref order, sequence);
                    }
                    else
                    {
                        foreach (string s in sequence)
                        {
                            if (s != "-1")
                            {
                                sources.Remove(s);
                            }
                        }
                    }
                }
            }
            for (int i = 1; i < networkTable.Count; i++)
            {
                string comid = networkTable[i][0].ToString();
                string hydro = networkTable[i][1].ToString();
                Dictionary<string, string> segSources = this.getSegmentSources(hydro);
                if (segSources.ContainsKey("ComID"))
                {
                    List<string> segSourceList = segSources["ComID"].Split(",").ToList();
                    foreach (string s in segSourceList)
                    {
                        if (!sources[comid].Contains(s))
                        {
                            sources[comid].Add(s);
                        }
                        if (!outNetwork.Contains(s) && !sources.ContainsKey(s))
                        {
                            outNetwork.Add(s);
                        }
                    }
                }
            }
            networkTable = filteredNetworkTable;

            //if pourpoint is divergent, merge divergent path results with pseudo-pourpoint results
            if (divergent)
            {
                foreach (KeyValuePair<string, List<string>> divSource in dResults["sources"] as Dictionary<string, List<string>>)
                {
                    if (!sources.ContainsKey(divSource.Key))
                    {
                        sources.Add(divSource.Key, divSource.Value);
                    }
                    else
                    {
                        sources[divSource.Key].Union(divSource.Value);
                    }
                }
                foreach (List<string> dOrder in dResults["order"] as List<List<string>>)
                {
                    order.Add(dOrder);
                }
                foreach (KeyValuePair<string, List<string>> divPar in dResults["divergent-paths"] as Dictionary<string, List<string>>)
                {
                    if (!divParallel.ContainsKey(divPar.Key))
                    {
                        divParallel.Add(divPar.Key, divPar.Value);
                    }
                    else
                    {
                        foreach (string pV in divPar.Value)
                        {
                            if (!divParallel[divPar.Key].Contains(pV))
                            {
                                divParallel[divPar.Key].Add(pV);
                            }
                        }
                    }
                }
            }
            else if (divergentUp)
            {
                divParallel[divSegment["mainstem"]] = new List<string>() { divSegment["divergence"] };
            }

            return new Dictionary<string, object>()
            {
                { "sources", sources },
                { "order", order },
                { "boundary", new Dictionary<string, object>(){ {"headwater", headwaters}, {"out-of-network", outNetwork}, {"divergence", divComids } } },
                { "divergent-paths", divParallel}
            };

        }

        public Dictionary<string, object> checkWaterbodies(List<string> comids)
        {
            string dbPath = Path.Combine(".", "App_Data", "catchments.sqlite");

            Dictionary<string, object> waterbodies = new Dictionary<string, object>();
            StringBuilder sb = new StringBuilder();
            Dictionary<string, object> tableMap = new Dictionary<string, object>();
            List<string> wbFields = new List<string>() { "GNIS_NAME", "AREASQKM", "ELEVATION", "REACHCODE", "FTYPE", "FCODE", "MeanDepth", "LakeVolume" };
            List<string> wbFieldTypes = new List<string>() { "string", "string", "double", "double", "int", "string", "int", "double", "double" };
            for(int i = 0; i < comids.Count; i++)
            {
                string comid = comids[i];
                //sb.Append("");
                sb.Append(comid);
                //sb.Append("");
                if(i < comids.Count - 1)
                {
                    sb.Append(", ");
                }
            }
            string comidsStr = sb.ToString();
            string query1 = "SELECT ComID, WBAREACOMI FROM PlusFlowlineVAA WHERE COMID IN (" + comidsStr + ") AND WBAREACOMI IS NOT NULL";
            Dictionary<string, string> aoiHUCq = Utilities.SQLite.GetData(dbPath, query1);
            List<List<object>> wbTable = new List<List<object>>();
            if (aoiHUCq.Count > 0) {
                List<string> catComids = aoiHUCq["ComID"].Split(",").ToList<string>();
                List<string> wbComids = aoiHUCq["WBAREACOMI"].Split(",").ToList<string>();

                StringBuilder wbsb = new StringBuilder();
                for (int i = 0; i < wbComids.Count; i++)
                {
                    waterbodies.Add(catComids[i], wbComids[i]);
                    string comid = wbComids[i];
                    //wbsb.Append("\"");
                    wbsb.Append(comid);
                    //wbsb.Append("\"");
                    if (i < wbComids.Count - 1)
                    {
                        wbsb.Append(", ");
                    }
                }

                string query2 = "SELECT ComID, " + String.Join(", ", wbFields) + " FROM Waterbodies WHERE COMID IN (" + wbsb.ToString() + ") AND NWM=1";
                Dictionary<string, object> wbParams = Utilities.SQLite.GetDataObject(dbPath, query2);
                wbTable.Add(wbParams.Keys.ToList<object>());

                bool initialized = false;
                int m = 0;
                foreach(KeyValuePair<string, object> kv in wbParams)
                {
                    List<string> values = kv.Value.ToString().Split(",").ToList();
                    if (!initialized)
                    {
                        for (int i = 0; i < values.Count; i++)
                        {
                            wbTable.Add(new List<object>());
                        }
                        initialized = true;
                    }
                    int n = 1;
                    for (int j = 0; j < values.Count; j++)
                    {
                        wbTable[n].Add(this.convertType(values[j], m, wbFieldTypes));
                        n++;
                    }
                    m++;
                }
            }
            return new Dictionary<string, object>{
                {"comid-wbcomid", waterbodies },
                {"waterbody-table", wbTable }
            };

        }

        private object convertType(object value, int typeI, List<string> typeList)
        {
            switch (typeList[typeI])
            {
                default:
                case "string":
                    return value.ToString();
                case "int":
                    if(value.Equals("NA"))
                    {
                        return -9998.0;
                    } 
                    return Int64.Parse(value.ToString());
                case "double":
                    if (value.Equals("NA"))
                    {
                        return -9998.0;
                    }
                    return Double.Parse(value.ToString());
                case "bool":
                    return Boolean.Parse(value.ToString());
            }

        }
    }
}
