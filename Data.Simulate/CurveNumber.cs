using Data.Source;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;

namespace Data.Simulate
{

    /// <summary>
    /// Curve Number base class.
    /// Classified as simulation data, simulation takes place on a different server so only a standard data call is made.
    /// </summary>
    public class CurveNumber
    {
        /// <summary>
        /// Get simulated curve number data.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public ITimeSeriesOutput Simulate(out string errorMsg, ITimeSeriesInput input, ITimeSeriesOutput precipData)
        {
            errorMsg = "";
            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput output = oFactory.Initialize();
            output.Dataset = "Surface Runoff";
            output.DataSource = "curve number";
            //output.Metadata = precipData.Metadata;

            // Curve number algorithm
            // Runoff calculation: https://en.wikipedia.org/wiki/Runoff_curve_number
            // Calculate soil moisture retention (S): S = 1000/CN - 10
            // Calculate initial abstraction (Ia): Ia = 0.2S (old initial abstraction was Ia = 0.05S)
            // Iterate over precipitation data (P) by date
            // If precipitation <= Ia: Runoff (Q) = 0
            // Else precipitation > Ia: Runoff (Q) = (P - Ia)^2/(P- Ia + S)

            int day0 = DateTime.Parse(precipData.Data.Keys.First().Split(' ')[0]).DayOfYear;
            int cnI = ( day0 / 16 ) + 1;
            Dictionary<int, double> cn = GetCN(out errorMsg, input.Geometry.ComID);
            if (errorMsg.Contains("ERROR")){ return null; }
            if (cn.Count == 0)
            {
                errorMsg = "ERROR: No curve number values found for the specified catchment. ComID: " + input.Geometry.ComID;
                return null;
            }
            double s = 1000.0 / cn[cnI] - 10.0;

            double ia = 0.2 * s;


            foreach (KeyValuePair<string, List<string>> dateValue in precipData.Data)
            {
                string date = dateValue.Key;

                cnI = (DateTime.Parse(dateValue.Key.Split(' ')[0]).DayOfYear / 16) + 1;
                s = 1000.0 / cn[cnI] - 10.0;
                ia = 0.2 * s;
                double p = double.Parse(dateValue.Value[0]);
                double q = (p <= ia) ? 0 : ((p - ia) * (p - ia)) / (p - ia + s);
                List<string> d = new List<string>();
                d.Add(q.ToString(input.DataValueFormat));
                output.Data.Add(date, d);
            }

            output.Metadata.Add("cnValues", string.Join(", ", cn.Values));
            return output;
        }

        private Dictionary<int, double> GetCN(out string errorMsg, int comid)
        {
            errorMsg = "";
            string dbPath = "./App_Data/hms_database.db";
            string query = "SELECT CN_00, CN_01, CN_02, CN_03, CN_04, CN_05, CN_06, CN_07, CN_08, CN_09, CN_10, CN_11, CN_12, CN_13, CN_14, CN_15, CN_16, CN_17, CN_18, CN_19, CN_20, CN_21, CN_22 " +
                "FROM CurveNumber WHERE ComID = '" + comid.ToString() + "'";
            Dictionary<string, string> data = Utilities.SQLite.GetData(dbPath, query);
            Dictionary<int, double> cnData = new Dictionary<int, double>();
            int i = 0;
            foreach(string key in data.Keys)
            {
                cnData.Add(i + 1, double.Parse(data[key]));
                i++;
            }
            return cnData;
        }

        private double CalculateCN(out string errorMsg, ITimeSeriesInput input)
        {
            dynamic cn = GetNLCDCN();
            dynamic conditions = GetCNConditions();
            dynamic ndvi = GetNDVI();
            List<int> ndviCN = new List<int>() { 41, 42, 43, 52, 71, 81, 82 };

            Streamcat sc = new Streamcat();
            Catchment catchment = sc.GetCatchmentData(out errorMsg, input.Geometry.ComID);
            double catchmentCN = 0.0;

            foreach (int c in catchment.landcover.Keys)
            {
                double v = catchment.landcover[c];
                var nlcd = cn[c.ToString()];
                double n = 0.0;

                if (ndviCN.Contains(c))
                {
                    // TODO: ndviType is currently a placeholder value, but will be determined from modis ndvi data using the ranges found in curvenumber_ndvi.json
                    string ndviType = "GOOD";

                    switch (catchment.hsg)
                    {
                        case "A":
                            n = conditions[c.ToString()][ndviType].A;
                            break;
                        case "B":
                            n = conditions[c.ToString()][ndviType].B;
                            break;
                        case "C":
                            n = conditions[c.ToString()][ndviType].C;
                            break;
                        case "D":
                            n = conditions[c.ToString()][ndviType].D;
                            break;
                        default:
                            n = conditions[c.ToString()][ndviType].A;
                            break;
                    }
                }
                else
                {
                    switch (catchment.hsg)
                    {
                        case "A":
                            n = nlcd.A;
                            break;
                        case "B":
                            n = nlcd.B;
                            break;
                        case "C":
                            n = nlcd.C;
                            break;
                        case "D":
                            n = nlcd.D;
                            break;
                        default:
                            n = nlcd.A;
                            break;
                    }
                }
                if (n == -1)
                {
                    continue;
                }
                catchmentCN += v / 100 * n;
            }
            if (catchmentCN < 30)
            {
                catchmentCN = 30.0;
            }
            return catchmentCN;
        }

        private dynamic GetNLCDCN()
        {
            string filePath = @".\App_Data\curvenumber.json";
            if (!File.Exists(filePath))
            {
                filePath = "/app/App_Data/curvenumber.json";
            }
            using(StreamReader r = new StreamReader(filePath, System.Text.Encoding.UTF8))
            {
                string jsonString = r.ReadToEnd();
                dynamic cnData = JsonConvert.DeserializeObject(jsonString);
                return cnData;
            }
        }

        private dynamic GetCNConditions()
        {
            string filePath = @".\App_Data\curvenumber_conditions.json";
            if (!File.Exists(filePath))
            {
                filePath = "/app/App_Data/curvenumber_conditions.json";
            }
            using (StreamReader r = new StreamReader(filePath))
            {
                string jsonString = r.ReadToEnd();
                dynamic cnConditions = JsonConvert.DeserializeObject(jsonString);
                return cnConditions;
            }
        }

        private dynamic GetNDVI()
        {
            string filePath = @".\App_Data\curvenumber_ndvi.json";
            if (!File.Exists(filePath))
            {
                filePath = "/app/App_Data/curvenumber_ndvi.json";
            }
            using (StreamReader r = new StreamReader(filePath))
            {
                string jsonString = r.ReadToEnd();
                dynamic ndvi = JsonConvert.DeserializeObject(jsonString);
                return ndvi;
            }
        }

    }
}
