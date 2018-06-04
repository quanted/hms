using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Globalization;
using Data;
using System.Collections;
using System.Net;
using System.IO;
using System.Threading;

namespace Evapotranspiration
{
    public class McJannett
    {
        private double latitude;
        private double longitude;
        private double elevation;  // meters
        private double albedo;
        private double surfaceArea;  // km^2
        private double lakeDepth;  // meters
        public Hashtable airToWaterTempFactor = new Hashtable(12);

        public McJannett()
        {
            latitude = 33.925673;
            longitude = -83.355723;
            elevation = 213.36;
            albedo = 0.23;
            surfaceArea = .005;
            lakeDepth = 0.2;
        }

        public double Latitude
        {
            get
            {
                return latitude;
            }
            set
            {
                latitude = value;
            }
        }

        public double Longitude
        {
            get
            {
                return longitude;
            }
            set
            {
                longitude = value;
            }
        }

        public double Elevation
        {
            get
            {
                return elevation;
            }
            set
            {
                elevation = value;
            }
        }

        public double Albedo
        {
            get
            {
                return albedo;
            }
            set
            {
                albedo = value;
            }
        }

        public double SurfaceArea
        {
            get
            {
                return surfaceArea;
            }
            set
            {
                surfaceArea = value;
            }
        }

        public double LakeDepth
        {
            get
            {
                return lakeDepth;
            }
            set
            {
                lakeDepth = value;
            }
        }



        public void McJannettMethod(double tmin, double tmax, double tmean, int jday, DateTime time, double shmin, double shmax,
                                    double wind, double solarRad, out double relHumidityMin, out double relHumidityMax,
                                    out double waterTemp, out double petMcJ, out string errorMsg)
        {
            errorMsg = "";
            waterTemp = 0.0;
            relHumidityMin = 0.0;
            relHumidityMax = 0.0;

            double EL = elevation;  //mts
            double lat = latitude;

            double PR = 101.3;  // pressure kilo pascals
            double lake_depth = lakeDepth;

            // CONSTANTS
            const double A = 17.27;
            const double B = 237.3;

            // Specific heat at constant pressure[MJ kg-1 deg C-1
            double CP = 1.013 * Math.Pow(10, -3);

            // ratio of mol wt of water vapour to dry air
            const double e = 0.622;

            //Latent heat of Vaporization [MJ kg-1]
            const double LW = 2.45;
            const double pi = Math.PI;

            double ALBEDO = albedo;
            const double latent = 2.45;

            // extracted from input data
            double TMIN = 0.0;   // celsisus
            double TMAX = 0.0;   // celsisus
            double jd1 = (double)jday;
            double RHmin = 0.0;
            double RHmax = 0.0;
            double u = 0.0;

            // EVAPO TRANSPIRATION
            double PET = 0.0;
            petMcJ = 0;

            // CALCULATED VALUES
            // Main calculated parameters used in the final evapo transpiration formulae
            double T = 0.0;
            double RA = 0.0;

            // Other calculated parameters
            double es1 = 0.0;
            double es2 = 0.0;
            double es = 0.0;
            double slope = 0.0;
            double ea = 0.0;
            double deficit = 0.0;
            double P = 0.0; //pressure
            double gama = 0.0; //psychometric const
            double radian = 0.0;
            double dr = 0.0;
            double sigma2 = 0.0;
            double xx = 0.0;
            double ws = 0.0;
            double RS = 0.0;
            double RSO = 0.0;
            double RR = 0.0;
            double RNS = 0.0;
            double tmaxk = 0.0;
            double tmink = 0.0;
            double AVGTK = 0.0;
            double EB = 0.0;
            double RB = 0.0;
            double RNL = 0.0;
            double Rnet = 0.0;
            double NN = 0.0;
            double cf = 0.0;
            double ul = 0.0;
            double lh = 0.0;
            double dpt = 0.0;
            double densq = 0.0;
            double twb = 0.0;
            double dendlt = 0.0;
            double deltawb = 0.0;
            double bulb = 0.0;
            double SA = 0.0; //SA is A=1.2 in vb
            double windfu = 0.0;
            double raa = 0.0;
            double densitya = 1.2;
            double heata = 0.001013;
            double t0 = 0.0;
            double ril = 0.0;
            double bolz = 4.903 * 1.0e-9;
            double rolwb = 0.0;
            double QWB = 0.0;
            double TILDA = 0.0;
            double densityw = 997.9;
            double heatw = 0.00419;
            double Teq = 0.0;
            double Twat = 0.0;
            double GW = 0.0;
            double delw = 0.0;
            double vw = 0.0;
            double rols = 0.0;
            double Qstar = 0.0;
            double factor = 0.0;
            double u2 = 0.0;
            double ALBEDOW = 0.08;

            SA = surfaceArea;
            lh = lakeDepth;
            DateTime d = time;
            int mon = d.Month;
            factor = (double)airToWaterTempFactor[mon];

            try
            {
                TMIN = tmin;
                TMAX = tmax;

                // Convert specific humidity to relative humidity here.  
                RHmin = Utilities.Utility.CalculateRHmin(shmin, tmin);
                RHmax = Utilities.Utility.CalculateRHmax(shmax, tmax);

                // Set relHumidityMin to RHmin and relHumidityMax to RHmax.
                relHumidityMin = RHmin;
                relHumidityMax = RHmax;


                u2 = wind;  // Wind is in units of meters/second

                double ht = 10.0;  // The wind data in the NLDAS database is given at a height of 10 meters.

                // Wind speed correction based on height
                if (ht != 2.0)
                {
                    u = u2 * (4.87 / (Math.Log(67.8 * ht - 5.42)));
                }
                else
                {
                    u = u2;
                }

                // T = (TMIN + TMAX) / 2.0;
                T = tmean;

                es1 = 0.6108 * Math.Exp((A * TMIN) / (B + TMIN));
                es2 = 0.6108 * Math.Exp((A * TMAX) / (B + TMAX));
                es = (es1 + es2) / 2.0;

                //Yusuf:  slope of saturation vapour pressure curve
                slope = (4098 * 0.6108 * Math.Exp((17.27 * T) / (237.3 + T))) / Math.Pow((T + 237.3), 2.0);

                // Actual water vapor pressure in the atmosphere
                ea = (es2 * (RHmin / 100.0) + (es1 * RHmax / 100.0)) / 2.0;

                deficit = es - ea;

                // Calculate Dew point temperature 
                double Tdew = 0;
                Tdew = 237.3 / (17.27 / (Math.Log(ea) - Math.Log(0.6108)) - 1);

                P = PR * Math.Pow(((293.0 - 0.0065 * EL) / 293.0), 5.26);

                // psychometric constant
                gama = CP * P / (LW * e);


                // ############# ENERGY EQUATIONS ########################################

                /*CALACULATION OF EXTRATERRESTIAL RADIATION FOR DAILY PERIOIDS
                  ---- THIS PORTION OF THE PROGRAM CALCULATES THE ENERGY COMPONENT  */

                radian = (pi / 180.0) * lat;
                dr = 1.0 + 0.033 * Math.Cos(2 * (pi / 365) * jd1);
                sigma2 = 0.409 * Math.Sin((2.0 * pi * jd1 / 365.0) - 1.39);
                xx = 1.0 - (Math.Pow(Math.Tan(radian), 2.0) * Math.Pow(Math.Tan(sigma2), 2.0));

                if (xx <= 0) xx = 0.00001;

                ws = (pi / 2.0) - (Math.Atan(-Math.Tan(radian) * (Math.Tan(sigma2) / Math.Pow(xx, 0.5))));

                RA = (24.0 * 60.0 / pi) * 0.082 * dr * (ws * Math.Sin(radian) * Math.Sin(sigma2) +
                      Math.Cos(radian) * Math.Cos(sigma2) * Math.Sin(ws));

                // CALCULATION OF SOLAR OR SHORT WAVE RADIATION
                NN = (24.0 / pi) * ws;

                RS = solarRad;      // Units of solar radiation are in MJ/(m^2 day) 

                RSO = (0.75 + 2.0 * (EL / 100000.0)) * RA;
                RR = RS / RSO;
                cf = 2 * (1 - RR);
                RNS = (1 - ALBEDO) * RS;

                tmaxk = 4.903 * Math.Pow(10, -9.0) * Math.Pow(TMAX + 273.16, 4.0);
                tmink = 4.903 * Math.Pow(10, -9.0) * Math.Pow(TMIN + 273.16, 4.0);

                AVGTK = (tmaxk + tmink) / 2.0;

                EB = 0.34 - 0.14 * Math.Pow(ea, 0.5);
                RB = 1.35 * RR - 0.35;
                RNL = AVGTK * EB * RB;
                Rnet = RNS - RNL;

                ul = u * Math.Log(lh / 0.0002) / (Math.Log(2 / 0.0002));
                dpt = (116.9 + 237.3 * Math.Log(ea)) / (16.78 - Math.Log(ea));
                densq = Math.Pow((-1.1579 + 237.3), 2);
                twb = (0.00066 * 100 * T + (dpt * (4098 * ea / densq))) / (0.00066 * 100 + 4098 * ea / densq);
                dendlt = Math.Pow((twb + 237.3), 2);

                bulb = Math.Exp(17.27 * twb / (twb + 237.3));

                deltawb = 4098 * 0.6108 * (bulb / dendlt);

                windfu = Math.Pow((5 / SA), 0.05) * (3.8 + 1.57 * ul); // Lake Surface Area

                raa = densitya * heata / (gama * (windfu / 86400));

                // Obtain water temperature by multiplying air temperarute by conversion factor
                t0 = factor * T;
                waterTemp = t0;

                ril = (cf + (1 - cf) * (1 - 0.261 * Math.Exp(-7.77 * 1.0e-4 * Math.Pow(T, 2)))) * bolz * Math.Pow((T + 273.15), 4);
                rolwb = bolz * Math.Pow((T + 273.15), 4) + 4 * bolz * Math.Pow((T + 273.15), 3) * (twb - T);
                QWB = RS * (1 - ALBEDOW) + (ril - rolwb); // RSW IS RS
                TILDA = (densityw * heatw * lh) / (4 * bolz * Math.Pow((twb + 273.15), 3) + windfu * (deltawb + gama));

                Teq = twb + QWB / (4 * bolz * Math.Pow((twb + 273.15), 3) + windfu * (deltawb + gama));
                Twat = Teq + (t0 - Teq) * Math.Exp(-1 / TILDA);

                GW = densityw * heatw * lake_depth * (Twat - t0);

                delw = 4098 * 0.6108 * Math.Exp(17.27 * Twat / (Twat + 237.3)) * (1 / Math.Pow((Twat + 237.3), 2));

                vw = 0.6108 * Math.Exp(17.27 * Twat / (Twat + 237.3));

                rols = 0.97 * bolz * Math.Pow((Twat + 273.15), 4);

                Qstar = RS * (1 - ALBEDOW) + (ril - rols);

                PET = 1 / latent * (delw * (Qstar - GW) + (86400 * densitya * heata * (vw - ea)) / raa) / (delw + gama);

            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }

            // Convert mm/day to inches/day
            petMcJ = (PET / 25.4);

        }

        public ITimeSeriesOutput Compute(ITimeSeriesInput inpt, ITimeSeriesOutput outpt, double lat, double lon, string startDate, string endDate, int timeZoneOffset, out string errorMsg)
        {
            errorMsg = "";
            //NLDAS2 nldas = new NLDAS2(inpt.Source, lat, lon, startDate, endDate);
            double petMcJ = 0;
            double wTemp = 0;
            double relHMax = 0;
            double relHMin = 0.0;
            string strDate;
            CultureInfo CInfoUS = new CultureInfo("en-US");

            //DataTable dt = nldas.getData4(timeZoneOffset, out errorMsg);
            
            DataTable dt = new DataTable();
            DataTable daymets = new DataTable();
            switch (inpt.Source)
            {
                case "daymet":
                    daymets = daymetData(inpt, outpt);
                    inpt.Source = "nldas";
                    NLDAS2 nldas = new NLDAS2(inpt.Source, lat, lon, startDate, endDate);
                    dt = nldas.getData4(timeZoneOffset, out errorMsg);
                    for (int i = 0; i < daymets.Rows.Count; i++)
                    {
                        DataRow dr = dt.Rows[i];
                        dr["TMin_C"] = daymets.Rows[i]["TMin_C"];
                        dr["TMax_C"] = daymets.Rows[i]["TMax_C"];
                        dr["TMean_C"] = daymets.Rows[i]["TMean_C"];
                        dr["SolarRadMean_MJm2day"] = daymets.Rows[i]["SolarRadMean_MJm2day"];
                    }
                    daymets = null;
                    break;
                case "custom":
                    CustomData cd = new CustomData();
                    dt = cd.ParseCustomData(inpt, outpt, inpt.Geometry.GeometryMetadata["userdata"].ToString(), "mcjannett");
                    break;
                case "nldas":
                case "gldas":
                default:
                    NLDAS2 nldas2 = new NLDAS2(inpt.Source, lat, lon, startDate, endDate);
                    if (inpt.TemporalResolution == "hourly")
                    {
                        NLDAS2 nldasday = new NLDAS2(inpt.Source, lat, lon, startDate, endDate);
                        DataTable dtd = nldasday.getData4(timeZoneOffset, out errorMsg);
                        dt = nldas2.getDataHourly(timeZoneOffset, false, out errorMsg);
                        dt.Columns["THourly_C"].ColumnName = "TMean_C";
                        dt.Columns["SolarRad_MJm2day"].ColumnName = "SolarRadMean_MJm2day";
                        dt.Columns["WindSpeed_m/s"].ColumnName = "WindSpeedMean_m/s";
                        dt.Columns.Remove("SH_Hourly");
                        dt.Columns.Add("TMin_C");
                        dt.Columns.Add("TMax_C");
                        dt.Columns.Add("SHmin");
                        dt.Columns.Add("SHmax");
                        int j = -1;
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            if ((inpt.Source == "nldas" && (i % 24 == 0)) || (inpt.Source == "gldas" && (i % 8 == 0)))
                            {
                                j++;
                            }
                            DataRow dr = dtd.Rows[j];
                            dt.Rows[i]["TMin_C"] = dr["TMin_C"];
                            dt.Rows[i]["TMax_C"] = dr["TMax_C"];
                            dt.Rows[i]["SHmin"] = dr["SHmin"];
                            dt.Rows[i]["SHmax"] = dr["SHmax"];
                        }
                        dtd = null;
                    }
                    else
                    {
                        dt = nldas2.getData4(timeZoneOffset, out errorMsg);
                    }
                    break;
            }

            if (errorMsg != "")
            {
                Utilities.ErrorOutput err = new Utilities.ErrorOutput();
                return err.ReturnError(errorMsg);
            }

            dt.Columns.Add("RHmin");
            dt.Columns.Add("RHmax");
            dt.Columns.Add("WaterTemp_C");
            dt.Columns.Add("McJannettPET_in");

            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput output = oFactory.Initialize();
            output.Dataset = "Evapotranspiration";
            output.DataSource = "mcjannett";
            string airtemps = "{ ";
            foreach(int key in airToWaterTempFactor.Keys)
            {
                airtemps += String.Format("{0}: {1}, ", key, airToWaterTempFactor[key]);
            }
            airtemps += "}";
            output.Metadata = new Dictionary<string, string>()
            {
                { "elevation", elevation.ToString() },
                { "latitude", latitude.ToString() },
                { "longitude", longitude.ToString() },
                { "albedo", albedo.ToString() },
                { "lake_surface_area", surfaceArea.ToString() },
                { "lake_depth", lakeDepth.ToString() },
                { "air_temp_coeffs", airtemps},
                { "request_time", DateTime.Now.ToString() },
                { "column_1", "Date" },
                { "column_2", "Julian Day" },
                { "column_3", "Minimum Temperature" },
                { "column_3.1", "Maximum Temperature" },
                { "column_3.2", "Mean Temperature" },
                { "column_4", "Mean Solar Radiation" },
                { "column_5", "Mean Wind Speed" },
                { "column_6", "Minimum Relative Humidity" },
                { "column_7", "Maximum Relative Humidity" },
                { "column_8", "Water Temperature" },
                { "column_9", "Potential Evapotranspiration" }
            };
            if (inpt.TemporalResolution == "hourly")
            {
                output.Metadata = new Dictionary<string, string>()
                {
                    { "latitude", latitude.ToString() },
                    { "longitude", longitude.ToString() },
                    { "request_time", DateTime.Now.ToString() },
                    { "column_1", "DateHour" },
                    { "column_2", "Julian Day" },
                    { "column_3", "Hourly Temperature" },
                    { "column_4", "Mean Solar Radiation" },
                    { "column_5", "Minimum Daily Temperature" },
                    { "column_6", "Maximum Daily Temperature" },
                    { "column_6.1", "Mean Wind Speed" },
                    { "column_7", "Minimum Relative Humidity" },
                    { "column_8", "Maximum Relative Humidity" },
                    { "column_8.1", "Water Temperature" },
                    { "column_9", "Potential Evapotranspiration" }
                };
            }
            output.Data = new Dictionary<string, List<string>>();

            foreach (DataRow dr in dt.Rows)
            {
                double tmean = Convert.ToDouble(dr["TMean_C"].ToString());
                double tmin = Convert.ToDouble(dr["TMin_C"].ToString());
                double tmax = Convert.ToDouble(dr["TMax_C"].ToString());
                double shmin = Convert.ToDouble(dr["SHmin"].ToString());
                double shmax = Convert.ToDouble(dr["SHmax"].ToString());
                if (inpt.TemporalResolution == "hourly")
                {
                    DateTime days = DateTime.Parse(dr["DateHour"].ToString());
                    strDate = days.ToString("yyyy-MM-dd");
                }
                else
                {
                    strDate = dr["Date"].ToString();
                }
                DateTime time1 = DateTime.ParseExact(strDate, "yyyy-MM-dd", CInfoUS);
                double wind = Convert.ToDouble(dr["WindSpeedMean_m/s"].ToString());
                double solarRad = Convert.ToDouble(dr["SolarRadMean_MJm2day"].ToString());
                int jday = Convert.ToInt32(dr["Julian_Day"].ToString());

                McJannettMethod(tmin, tmax, tmean, jday, time1, shmin, shmax, wind, solarRad, out relHMin, out relHMax, out wTemp,
                                out petMcJ, out errorMsg);

                dr["RHmin"] = relHMin.ToString("F2", CultureInfo.InstalledUICulture);
                dr["RHmax"] = relHMax.ToString("F2", CultureInfo.InstalledUICulture);
                dr["WaterTemp_C"] = wTemp.ToString("F2", CultureInfo.InstalledUICulture);
                dr["McJannettPET_in"] = petMcJ.ToString("F4", CultureInfo.InvariantCulture);
            }

            dt.Columns.Remove("SHmin");
            dt.Columns.Remove("SHmax");

            foreach (DataRow dr in dt.Rows)
            {
                List<string> lv = new List<string>();
                foreach (Object g in dr.ItemArray.Skip(1))
                {
                    lv.Add(g.ToString());
                }
                output.Data.Add(dr[0].ToString(), lv);
            }
            return output;
        }

        public DataTable daymetData(ITimeSeriesInput inpt, ITimeSeriesOutput outpt)
        {
            string errorMsg = "";
            string data = "";
            StringBuilder st = new StringBuilder();
            int yearDif = (inpt.DateTimeSpan.EndDate.Year - inpt.DateTimeSpan.StartDate.Year);
            for (int i = 0; i <= yearDif; i++)
            {
                string year = inpt.DateTimeSpan.StartDate.AddYears(i).Year.ToString();
                st.Append(year + ",");
            }
            st.Remove(st.Length - 1, 1);

            string url = "https://daymet.ornl.gov/data/send/saveData?" + "lat=" + inpt.Geometry.Point.Latitude + "&lon=" + inpt.Geometry.Point.Longitude
                + "&measuredParams=" + "tmax,tmin,srad,dayl" + "&years=" + st.ToString();
            WebClient myWC = new WebClient();
            try
            {
                int retries = 5;                                        // Max number of request retries
                string status = "";                                     // response status code

                while (retries > 0 && !status.Contains("OK"))
                {
                    Thread.Sleep(100);
                    WebRequest wr = WebRequest.Create(url);
                    HttpWebResponse response = (HttpWebResponse)wr.GetResponse();
                    status = response.StatusCode.ToString();
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    data = reader.ReadToEnd();
                    reader.Close();
                    response.Close();
                    retries -= 1;
                }
            }
            catch (Exception ex)
            {
                errorMsg = "ERROR: Unable to download data from Daymet. " + ex.Message;
                return null;
            }

            DataTable tab = new DataTable();
            tab.Columns.Add("Date");
            tab.Columns.Add("Julian_Day");
            tab.Columns.Add("TMin_C");
            tab.Columns.Add("TMax_C");
            tab.Columns.Add("TMean_C");
            tab.Columns.Add("SolarRadMean_MJm2day");

            string[] splitData = data.Split(new string[] { "year,yday,prcp (mm/day)", "year,yday,tmax (deg c),tmin (deg c)", "year,yday,dayl (s),srad (W/m^2),tmax (deg c),tmin (deg c)" }, StringSplitOptions.RemoveEmptyEntries);
            string[] lines = splitData[1].Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            Boolean julianflag = false;
            foreach (string line in lines)
            {
                string[] linedata = line.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (Convert.ToInt32(Convert.ToDouble(linedata[1])) >= inpt.DateTimeSpan.StartDate.DayOfYear && (Convert.ToInt32(Convert.ToDouble(linedata[0])) == inpt.DateTimeSpan.StartDate.Year))
                {
                    julianflag = true;
                }
                if (Convert.ToInt32(Convert.ToDouble(linedata[1])) > inpt.DateTimeSpan.EndDate.DayOfYear && (Convert.ToInt32(Convert.ToDouble(linedata[0])) == inpt.DateTimeSpan.EndDate.Year))
                {
                    julianflag = false;
                    break;
                }
                if (julianflag)
                {
                    DataRow tabrow = tab.NewRow();
                    tabrow["Date"] = (new DateTime(Convert.ToInt32(Convert.ToDouble(linedata[0])), 1, 1).AddDays(Convert.ToInt32(Convert.ToDouble(linedata[1])) - 1)).ToString(inpt.DateTimeSpan.DateTimeFormat);
                    tabrow["Julian_Day"] = Convert.ToInt32(Convert.ToDouble(linedata[1]));
                    tabrow["TMin_C"] = linedata[5];
                    tabrow["TMax_C"] = linedata[4];
                    tabrow["TMean_C"] = (Convert.ToDouble(linedata[4]) + Convert.ToDouble(linedata[5])) / 2.0;
                    double srad = Convert.ToDouble(linedata[3]);
                    double dayl = Convert.ToDouble(linedata[2]);
                    tabrow["SolarRadMean_MJm2day"] = Math.Round((srad * dayl) / 1000000, 2);
                    tab.Rows.Add(tabrow);
                }
            }
            return tab;
        }
    }
}
