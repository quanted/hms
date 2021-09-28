using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WatershedDelineation;

namespace Web.Services.Models
{
    /// <summary>
    /// HMS Web Service Stream Model
    /// </summary>
    public class WSStream
    {

        /// <summary>
        /// Gets stream network data for a provided comid
        /// </summary>
        /// <param name="comid"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, object>> Get(string comid, string endComid = null, string huc = null, double maxDistance = 50.0, bool mainstem = false)
        {
            string errorMsg = "";

            // Constructs default error output object containing error message.
            Utilities.ErrorOutput err = new Utilities.ErrorOutput();

            // Check comid
            if (comid is null) { return this.Error("ERROR: comid input is not valid."); }

            Dictionary<string, object> result = new Dictionary<string, object>();

            WatershedDelineation.Streams streamN = new WatershedDelineation.Streams(comid, null, null);
            var streamNetwork = streamN.GetNetwork(maxDistance, endComid, mainstem);
            List<List<object>> networkTable = StreamNetwork.generateTable(streamNetwork, null);
            List<List<int>> segOrder = new List<List<int>>();
            if (mainstem)
            {
                string switchedComid = null;
                if (!String.IsNullOrEmpty(endComid))
                {
                    switchedComid = comid;
                }
                segOrder = this.generateMainstemOrder(networkTable, switchedComid);

            }
            else
            {
                //segOrder = this.generateOrder(networkTable);
                result = this.generateOrderAndSources(networkTable, comid, huc);
                result.Add("network", networkTable);
                return result;
            }
            if (!String.IsNullOrEmpty(endComid))
            {
                networkTable = this.purgeTable(networkTable, segOrder);
            }
            result = this.getSourceComids(segOrder, networkTable);

            result.Add("network", networkTable);
            result.Add("order", segOrder);
            //result.Add("sources", sourceComIDs);
            return result;
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

        public List<List<int>> generateOrder(List<List<object>> networkTable)
        {
            string dbPath = Path.Combine(".", "App_Data", "catchments.sqlite");

            List<int> dag = new List<int>();
            for (int i = networkTable.Count - 1; i > 0; i--)
            {
                int hydroseq = Int32.Parse(networkTable[i][1].ToString());
                dag.Add(hydroseq);
            }

            List<List<int>> seqOrder = new List<List<int>>();
            seqOrder.Add(new List<int>()
            {
                Int32.Parse(networkTable[1][1].ToString())
            });
            List<List<int>> comidOrder = new List<List<int>>();
            comidOrder.Add(new List<int>()
            {
                Int32.Parse(networkTable[1][0].ToString())
            });
            for (int i = 2; i <= dag.Count; i++)
            {
                seqOrder.Add(new List<int>());
                comidOrder.Add(new List<int>());
            }
            for (int i = 2; i <= dag.Count; i++)
            {
                int comid = Int32.Parse(networkTable[i][0].ToString());
                int hydroseq = Int32.Parse(networkTable[i][1].ToString());
                int dnhydroseq = Int32.Parse(networkTable[i][3].ToString());
                string query = "SELECT Hydroseq FROM PlusFlowlineVAA WHERE DnHydroseq=" + hydroseq.ToString();
                Dictionary<string, string> sourceComid = Utilities.SQLite.GetData(dbPath, query);
                List<int> uphydroseq = new List<int>();
                if (sourceComid.ContainsKey("Hydroseq"))
                {
                    foreach(string c in sourceComid["Hydroseq"].Split(","))
                    {
                        uphydroseq.Add(Int32.Parse(c));
                    }
                }
                else
                {
                    uphydroseq.Add(Int32.Parse(networkTable[i][2].ToString()));
                }
                int seq_j = 0;
                for (int j = seqOrder.Count - 1; j >= 0; j--)
                {
                    foreach(int up in uphydroseq)
                    {
                        if (seqOrder[j].Contains(up))
                        {
                            seq_j = j - 1;
                            break;
                        }
                    }             
                    if (seqOrder[j].Contains(dnhydroseq))
                    {
                        seq_j = j + 1;
                        break;
                    }
                }
                if (seq_j == -1)
                {
                    List<int> newOrderLevel = new List<int>();
                    newOrderLevel.Add(comid);
                    comidOrder = comidOrder.Prepend(newOrderLevel).ToList();
                    List<int> newSeqLevel = new List<int>();
                    newSeqLevel.Add(hydroseq);
                    seqOrder = seqOrder.Prepend(newSeqLevel).ToList();
                }
                else
                {
                    comidOrder[seq_j].Add(comid);
                    seqOrder[seq_j].Add(hydroseq);
                }
            }
            int nOrder = comidOrder.Count;
            for (int i = nOrder - 1; i >= 0; i--)
            {
                if (comidOrder[i].Count == 0)
                {
                    comidOrder.RemoveAt(i);
                }
            }
            comidOrder.Reverse();

            return comidOrder;
        }

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
                        order[m].Add(sequence[i]);
                    }
                    j++;
   
                }
            }
        }


        public Dictionary<string, object> generateOrderAndSources(List<List<object>> networkTable, string pourpoint, string huc = "")
        {
            string dbPath = Path.Combine(".", "App_Data", "catchments.sqlite");

            huc = (huc == null) ? "" : huc;
            List<int> networkHydro = new List<int>();
            Dictionary<string, List<string>> sources = new Dictionary<string, List<string>>();
            Dictionary<int, List<object>> hydroMapping = new Dictionary<int, List<object>>();
            Dictionary<int, string> hydroComid = new Dictionary<int, string>();
            List<int> outOfNetwork = new List<int>();
            string networkHuc = networkTable[1][7].ToString();

            for(int i = 1; i < networkTable.Count; i++)
            {
                string comid = networkTable[i][0].ToString();
                int hydro = Int32.Parse(networkTable[i][1].ToString());
                string huc12 = networkTable[i][7].ToString();
                if (huc.Equals("12") && !huc12.Equals(networkHuc))
                {
                    outOfNetwork.Add(hydro);
                }
                else
                {
                    networkHydro.Add(hydro);
                    sources.Add(comid, new List<string>());
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
                    if (!outOfNetwork.Contains(dnhydro))
                    {
                        if (!sources[hydroMapping[dnhydro][0].ToString()].Contains(comid))
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
                else if (!networkHydro.Contains(uphydro))
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
                    ReorderSequence(ref order, sequence);
                }
                if(networkHydro.Count != traversed.Count)
                {
                    List<int> diff = new List<int>();
                    foreach(int hydro in hydroComid.Keys)
                    {
                        if (!traversed.Contains(hydro) && hydroComid[hydro] != pourpoint)
                        {
                            diff.Add(hydro);
                        }
                    }
                    for(int i = 0; i < diff.Count; i++)
                    {
                        if (traversed.Contains(Int32.Parse(hydroMapping[diff[i]][2].ToString())))
                        {
                            diffEdges.Add(diff[i]);
                            divComids.Add(hydroComid[diff[i]]);
                            sources[hydroComid[diff[i]]].Add(hydroComid[Int32.Parse(hydroMapping[diff[i]][2].ToString())]);
                        }
                    }
                    for(int i = 0; i < diffEdges.Count; i++)
                    {
                        traversed.Add(diffEdges[i]);
                        List<string> sequence = new List<string>();
                        sequence.Add(hydroComid[diffEdges[i]]);
                        int dnHydro = Int32.Parse(hydroMapping[diffEdges[i]][3].ToString());
                        recTraverse(dnHydro, hydroComid[diffEdges[i]], pourpoint, ref sequence, ref traversed, ref sources, hydroComid, hydroMapping);
                        ReorderSequence(ref order, sequence);
                    }
                }
            }

            return new Dictionary<string, object>()
            {
                { "sources", sources },
                { "order", order },
                { "boundary", new Dictionary<string, object>(){ {"headwater", headwaters}, {"out-of-network", outNetwork}, {"divergence", divComids } }
                }
            };

        }


    }
}
