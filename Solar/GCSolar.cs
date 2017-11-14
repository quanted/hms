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
            //result.Add("wavelength table", ToOutputArray(inputTable));
            result.Add("wavelength table", ToDictionary(inputTable));

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
                dic.Add(key, rowDict);
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
        /// Sets the instance Common variables from the input dictionary provided and validates inputs.
        /// </summary>
        /// <param name="cc"></param>
        public void SetCommonVariables(Dictionary<string, object> cc, out List<string> errors)
        {
            errors = new List<string>();
            double[] waves = common.getWave();
            bool ephemerideValues = false;
            foreach (KeyValuePair<string, object> p in cc)
            {
                switch(p.Key.ToLower())
                {
                    case "contaminant name":
                        string name = p.Value.ToString();
                        if (!string.IsNullOrWhiteSpace(name))
                        {
                            common.contaminantName = name;
                        }
                        break;
                    case "contaminant type":
                        string type = p.Value.ToString();
                        common.contaminantType = (type == "Biological") ? "Biological": "Chemical";
                        break;
                    case "water type name":
                        string wName = p.Value.ToString();
                        common.bodyWaterName = (!string.IsNullOrWhiteSpace(wName)) ? wName : "Default";
                        break;
                    case "min wavelength":
                        double minWave = 0;
                        try
                        {
                            minWave = Convert.ToDouble(p.Value);
                        }
                        catch (FormatException ex)
                        {
                            errors.Add("Minimum Wavelength Error: " + ex.Message);
                            break;
                        }
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
                        double maxWave = 0;
                        try
                        {
                            maxWave = Convert.ToDouble(p.Value);
                        }
                        catch(FormatException ex)
                        {
                            errors.Add("Maximum Wavelength Error: " + ex.Message);
                            break;
                        }
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
                        double lon = 0;
                        try
                        {
                            lon = Convert.ToDouble(p.Value);
                        }
                        catch(FormatException ex)
                        {
                            errors.Add("Longitude Error: " + ex.Message);
                            break;
                        }
                        common.xlon = lon;
                        break;
                    case "latitude(s)":
                        double[] latitudeArray;
                        try
                        {
                            latitudeArray = JsonConvert.DeserializeObject<double[]>(p.Value.ToString());
                        }
                        catch(FormatException ex)
                        {
                            errors.Add("Latitute(s) Error: " + ex.Message);
                            break;
                        }
                        foreach(double l in latitudeArray)
                        {
                            if(l < 0 && l != -99)
                            {
                                errors.Add("Latitude(s) Error: Latitude value must be greater than or equal to 0 and less than 70, a value of =99 is used for unused latitudes.");
                            }
                            if (l >= 70)
                            {
                                errors.Add("Latitude(s) Error: Latitude value must be greater than or equal to 0 and less than 70, a value of =99 is used for unused latitudes.");
                            }
                        }
                        common.ilattm = latitudeArray;
                        break;
                    case "season(s)":
                        string[] seasonArray;
                        try
                        {
                            seasonArray = JsonConvert.DeserializeObject<string[]>(p.Value.ToString());
                        }
                        catch(FormatException ex)
                        {
                            errors.Add("Season(s) Error: " + ex.Message);
                            break;
                        }
                        common.sease = seasonArray;
                        ephemerideValues = true;
                        break;
                    case "latitude":
                        double lat;
                        try
                        {
                            lat = Convert.ToDouble(p.Value);
                        }
                        catch(FormatException ex)
                        {
                            errors.Add("Latitude Error: " + ex.Message);
                            break;
                        }
                        if (lat < 0 && lat != -99)
                        {
                            errors.Add("Latitude(s) Error: Latitude value must be greater than or equal to 0 and less than 70.");
                        }
                        if (lat >= 70)
                        {
                            errors.Add("Latitude(s) Error: Latitude value must be greater than or equal to 0 and less than 70.");
                        }
                        if (!ephemerideValues)
                            common.typlat = Convert.ToDouble(p.Value);
                        break;
                    case "solar declination":
                        double[] sd_values;
                        try
                        {
                            sd_values = JsonConvert.DeserializeObject<double[]>(p.Value.ToString());
                        }
                        catch(FormatException ex)
                        {
                            errors.Add("Solar Declination Error: " + ex.Message);
                            break;
                        }
                        if (!ephemerideValues)
                        {
                            common.xx[0] = sd_values[0];
                            common.xx[1] = sd_values[1];
                            common.xx[2] = sd_values[2];
                        }
                        break;
                    case "right declination":
                        double[] rd_values;
                        try
                        {
                            rd_values = JsonConvert.DeserializeObject<double[]>(p.Value.ToString());
                        }
                        catch(FormatException ex)
                        {
                            errors.Add("Right Declination Error: " + ex.Message);
                            break;
                        }
                        if (!ephemerideValues)
                        {
                            common.xx[3] = rd_values[0];
                            common.xx[4] = rd_values[1];
                            common.xx[5] = rd_values[2];
                        }
                        break;
                    case "sidereal time":
                        double[] st_values;
                        try
                        {
                            st_values = JsonConvert.DeserializeObject<double[]>(p.Value.ToString());
                        }
                        catch(FormatException ex)
                        {
                            errors.Add("Sidereal Time Error: " + ex.Message);
                            break;
                        }
                        if (!ephemerideValues)
                        {
                            common.xx[6] = st_values[0];
                            common.xx[7] = st_values[1];
                            common.xx[8] = st_values[2];
                        }
                        break;
                    case "atmospheric ozone layer":
                        double ozone;
                        try
                        {
                            ozone = Convert.ToDouble(p.Value);
                        }
                        catch(FormatException ex)
                        {
                            errors.Add("Atmospheric Ozone Layer Error: " + ex.Message);
                            break;
                        }
                        if (ephemerideValues)
                        {
                            common.aveozo = ozone;
                            common.ioz = 1;
                        }
                        break;
                    case "initial depth":
                        double idepth;
                        try
                        {
                            idepth = Convert.ToDouble(p.Value);
                        }
                        catch(FormatException ex)
                        {
                            errors.Add("Initial Depth Error: " + ex.Message);
                            break;
                        }
                        common.dinit = idepth;
                        break;
                    case "final depth":
                        double fdepth;
                        try
                        {
                            fdepth = Convert.ToDouble(p.Value);
                        }
                        catch(FormatException ex)
                        {
                            errors.Add("Final Depth Error: " + ex.Message);
                            break;
                        }
                        common.dfinal = fdepth;
                        break;
                    case "depth increment":
                        double dinc;
                        try
                        {
                            dinc = Convert.ToDouble(p.Value);
                        }
                        catch(FormatException ex)
                        {
                            errors.Add("Depth Increment Error: " + ex.Message);
                            break;
                        }
                        common.dinc = dinc;
                        break;
                    case "depth point":
                        double dPoint;
                        try
                        {
                            dPoint = Convert.ToDouble(p.Value);
                        }
                        catch(FormatException ex)
                        {
                            errors.Add("Depth Point Error: " + ex.Message);
                            break;
                        }
                        common.deltaz = dPoint;
                        common.useDeltaz = true;
                        break;
                    case "quantum yield":
                        double Q;
                        try
                        {
                            Q = Convert.ToDouble(p.Value);
                        }
                        catch(FormatException ex)
                        {
                            errors.Add("Quantum Yield Error: " + ex.Message);
                            break;
                        }
                        common.q = Q;
                        break;
                    case "refractive index":
                        double refI;
                        try
                        {
                            refI = Convert.ToDouble(p.Value);
                        }
                        catch(FormatException ex)
                        {
                            errors.Add("Refractive Index Error: " + ex.Message);
                            break;
                        }
                        common.musubr = refI;
                        break;
                    case "elevation":
                        double ele;
                        try
                        {
                            ele = Convert.ToDouble(p.Value);
                        }
                        catch(FormatException ex)
                        {
                            errors.Add("Elevation Error: " + ex.Message);
                            break;
                        }
                        common.elevation = ele;
                        break;
                    case "wavelength table":
                        double minWaveTemp = 100000.0;
                        double maxWaveTemp = 0.0;
                        Dictionary<string, object> waveTable = JsonConvert.DeserializeObject <Dictionary<string, object>>(p.Value.ToString());
                        foreach(KeyValuePair<string, object> q in waveTable)
                        {
                            int index = Array.IndexOf(waves,Convert.ToDouble(q.Key));
                            Dictionary<string, object> values = JsonConvert.DeserializeObject<Dictionary<string, object>>(q.Value.ToString());
                            values = new Dictionary<string, object>(values, StringComparer.OrdinalIgnoreCase);
                            double wac;
                            double cac;
                            string contaminantKey = (common.contaminantType + " " + common.contaminantUnits).ToLower();
                            try
                            {
                                wac = Convert.ToDouble(values["water attenuation coefficients (m**-1)"]) / 100.0;
                                cac = Convert.ToDouble(values[contaminantKey]);
                            }
                            catch(Exception ex)
                            {
                                errors.Add("Wavelength Table Error: " + ex.Message);
                                break;
                            }
                            common.setAbwat(wac, index - 1);
                            common.setEppest(cac, index - 1);
                            double waveTemp = Convert.ToDouble(q.Key);
                            if(waveTemp < minWaveTemp)
                            {
                                minWaveTemp = waveTemp;
                            }
                            if(waveTemp > maxWaveTemp)
                            {
                                maxWaveTemp = waveTemp;
                            }
                        }
                        common.setMinWave(minWaveTemp);
                        common.setMaxWave(maxWaveTemp);
                        break;
                    default:
                        break;
                }
            }


        }

    }
}
