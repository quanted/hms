using Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Web.Services.Models
{
    /// <summary>
    /// HMS Web Service Catchment Model
    /// </summary>
    public class WSCatchment
    {

        /// <summary>
        /// Gets catchment data for a provided comid
        /// </summary>
        /// <param name="comid"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, object>> Get(string comid)
        {
            string errorMsg = "";
            
            // Constructs default error output object containing error message.
            Utilities.ErrorOutput err = new Utilities.ErrorOutput();

            // Check comid
            if(comid is null) { return this.Error("ERROR: comid input is not valid."); }

            GIS.Operations.Catchment catchment = new GIS.Operations.Catchment(comid);
            if (catchment.data is null || catchment.data.features.Count == 0)
            {
                errorMsg = "ERROR: Unable to get catchment data for COMID: " + comid;
                return this.Error(errorMsg);
            }
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            PointCoordinate centroid = Utilities.COMID.GetCentroid(Convert.ToInt32(comid), out errorMsg);
            Dictionary<string, Dictionary<string, string>> nwisGauges;
            if (centroid != null)
            {
                nwisGauges = catchment.GetNWISGauges(new List<double>() { centroid.Latitude, centroid.Longitude });
                metadata.Add("catchment_centroid_latitude", centroid.Latitude.ToString());
                metadata.Add("catchment_centroid_longitude", centroid.Longitude.ToString());
            }
            else
            {
               nwisGauges = catchment.GetNWISGauges();

            }
            Dictionary<string, object> streamcat = catchment.GetStreamcatData();

            Dictionary<string, object> result = new Dictionary<string, object>();
            if(catchment.data != null) { result.Add("nhdplus", catchment.data); }
            if(nwisGauges != null) { result.Add("nwisGauges", nwisGauges); }
            if(streamcat != null) { result.Add("streamcat", streamcat); }
            result.Add("metadata", metadata);

            // Weather Station
            // URLS to data sources (NLDAS, GLDAS), local or outside
            // Flowlines TO/FROM comids from DB

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