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
    public class Hargreaves
    {
        private double latitude;
        private double longitude;

        private int icounter = 0;
        private double mean1 = 0.0;

        public Hargreaves()
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

        public void HargreavesMethod(double tmax, double tmin, double tmean, double rad, int jday, out double petHargreaves, out string errorMsg)
        {
            petHargreaves = 0.0;
            errorMsg = "";

            double deg = latitude;  //  latitude is input from the GUI.

            double TR = tmax - tmin;
            double JulianDay = Convert.ToDouble(jday);

            try
            {
                TR = tmax - tmin;
                petHargreaves = 0.0023 * rad * (tmean + 17.8) * Math.Pow(TR, 0.5);
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
        }

        public ITimeSeriesOutput Compute(ITimeSeriesInput inpt, ITimeSeriesOutput outpt, double lat, double lon, string startDate, string endDate, int timeZoneOffset, out string errorMsg)
        {
            errorMsg = "";

            DataTable dt = new DataTable();
            List<ITimeSeriesOutput> outputList = new List<ITimeSeriesOutput>();

            switch (inpt.Source)
            {
                case "daymet":
                    ITimeSeriesOutput daymetFinal = getDaymetData(out errorMsg, inpt);
                    return daymetFinal;
                case "gldas":
                    ITimeSeriesOutput final = getGldasData(out errorMsg, inpt);
                    return final;
                case "nldas":
                default:
                    ITimeSeriesOutput nldasFinal = getNldasData(out errorMsg, inpt);
                    return nldasFinal;
            }
        }

        public ITimeSeriesOutput getDaymetData(out string errorMsg, ITimeSeriesInput inpt)
        {
            errorMsg = "";
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
                + "&measuredParams=" + "tmax,tmin,srad" + "&years=" + st.ToString();
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
            //tab.Columns.Add("Julian_Day");
            tab.Columns.Add("TMin_C");
            tab.Columns.Add("TMax_C");
            tab.Columns.Add("TMean_C");
            tab.Columns.Add("Radiation");

            string[] splitData = data.Split(new string[] { "year,yday,srad (W/m^2),tmax (deg c),tmin (deg c)" }, StringSplitOptions.RemoveEmptyEntries);
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
                    //tabrow["Julian_Day"] = Convert.ToInt32(Convert.ToDouble(linedata[1]));
                    tabrow["TMin_C"] = linedata[4];
                    tabrow["TMax_C"] = linedata[3];
                    tabrow["TMean_C"] = (Convert.ToDouble(linedata[4]) + Convert.ToDouble(linedata[3])) / 2.0;
                    tabrow["Radiation"] = Convert.ToDouble(linedata[2]) * 0.0864;// * 0.408;//Convert W/M^2 to mm/day
                    tab.Rows.Add(tabrow);
                }
            }

            tab = Utilities.Utility.aggregateData(inpt, tab, "hamon");
            tab.Columns.Add("HargreavesPET_in");

            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput output = oFactory.Initialize();
            output.Dataset = "Evapotranspiration";
            output.DataSource = "hargreaves";
            output.Metadata = new Dictionary<string, string>()
            {
                { "latitude", latitude.ToString() },
                { "longitude", longitude.ToString() },
                { "request_time", DateTime.Now.ToString() },
                { "column_1", "Date" },
                { "column_2", "Minimum Temperature" },
                { "column_3", "Maximum Temperature" },
                { "column_4", "Mean Temperature" },
                { "column_5", "Sunshine Hours" },
                { "column_6", "Potential Evapotranspiration" }
            };

            output.Data = new Dictionary<string, List<string>>();

            int jday = 0;
            foreach (DataRow dr in tab.Rows)
            {
                double petHargreaves = 0.0;
                double tmean = Convert.ToDouble(dr["TMean_C"].ToString());
                double tmin = Convert.ToDouble(dr["TMin_C"].ToString());
                double tmax = Convert.ToDouble(dr["TMax_C"].ToString());
                double rad = Convert.ToDouble(dr["Radiation"].ToString());
                //int jday = Convert.ToInt32(dr["Julian_Day"]);
                HargreavesMethod(tmax, tmin, tmean, rad, ++jday, out petHargreaves, out errorMsg);
                dr["HargreavesPET_in"] = petHargreaves.ToString("F4", CultureInfo.InvariantCulture);
                List<string> lv = new List<string>();
                foreach (Object g in dr.ItemArray.Skip(1))
                {
                    lv.Add(g.ToString());
                }
                output.Data.Add(dr[0].ToString(), lv);
            }
            return output;
        }

        public ITimeSeriesOutput getNldasData(out string errorMsg, ITimeSeriesInput inpt)
        {
            Temperature.NLDAS nldasTemp = new Temperature.NLDAS();
            ITimeSeriesOutputFactory ntFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput nTempOutput = ntFactory.Initialize();
            ITimeSeriesInputFactory ntiFactory = new TimeSeriesInputFactory();
            ITimeSeriesInput ntiInput = ntiFactory.SetTimeSeriesInput(inpt, new List<string>() { "temperature" }, out errorMsg);
            ITimeSeriesOutput nldasTempOutput = nldasTemp.GetData(out errorMsg, nTempOutput, ntiInput);

            Radiation.NLDAS nldasRad = new Radiation.NLDAS();
            ITimeSeriesOutputFactory nrFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput nRadOutput = nrFactory.Initialize();
            ITimeSeriesInputFactory nriFactory = new TimeSeriesInputFactory();
            ITimeSeriesInput nriInput = nriFactory.SetTimeSeriesInput(inpt, new List<string>() { "radiation" }, out errorMsg);
            ITimeSeriesOutput nldasRadOutput = nldasRad.GetData(out errorMsg, nRadOutput, nriInput);

            if (errorMsg != "")
            {
                Utilities.ErrorOutput err = new Utilities.ErrorOutput();
                return err.ReturnError(errorMsg);
            }

            nldasTempOutput = Utilities.Merger.MergeTimeSeries(nldasTempOutput, nldasRadOutput);
            if (nldasRadOutput.Metadata.Values.Contains("ERROR"))
            {
                nldasTempOutput.Metadata.Add(nldasRadOutput.DataSource.ToString() + " ERROR", "The service is unavailable or returned no valid data.");
            }

            int julian = 0;
            nldasTempOutput.Data.Remove("Total Average");
            nldasTempOutput.Data.Remove("Min Temp");
            nldasTempOutput.Data.Remove("Max Temp");
            foreach (KeyValuePair<string, List<string>> timeseries in nldasTempOutput.Data)
            {
                timeseries.Value[0] = (Convert.ToDouble(timeseries.Value[0]) - 273.15).ToString("F2", CultureInfo.InstalledUICulture);
                timeseries.Value[1] = (Convert.ToDouble(timeseries.Value[1]) - 273.15).ToString("F2", CultureInfo.InstalledUICulture);
                timeseries.Value[2] = (Convert.ToDouble(timeseries.Value[2]) - 273.15).ToString("F2", CultureInfo.InstalledUICulture);
                double tmin = Convert.ToDouble(timeseries.Value[1]);
                double tmax = Convert.ToDouble(timeseries.Value[0]);
                double tmean = Convert.ToDouble(timeseries.Value[2]);
                double rad = ((Convert.ToDouble(timeseries.Value[3]) + Convert.ToDouble(timeseries.Value[4])) / 2) * 0.0864;
                double petHargreaves = 0.0;
                int jday = ++julian;

                HargreavesMethod(tmax, tmin, tmean, rad, jday, out petHargreaves, out errorMsg);

                //Setting order of all items
                //timeseries.Value[0] = jday.ToString();
                timeseries.Value[0] = tmin.ToString("F2", CultureInfo.InstalledUICulture);
                timeseries.Value[1] = tmax.ToString("F2", CultureInfo.InstalledUICulture);
                timeseries.Value[2] = tmean.ToString("F2", CultureInfo.InstalledUICulture);
                timeseries.Value[3] = rad.ToString("F2", CultureInfo.InstalledUICulture);
                timeseries.Value[4] = petHargreaves.ToString("F4", CultureInfo.InvariantCulture);//timeseries.Value.Add(petHargreaves.ToString("F4", CultureInfo.InvariantCulture));
            }
            nldasTempOutput.Dataset = "Evapotranspiration";
            nldasTempOutput.DataSource = "hargreaves";
            nldasTempOutput.Metadata = new Dictionary<string, string>()
            {
                { "latitude", latitude.ToString() },
                { "longitude", longitude.ToString() },
                { "request_time", DateTime.Now.ToString() },
                { "column_1", "Date" },
                { "column_2", "Minimum Temperature" },
                { "column_3", "Maximum Temperature" },
                { "column_4", "Mean Temperature" },
                { "column_5", "Radiation" },
                { "column_6", "Potential Evapotranspiration" }
            };
            return nldasTempOutput;
        }

        public ITimeSeriesOutput getGldasData(out string errorMsg, ITimeSeriesInput inpt)
        {
            Temperature.GLDAS gldasTemp = new Temperature.GLDAS();
            ITimeSeriesOutputFactory gtFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput gTempOutput = gtFactory.Initialize();
            ITimeSeriesInputFactory gtiFactory = new TimeSeriesInputFactory();
            ITimeSeriesInput gtiInput = gtiFactory.SetTimeSeriesInput(inpt, new List<string>() { "temperature" }, out errorMsg);
            gtiInput.Geometry.GeometryMetadata.Add("ETGLDAS", ".");
            ITimeSeriesOutput gldasTempOutput = gldasTemp.GetData(out errorMsg, gTempOutput, gtiInput);

            Radiation.GLDAS gldasRad = new Radiation.GLDAS();
            ITimeSeriesOutputFactory grFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput gRadOutput = grFactory.Initialize();
            ITimeSeriesInputFactory griFactory = new TimeSeriesInputFactory();
            ITimeSeriesInput griInput = griFactory.SetTimeSeriesInput(inpt, new List<string>() { "radiation" }, out errorMsg);
            ITimeSeriesOutput gldasRadOutput = gldasRad.GetData(out errorMsg, gRadOutput, griInput);


            if (errorMsg != "")
            {
                Utilities.ErrorOutput err = new Utilities.ErrorOutput();
                return err.ReturnError(errorMsg);
            }

            gldasTempOutput = Utilities.Merger.MergeTimeSeries(gldasTempOutput, gldasRadOutput);
            if (gldasRadOutput.Metadata.Values.Contains("ERROR"))
            {
                gldasTempOutput.Metadata.Add(gldasRadOutput.DataSource.ToString() + " ERROR", "The service is unavailable or returned no valid data.");
            }

            int julianday = 0;
            gldasTempOutput.Data.Remove("Total Average");
            gldasTempOutput.Data.Remove("Min Temp");
            gldasTempOutput.Data.Remove("Max Temp");
            foreach (KeyValuePair<string, List<string>> timeseries in gldasTempOutput.Data)
            {
                timeseries.Value[0] = (Convert.ToDouble(timeseries.Value[0]) - 273.15).ToString("F2", CultureInfo.InstalledUICulture);
                timeseries.Value[1] = (Convert.ToDouble(timeseries.Value[1]) - 273.15).ToString("F2", CultureInfo.InstalledUICulture);
                timeseries.Value[2] = (Convert.ToDouble(timeseries.Value[2]) - 273.15).ToString("F2", CultureInfo.InstalledUICulture);
                double tmin = Convert.ToDouble(timeseries.Value[1]);
                double tmax = Convert.ToDouble(timeseries.Value[0]);
                double tmean = Convert.ToDouble(timeseries.Value[2]);
                double rad = ((Convert.ToDouble(timeseries.Value[3]) + Convert.ToDouble(timeseries.Value[4])) / 2) * 0.0864;
                double petHargreaves = 0.0;
                int jday = ++julianday;

                HargreavesMethod(tmax, tmin, tmean, rad, jday, out petHargreaves, out errorMsg);

                //Setting order of all items
                //timeseries.Value[0] = jday.ToString();
                timeseries.Value[0] = tmin.ToString("F2", CultureInfo.InstalledUICulture);
                timeseries.Value[1] = tmax.ToString("F2", CultureInfo.InstalledUICulture);
                timeseries.Value[2] = tmean.ToString("F2", CultureInfo.InstalledUICulture);
                timeseries.Value[3] = rad.ToString("F2", CultureInfo.InstalledUICulture);
                timeseries.Value[4] = petHargreaves.ToString("F4", CultureInfo.InvariantCulture);//timeseries.Value.Add(petHargreaves.ToString("F4", CultureInfo.InvariantCulture));
            }
            gldasTempOutput.Dataset = "Evapotranspiration";
            gldasTempOutput.DataSource = "hargreaves";
            gldasTempOutput.Metadata = new Dictionary<string, string>()
                    {
                        { "latitude", latitude.ToString() },
                        { "longitude", longitude.ToString() },
                        { "request_time", DateTime.Now.ToString() },
                        { "column_1", "Date" },
                        { "column_2", "Minimum Temperature" },
                        { "column_3", "Maximum Temperature" },
                        { "column_4", "Mean Temperature" },
                        { "column_5", "Radiation" },
                        { "column_6", "Potential Evapotranspiration" }
                    };
            return gldasTempOutput;
        }        
    }
}