using Data;
using System;
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
        public async Task<WatershedDelineationOutput> GetDelineationData(WatershedDelineationInput input)
        {
            string errorMsg = "";
            string comids = "";
            // Constructs default error output object containing error message.
            Utilities.ErrorOutput err = new Utilities.ErrorOutput();
            

            WatershedDelineation.Streams streamNetwork = new WatershedDelineation.Streams(input.Geometry.GeometryMetadata["startCOMID"], input.Geometry.GeometryMetadata["endCOMID"], null);
            LinkedList<StreamSegment> travelPath = streamNetwork.GetStreams(input, input.contaminantInflow, input.inflowSource, out comids);

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

            //ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            //ITimeSeriesOutput delinOutput = oFactory.Initialize();

            WatershedDelineationOutput delinOutput = new WatershedDelineationOutput();

            /*Format data into ITimeSeriesOutput format. Dates as primary keys
            foreach(string date in travelPath.First.Value.timestepData.Keys)
            {
                List<string> lv = new List<string>();
                foreach(StreamSegment node in travelPath)
                {
                    lv.AddRange(node.timestepData[date]);
                }
                delinOutput.Data.Add(date,lv);//delinOutput.Data.Add(date, travelPath.First.Value.timestepData[date]);
            }*/

            delinOutput.Data = new Dictionary<string, Dictionary<string, List<string>>>();
            List<string> lv = new List<string>();
            foreach (StreamSegment node in travelPath)
            {
                Dictionary<string, List<string>> timesteps = node.timestepData;
                delinOutput.Data.Add(node.comID, timesteps);//delinOutput.Data.Add(date, travelPath.First.Value.timestepData[date]);
            }



            delinOutput.Dataset = "time_of_travel";
            delinOutput.DataSource = "nwm";
            delinOutput.Metadata = new Dictionary<string, string>()
            {
                { "request_time", DateTime.Now.ToString() },
                { "column_1", "Date" },
                { "column_2", "ComID" },
                { "column_3", "Length (km)" },
                { "column_4", "Velocity(m/s)" },
                { "column_5", "Flow (m^3/s)" },
                { "column_6", "Contaminated" },
                { "catchments", comids.TrimEnd(',')}/*,
                { "column_7", "ComID" },
                { "column_8", "Length (km)" },
                { "column_9", "Velocity(m/s)" },
                { "column_10", "Flow (m^3/s)" },
                { "column_11", "Contaminated" }*/
            };
            // Adds Timezone info to metadata
            //delinOutput.Metadata.Add(input.Source + "_timeZone", input.Geometry.Timezone.Name);
            //delinOutput.Metadata.Add(input.Source + "_tz_offset", input.Geometry.Timezone.Offset.ToString());

            return delinOutput;
        }
    }
}
