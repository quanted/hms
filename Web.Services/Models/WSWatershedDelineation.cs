using Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using WatershedDelineation;
using Web.Services.Controllers;

namespace Web.Services.Models
{
    public class WSWatershedDelineation
    {
        /// <summary>
        /// Gets workflow data.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ITimeSeriesOutput> GetDelineationData(WatershedDelineationInput input)
        {
            string errorMsg = "";
            // Constructs default error output object containing error message.
            Utilities.ErrorOutput err = new Utilities.ErrorOutput();
            

            WatershedDelineation.Streams streamNetwork = new WatershedDelineation.Streams(input.Geometry.GeometryMetadata["startCOMID"], input.Geometry.GeometryMetadata["stopCOMID"], null);
            LinkedList<StreamSegment> travelPath = streamNetwork.GetStreams(input);

            /* Stream Network Delineation
            List<string> lst = new List<string>();
            WatershedDelineation.StreamNetwork sn = new WatershedDelineation.StreamNetwork();
            string gtype = "";
            if (input.Geometry.GeometryMetadata.ContainsKey("huc_8_num"))
            {
                gtype = "huc_8_num";
                input.Geometry.HucID = input.Geometry.GeometryMetadata["huc_8_num"];
            }
            else if (input.Geometry.GeometryMetadata.ContainsKey("huc_12_num"))
            {
                gtype = "huc_12_num";
                input.Geometry.HucID = input.Geometry.GeometryMetadata["huc_12_num"];
            }
            else if (input.Geometry.GeometryMetadata.ContainsKey("com_id_num"))
            {
                gtype = "com_id_num";
                input.Geometry.ComID = int.Parse(input.Geometry.GeometryMetadata["huc_8_num"]);
            }
            else if (input.Geometry.GeometryMetadata.ContainsKey("com_id_list"))
            {
                gtype = "com_id_list";
            }
            DataTable dt = sn.prepareStreamNetworkForHUC(input.Geometry.HucID.ToString(), gtype, out errorMsg, out lst);             //list of coms?
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }*/

            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput delinOutput = oFactory.Initialize();

            //Format data into ITimeSeriesOutput format.
            foreach(string date in travelPath.First.Value.timestepData.Keys)
            {
                List<string> lv = new List<string>();
                foreach(StreamSegment node in travelPath)
                {
                    lv.AddRange(node.timestepData[date]);
                }
                delinOutput.Data.Add(date,lv);//delinOutput.Data.Add(date, travelPath.First.Value.timestepData[date]);
            }

            return delinOutput;
        }
    }
}
