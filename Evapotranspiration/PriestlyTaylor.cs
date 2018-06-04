using Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Precipitation;

namespace Evapotranspiration
{
    public class PriestleyTaylor
    {
        private double latitude;
        private double longitude;
        private double elevation;
        private double albedo;

        public PriestleyTaylor()
        {
            latitude = 33.925673;
            longitude = -83.355723;
            elevation = 213.36;
            albedo = 0.23;
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

        public void PriestlyTaylorMethod(double tmin, double tmax, double tmean, double solarRad, int jday,
                                         out double petPT, out string errorMsg)
        {

            double EL = elevation;
            double lat = latitude;

            // CONSTANTS
            const double A = 17.27;
            const double B = 237.3;
            const double e = 0.622;
            const double LW = 2.45;
            const double pi = Math.PI;

            int G = 0;

            // Specific heat at constant pressure[MJ kg-1 deg C-1
            double CP = 1.013 * Math.Pow(10, -3);

            double ALBEDO = albedo;

            // extracted from input data
            double TMIN = tmin;   // celsisus
            double TMAX = tmax;   // celsisus
            double jd1 = (double)jday;

            // EVAPO TRANSPIRATION
            petPT = 0;
            errorMsg = "";

            // CALCULATED VALUES
            // main calculated parameters used in the final evapo transpiration formulae
            double T = 0.0;
            double RA = 0.0;

            // other calculated parameters
            double es1 = 0.0;
            double es2 = 0.0;
            double es = 0.0;
            double slope = 0.0;
            double P = 0.0;     //pressure
            double gama = 0.0;    //psychometric const
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
            double ENERG_ET = 0.0;
            double NN = 0.0;
            double latent = 0.0;
            double PRT = 0.0;


            try
            {
                TMIN = tmin;
                TMAX = tmax;
                T = (TMIN + TMAX) / 2.0;

                es1 = 0.6108 * Math.Exp((A * TMIN) / (B + TMIN));
                es2 = 0.6108 * Math.Exp((A * TMAX) / (B + TMAX));

                es = (es1 + es2) / 2.0;



                // slope of saturation vapour pressure curve
                //          slope = es*A*B/Math.pow((T+B),2.0);
                slope = (4098 * 0.6108 * Math.Exp((17.27 * T) / (237.3 + T))) / Math.Pow((T + 237.3), 2.0);

                P = 101.3 * Math.Pow(((293.0 - 0.0065 * EL) / 293.0), 5.26);

                // psychometric constant
                gama = (CP * P) / (LW * e);


                // ############# ENERGY EQUATIONS ########################################

                /* CALCULATION OF EXTRATERRESTIAL RADIATION FOR DAILY PERIOIDS
                 ---- THIS PORTION OF THE PROGRAM CALCULATES THE ENERGY COMPONENT  */

                radian = (pi / 180.0) * lat;
                dr = 1.0 + 0.033 * Math.Cos(2 * (pi / 365.0) * jd1);
                sigma2 = 0.409 * Math.Sin((2.0 * pi * jd1 / 365.0) - 1.39);
                xx = 1.0 - (Math.Pow(Math.Tan(radian), 2.0) * Math.Pow(Math.Tan(sigma2), 2.0));

                if (xx <= 0) xx = 0.00001;

                ws = (pi / 2.0) - (Math.Atan(-Math.Tan(radian) * (Math.Tan(sigma2) / Math.Pow(xx, 0.5))));

                RA = (24.0 * 60.0 / pi) * 0.082 * dr * (ws * Math.Sin(radian) * Math.Sin(sigma2) +
                      Math.Cos(radian) * Math.Cos(sigma2) * Math.Sin(ws));

                // CALCULATION OF SOLAR OR SHORT WAVE RADIATION

                NN = ws * (24 / pi);

                RS = solarRad;  // Solar radiation's units are in microJ/(m^2 day). Conversion from W/m^2 to MicroJ/(m^2 day) was done
                                // in NLDAS2.

                RSO = (0.75 + 2.0 * (EL / 100000.0)) * RA;
                RR = RS / RSO;
                RNS = (1 - ALBEDO) * RS;

                tmaxk = 4.903 * Math.Pow(10, -9.0) * Math.Pow(TMAX + 273.2, 4.0);
                tmink = 4.903 * Math.Pow(10, -9.0) * Math.Pow(TMIN + 273.2, 4.0);

                AVGTK = (tmaxk + tmink) / 2.0;
                EB = 0.34 - 0.14 * Math.Pow(es, 0.5);
                RB = 1.35 * RR - 0.35;
                RNL = AVGTK * EB * RB;
                Rnet = RNS - RNL;



                // converted into depth of water
                ENERG_ET = 0.408 * Rnet;
                latent = 2.45;   //2.501 - 0.0023 * T;
                PRT = slope / (slope + gama);

                petPT = 1.26 * ((PRT * (Rnet - G) / latent));  // mm/day

            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }

            // Convert mm/day to inches per day
            petPT = petPT / 25.4;

        }

        public ITimeSeriesOutput Compute(ITimeSeriesInput inpt, ITimeSeriesOutput outpt, double lat, double lon, string startDate, string endDate, int timeZoneOffset, out string errorMsg)
        {
            errorMsg = "";
            double petPT = 0;

            //NLDAS2 nldas = new NLDAS2(inpt.Source, lat, lon, startDate, endDate);
                        
            DataTable dt = new DataTable();

            switch (inpt.Source)
            {
               case "daymet":
                    dt = daymetData(inpt, outpt);
                    break;
                case "custom":
                    CustomData cd = new CustomData();
                    dt = cd.ParseCustomData(inpt, outpt, inpt.Geometry.GeometryMetadata["userdata"].ToString(), "priestlytaylor");
                    break;
                case "nldas":
                case "gldas":
                default:
                    NLDAS2 nldas = new NLDAS2(inpt.Source, lat, lon, startDate, endDate);
                    if (inpt.TemporalResolution == "hourly")
                    {
                        NLDAS2 nldasday = new NLDAS2(inpt.Source, lat, lon, startDate, endDate);
                        DataTable dtd = nldasday.getData2(timeZoneOffset, out errorMsg);
                        dt = nldas.getDataHourly(timeZoneOffset, false, out errorMsg);
                        dt.Columns["THourly_C"].ColumnName = "TMean_C";
                        dt.Columns["SolarRad_MJm2day"].ColumnName = "SolarRadMean_MJm2day";
                        dt.Columns.Remove("SH_Hourly");
                        dt.Columns.Remove("WindSpeed_m/s");
                        dt.Columns.Add("TMin_C");
                        dt.Columns.Add("TMax_C");
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
                        }
                        dtd = null;
                    }
                    else
                    {
                        dt = nldas.getData2(timeZoneOffset, out errorMsg);
                    }
                    break;
            }
            
            //dt = nldas.getData2(timeZoneOffset, out errorMsg);

            if (errorMsg != "")
            {
                Utilities.ErrorOutput err = new Utilities.ErrorOutput();
                return err.ReturnError(errorMsg);
            }

            dt.Columns.Add("PriestleyTaylorPET_in");

            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput output = oFactory.Initialize();
            output.Dataset = "Evapotranspiration";
            output.DataSource = "priestlytaylor";
            output.Metadata = new Dictionary<string, string>()
            {
                { "elevation", elevation.ToString() },
                { "latitude", latitude.ToString() },
                { "longitude", longitude.ToString() },
                { "albedo", albedo.ToString() },
                { "request_time", DateTime.Now.ToString() },
                { "column_1", "Date" },
                { "column_2", "Julian Day" },
                { "column_3", "Minimum Temperature" },
                { "column_4", "Maximum Temperature" },
                { "column_5", "Mean Temperature" },
                { "column_6", "Mean Solar Radiation" },
                { "column_7", "Potential Evapotranspiration" }
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
                    { "column_7", "Potential Evapotranspiration" }
                };
            }
            output.Data = new Dictionary<string, List<string>>();

            foreach (DataRow dr in dt.Rows)
            {
                double tmean = Convert.ToDouble(dr["TMean_C"].ToString());
                double tmin = Convert.ToDouble(dr["TMin_C"].ToString());
                double tmax = Convert.ToDouble(dr["TMax_C"].ToString());
                double solarRad = Convert.ToDouble(dr["SolarRadMean_MJm2day"].ToString());
                int jday = Convert.ToInt32(dr["Julian_Day"].ToString());
                PriestlyTaylorMethod(tmin, tmax, tmean, solarRad, jday, out petPT, out errorMsg);
                dr["PriestleyTaylorPET_in"] = petPT.ToString("F4", CultureInfo.InvariantCulture);
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

        public DataTable priestGLDASData(ITimeSeriesOutput outpt, ITimeSeriesInput inpt)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Date");
            dt.Columns.Add("Hour");
            dt.Columns.Add("Value");
            foreach (KeyValuePair<string, List<string>> entry in outpt.Data)
            {
                DataRow dr = dt.NewRow(); 
                string[] datetime = entry.Key.Split(' ');
                dr["Date"] = datetime[0];
                dr["Hour"] = datetime[1];
                dr["Value"] = Convert.ToDouble(entry.Value[0]) - 273.15;
                dt.Rows.Add(dr);
            }
            if (inpt.Geometry.Timezone.Offset == 0) //No need to adjust time series for time zone offset
            {
                return dt;
            }
            for (int i = 0; i < inpt.Geometry.Timezone.Offset; i++)
            {
                dt.Rows[0].Delete();
            }
            string date = "";
            DataTable dtMinMax = new DataTable();
            int remainder = 0;
            dtMinMax.Columns.Add("Date");
            dtMinMax.Columns.Add("Julian_Day");
            dtMinMax.Columns.Add("TMin_C");
            dtMinMax.Columns.Add("TMax_C");
            dtMinMax.Columns.Add("TMean_C");

            DataRow dr1 = null;
            double meanTemp = 0;
            DateTime startDate = inpt.DateTimeSpan.StartDate;
            int startYear = startDate.Year - 1;
            double elapsedJulianDate = Convert.ToDateTime(startYear.ToString() + "-12-31").ToOADate();
            int julianDayOfYear = 0;
            List<Double> list = new List<double>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Math.DivRem(i, 8, out remainder);
                if (remainder == 0)
                {
                    date = dt.Rows[i]["Date"].ToString();
                    dr1 = dtMinMax.NewRow();
                    list = new List<double>();

                }
                dt.Rows[i]["Date"] = date;
                dt.Rows[i]["Hour"] = remainder;
                list.Add(Convert.ToDouble(dt.Rows[i]["Value"].ToString()));

                if (remainder == 7)
                {
                    dr1["Date"] = date;
                    dr1["TMin_C"] = list.Min().ToString("F2", CultureInfo.InvariantCulture);
                    dr1["TMax_C"] = list.Max().ToString("F2", CultureInfo.InvariantCulture);
                    meanTemp = (list.Min() + list.Max()) / 2.0;
                    dr1["TMean_C"] = meanTemp.ToString("F2", CultureInfo.InvariantCulture);

                    startDate = Convert.ToDateTime(date);
                    startYear = startDate.Year - 1;
                    elapsedJulianDate = Convert.ToDateTime(startYear.ToString() + "-12-31").ToOADate();
                    julianDayOfYear = Convert.ToInt32(Convert.ToDateTime(date).ToOADate() - elapsedJulianDate);
                    dr1["Julian_Day"] = julianDayOfYear;
                    dtMinMax.Rows.Add(dr1);
                }
            }

            //Finding Solar Radiation
            dtMinMax.Columns.Add("SolarRadMean_MJm2day");
            remainder = 0;

            double meanSR = 0;
            DataRow dr2 = null;
            List<Double> listSR = new List<double>();
            int rowindx = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Math.DivRem(i, 8, out remainder);
                if (remainder == 0)
                {
                    dr2 = dtMinMax.NewRow();
                    listSR = new List<double>();
                }

                listSR.Add(Convert.ToDouble(dt.Rows[i]["Value"].ToString()));

                if (remainder == 7)
                {
                    meanSR = listSR.Average();
                    meanSR = meanSR * 0.0864;  // Convert units from W/s to MJ/(m^2 day)
                    dr2["SolarRadMean_MJm2day"] = meanSR.ToString("F2", CultureInfo.InvariantCulture);
                    dtMinMax.Rows[rowindx++]["SolarRadMean_MJm2day"] = dr2["SolarRadMean_MJm2day"];
                }
            }
            return dtMinMax;
        }
    }
}
