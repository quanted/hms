using Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
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

            //Stream Network Delineation
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
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput delinOutput = oFactory.Initialize();

            //Turn delineation table to ITimeseries
            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                List<string> lv = new List<string>();
                foreach (Object g in dr.ItemArray)
                {
                    lv.Add(g.ToString());
                }
                delinOutput.Data.Add(i++.ToString(), lv);
            }
            return delinOutput;
        }
    }
}
