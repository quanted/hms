﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Globalization;
using Data;
using System.Threading;
using System.Net;
using System.IO;

namespace Evapotranspiration
{
    public class MortonCRWE
    {
        private double latitude;
        private double longitude;
        private double elevation;
        private double albedo;
        private double emissivity;
        private double annualPrecipitation;
        private double azenith;

        public MortonCRWE()
        {
            latitude = 33.925673;
            longitude = -83.355723;
            elevation = 213.36;
            albedo = 0.23;
            emissivity = 0.92;
            annualPrecipitation = 1185.9;
            azenith = 0.05;
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

        public double Emissivity
        {
            get
            {
                return emissivity;
            }
            set
            {
                emissivity = value;
            }
        }

        public double AnnualPrecipitation
        {
            get
            {
                return annualPrecipitation;
            }
            set
            {
                annualPrecipitation = value;
            }
        }

        public double Azenith
        {
            get
            {
                return azenith;
            }
            set
            {
                azenith = value;
            }
        }

        public void MortonCRWEMethod(double tmin, double tmax, double tmean, int jday, DateTime Time, double shmin, double shmax,
                                     double solarRad, int model, out double relHumidityMin, out double relHumidityMax,
                                     out double petMCW, out string errorMsg)
        {
            double EL = elevation;
            double lat = latitude;
            double ALBEDO = albedo;
            double EE = emissivity;
            double pa = annualPrecipitation;
            double azz = azenith;
            errorMsg = "";
            relHumidityMin = 0.0;
            relHumidityMax = 0.0;

            // CONSTANTS
            const double A = 17.27;
            const double B = 237.3;

            // Specific heat at constant pressure[MJ kg-1 deg C-1
            double CP = 1.013 * Math.Pow(10, -3);

            const double pi = Math.PI;

            // extracted from input data
            double TMIN = 0.0;   // celsisus
            double TMAX = 0.0;   // celsisus
            double jd1 = (double)jday;
            double RHmin = 0.0;
            double RHmax = 0.0;


            // EVAPO TRANSPIRATION
            double PET = 0.0;

            // CALCULATED VALUES
            // main calculated parameters used in the final evapo transpiration formulae
            double T = 0.0;
            double RA = 0.0;
            double TD = 0.0;

            // other calculated parameters
            double radian = 0.0;
            double dr = 0.0;
            double sigma2 = 0.0;
            double xx = 0.0;
            double ws = 0.0;
            double RS = 0.0;
            double NL = 0.0;
            double denom = 0.0;
            double nenom = 0.0;
            double pp = 0.0;
            double AZ = 0.0;
            double RH = 0.0;
            double VD = 0.0;
            double V = 0.0;
            double SLOPE = 0.0;
            double a = 0.0;
            double bt = 0.0;
            double AL = 0.0;
            int MON = 0;
            double RAD = 0.0;

            double COSZ = 0.0;
            double COSW = 0.0;
            double COSZA = 0.0;
            double W = 0.0;
            double ZZ = 0.0;
            double NI = 0.0;
            double GE = 0.0;
            double C0 = 0.0;
            double AZ1 = 0.0;
            double AH = 0.0;
            double AU = 0.0;
            double A0 = 0.0;
            double C11 = 0.0;
            double J = 0.0;
            double TAU = 0.0;
            double TAUU = 0.0;
            double TMP = 0.0;
            double G0 = 0.0;
            double NS = 0.0;
            double S = 0.0;
            double GG = 0.0;
            double AA = 0.0;
            double C2 = 0.0;
            double CLOUD = 0.0;
            double SB = 5.5 * 1.0e-8;
            double BB = 0.0;
            double CONSTRAINT1 = 0.0;
            double NETR = 0.0;
            double RTC1 = 0.0;
            double FZ = 28;
            double YPS = 0.0;
            double SF = 0.0;
            double FCTOR = 0.0;
            double FT1 = 0.0;
            double LAMB = 0.0;
            double TP = 0.0;
            double VP = 0.0;
            double DELP = 0.0;
            double DELT = 0.0;
            double ETP = 0.0;
            double RTP = 0.0;
            double B1 = 13;
            double B2 = 1.12;
            double ETW = 0.0;
            double ETP1 = 0.0;
            double ETW1 = 0.0;
            double ETA = 0.0;
            double ETA1 = 0.0;
            double ETPM = 0.0;
            double ETWM = 0.0;
            double ETAM = 0.0;
            double CONV1 = 28.5;

            try
            {
                TMIN = tmin;
                TMAX = tmax;

                // Convert specific humidity to relative humidity here.  
                RHmin = Utilities.Utility.CalculateRH(shmin, tmin, 1013.25);
                RHmax = Utilities.Utility.CalculateRH(shmax, tmax, 1013.25);

                // Check relative humidities
                if (RHmax < RHmin)
                {
                    double swap = RHmin;
                    RHmin = RHmax;
                    RHmax = swap;
                }

                // Set relHumidityMin to RHmin and relHumidityMax to RHmax.
                relHumidityMin = RHmin;
                relHumidityMax = RHmax;


                RH = 0.5 * (RHmin + RHmax);

                // T = (TMIN + TMAX) / 2.0;
                T = tmean;
                NS = solarRad;  //  Wilson Melendez: this should be checked.  NS = Sunshine hours??

                pp = Math.Pow(((288.0 - 0.0065 * EL) / 288.0), 5.256);

                AZ = 0.26 - 0.00012 * pa * (Math.Pow(pp, 0.5)) * (1 + (Math.Abs(lat / 42) + Math.Pow((lat / 42), 2)));

                if (AZ <= 0.11)
                {
                    AZ = 0.11;
                }

                if (AZ >= 0.17)
                {
                    AZ = 0.17;
                }


                nenom = 243.04 * (Math.Log(RH / 100)) + 243.04 * (17.625 * T) / (243.04 + T);
                denom = (17.625 - Math.Log(RH / 100) - (17.625 * T) / (243.4 + T));
                TD = nenom / denom;
                VD = 6.11 * Math.Exp(A * TD / (TD + B));
                V = 6.11 * Math.Exp(A * T / (T + B));
                if (T >= 0)
                {
                    a = A;
                    bt = B;
                }
                else
                {
                    a = 21.88;
                    bt = 265.5;
                }

                SLOPE = a * V * bt / (Math.Pow((bt + T), 2));

                RAD = pi / 180;

                DateTime d = Time;
                MON = d.Month;

                AL = 23.2 * Math.Sin((29.5 * MON - 94) * RAD) * RAD;
                COSZ = Math.Cos(lat * (pi / 180) - AL);
                ZZ = Math.Acos(COSZ);

                if (COSZ < 0.001)
                {
                    COSZ = 0.001;
                }

                COSW = 1 - Math.Cos(ZZ) / (Math.Cos(AL) * Math.Cos(lat * (pi / 180)));
                W = Math.Acos(COSW);
                radian = (pi / 180.0) * lat;

                COSZA = COSZ + (Math.Sin(W) / W - 1) * Math.Cos(radian) * Math.Cos(AL);
                NI = Math.Sin((29.5 * MON - 106) * RAD) * (1 / 60.0);
                NI = 1 + NI;
                GE = (1354 / Math.Pow(NI, 2)) * (W / pi) * COSZA;

                C0 = V - VD;

                if (C0 < 0)
                {
                    C0 = 0;
                }

                if (C0 > 1)
                {
                    C0 = 1;
                }


                AH = 180 / pi;
                AZ1 = azz + (1 - Math.Pow(C0, 2)) * (0.34 - azz);

                AU = Math.Exp(1.08) - (2.16 * COSZ * (1 / pi) + Math.Sin(ZZ)) * Math.Exp(0.012 * ZZ * AH);

                A0 = (AZ1 * AU) / (1.473 * (1 - Math.Sin(ZZ)));

                W = VD / (0.49 + (T / 129));

                C11 = 21 - T;
                if (C11 < 0)
                {
                    C11 = 0;
                }

                if (C11 > 5)
                {
                    C11 = 5;
                }

                J = (0.5 + 2.5 * Math.Pow(COSZA, 2)) * Math.Exp(C11 * (pp - 1));

                TAU = Math.Exp(-0.089 * Math.Pow(pp * 1 / COSZA, 0.75) - 0.083 * Math.Pow(J / COSZA, 0.9) - 0.029 * Math.Pow(W / COSZA, 0.6));

                TAUU = Math.Exp(-0.0415 * Math.Pow(J / COSZA, 0.9) - Math.Pow(0.0029, 0.5) * Math.Pow((W / COSZA), 0.3));

                TMP = Math.Exp(-0.0415 * Math.Pow(pp * (J / COSZA), 0.9) - Math.Pow(0.029, 0.5) * Math.Pow((W / COSZA), 0.6));

                if (TAUU < TMP)
                {
                    TAUU = TMP;
                }

                G0 = GE * TAU * (1 + (1 - TAU / TAUU) * (1 + A0 * TAU));


                // ############# ENERGY EQUATIONS ########################################

                /*CALACULATION OF EXTRATERRESTIAL RADIATION FOR DAILY PERIOIDS
                ---- THIS PORTION OF THE PROGRAM CALCULATES THE ENERGY COMPONENT  */


                dr = 1.0 + 0.033 * Math.Cos(2 * (pi / 365) * jd1);
                sigma2 = 0.409 * Math.Sin((2.0 * pi * jd1 / 365.0) - 1.39);
                xx = 1.0 - (Math.Pow(Math.Tan(radian), 2.0) * Math.Pow(Math.Tan(sigma2), 2.0));

                if (xx <= 0) xx = 0.00001;

                ws = (pi / 2.0) - (Math.Atan(-Math.Tan(radian) * (Math.Tan(sigma2) / Math.Pow(xx, 0.5))));

                RA = (24.0 * 60.0 / pi) * 0.082 * dr * (ws * Math.Sin(radian) * Math.Sin(sigma2) +
                      Math.Cos(radian) * Math.Cos(sigma2) * Math.Sin(ws));

                // CALCULATION OF SOLAR OR SHORT WAVE RADIATION

                NL = (24.0 / pi) * ws;
                // YUSUF - Add if statement
                S = NS / NL;
                if (NS > NL) S = 0.99;

                RS = (0.25 + 0.5 * S) * RA;   // Wilson Melendez: RS is calculated here but not used after this line.

                GG = S * G0 + (0.08 + 0.3 * S) * (1 - S) * GE;

                AA = A0 * (S + (1 - S) * (1 - ZZ / 330 * (180 / pi)));
                C2 = 10 * (VD / V - S - 0.42);

                if (C2 < 0)
                {
                    C2 = 0;
                }

                if (C2 > 1)
                {
                    C2 = 1;
                }

                CLOUD = 0.18 * ((1 - C2) * Math.Pow((1 - S), 2) + C2 * Math.Pow((1 - S), 0.5)) * (1 / pp);

                BB = SB * Math.Pow((T + 273), 4) * (1 - (0.71 + 0.007 * VD * pp) * (1 + CLOUD));

                CONSTRAINT1 = 0.05 * SB * Math.Pow((T + 273), 4);

                if (BB > CONSTRAINT1)
                {
                    BB = BB;  //YUSUF;  Wilson Melendez: this is considered an assignment to itself and consequently it needs to be revised.
                }

                RTC1 = NETR = (1 - AA) * GG - BB;

                YPS = 0.66 * pp;

                if (T <= 0)
                {
                    FZ = 28.75;  //YUSUF
                }
                else
                {
                    FZ = 25;
                }

                SF = 0.28 * (1 + VD / V) + SLOPE * RTC1 / (YPS * Math.Pow((1 / pp), 0.5) * FZ * (V - VD));
                FCTOR = 1 / SF;
                FT1 = SF * Math.Pow((1 / pp), 0.5) * FZ;
                LAMB = YPS + 4 * SB * Math.Pow((T + 273), 3) / FT1;

                TP = T;
                VP = V;
                DELP = SLOPE;

                //YUSUF For JJ = 1 To n

                for (int jj = 0; jj < 1; jj++)
                {
                    DELT = ((RTC1 / FT1) + VD - VP + LAMB * (T - TP)) / (DELP + LAMB);
                    TP = TP + DELT;
                    VP = 6.11 * Math.Exp(a * TP / (TP + bt));
                    DELP = a * bt * VP / (Math.Pow((TP + bt), 2));  //BT TO B
                    if (Math.Abs(DELT) < 0.01)
                    {
                        ETP = RTC1 - LAMB * FT1 * (TP - T);  //YUSUF
                        break;
                    }
                }

                ETP = RTC1 - LAMB * FT1 * (TP - T);
                //YUSUF     Next JJ

                RTP = ETP + YPS * FT1 * (TP - T);

                ETW = B1 + B2 * RTP / (1 + YPS / DELP);

                ETA = 2 * ETW - ETP;

                ETP1 = ETP / CONV1;

                ETW1 = ETW / CONV1;
                ETA1 = ETA / CONV1;

                if (ETW1 < 0.0)
                {
                    ETW1 = 0.0;

                }


                ETPM = ETP1 * 31;
                ETWM = ETW1 * 31;
                ETAM = ETA1 * 31;

                TP = 0;
                VP = 0;
                DELP = 0;

                if (model == 0)
                {
                    PET = ETP1;
                }
                else if (model == 1)
                {
                    PET = ETW1;
                }

            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }

            // Convert mm/day to inches/day
            petMCW = PET / 25.4;

        }

        public ITimeSeriesOutput Compute(ITimeSeriesInput inpt, ITimeSeriesOutput outpt, double lat, double lon, string startDate, string endDate, int timeZoneOffset, int model, out double aprecip, out string errorMsg)
        {
            errorMsg = "";
            //NLDAS2 nldas = new NLDAS2(inpt.Source, lat, lon, startDate, endDate);
            aprecip = 0.0; //nldas.getAnnualPrecipitation();
            //AnnualPrecipitation = aprecip;
            double relHMax = 0;
            double relHMin = 0.0;
            double petMCW = 0;
            string strDate;
            CultureInfo CInfoUS = new CultureInfo("en-US");

            //DataTable dt = nldas.getData3(timeZoneOffset, out errorMsg);
            DataTable dt = new DataTable();
            DataTable data3 = new DataTable();
            switch (inpt.Source)
            {
                case "daymet":
                    dt = daymetData(inpt, outpt);
                    dt = Utilities.Utility.aggregateData(inpt, dt, "mortoncrwe");
                    inpt.Source = "nldas";
                    NLDAS2 nldas = new NLDAS2(inpt.Source, lat, lon, startDate, endDate);
                    data3 = nldas.getData3(timeZoneOffset, out errorMsg);
                    dt.Columns.Add("SHmin");
                    dt.Columns.Add("SHmax");
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dt.Rows[i]["SHmin"] = data3.Rows[i]["SHmin"];
                        dt.Rows[i]["SHmax"] = data3.Rows[i]["SHmin"];
                    }
                    data3 = null;
                    inpt.Source = "daymet";
                    break;
                case "nldas":
                case "gldas":
                default:
                    NLDAS2 nldas2 = new NLDAS2(inpt.Source, lat, lon, startDate, endDate);
                    if (inpt.TemporalResolution == "hourly")
                    {
                        NLDAS2 nldasday = new NLDAS2(inpt.Source, lat, lon, startDate, endDate);
                        DataTable dtd = nldasday.getData3(timeZoneOffset, out errorMsg);
                        dt = nldas2.getDataHourly(timeZoneOffset, false, out errorMsg);
                        dt.Columns["THourly_C"].ColumnName = "TMean_C";
                        dt.Columns["SolarRad_MJm2day"].ColumnName = "SolarRadMean_MJm2day";
                        dt.Columns.Remove("WindSpeed_m/s");
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
                        dt = nldas2.getData3(timeZoneOffset, out errorMsg);
                        DataRow dr1 = null;
                        List<Double> tList = new List<double>();
                        List<Double> sList = new List<double>();
                        double sol = 0.0;
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
                                }
                                tList.Add(Convert.ToDouble(dt.Rows[i]["TMin_C"].ToString()));
                                tList.Add(Convert.ToDouble(dt.Rows[i]["TMax_C"].ToString()));
                                sol += Convert.ToDouble(dt.Rows[i]["SolarRadMean_MJm2day"]);
                                sList.Add(Convert.ToDouble(dt.Rows[i]["SHmin"].ToString()));
                                sList.Add(Convert.ToDouble(dt.Rows[i]["SHmax"].ToString()));
                                if (j == 6 || i == dt.Rows.Count - 1)
                                {
                                    dr1["TMin_C"] = tList.Min().ToString("F2", CultureInfo.InvariantCulture);
                                    dr1["TMax_C"] = tList.Max().ToString("F2", CultureInfo.InvariantCulture);
                                    dr1["TMean_C"] = (tList.Min() + tList.Max()) / 2.0;
                                    dr1["SolarRadMean_MJm2day"] = Math.Round(sol / (j + 1), 2);
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
                                    newmonth = false;
                                    curmonth = Convert.ToDateTime(dt.Rows[i]["Date"]).Month;
                                }
                                tList.Add(Convert.ToDouble(dt.Rows[i]["TMin_C"].ToString()));
                                tList.Add(Convert.ToDouble(dt.Rows[i]["TMax_C"].ToString()));
                                sol += Convert.ToDouble(dt.Rows[i]["SolarRadMean_MJm2day"]);
                                sList.Add(Convert.ToDouble(dt.Rows[i]["SHmin"].ToString()));
                                sList.Add(Convert.ToDouble(dt.Rows[i]["SHmax"].ToString()));
                                if (i + 1 < dt.Rows.Count && (Convert.ToDateTime(dt.Rows[i + 1]["Date"]).Month != curmonth) || i == dt.Rows.Count - 1)
                                {
                                    dr1["TMin_C"] = tList.Min().ToString("F2", CultureInfo.InvariantCulture);
                                    dr1["TMax_C"] = tList.Max().ToString("F2", CultureInfo.InvariantCulture);
                                    dr1["TMean_C"] = (tList.Min() + tList.Max()) / 2.0;
                                    dr1["SolarRadMean_MJm2day"] = Math.Round(sol / (j + 1), 2);
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
                    aprecip = nldas2.getAnnualPrecipitation();
                    AnnualPrecipitation = aprecip;
                    break;
            }

            if (errorMsg != "")
            {
                Utilities.ErrorOutput err = new Utilities.ErrorOutput();
                return err.ReturnError(errorMsg);
            }

            dt.Columns.Add("RHmin");
            dt.Columns.Add("RHmax");
            dt.Columns.Add("MortonCRWEPET_in");

            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput output = oFactory.Initialize();
            output.Dataset = "Evapotranspiration";
            output.DataSource = "mortoncrwe";
            output.Metadata = new Dictionary<string, string>()
            {
                { "elevation", elevation.ToString() },
                { "latitude", latitude.ToString() },
                { "longitude", longitude.ToString() },
                { "albedo", albedo.ToString() },
                { "emissivity", emissivity.ToString() },
                { "annual_precipitation", annualPrecipitation.ToString() },
                { "zenith", azenith.ToString() },
                { "model", model.ToString() },
                { "request_time", DateTime.Now.ToString() },
                { "column_1", "Date" },
                { "column_2", "Julian Day" },
                { "column_3", "Minimum Temperature" },
                { "column_4", "Maximum Temperature" },
                { "column_5", "Mean Temperature" },
                { "column_6", "Mean Solar Radiation" },
                { "column_7", "Minimum Relative Humidity" },
                { "column_8", "Maximum Relative Humidity" },
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
                    strDate = dr["Date"].ToString().Trim();
                }
                DateTime time = DateTime.ParseExact(strDate, "yyyy-MM-dd", CInfoUS);
                double solarRad = Convert.ToDouble(dr["SolarRadMean_MJm2day"].ToString());
                int jday = Convert.ToInt32(dr["Julian_Day"].ToString());

                MortonCRWEMethod(tmin, tmax, tmean, jday, time, shmin, shmax, solarRad, model, out relHMin, out relHMax,
                                 out petMCW, out errorMsg);
                if (inpt.Source == "daymet")
                {
                    double vapor = Convert.ToDouble(dr["VaPress"].ToString());
                    dr["RHmin"] = daymetHumid(tmin, vapor).ToString("F2", CultureInfo.InstalledUICulture);
                    dr["RHmax"] = daymetHumid(tmax, vapor).ToString("F2", CultureInfo.InstalledUICulture);
                }
                else
                {
                    dr["RHmin"] = relHMin.ToString("F2", CultureInfo.InstalledUICulture);
                    dr["RHmax"] = relHMax.ToString("F2", CultureInfo.InstalledUICulture);
                }
                dr["MortonCRWEPET_in"] = petMCW.ToString("F4", CultureInfo.InvariantCulture);
            }
            if (inpt.Source == "daymet")
            {
                dt.Columns.Remove("VaPress");
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

        public double daymetHumid(double temp, double vapor)
        {
            double humid = 0.0;
            double es = 611 * Math.Exp((17.27 * temp) / (237.3 + temp));
            humid = vapor / es;
            return humid * 100;
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
                + "&measuredParams=" + "tmax,tmin,srad,dayl,vp" + "&years=" + st.ToString();
            WebClient myWC = new WebClient();
            try
            {
                int retries = 5;                                        // Max number of request retries
                string status = "";                                     // response status code

                while (retries > 0 && !status.Contains("OK"))
                {
                    WebRequest wr = WebRequest.Create(url);
                    HttpWebResponse response = (HttpWebResponse)wr.GetResponse();
                    status = response.StatusCode.ToString();
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    data = reader.ReadToEnd();
                    reader.Close();
                    response.Close();
                    retries -= 1;
                    if (!status.Contains("OK"))
                    {
                        Thread.Sleep(100);
                    }
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
            tab.Columns.Add("VaPress");

            string[] splitData = data.Split(new string[] { "year,yday,prcp (mm/day)", "year,yday,tmax (deg c),tmin (deg c)", "year,yday,dayl (s),srad (W/m^2),tmax (deg c),tmin (deg c),vp (Pa)" }, StringSplitOptions.RemoveEmptyEntries);
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
                    tabrow["Date"] = (new DateTime(Convert.ToInt32(Convert.ToDouble(linedata[0])), 1, 1).AddDays(Convert.ToInt32(Convert.ToDouble(linedata[1])) - 1)).ToString("yyyy-MM-dd");
                    tabrow["Julian_Day"] = Convert.ToInt32(Convert.ToDouble(linedata[1]));
                    tabrow["TMin_C"] = linedata[5];
                    tabrow["TMax_C"] = linedata[4];
                    tabrow["TMean_C"] = (Convert.ToDouble(linedata[4]) + Convert.ToDouble(linedata[5])) / 2.0;
                    double srad = Convert.ToDouble(linedata[3]);
                    double dayl = Convert.ToDouble(linedata[2]);
                    tabrow["SolarRadMean_MJm2day"] = Math.Round((srad * dayl) / 1000000, 2);
                    tabrow["VaPress"] = linedata[6];
                    tab.Rows.Add(tabrow);
                }
            }
            return tab;
        }
    }
}
