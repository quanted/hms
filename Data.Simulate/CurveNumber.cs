using Data.Source;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;

namespace Data.Simulate
{

    //public class CN
    //{
    //    public Dictionary<string, NLCDClass> NLCD { get; set; }
    //}

    //public class CNConditions
    //{
    //    public Dictionary<string, ConditionGroup> Class { get; set; }
    //}

    //public class NDVI
    //{
    //    public Dictionary<string, NDVIGroup> Class { get; set; }
    //}

    //public class ConditionGroup
    //{
    //    public Dictionary<string, NLCDClass> Condition { get; set; }
    //}

    //public class NLCDClass
    //{
    //    [JsonProperty("A")]
    //    public int A { get; set; }
    //    [JsonProperty("B")]
    //    public int B { get; set; }
    //    [JsonProperty("C")]
    //    public int C { get; set; }
    //    [JsonProperty("D")]
    //    public int D { get; set; }
    //}

    //public class NDVIGroup
    //{
    //    public int POOR { get; set; }
    //    public int GOOD { get; set; }
    //}

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

            double cn = CalculateCN(out errorMsg, input);

            // Curve number algorithm
            // Runoff calculation: https://en.wikipedia.org/wiki/Runoff_curve_number
            // Calculate soil moisture retention (S): S = 1000/CN - 10
            // Calculate initial abstraction (Ia): Ia = 0.2S (old initial abstraction was Ia = 0.05S)
            // Iterate over precipitation data (P) by date
            // If precipitation <= Ia: Runoff (Q) = 0
            // Else precipitation > Ia: Runoff (Q) = (P - Ia)^2/(P- Ia + S)

            double s = 1000.0 / cn - 10.0;
            double ia = 0.2 * s;
            
            foreach(KeyValuePair<string, List<string>> dateValue in precipData.Data)
            {
                string date = dateValue.Key;
                double p = double.Parse(dateValue.Value[0]);
                double q = (p <= ia) ? 0 : ((p - ia) * (p - ia)) / (p - ia + s);
                //Debug.WriteLine(date + ": " + q.ToString("E3"));
                List<string> d = new List<string>();
                d.Add(q.ToString(input.DataValueFormat));
                output.Data.Add(date, d);
            }
            return output;
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
            string cnFile = @".\App_Data\curvenumber.json";
            using(StreamReader r = new StreamReader(cnFile, System.Text.Encoding.UTF8))
            {
                string jsonString = r.ReadToEnd();
                dynamic cnData = JsonConvert.DeserializeObject(jsonString);
                return cnData;
            }
        }

        private dynamic GetCNConditions()
        {
            string cnFile = @".\App_Data\curvenumber_conditions.json";
            using (StreamReader r = new StreamReader(cnFile))
            {
                string jsonString = r.ReadToEnd();
                dynamic cnConditions = JsonConvert.DeserializeObject(jsonString);
                return cnConditions;
            }
        }

        private dynamic GetNDVI()
        {
            string cnFile = @".\App_Data\curvenumber_ndvi.json";
            using (StreamReader r = new StreamReader(cnFile))
            {
                string jsonString = r.ReadToEnd();
                dynamic ndvi = JsonConvert.DeserializeObject(jsonString);
                return ndvi;
            }
        }

    }
}
