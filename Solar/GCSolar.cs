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

        static Common common = new Common();
        /// <summary>
        /// Gets output data for the default values for GCSolar,
        /// equivalent to the third option on the windows start form.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> GetOutput()
        {
            DataTable dtDay = new DataTable();
            DataTable dtKL = new DataTable();
            Depend dp = new Depend();
            dp.CalculatePhotolysisRatesHalfLivesTDay(out dtDay, out dtKL, common);
            Dictionary<string, object> t1 = ToDictionary(dtDay);
            Dictionary<string, object> t2 = ToDictionary(dtKL);
            Dictionary<string, object> result = new Dictionary<string, object>
            {
                ["dtDay"] = t1,
                ["dtKL"] = t2
            };
            return result;
        }

        /// <summary>
        /// Gets the default input values for GCSolar,
        /// equivalent to the first option on the windows start form.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> GetDefaultInputs()
        {
            DataTable dtList = new DataTable();
            common.Listing(out dtList);
            Dictionary<string, object> result = new Dictionary<string, object>();
            result.Add("Containment Name", common.contaminantName);
            result.Add("Water Type Name", common.bodyWaterName);
            result.Add("Type of Atmosphere", common.typeAtmos);
            result.Add("Longitude", Convert.ToString(common.xlon));
            result.Add("Elevation (km)", Convert.ToString(common.elevation));
            result.Add("Quantum Yield", Convert.ToString(common.q));
            result.Add("Initial Depth (cm)", Convert.ToString(common.dinit));
            result.Add("Depth Increment (cm)", Convert.ToString(common.dinc));
            result.Add("Final Depth (cm)", Convert.ToString(common.dfinal));
            result.Add("Refractive Index", Convert.ToString(common.musubr));
            if (common.useDeltaz == true)
            {
                result.Add("Depth Point", Convert.ToString(common.deltaz));
            }
            else
            {
                result.Add("Depth Point", "None");
            }
            result.Add("Season(s)", common.sease);
            result.Add("Latitude(s)", common.ilattm);
            Dictionary<string, object> tbl = ToDictionary(dtList);
            result.Add("Input Table", tbl);
            return result;
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

        /// <summary>
        /// Sets the instance Common variables from the input dictionary provided.
        /// </summary>
        /// <param name="cc"></param>
        public void SetCommonVariables(Dictionary<string, object> cc)
        {
            string test = "";
        }

    }
}
