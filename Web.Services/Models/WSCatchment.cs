﻿using Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WatershedDelineation;

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
        public async Task<Dictionary<string, object>> Get(string comid, bool streamcat = false, bool geometry = false, bool nwis = false, bool streamGeometry = false, bool cn = false, bool network = false)
        {
            string errorMsg = "";

            // Constructs default error output object containing error message.
            Utilities.ErrorOutput err = new Utilities.ErrorOutput();

            // Check comid
            if (comid is null) { return this.Error("ERROR: comid input is not valid."); }

            bool auto = nwis || geometry;
            GIS.Operations.Catchment catchment = new GIS.Operations.Catchment(comid, auto);

            Dictionary<string, string> metadata = Utilities.COMID.GetDbData(Convert.ToInt32(comid), out errorMsg);
            PointCoordinate centroid = Utilities.COMID.GetCentroid(Convert.ToInt32(comid), out errorMsg);

            Dictionary<string, object> result = new Dictionary<string, object>();
            Dictionary<string, Dictionary<string, string>> nwisGauges;
            if (errorMsg != "")
            {
                return this.Error(errorMsg);
            }

            if (nwis)
            {
                Dictionary<string, object> comidGages = Utilities.COMID.GetGageID(Convert.ToInt32(comid), out errorMsg);
                if (comidGages.Count > 0)
                {
                    Data.Source.StreamGauge sg = new Data.Source.StreamGauge();
                    nwisGauges = sg.StationLookup(Convert.ToInt32(comidGages["GAGEID"]));
                    if (nwisGauges != null)
                    {
                        nwisGauges.Add("metadata", Utilities.Converter.ConvertDict(comidGages));
                    }
                }
                else
                {
                    if (centroid != null)
                    {
                        nwisGauges = catchment.GetNWISGauges(new List<double>() { centroid.Latitude, centroid.Longitude });
                    }
                    else
                    {
                        nwisGauges = catchment.GetNWISGauges();
                    }


                    if (nwisGauges != null)
                    {
                        nwisGauges.Add("metadata", new Dictionary<string, string>()
                        {
                            {"NWIS_GAGE_NOTE", "Direct linkage between NWIS gage ID and NHDPlus catchment COMID was not found, gage determined from expanding bounds of catchment." }
                        });
                    }
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
                result.Add("stream_geometry", catchment.GetStreamGeometryV2(comid));
                //result.Add("stream_geometry", catchment.GetStreamGeometry(centroid.Latitude, centroid.Longitude));
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
            if (network)
            {
                WatershedDelineation.Streams streamN = new WatershedDelineation.Streams(comid, null, null);
                var streamNetwork = streamN.GetNetwork();
                result.Add("network", streamNetwork);
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
