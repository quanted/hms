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

namespace Evapotranspiration
{
    public class Hamon
    {
        private double latitude;
        private double longitude;

        private int icounter = 0;
        private double mean1 = 0.0;

        public Hamon()
        {
            latitude = 33.925673;
            longitude = -83.355723;
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

        public void HamonMethod(double mean, int jday, out double petHamon, out double dayLightHours, out string errorMsg)
        {
            petHamon = 0;
            dayLightHours = 0;
            errorMsg = "";

            double deg = latitude;  //  latitude is input from the GUI.

            double pi, esat, sunhours, number, angle, ACOS;
            double declination, sunsetangle;
            double pot_evap = 0.0;
            double MMEAN = 0.0;
            double JulianDay = Convert.ToDouble(jday);

            pi = Math.PI;
            const double IN_TO_CM = 2.54;


            try
            {
                number = (deg * pi / 180.0);

                if (Math.Abs(number) != 1)
                {
                    ACOS = (pi / 2.0) - Math.Atan(number / Math.Sqrt(1 - (number * number)));
                }


                if (number == -1.0) ACOS = pi;

                declination = (0.4093 * Math.Sin(2 * pi * JulianDay / 365.0 - 1.405));
                angle = (-Math.Tan(number) * Math.Tan(declination));
                sunsetangle = (pi / 2.0) - Math.Atan(angle / Math.Sqrt(1 - (angle * angle)));

                sunhours = 24.0 * sunsetangle / pi;
                dayLightHours = sunhours;
                esat = (0.6108 * Math.Exp(17.27 * mean / (237.3 + mean)));

                if ((icounter > 0) && (mean1 > 0.0) && (mean < 0.0)) //  Wilson Melendez: this part needs to be clarified with Yusuf.
                {
                    MMEAN = mean1;
                }
                //    else
                //  {
                //     mean = mean;
                //  } 

                icounter++;
                mean1 = mean;

                if (mean >= 0.0)
                    pot_evap = 0.21 * Math.Pow(sunhours, 2.0) * esat / (mean + 273.2);
                else
                    pot_evap = 0.21 * Math.Pow(sunhours, 2.0) * esat / (MMEAN + 273.2);

            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }

            // Convert cm/day to inches/day
            petHamon = pot_evap / IN_TO_CM;

        }

        public ITimeSeriesOutput Compute(ITimeSeriesInput inpt, ITimeSeriesOutput outpt, double lat, double lon, string startDate, string endDate, int timeZoneOffset, out string errorMsg)
        {
            errorMsg = "";
            double petHamon = 0;
            double sunshineHours = 0;
            
            DataTable dt = new DataTable();

            switch (inpt.Source)
            {
                case "ncdc":
                    NCDC ncd = new NCDC();
                    dt = ncd.DownloadData(outpt, inpt);
                    if(dt == null)
                    {
                        errorMsg = "ERROR: Unable to download data from Daymet. ";
                    }
                    else
                    {
                        dt = ncd.Aggregate(inpt, dt);
                    }
                    break;
                case "daymet":
                    dt = daymetData(inpt, outpt);
                    dt = Utilities.Utility.aggregateData(inpt, dt, "hamon");
                    break;
                case "custom":
                    CustomData cd = new CustomData();
                    dt = cd.ParseCustomData(inpt, outpt, inpt.Geometry.GeometryMetadata["userdata"].ToString(), "hamon");
                    break;
                case "nldas":
                case "gldas":
                default:
                    NLDAS2 nldas = new NLDAS2(inpt.Source, lat, lon, startDate, endDate);
                    if (inpt.TemporalResolution == "hourly")
                    {
                        dt = nldas.getDataHourly(timeZoneOffset, false, out errorMsg);
                        dt.Columns["THourly_C"].ColumnName = "TMean_C";
                        dt.Columns.Remove("SolarRad_MJm2day");
                        dt.Columns.Remove("SH_Hourly");
                        dt.Columns.Remove("WindSpeed_m/s");
                    }
                    else
                    {
                        dt = nldas.getData1(timeZoneOffset, out errorMsg);
                        DataRow dr1 = null;
                        List<Double> tList = new List<double>();
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
                                }
                                tList.Add(Convert.ToDouble(dt.Rows[i]["TMin_C"].ToString()));
                                tList.Add(Convert.ToDouble(dt.Rows[i]["TMax_C"].ToString()));
                                if (j == 6 || i == dt.Rows.Count - 1)
                                {
                                    dr1["TMin_C"] = tList.Min().ToString("F2", CultureInfo.InvariantCulture);
                                    dr1["TMax_C"] = tList.Max().ToString("F2", CultureInfo.InvariantCulture);
                                    dr1["TMean_C"] = (tList.Min() + tList.Max()) / 2.0;
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
                                    newmonth = false;
                                    curmonth = Convert.ToDateTime(dt.Rows[i]["Date"]).Month;
                                }
                                tList.Add(Convert.ToDouble(dt.Rows[i]["TMin_C"].ToString()));
                                tList.Add(Convert.ToDouble(dt.Rows[i]["TMax_C"].ToString()));
                                if (i + 1 < dt.Rows.Count && (Convert.ToDateTime(dt.Rows[i + 1]["Date"]).Month != curmonth) || i == dt.Rows.Count - 1)
                                {
                                    dr1["TMin_C"] = tList.Min().ToString("F2", CultureInfo.InvariantCulture);
                                    dr1["TMax_C"] = tList.Max().ToString("F2", CultureInfo.InvariantCulture);
                                    dr1["TMean_C"] = (tList.Min() + tList.Max()) / 2.0;
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

            dt.Columns.Add("Sunshine_Hours");
            dt.Columns.Add("HamonPET_in");

            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput output = oFactory.Initialize();
            output.Dataset = "Evapotranspiration";
            output.DataSource = "hamon";
            output.Metadata = new Dictionary<string, string>()
            {
                { "latitude", latitude.ToString() },
                { "longitude", longitude.ToString() },
                { "request_time", DateTime.Now.ToString() },
                { "column_1", "Date" },
                { "column_2", "Julian Day" },
                { "column_3", "Minimum Temperature" },
                { "column_4", "Maximum Temperature" },
                { "column_5", "Mean Temperature" },
                { "column_6", "Sunshine Hours" },
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
                    { "column_4", "Sunshine Hours" },
                    { "column_5", "Potential Evapotranspiration" }
                };
            }

            output.Data = new Dictionary<string, List<string>>();

            foreach (DataRow dr in dt.Rows)
            {
                double tmean = Convert.ToDouble(dr["TMean_C"].ToString());
                int jday = Convert.ToInt32(dr["Julian_Day"]);
                HamonMethod(tmean, jday, out petHamon, out sunshineHours, out errorMsg);
                dr["Sunshine_Hours"] = sunshineHours.ToString("F2", CultureInfo.InvariantCulture);
                dr["HamonPET_in"] = petHamon.ToString("F4", CultureInfo.InvariantCulture);
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
            //string years = inpt.DateTimeSpan.StartDate.Year.ToString();
            StringBuilder st = new StringBuilder();
            int yearDif = (inpt.DateTimeSpan.EndDate.Year - inpt.DateTimeSpan.StartDate.Year);
            for (int i = 0; i <= yearDif; i++)
            {
                string year = inpt.DateTimeSpan.StartDate.AddYears(i).Year.ToString();
                st.Append(year + ",");
            }
            st.Remove(st.Length - 1, 1);

            //years = inpt.DateTimeSpan.StartDate.Year.ToString() + "," + inpt.DateTimeSpan.EndDate.Year.ToString();//need to find every year between
            string url = "https://daymet.ornl.gov/data/send/saveData?" + "lat=" + inpt.Geometry.Point.Latitude + "&lon=" + inpt.Geometry.Point.Longitude
                + "&measuredParams=" + "tmax,tmin" + "&years=" + st.ToString();
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

            string[] splitData = data.Split(new string[] { "year,yday,prcp (mm/day)", "year,yday,tmax (deg c),tmin (deg c)" }, StringSplitOptions.RemoveEmptyEntries);
            string[] lines = splitData[1].Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            Boolean julianflag = false;
            foreach (string line in lines)//for (int i = inpt.DateTimeSpan.StartDate.DayOfYear-1; i < inpt.DateTimeSpan.EndDate.DayOfYear; i++)
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
                    tabrow["TMin_C"] = linedata[3];
                    tabrow["TMax_C"] = linedata[2];
                    tabrow["TMean_C"] = (Convert.ToDouble(linedata[3]) + Convert.ToDouble(linedata[2])) / 2.0;
                    tab.Rows.Add(tabrow);
                }
            }
            return tab;
        }
    }
}