using Data;
using System;
using System.Data;

namespace Evapotranspiration
{
    public class CustomData
    {
        public DataTable ParseCustomData(ITimeSeriesInput inpt, ITimeSeriesOutput outpt, string data, string algorithm)
        {
            DataTable tab = new DataTable();
            tab.Columns.Add("Date");
            tab.Columns.Add("Julian_Day");
            tab.Columns.Add("TMin_C");
            tab.Columns.Add("TMax_C");
            tab.Columns.Add("TMean_C");
            switch (algorithm)
            {
                case "hamon":
                    //temp
                    break;
                case "priestlytaylor":
                    //temp, solar
                    tab.Columns.Add("SolarRadMean_MJm2day");
                    break;
                case "mortoncrae":
                case "mortoncrwe":
                    //temp, solar, spec
                    tab.Columns.Add("SolarRadMean_MJm2day");
                    tab.Columns.Add("SHmin");
                    tab.Columns.Add("SHmax");
                    break;
                case "grangergray":
                case "penpan":
                case "mcjannett":
                case "penmanopenwater":
                case "penmandaily":
                case "shuttleworthwallace":
                    //temp, solar, spec, wind
                    tab.Columns.Add("SolarRadMean_MJm2day");
                    tab.Columns.Add("SHmin");
                    tab.Columns.Add("SHmax");
                    tab.Columns.Add("WindSpeedMean_m/s");
                    break;
            }
            
            string[] lines = data.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                string[] linedata = line.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                DataRow tabrow = tab.NewRow();
                tabrow["Date"] = linedata[0];
                tabrow["Julian_Day"] = Convert.ToInt32(Convert.ToDouble(linedata[1]));
                tabrow["TMin_C"] = linedata[2];
                tabrow["TMax_C"] = linedata[3];
                tabrow["TMean_C"] = linedata[4];//(Convert.ToDouble(linedata[2]) + Convert.ToDouble(linedata[3])) / 2.0;
                switch (algorithm)
                {
                    case "hamon":
                        //temp
                        break;
                    case "priestlytaylor":
                        //temp, solar
                        tabrow["SolarRadMean_MJm2day"] = linedata[5];
                        break;
                    case "mortoncrae":
                    case "mortoncrwe":
                        //temp, solar, spec
                        tabrow["SolarRadMean_MJm2day"] = linedata[5];
                        tabrow["SHmin"] = linedata[6];
                        tabrow["SHmax"] = linedata[7];
                        break;
                    case "grangergray":
                    case "penpan":
                    case "mcjannett":
                    case "penmanopenwater":
                    case "penmandaily":
                    case "shuttleworthwallace":
                        //temp, solar, spec, wind
                        tabrow["SolarRadMean_MJm2day"] = linedata[5];
                        tabrow["SHmin"] = linedata[6];
                        tabrow["SHmax"] = linedata[7];
                        tabrow["WindSpeedMean_m/s"] = linedata[8];
                        break;
                }
                tab.Rows.Add(tabrow);
            }
            return tab;
        }
    }
    /*Custom data source sample
        2010-01-01,1,-0.84,6.86,3.01,8.52,0.002813,0.005167,4.72
        2010-01-02,2,-4.11,1.95,-1.08,12.05,0.001658,0.002719,4.77
        2010-01-03,3,-6.23,0.56,-2.83,7.74,0.001032,0.001570,3.26
        2010-01-04,4,-7.76,0.46,-3.65,12.05,0.001371,0.002013,4.03
        2010-01-05,5,-8.33,0.07,-4.13,12.06,0.001576,0.001899,5.13
        2010-01-06,6,-8.84,2.40,-3.22,12.23,0.001531,0.002270,3.65
        2010-01-07,7,-5.11,4.97,-0.07,8.54,0.001713,0.003333,3.36
        2010-01-08,8,-7.34,-0.42,-3.88,11.28,0.001212,0.003427,5.26
    */
}