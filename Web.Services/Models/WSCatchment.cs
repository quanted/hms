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
        public async Task<Dictionary<string, object>> Get(string comid, bool streamcat=true, bool geometry=true, bool nwis=true, bool streamGeometry=false, bool cn=false)
        {
            string errorMsg = "";
            
            // Constructs default error output object containing error message.
            Utilities.ErrorOutput err = new Utilities.ErrorOutput();

            // Check comid
            if(comid is null) { return this.Error("ERROR: comid input is not valid."); }

            GIS.Operations.Catchment catchment = new GIS.Operations.Catchment(comid, geometry);

            Dictionary<string, string> metadata = Utilities.COMID.GetDbData(Convert.ToInt32(comid), out errorMsg);
            PointCoordinate centroid = Utilities.COMID.GetCentroid(Convert.ToInt32(comid), out errorMsg);

            Dictionary<string, object> result = new Dictionary<string, object>();

            Dictionary<string, Dictionary<string, string>> nwisGauges;
            if (nwis)
            {
                if (centroid != null)
                {
                    nwisGauges = catchment.GetNWISGauges(new List<double>() { centroid.Latitude, centroid.Longitude });
                }
                else
                {
                    nwisGauges = catchment.GetNWISGauges();

                }
                if (nwisGauges != null) { result.Add("nwisGauges", nwisGauges); }
            }
            if (geometry)
            {
                if (catchment.data is null || catchment.data.features.Count == 0)
                {
                    result.Add("nhdplus", "ERROR: Unable to get catchment data for COMID: " + comid);
                }
                else
                {
                    result.Add("nhdplus", catchment.data); 
                }
            }
            if (streamcat)
            {
                Dictionary<string, object> streamcatData = catchment.GetStreamcatData();

                if (streamcatData != null) { result.Add("streamcat", streamcatData); }
            }
            if (streamGeometry && centroid != null)
            {
                result.Add("stream_geometry", catchment.GetStreamGeometry(centroid.Latitude, centroid.Longitude));
            }
            if (cn)
            {
                Data.Simulate.CurveNumber curveNumber = new Data.Simulate.CurveNumber();
                Dictionary<int, double> cnValues = curveNumber.GetCN(out errorMsg, Convert.ToInt32(comid));
                if (errorMsg.Contains("ERROR"))
                {
                    result.Add("curve_number", errorMsg);
                }
                else
                {
                    DateTime dt = new DateTime(2000, 1, 1);
                    Dictionary<string, string> cnData = new Dictionary<string, string>();
                    foreach (KeyValuePair<int, double> kv in cnValues) 
                    {
                        cnData.Add(dt.ToString("MM-dd"), kv.Value.ToString());
                        dt = dt.AddDays(16);
                    }
                    result.Add("curve_number", cnData);
                }
            }
            result.Add("metadata", metadata);

            // Weather Station
            // URLS to data sources (NLDAS, GLDAS), local or outside
            // Add NWM short term forecast, from Time of Travel.
            
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
