using Data;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
        public async Task<Dictionary<string, object>> Get(string comid, string endComid=null, string huc=null, double maxDistance=50.0)
        {
            string errorMsg = "";
            
            // Constructs default error output object containing error message.
            Utilities.ErrorOutput err = new Utilities.ErrorOutput();

            // Check comid
            if(comid is null) { return this.Error("ERROR: comid input is not valid."); }

            Dictionary<string, object> result = new Dictionary<string, object>();

            WatershedDelineation.Streams streamN = new WatershedDelineation.Streams(comid, null, null);
            var streamNetwork = streamN.GetNetwork(maxDistance, endComid);
            List<List<object>> networkTable = StreamNetwork.generateTable(streamNetwork, huc);
            result.Add("network", networkTable);
            List<List<int>> segOrder = this.generateOrder(networkTable);
            result.Add("order", segOrder);

            return result;
        }

        public List<List<int>> generateOrder(List<List<object>> networkTable)
        {
            List<List<int>> networkOrder = new List<List<int>>();
            List<List<int>> comidOrder = new List<List<int>>();
            // Initialize network order table
            for(int i = 0; i < networkTable.Count; i++)
            {
                networkOrder.Add(new List<int>());
                comidOrder.Add(new List<int>());
            }
            // Build segment order
            for(int i = networkTable.Count-1; i > 0; i--)
            {
                int comid = Int32.Parse(networkTable[i][0].ToString());
                int hydroseq = Int32.Parse(networkTable[i][1].ToString());
                int uphydroseq = Int32.Parse(networkTable[i][2].ToString());

                int orderLvl = 0;

                for (int j = networkTable.Count-1; j >= 0; j--)
                {
                    if (networkOrder[j].Contains(uphydroseq))
                    {
                        orderLvl = j+1;
                        break;
                    }
                }
                networkOrder[orderLvl].Add(hydroseq);
                comidOrder[orderLvl].Add(comid);
            }
            // List cleanup
            for(int i = networkOrder.Count-1; i > 0; i--)
            {
                if(comidOrder[i].Count == 0)
                {
                    comidOrder.RemoveAt(i);
                }
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
