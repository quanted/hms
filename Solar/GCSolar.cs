using GCSOLAR;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Solar
{
    public class GCSolar
    {

        public Common common = new Common();
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
            //Dictionary<string, object> t1 = ToDictionary(dtDay);
            //Dictionary<string, object> t2 = ToDictionary(dtKL);
            object[] t1 = ToOutputArray(dtDay);
            object[] t2 = ToOutputArray(dtKL);
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
            result.Add("Contaminant Name", common.contaminantName);
            result.Add("Contaminant Type", common.contaminantType);
            result.Add("Water Type Name", common.bodyWaterName);

            result.Add("Min Wavelength", common.getMinWave(common.minwav));
            result.Add("Max Wavelength", common.getMaxWave(common.maxwav));

            result.Add("Longitude", Convert.ToString(common.xlon));
            result.Add("Latitude(s)", common.ilattm);
            result.Add("Season(s)", common.sease);
            result.Add("Atmospheric Ozone Layer", common.aveozo);

            result.Add("Initial Depth (cm)", Convert.ToString(common.dinit));
            result.Add("Final Depth (cm)", Convert.ToString(common.dfinal));
            result.Add("Depth Increment (cm)", Convert.ToString(common.dinc));

            result.Add("Quantum Yield", Convert.ToString(common.q));
            result.Add("Refractive Index", Convert.ToString(common.musubr));
            result.Add("Elevation (km)", Convert.ToString(common.elevation));

            DataTable inputTable = new DataTable();
            common.Listing(out inputTable);
            result.Add("wavelength table", ToOutputArray(inputTable));

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
                for (int j = 1; j < row.ItemArray.Count(); j++)
                {
                    string k = dt.Columns[j].ToString();
                    string v = row.ItemArray[j].ToString();
                    rowDict.Add(k, v);
                }
                string key = dt.Rows[i][0].ToString();
                dic.Add(i.ToString(), rowDict);
            }
            return dic;
        }

        /// <summary>
        /// Converts a DataTable into a Dictionary. DataTable enumeration is not yet supported in .NET Core 2.
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        internal object[] ToOutputArray(DataTable dt)
        {
            int iMax = dt.Rows.Count;
            object[] values = new object[iMax];

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var row = dt.Rows[i];
                Dictionary<string, string> rowDict = new Dictionary<string, string>();
                for (int j = 1; j < row.ItemArray.Count(); j++)
                {
                    string k = dt.Columns[j].ToString();
                    string v = row.ItemArray[j].ToString();
                    rowDict.Add(k, v);
                }
                //string key = dt.Rows[i][0].ToString();
                string value = JsonConvert.SerializeObject(rowDict);
                values[i] = rowDict;
                //dic.Add(i.ToString(), rowDict);

            }
            return values;
        }

        /// <summary>
        /// Sets the instance Common variables from the input dictionary provided.
        /// </summary>
        /// <param name="cc"></param>
        public void SetCommonVariables(Dictionary<string, object> cc)
        {
            // TODO: Add variable validation for each assignment (with corresponding error messages for each)
            double[] waves = common.getWave();
            bool ephemerideValues = false;
            foreach (KeyValuePair<string, object> p in cc)
            {
                switch(p.Key.ToLower())
                {
                    case "contaminant name":
                        common.contaminantName = p.Value.ToString();
                        break;
                    case "contaminant type":
                        common.contaminantType = p.Value.ToString();
                        break;
                    case "water type name":
                        common.bodyWaterName = p.Value.ToString();
                        break;
                    case "min wavelength":
                        double minWave = Convert.ToDouble(p.Value);
                        if (minWave != 0)
                        {
                            int iMax = waves.Count();
                            for(int i=1; i <= iMax; i++)
                            {
                                double t1 = waves[i];
                                if(Math.Abs(minWave) - waves[i-1] < 0.001)
                                {
                                    common.minwav = i;
                                    break;
                                }
                            }
                        }
                        break;
                    case "max wavelength":
                        double maxWave = Convert.ToDouble(p.Value);
                        if (maxWave != 0)
                        {
                            int iMax = waves.Count();
                            for (int i = 1; i <= iMax; i++)
                            {
                                if (Math.Abs(maxWave) - waves[i-1] < 0.001)
                                {
                                    common.maxwav = i;
                                    break;
                                }
                            }
                        }
                        break;
                    case "longitude":
                        common.xlon = Convert.ToDouble(p.Value);
                        break;
                    case "latitude(s)":
                        double[] latitudeArray = JsonConvert.DeserializeObject<double[]>(p.Value.ToString());
                        common.ilattm = latitudeArray;
                        break;
                    case "season(s)":
                        string[] seasonArray = JsonConvert.DeserializeObject<string[]>(p.Value.ToString());
                        common.sease = seasonArray;
                        ephemerideValues = true;
                        break;
                    case "latitude":
                        if(!ephemerideValues)
                            common.typlat = Convert.ToDouble(p.Value);
                        break;
                    case "solar declination":
                        if (!ephemerideValues)
                        {
                            double[] sd_values = JsonConvert.DeserializeObject<double[]>(p.Value.ToString());
                            common.xx[0] = sd_values[0];
                            common.xx[1] = sd_values[1];
                            common.xx[2] = sd_values[2];
                        }
                        break;
                    case "right declination":
                        if (!ephemerideValues)
                        {
                            double[] rd_values = JsonConvert.DeserializeObject<double[]>(p.Value.ToString());
                            common.xx[3] = rd_values[0];
                            common.xx[4] = rd_values[1];
                            common.xx[5] = rd_values[2];
                        }
                        break;
                    case "sidereal time":
                        if (!ephemerideValues)
                        {
                            double[] st_values = JsonConvert.DeserializeObject<double[]>(p.Value.ToString());
                            common.xx[6] = st_values[0];
                            common.xx[7] = st_values[1];
                            common.xx[8] = st_values[2];
                        }
                        break;
                    case "atmospheric ozone layer":
                        if (ephemerideValues)
                        {
                            common.aveozo = Convert.ToDouble(p.Value);
                        }
                        break;
                    case "initial depth":
                        common.dinit = Convert.ToDouble(p.Value);
                        break;
                    case "final depth":
                        common.dfinal = Convert.ToDouble(p.Value);
                        break;
                    case "depth increment":
                        common.dinc = Convert.ToDouble(p.Value);
                        break;
                    case "quantum yield":
                        common.q = Convert.ToDouble(p.Value);
                        break;
                    case "refractive index":
                        common.musubr = Convert.ToDouble(p.Value);
                        break;
                    case "elevation":
                        common.elevation = Convert.ToDouble(p.Value);
                        break;
                    case "wavelength table":
                        Dictionary<string, object> waveTable = JsonConvert.DeserializeObject <Dictionary<string, object>>(p.Value.ToString());
                        foreach(KeyValuePair<string, object> q in waveTable)
                        {
                            int index = Array.IndexOf(waves,Convert.ToDouble(q.Key));
                            Dictionary<string, object> values = JsonConvert.DeserializeObject<Dictionary<string, object>>(q.Value.ToString());
                            values = new Dictionary<string, object>(values, StringComparer.OrdinalIgnoreCase);
                            double wac = Convert.ToDouble(values["water attenuation coefficients (m**-1)"]) / 100.0;
                            string contaminantKey = (common.contaminantType + " " + common.contaminantUnits).ToLower();
                            double cac = Convert.ToDouble(values[contaminantKey]);
                            common.setAbwat(wac, index - 1);
                            common.setEppest(cac, index - 1);
                        }
                        break;
                    default:
                        break;
                }
            }


        }

    }
}
