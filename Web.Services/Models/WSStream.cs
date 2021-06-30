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
            List<List<object>> networkTable = StreamNetwork.generateTable(streamNetwork, huc);
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
                segOrder = this.generateOrder(networkTable);
            }
            if (!String.IsNullOrEmpty(endComid))
            {
                networkTable = this.purgeTable(networkTable, segOrder);
            }
            Dictionary<string, List<string>> sourceComIDs = this.getSourceComids(segOrder, networkTable);

            result.Add("network", networkTable);
            result.Add("order", segOrder);
            result.Add("sources", sourceComIDs);
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

        private Dictionary<string, List<string>> getSourceComids(List<List<int>> order, List<List<object>> table)
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
            sourceCOMIDs.Add("boundaries", boundaryCOMIDS);
            return sourceCOMIDs;
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
    }
}
