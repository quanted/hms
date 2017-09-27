using GCSOLAR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Solar
{
    public class GCSolar
    {
        /// <summary>
        /// Gets output data for the default values for GCSolar,
        /// equivalent to the third option on the windows start form.
        /// </summary>
        /// <returns></returns>
        public string GetDefaultData()
        {
            DataTable dtDay = new DataTable();
            DataTable dtKL = new DataTable();
            Depend dp = new Depend();
            dp.CalculatePhotolysisRatesHalfLivesTDay(out dtDay, out dtKL);
            Dictionary<string, object> t1 = ToDictionary(dtDay);
            Dictionary<string, object> t2 = ToDictionary(dtKL);
            Dictionary<string, object> result = new Dictionary<string, object>
            {
                ["dtDay"] = t1,
                ["dtKL"] = t2
            };
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// Gets the default input values for GCSolar,
        /// equivalent to the first option on the windows start form.
        /// </summary>
        /// <returns></returns>
        public string GetDefaultInputs()
        {
            DataTable dtList = new DataTable();
            Common.Listing(out dtList);
            Dictionary<string, object> result = new Dictionary<string, object>();
            result.Add("Containment Name", Common.contaminantName);
            result.Add("Water Type Name", Common.bodyWaterName);
            result.Add("Type of Atmosphere", Common.typeAtmos);
            result.Add("Longitude", Convert.ToString(Common.xlon));
            result.Add("Elevation (km)", Convert.ToString(Common.elevation));
            result.Add("Quantum Yield", Convert.ToString(Common.q));
            result.Add("Initial Depth (cm)", Convert.ToString(Common.dinit));
            result.Add("Depth Increment (cm)", Convert.ToString(Common.dinc));
            result.Add("Final Depth (cm)", Convert.ToString(Common.dfinal));
            result.Add("Refractive Index", Convert.ToString(Common.musubr));
            if (Common.useDeltaz == true)
            {
                result.Add("Depth Point", Convert.ToString(Common.deltaz));
            }
            else
            {
                result.Add("Depth Point", "None");
            }
            result.Add("Season(s)", Common.sease.ToString());
            result.Add("Latitude(s)", Common.ilattm.ToString());
            Dictionary<string, object> tbl = ToDictionary(dtList);
            result.Add("Input Table", tbl);
            return JsonConvert.SerializeObject(result);
        }

        /// <summary>
        /// Converts a DataTable into a Dictionary. DataTable enumeration is not yet supported in .NET Core 2.
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        internal Dictionary<string, object> ToDictionary(DataTable dt)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var row = dt.Rows[i];
                Dictionary<string, string> rowDict = new Dictionary<string, string>();
                for (int j = 0; j < row.ItemArray.Count(); j++)
                {
                    string k = dt.Columns[j].ToString();
                    string v = row.ItemArray[j].ToString();
                    rowDict.Add(k, v);
                }
                dic.Add(i.ToString(), rowDict);
            }
            return dic;
        }

    }
}
