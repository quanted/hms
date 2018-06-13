using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Globalization;
using Data;
using System.Net;
using System.Threading;
using System.IO;
using System.Text;

namespace Evapotranspiration
{
    public class ShuttleworthWallace
    {
        private double latitude;
        private double longitude;
        private double elevation;
        private double albedo;
        private double resistanceSurfaceSoil;
        private double resistanceStomatal;
        public Hashtable leafAreaIndex = new Hashtable(12);
        private double widthLeaf;
        private double groundRoughnessLength;
        private double vegetationHeight;

        public ShuttleworthWallace()
        {
            latitude = 33.925673;
            longitude = -83.355723;
            elevation = 213.36;
            albedo = 0.23;
            resistanceSurfaceSoil = 500.0;
            resistanceStomatal = 400.0;
            widthLeaf = 0.02;
            groundRoughnessLength = 0.02;
            vegetationHeight = 0.12;
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

        public double ResistanceSurfaceSoil
        {
            get
            {
                return resistanceSurfaceSoil;
            }
            set
            {
                resistanceSurfaceSoil = value;
            }
        }

        public double ResistanceStomatal
        {
            get
            {
                return resistanceStomatal;
            }
            set
            {
                resistanceStomatal = value;
            }
        }



        public double WidthLeaf
        {
            get
            {
                return widthLeaf;
            }
            set
            {
                widthLeaf = value;
            }
        }

        public double GroundRoughnessLength
        {
            get
            {
                return groundRoughnessLength;
            }
            set
            {
                groundRoughnessLength = value;
            }
        }

        public double VegetationHeight
        {
            get
            {
                return vegetationHeight;
            }
            set
            {
                vegetationHeight = value;
            }
        }

        public void ShuttleworthWallaceMethod(double tmin, double tmax, double tmean, int jday, DateTime time, double shmin, double shmax,
                                              double wind, double solarRad, out double relHumidityMin, out double relHumidityMax,
                                              out double petSW, out string errorMsg)
        {
            // Assign GUI-specified parameters to local variables.
            double EL = elevation;
            double lat = latitude;
            double ALBEDO = albedo;
            DateTime d = time;
            int mon = d.Month;
            double LAI = (double)leafAreaIndex[mon];
            double hc = vegetationHeight;

            double zog = groundRoughnessLength; // Yusuf       
            double rss = resistanceSurfaceSoil; // Yusuf

            double rstmin = resistanceStomatal;
            double wleaf = widthLeaf; // Yusuf

            errorMsg = "";
            relHumidityMin = 0.0;
            relHumidityMax = 0.0;

            // CONSTANTS
            const double A = 17.27;
            const double B = 237.3;
            const double PR = 101.13;  // pressure kilo pascals

            // Specific heat at constant pressure[MJ kg-1 deg C-1
            double CP = 1.013 * Math.Pow(10, -3);

            // ratio of mol wt of water vapour to dry air
            const double e = 0.622;

            const double pi = Math.PI;

            double LW = 0.0;

            // extracted from input data
            double TMIN = 0.0;   // celsisus
            double TMAX = 0.0;   // celsisus
            double jd1 = (double)jday;
            double RHmin = 0.0;
            double RHmax = 0.0;
            double u = 0.0;

            // EVAPO TRANSPIRATION
            double PET = 0.0;

            // CALCULATED VALUES
            // main calculated parameters used in the final evapo transpiration formulae
            double T = 0.0;
            double RA = 0.0;

            // other calculated parameters
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
            double wj = 0.08; // w in VB
            double fw = 5000.0;
            double kvon = 0.41;
            double za = hc + 2.0;
            double zow = 0.05;
            double Z0 = 0.13 * hc;
            double dp = 0.63 * hc;
            double densitya = 1.2;
            double heata = 0.001013;
            double ne = 0.0;
            double u2 = 0.0;
            double zoc = 0.0;
            double test1 = 0.0;
            double test2 = 0.0;
            double cd = 0.0;
            double d0 = 0.0;
            double ux = 0.0;
            double kh = 0.0;
            double zb = 0.0;
            double rsa = 0.0;
            double ua = 0.0;
            double raaa = 0.0;
            double rcs = 0.0;
            double rb1 = 0.0;
            double rca = 0.0;
            double cr = 0.7;
            double RSN = 0.0;
            double RA1 = 0.0;
            double RC = 0.0;
            double CC1 = 0.0;
            double CS1 = 0.0;
            double NUMC = 0.0;
            double DENC = 0.0;
            double NUMS = 0.0;
            double DENS1 = 0.0;
            double ETC = 0.0;
            double ETS1 = 0.0;
            double zw = 0.0;

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

                // wind speed correction based on height
                double ht = 10.0;  // The wind data in the NLDAS database is given at a height of 10 meters.
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

                if (u <= 0.6)
                {
                    u = 0.6;
                }

                zw = ht;
                T = (TMIN + TMAX) / 2.0;

                es1 = 0.6108 * Math.Exp((A * TMIN) / (B + TMIN));
                es2 = 0.6108 * Math.Exp((A * TMAX) / (B + TMAX));

                es = (es1 + es2) / 2.0;
                double ee = 0.0;
                double slope1 = 0.0;
                //yusuf             
                // slope of saturation vapour pressure curve           
                slope = (4098 * 0.6108 * Math.Exp((17.27 * T) / (237.3 + T))) / Math.Pow((T + 237.3), 2.0);

                // Actual water vapor pressure in the atmosphere
                ea = (es2 * (RHmin / 100.0) + (es1 * RHmax / 100.0)) / 2.0;

                if (T >= 0.0)    //  Wilson Melendez: removed semicolon that had been placed after the parenthesis.      
                {
                    ee = 0.61017 * Math.Exp(17.2639 * T / (T + 237.3));  //From Federer
                    slope1 = (4098 * ee) / Math.Pow((T + 237.3), 2.0);
                }

                if (T < 0.0)
                {
                    ee = 0.61017 * Math.Exp(21.87456 * T / (T + 265.5));//From Federer
                    slope1 = (5808 * ee) / Math.Pow((T + 265.5), 2.0);
                }

                double slope2 = Math.Pow((0.00815 * T + 0.8912), 7.0);
                deficit = es - ea;

                P = PR * Math.Pow(((293.0 - 0.0065 * EL) / 293.0), 5.26);
                LW = 2.45;    // 2.501-0.00236*T;

                // psychometric constant
                gama = CP * P / (LW * e);

                // ############# ENERGY EQUATIONS ########################################

                /* CALCULATION OF EXTRATERRESTIAL RADIATION FOR DAILY PERIODS
                ---- THIS PORTION OF THE PROGRAM CALCULATES THE ENERGY COMPONENT  */

                radian = (pi / 180.0) * lat;
                dr = 1.0 + 0.033 * Math.Cos(2 * (pi / 365) * jd1);
                sigma2 = 0.409 * Math.Sin((2.0 * pi * jd1 / 365.0) - 1.39);
                xx = 1.0 - (Math.Pow(Math.Tan(radian), 2.0) * Math.Pow(Math.Tan(sigma2), 2.0));

                if (xx <= 0) xx = 0.00001;

                ws = (pi / 2.0) - (Math.Atan(-Math.Tan(radian) * (Math.Tan(sigma2) / Math.Pow(xx, 0.5))));  // Wilson Melendez: removed extra space between - and Math.

                RA = (24.0 * 60.0 / pi) * 0.082 * dr * (ws * Math.Sin(radian) * Math.Sin(sigma2) +
                      Math.Cos(radian) * Math.Cos(sigma2) * Math.Sin(ws));

                // CALCULATION OF SOLAR OR SHORT WAVE RADIATION

                NN = (24.0 / pi) * ws;
                RS = solarRad;      // Units are in MJ/(m^2 day) 

                if (hc <= 1)
                {
                    ne = 2.5;
                }
                else if (hc > 1 && hc < 10)
                {
                    ne = 2.306 + 0.19 * hc;
                }
                else if (hc >= 10)
                {
                    ne = 4.25;
                }

                if (hc <= 1)
                {
                    zoc = 0.13 * hc;
                }
                else if (hc > 1 && hc < 10)
                {
                    zoc = 0.139 * hc - 0.009 * hc * hc;
                }
                else if (hc >= 10)
                {
                    zoc = 0.05 * hc;
                }

                if (hc == 0)
                {
                    cd = 1.4 * 1.0e3;
                }
                else if (hc > 0)
                {
                    cd = (-1 + Math.Pow(Math.Exp(0.909 - 3.03 * (zoc / hc)), 4)) / 4; // Federer
                }

                if (LAI >= 4)
                {
                    d0 = hc - zoc / 0.3;
                }
                else
                {
                    d0 = 1.1 * hc * Math.Log(1 + Math.Pow((cd * LAI), 0.25));
                }

                if (d0 >= hc)
                {
                    d0 = LAI * 2.6266 / 4;
                }

                test1 = 0.3 * (hc - d0);
                test2 = zog + 0.3 * hc * Math.Pow((cd * LAI), 0.5);

                if (test1 < test2)
                {
                    Z0 = test1;
                }
                else
                {
                    Z0 = test2;
                }

                if (test1 == test2)
                {
                    Z0 = test1;
                }

                wj = wleaf;

                ux = kvon * u / (Math.Log(za - d0) / Z0);
                kh = kvon * ux * (hc - d0);

                zb = 0.334 * Math.Pow(fw, 0.875) * Math.Pow(zow, 0.125);
                rsa = hc * Math.Exp(ne) / (ne * kh) * (Math.Exp(-ne * zog / hc) - Math.Exp(-ne * (Z0 + dp) / hc));

                ua = u * Math.Log(zb / zow) / (Math.Log(zb / Z0)) * (Math.Log(za - d0) / Z0) / (Math.Log(zw / zow));
                raaa = (1 / (kvon * ux)) * Math.Log((za - d0) / (hc - d0)) + (hc / (ne * kh)) * (Math.Exp(ne * (1 - (zoc + dp) / hc)) - 1);

                rcs = (rstmin) / (LAI * 2);
                rb1 = (100 / ne) * Math.Pow((wj / ua), 0.5) * (1 / (1 - Math.Exp(-ne / 2)));

                rca = rb1 / (LAI * 2);

                // Surface resistance is set from the GUI       
                RSO = (0.75 + 2.0 * (EL / 100000.0)) * RA;
                RR = RS / RSO;

                RNS = (1 - ALBEDO) * RS;

                tmaxk = 4.903 * Math.Pow(10, -9.0) * Math.Pow(TMAX + 273.16, 4.0);
                tmink = 4.903 * Math.Pow(10, -9.0) * Math.Pow(TMIN + 273.16, 4.0);

                AVGTK = (tmaxk + tmink) / 2.0;

                EB = 0.34 - 0.14 * Math.Pow(ea, 0.5);
                RB = 1.35 * RR - 0.35;
                RNL = AVGTK * EB * RB;
                Rnet = ((RNS - RNL));  // Wilson Melendez: not sure why we have double parenthesis here.
                RSN = (Rnet * Math.Exp(-cr * LAI)); // beers equation

                double A1 = Rnet;
                double AS = RSN;
                RA1 = (slope1 + gama) * raaa;

                RC = (slope1 + gama) * rca + gama * rsa;
                RS = (slope1 + gama) * rsa + gama * rss;

                CC1 = 1 / (1 + (RC * RA1) / (RS * (RC + RA1)));

                CS1 = 1 / (1 + (RS * RA1) / (RC * (RS + RA1)));
                //yusuf removed 24*3600 from numc and nums
                NUMC = slope1 * A1 + (24 * 3600 * (densitya * heata * deficit) - slope1 * AS) / (raaa + rca);

                DENC = slope1 + gama * (1 + (rcs / (raaa + rca)));

                NUMS = (slope1 * A1) + (24 * 3600 * (densitya * heata * deficit) - slope1 * rca * (A1 - AS)) / (raaa + rsa);
                DENS1 = slope1 + gama * (1 + (rss / (raaa + rca)));

                ETC = CC1 * (NUMC / DENC) / LW;
                ETS1 = CS1 * (NUMS / DENS1) / LW;

                PET = ETC + ETS1;

            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }

            // Convert mm/day to inches/day
            petSW = (PET / 25.4);

        }

        public ITimeSeriesOutput Compute(ITimeSeriesInput inpt, ITimeSeriesOutput outpt, double lat, double lon, string startDate, string endDate, int timeZoneOffset, out string errorMsg)
        {
            errorMsg = "";
            //NLDAS2 nldas = new NLDAS2(inpt.Source, lat, lon, startDate, endDate);
            double relHMax = 0;
            double relHMin = 0.0;
            double petSW = 0;
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
                    dt = cd.ParseCustomData(inpt, outpt, inpt.Geometry.GeometryMetadata["userdata"].ToString(), "shuttleworthwallace");
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
                        DataRow dr1 = null;
                        List<Double> tList = new List<double>();
                        List<Double> sList = new List<double>();
                        double sol = 0.0;
                        double wind = 0.0;
                        if (inpt.TemporalResolution == "weekly")
                        {
                            DataTable wkly = dt.Clone();
                            int j = 0;
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                if (j == 0)
                                {
                                    dr1 = wkly.NewRow();
                                    dr1["Date"] = dt.Rows[i]["Date"].ToString();
                                    dr1["Julian_Day"] = dt.Rows[i]["Julian_Day"].ToString();
                                    tList = new List<double>();
                                    sList = new List<double>();
                                    sol = 0.0;
                                    wind = 0.0;
                                }
                                tList.Add(Convert.ToDouble(dt.Rows[i]["TMin_C"].ToString()));
                                tList.Add(Convert.ToDouble(dt.Rows[i]["TMax_C"].ToString()));
                                sol += Convert.ToDouble(dt.Rows[i]["SolarRadMean_MJm2day"]);
                                wind += Convert.ToDouble(dt.Rows[i]["WindSpeedMean_m/s"]);
                                sList.Add(Convert.ToDouble(dt.Rows[i]["SHmin"].ToString()));
                                sList.Add(Convert.ToDouble(dt.Rows[i]["SHmax"].ToString()));
                                if (j == 6 || i == dt.Rows.Count - 1)
                                {
                                    dr1["TMin_C"] = tList.Min().ToString("F2", CultureInfo.InvariantCulture);
                                    dr1["TMax_C"] = tList.Max().ToString("F2", CultureInfo.InvariantCulture);
                                    dr1["TMean_C"] = (tList.Min() + tList.Max()) / 2.0;
                                    dr1["SolarRadMean_MJm2day"] = Math.Round(sol / (j + 1), 2);
                                    dr1["WindSpeedMean_m/s"] = Math.Round(wind / (j + 1), 2);
                                    dr1["SHmin"] = sList.Min().ToString();
                                    dr1["SHmax"] = sList.Max().ToString();
                                    wkly.Rows.Add(dr1);
                                    j = -1;
                                }
                                j++;
                            }
                            dt = wkly;
                        }
                        else if (inpt.TemporalResolution == "monthly")
                        {
                            DataTable mnly = dt.Clone();
                            int curmonth = inpt.DateTimeSpan.StartDate.Month;
                            int j = 0;
                            bool newmonth = true;
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                if (newmonth)
                                {
                                    dr1 = mnly.NewRow();
                                    dr1["Date"] = dt.Rows[i]["Date"].ToString();
                                    dr1["Julian_Day"] = dt.Rows[i]["Julian_Day"].ToString();
                                    tList = new List<double>();
                                    sList = new List<double>();
                                    sol = 0.0;
                                    wind = 0.0;
                                    newmonth = false;
                                    curmonth = Convert.ToDateTime(dt.Rows[i]["Date"]).Month;
                                }
                                tList.Add(Convert.ToDouble(dt.Rows[i]["TMin_C"].ToString()));
                                tList.Add(Convert.ToDouble(dt.Rows[i]["TMax_C"].ToString()));
                                sol += Convert.ToDouble(dt.Rows[i]["SolarRadMean_MJm2day"]);
                                wind += Convert.ToDouble(dt.Rows[i]["WindSpeedMean_m/s"]);
                                sList.Add(Convert.ToDouble(dt.Rows[i]["SHmin"].ToString()));
                                sList.Add(Convert.ToDouble(dt.Rows[i]["SHmax"].ToString()));
                                if (i + 1 < dt.Rows.Count && (Convert.ToDateTime(dt.Rows[i + 1]["Date"]).Month != curmonth) || i == dt.Rows.Count - 1)
                                {
                                    dr1["TMin_C"] = tList.Min().ToString("F2", CultureInfo.InvariantCulture);
                                    dr1["TMax_C"] = tList.Max().ToString("F2", CultureInfo.InvariantCulture);
                                    dr1["TMean_C"] = (tList.Min() + tList.Max()) / 2.0;
                                    dr1["SolarRadMean_MJm2day"] = Math.Round(sol / (j + 1), 2);
                                    dr1["WindSpeedMean_m/s"] = Math.Round(wind / (j + 1), 2);
                                    dr1["SHmin"] = sList.Min().ToString();
                                    dr1["SHmax"] = sList.Max().ToString();
                                    mnly.Rows.Add(dr1);
                                    j = -1;
                                    newmonth = true;
                                }
                                j++;
                            }
                            dt = mnly;
                        }
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
            dt.Columns.Add("ShuttleworthWallacePET_in");

            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput output = oFactory.Initialize();
            output.Dataset = "Evapotranspiration";
            output.DataSource = "shuttleworthwallace";
            string leafareas = "{ ";
            foreach (int key in leafAreaIndex.Keys)
            {
                leafareas += String.Format("{0}: {1}, ", key, leafAreaIndex[key]);
            }
            leafareas += "}";
            output.Metadata = new Dictionary<string, string>()
            {
                { "elevation", elevation.ToString() },
                { "latitude", latitude.ToString() },
                { "longitude", longitude.ToString() },
                { "albedo", albedo.ToString() },
                { "stomatal_resistance", resistanceStomatal.ToString() },
                { "surface_resistance", resistanceSurfaceSoil.ToString() },
                { "ground_roughness_length", groundRoughnessLength.ToString() },
                { "leaf_width", widthLeaf.ToString() },
                { "vegetation_height", vegetationHeight.ToString() },
                { "leaf_area_indices", leafareas},
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
                { "column_8", "Potential Evapotranspiration" }
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

                ShuttleworthWallaceMethod(tmin, tmax, tmean, jday, time1, shmin, shmax, wind, solarRad,
                                          out relHMin, out relHMax, out petSW, out errorMsg);

                dr["RHmin"] = relHMin.ToString("F2", CultureInfo.InstalledUICulture);
                dr["RHmax"] = relHMax.ToString("F2", CultureInfo.InstalledUICulture);
                dr["ShuttleworthWallacePET_in"] = petSW.ToString("F4", CultureInfo.InvariantCulture);
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
