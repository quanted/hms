using Data;
using Newtonsoft.Json.Linq;
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
        public async Task<Dictionary<string, object>> Get(string comid, string endComid=null, string huc=null, double maxDistance=50.0, bool mainstem=false)
        {
            string errorMsg = "";
            
            // Constructs default error output object containing error message.
            Utilities.ErrorOutput err = new Utilities.ErrorOutput();

            // Check comid
            if(comid is null) { return this.Error("ERROR: comid input is not valid."); }

            Dictionary<string, object> result = new Dictionary<string, object>();

            WatershedDelineation.Streams streamN = new WatershedDelineation.Streams(comid, null, null);
            var streamNetwork = streamN.GetNetwork(maxDistance, endComid, mainstem);
            List<List<object>> networkTable = StreamNetwork.generateTable(streamNetwork, huc);
            result.Add("network", networkTable);
            List<List<int>> segOrder = this.generateOrder(networkTable);
            result.Add("order", segOrder);
            Dictionary<string, List<string>> sourceComIDs = this.getSourceComids(segOrder, networkTable);
            result.Add("sources", sourceComIDs);
            return result;
        }

        private Dictionary<string, List<string>> getSourceComids(List<List<int>> order, List<List<object>> table)
        {
            string dbPath = Path.Combine(".", "App_Data", "catchments.sqlite");
            Dictionary<string, List<string>> sourceCOMIDs = new Dictionary<string, List<string>>();
            List<int> hydroList = new List<int>();
            for(int i = 1; i < table.Count; i++)
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
                for(int j = 1; j < table.Count; j++)
                {
                    string comid2 = table[j][0].ToString();
                    int hydroseq2 = Int32.Parse(table[j][1].ToString());
                    if (hydroseq2 == dnhydroseq)
                    {
                        sourceCOMIDs[comid2].Add(comid);
                    }
                }
                if(uphydroseq != 0 && !hydroList.Contains(uphydroseq))
                {
                    string query = "SELECT COMID FROM PlusFlowlineVAA WHERE Hydroseq=" + hydroseq.ToString();
                    Dictionary<string, string> sourceComid = Utilities.SQLite.GetData(dbPath, query);
                    sourceCOMIDs[comid].Add(sourceComid["ComID"]);
                    boundaryCOMIDS.Add(sourceComid["ComID"]);
                }
            }
            sourceCOMIDs.Add("boundaries", boundaryCOMIDS);
            return sourceCOMIDs;
        }

        public List<List<int>> generateOrder(List<List<object>> networkTable)
        {

            List<int> dag = new List<int>();
            for(int i = networkTable.Count - 1; i > 0; i--)
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
                int uphydroseq = Int32.Parse(networkTable[i][2].ToString());
                int seq_j = 0;
                for (int j = seqOrder.Count - 1; j >= 0; j--)
                {
                    if (seqOrder[j].Contains(dnhydroseq))
                    {
                        seq_j = j + 1;
                        break;
                    }
                    else if (seqOrder[j].Contains(uphydroseq))
                    {
                        seq_j = j - 1;
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

        private Dictionary<string, object> Error(string errorMsg)
        {
            Dictionary<string, object> output = new Dictionary<string, object>();
            output.Add("ERROR", errorMsg);
            return output;
        }
    }
}
