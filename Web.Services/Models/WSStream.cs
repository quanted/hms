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
            
            return result;
        }

        private Dictionary<string, object> Error(string errorMsg)
        {
            Dictionary<string, object> output = new Dictionary<string, object>();
            output.Add("ERROR", errorMsg);
            return output;
        }
    }
}
