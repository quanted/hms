using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Globalization;
using Data;
using System.Net;
using System.IO;
using System.Threading;

namespace Evapotranspiration
{
    public class PenmanDaily
    {
        private double latitude;
        private double longitude;
        private double elevation;
        private double albedo;
        public Boolean prismCalc = false;

        public PenmanDaily()
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

        public void PenmanDailyMethod(double tmin, double tmax, double tmean, int jday, double shmin, double shmax,
                                      double wind, double solarRad, out double relHumidityMin, out double relHumidityMax,
                                      double press, out double petPMD, out string errorMsg)
        {
            double EL = elevation;
            double lat = latitude;
            double ALBEDO = albedo;
            errorMsg = "";
            relHumidityMin = 0.0;
            relHumidityMax = 0.0;

            double PR = 101.3;  // pressure kilo pascals

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
            double ENERG_ET = 0.0;
            double NN = 0.0;
            double denom = 0.0;
            double nenom = 0.0;
            double part1 = 0.0;
            double PET1 = 0.0;
            double PET2 = 0.0;
            double u2 = 0.0;

            try
            {
                TMIN = tmin;
                TMAX = tmax;

                if (!prismCalc)
                {
                    // Convert specific humidity to relative humidity here.  
                    RHmin = Utilities.Utility.CalculateRH(shmin, tmin, press);
                    RHmax = Utilities.Utility.CalculateRH(shmax, tmax, press);
                }
                else
                {
                    RHmin = shmin;
                    RHmax = shmax;
                }

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

                es1 = 0.6108 * Math.Exp((A * TMIN) / (B + TMIN));
                es2 = 0.6108 * Math.Exp((A * TMAX) / (B + TMAX));
                es = (es1 + es2) / 2.0;

                //Yusuf:  slope of saturation vapour pressure curve
                slope = (4098 * 0.6108 * Math.Exp((17.27 * T) / (237.3 + T))) / Math.Pow((T + 237.3), 2.0);

                // Actual water vapor pressure in the atmosphere
                ea = (es2 * (RHmin / 100.0) + (es1 * RHmax / 100.0)) / 2.0;
                deficit = es - ea;

                P = PR * Math.Pow(((293.0 - 0.0065 * EL) / 293.0), 5.26);

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

                ws = (pi / 2.0) - (Math.Atan(-Math.Tan(radian) * (Math.Tan(sigma2) / Math.Pow(xx, 0.5))));

                RA = (24.0 * 60.0 / pi) * 0.082 * dr * (ws * Math.Sin(radian) * Math.Sin(sigma2) +
                      Math.Cos(radian) * Math.Cos(sigma2) * Math.Sin(ws));

                NN = (24.0 / pi) * ws;

                RS = solarRad;      // Units are in MJ/(m^2 day) 
                RSO = (0.75 + 2.0 * (EL / 100000.0)) * RA;
                RR = RS / RSO;
                RNS = (1 - ALBEDO) * RS;

                tmaxk = 4.903 * Math.Pow(10, -9.0) * Math.Pow(TMAX + 273.16, 4.0);
                tmink = 4.903 * Math.Pow(10, -9.0) * Math.Pow(TMIN + 273.16, 4.0);

                AVGTK = (tmaxk + tmink) / 2.0;

                EB = 0.34 - 0.14 * Math.Pow(ea, 0.5);
                RB = 1.35 * RR - 0.35;
                RNL = AVGTK * EB * RB;
                Rnet = RNS - RNL;

                // converted into depth of water
                ENERG_ET = 0.408 * Rnet;

                // COMBINATION OF AREODYNAMIC AND ENERGY COMPONENTS

                denom = slope / (slope + gama * (1 + 0.34 * u));
                nenom = gama / (slope + gama * (1 + 0.34 * u));

                part1 = 900.0 / (273 + T) * u;

                PET1 = 0.408 * Rnet * denom;
                PET2 = part1 * deficit * nenom;
                PET = PET1 + PET2;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }


            // Convert mm/day to inches/day
            petPMD = (PET / 25.4);

        }

        public ITimeSeriesOutput Compute(ITimeSeriesInput inpt, ITimeSeriesOutput outpt, double lat, double lon, string startDate, string endDate, int timeZoneOffset, out string errorMsg)
        {
            errorMsg = "";
            //NLDAS2 nldas = new NLDAS2(inpt.Source, lat, lon, startDate, endDate);
            double relHMax = 0.0;
            double relHMin = 0.0;
            double petPMD = 0.0;

            //DataTable dt = nldas.getData4(timeZoneOffset, out errorMsg);
            DataTable dt = new DataTable();
            DataTable daymets = new DataTable();
            List<ITimeSeriesOutput> outputList = new List<ITimeSeriesOutput>();
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
                    dt = cd.ParseCustomData(inpt, outpt, inpt.Geometry.GeometryMetadata["userdata"].ToString(), "penmandaily");
                    break;
                case "gldas":
                    ITimeSeriesOutput final = getGldasData(out errorMsg, inpt);
                    return final;
                case "nldas":
                default:
                    ITimeSeriesOutput nldasFinal = getNldasData(out errorMsg, inpt);
                    return nldasFinal;
            }

            if (errorMsg != "")
            {
                Utilities.ErrorOutput err = new Utilities.ErrorOutput();
                return err.ReturnError(errorMsg);
            }

            dt.Columns.Add("RHmin");
            dt.Columns.Add("RHmax");
            dt.Columns.Add("PenmanDailyPET_in");

            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput output = oFactory.Initialize();
            output.Dataset = "Evapotranspiration";
            output.DataSource = "penmandaily";
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
                { "column_7", "Mean Wind Speed" },
                { "column_8", "Minimum Relative Humidity" },
                { "column_9", "Maximum Relative Humidity" },
                { "column_10", "Potential Evapotranspiration" }
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
                    { "column_7", "Mean Wind Speed" },
                    { "column_8", "Minimum Relative Humidity" },
                    { "column_9", "Maximum Relative Humidity" },
                    { "column_10", "Potential Evapotranspiration" }
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
                double wind = Convert.ToDouble(dr["WindSpeedMean_m/s"].ToString());
                double solarRad = Convert.ToDouble(dr["SolarRadMean_MJm2day"].ToString());
                int jday = Convert.ToInt32(dr["Julian_Day"].ToString());

                PenmanDailyMethod(tmin, tmax, tmean, jday, shmin, shmax, wind, solarRad, out relHMin, out relHMax, 1013.25,
                                  out petPMD, out errorMsg);

                dr["RHmin"] = relHMin.ToString("F2", CultureInfo.InstalledUICulture);
                dr["RHmax"] = relHMax.ToString("F2", CultureInfo.InstalledUICulture);
                dr["PenmanDailyPET_in"] = petPMD.ToString("F4", CultureInfo.InvariantCulture);
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

        public ITimeSeriesOutput getNldasData(out string errorMsg, ITimeSeriesInput inpt)
        {
            List<ITimeSeriesOutput> outputList = new List<ITimeSeriesOutput>();

            Temperature.NLDAS nldasTemp = new Temperature.NLDAS();
            ITimeSeriesOutputFactory ntFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput nTempOutput = ntFactory.Initialize();
            ITimeSeriesInputFactory ntiFactory = new TimeSeriesInputFactory();
            ITimeSeriesInput ntiInput = ntiFactory.SetTimeSeriesInput(inpt, new List<string>() { "temperature" }, out errorMsg);
            ITimeSeriesOutput nldasTempOutput = nldasTemp.GetData(out errorMsg, nTempOutput, ntiInput);
            inpt.DateTimeSpan.StartDate = inpt.DateTimeSpan.StartDate.AddHours(-6.0);
            inpt.DateTimeSpan.EndDate = inpt.DateTimeSpan.EndDate.AddDays(-1);
            inpt.DateTimeSpan.EndDate = inpt.DateTimeSpan.EndDate.AddHours(-5);

            Wind.NLDAS nldasWind = new Wind.NLDAS();
            ITimeSeriesOutputFactory nwFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput nWindOutput = nwFactory.Initialize();
            ITimeSeriesInputFactory nwiFactory = new TimeSeriesInputFactory();
            ITimeSeriesInput nwiInput = nwiFactory.SetTimeSeriesInput(inpt, new List<string>() { "wind" }, out errorMsg);
            ITimeSeriesOutput nldasWindOutput = nldasWind.GetData(out errorMsg, "SPEED/DIRECTION", nWindOutput, nwiInput);
            outputList.Add(nldasWindOutput);
            inpt.DateTimeSpan.StartDate = inpt.DateTimeSpan.StartDate.AddHours(-6.0);
            inpt.DateTimeSpan.EndDate = inpt.DateTimeSpan.EndDate.AddDays(-1);
            inpt.DateTimeSpan.EndDate = inpt.DateTimeSpan.EndDate.AddHours(-5);

            Humidity.NLDAS nldasHumid = new Humidity.NLDAS();
            ITimeSeriesOutputFactory nhFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput nHumidOutput = nhFactory.Initialize();
            ITimeSeriesInputFactory nhiFactory = new TimeSeriesInputFactory();
            inpt.Source = "nldas";
            ITimeSeriesInput nhiInput = nhiFactory.SetTimeSeriesInput(inpt, new List<string>() { "humidity" }, out errorMsg);
            ITimeSeriesOutput nldasHumidOutput = nldasHumid.GetData(out errorMsg, nHumidOutput, nhiInput);
            outputList.Add(nldasHumidOutput);
            inpt.DateTimeSpan.StartDate = inpt.DateTimeSpan.StartDate.AddHours(-6.0);
            inpt.DateTimeSpan.EndDate = inpt.DateTimeSpan.EndDate.AddDays(-1);
            inpt.DateTimeSpan.EndDate = inpt.DateTimeSpan.EndDate.AddHours(-5);

            Radiation.NLDAS nldasRad = new Radiation.NLDAS();
            ITimeSeriesOutputFactory nrFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput nRadOutput = nrFactory.Initialize();
            ITimeSeriesInputFactory nriFactory = new TimeSeriesInputFactory();
            ITimeSeriesInput nriInput = nriFactory.SetTimeSeriesInput(inpt, new List<string>() { "radiation" }, out errorMsg);
            ITimeSeriesOutput nldasRadOutput = nldasRad.GetData(out errorMsg, nRadOutput, nriInput);
            outputList.Add(nldasRadOutput);
            inpt.DateTimeSpan.StartDate = inpt.DateTimeSpan.StartDate.AddHours(-6.0);
            inpt.DateTimeSpan.EndDate = inpt.DateTimeSpan.EndDate.AddDays(-1);
            inpt.DateTimeSpan.EndDate = inpt.DateTimeSpan.EndDate.AddHours(-5);

            Pressure.GLDAS gpress2 = new Pressure.GLDAS();
            ITimeSeriesOutputFactory gpFactory2 = new TimeSeriesOutputFactory();
            ITimeSeriesOutput gpressOutput2 = gpFactory2.Initialize();
            inpt.Source = "gldas";
            ITimeSeriesInputFactory gpiFactory2 = new TimeSeriesInputFactory();
            ITimeSeriesInput gpInput2 = gpiFactory2.SetTimeSeriesInput(inpt, new List<string>() { "surfacepressure" }, out errorMsg);
            ITimeSeriesOutput pressOutput2 = gpress2.GetData(out errorMsg, gpressOutput2, gpInput2);
            outputList.Add(pressOutput2);


            foreach (ITimeSeriesOutput result in outputList)
            {
                nldasTempOutput = Utilities.Merger.MergeTimeSeries(nldasTempOutput, result);
                if (result.Metadata.Values.Contains("ERROR"))
                {
                    nldasTempOutput.Metadata.Add(result.DataSource.ToString() + " ERROR", "The service is unavailable or returned no valid data.");
                }
            }

            inpt.Source = "nldas";
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
                double shmin = Convert.ToDouble(timeseries.Value[5]);
                double shmax = Convert.ToDouble(timeseries.Value[6]);
                double wind = Convert.ToDouble(timeseries.Value[3]);
                double solarRad = Convert.ToDouble(timeseries.Value[8]) * 0.0864;
                if(timeseries.Value.Count != 10)
                {
                    timeseries.Value.Add("101325");
                }
                double pressure  = Convert.ToDouble(timeseries.Value[9]) / 100; //Convert Pa to mbar
                double relHMax = 0.0;
                double relHMin = 0.0;
                double petPMD = 0.0;
                int jday = ++julian;

                PenmanDailyMethod(tmin, tmax, tmean, jday, shmin, shmax, wind, solarRad, out relHMin, out relHMax, pressure,
                                          out petPMD, out errorMsg);

                //1013.25
                //Setting order of all items
                /*timeseries.Value[0] = jday.ToString();
                timeseries.Value[1] = tmin.ToString("F2", CultureInfo.InstalledUICulture);
                timeseries.Value[2] = tmax.ToString("F2", CultureInfo.InstalledUICulture);
                timeseries.Value[3] = tmean.ToString("F2", CultureInfo.InstalledUICulture);
                timeseries.Value[4] = solarRad.ToString("F2", CultureInfo.InstalledUICulture);
                timeseries.Value[5] = wind.ToString("F2", CultureInfo.InstalledUICulture);
                timeseries.Value[6] = relHMin.ToString("F2", CultureInfo.InstalledUICulture);
                timeseries.Value[7] = relHMax.ToString("F2", CultureInfo.InstalledUICulture);
                timeseries.Value[8] = petPMD.ToString("F4", CultureInfo.InvariantCulture);*/
                timeseries.Value[0] = tmin.ToString("F2", CultureInfo.InstalledUICulture);
                timeseries.Value[1] = tmax.ToString("F2", CultureInfo.InstalledUICulture);
                timeseries.Value[2] = tmean.ToString("F2", CultureInfo.InstalledUICulture);
                timeseries.Value[3] = solarRad.ToString("F2", CultureInfo.InstalledUICulture);
                timeseries.Value[4] = wind.ToString("F2", CultureInfo.InstalledUICulture);
                timeseries.Value[5] = relHMin.ToString("F2", CultureInfo.InstalledUICulture);
                timeseries.Value[6] = relHMax.ToString("F2", CultureInfo.InstalledUICulture);
                timeseries.Value[7] = petPMD.ToString("F4", CultureInfo.InvariantCulture);
                timeseries.Value.RemoveAt(9);
                timeseries.Value.RemoveAt(8);
            }
            nldasTempOutput.Dataset = "Evapotranspiration";
            nldasTempOutput.DataSource = "penmandaily";
            nldasTempOutput.Metadata = new Dictionary<string, string>()
                    {
                        { "elevation", elevation.ToString() },
                        { "latitude", latitude.ToString() },
                        { "longitude", longitude.ToString() },
                        { "albedo", albedo.ToString() },
                        { "request_time", DateTime.Now.ToString() },
                        { "column_1", "Date" },
                        { "column_2", "Minimum Temperature" },
                        { "column_3", "Maximum Temperature" },
                        { "column_4", "Mean Temperature" },
                        { "column_5", "Mean Solar Radiation" },
                        { "column_6", "Mean Wind Speed" },
                        { "column_7", "Minimum Relative Humidity" },
                        { "column_8", "Maximum Relative Humidity" },
                        { "column_9", "Potential Evapotranspiration" }
                    };
            return nldasTempOutput;
        }

        public ITimeSeriesOutput getGldasData(out string errorMsg, ITimeSeriesInput inpt)
        {
            List<ITimeSeriesOutput> outputList = new List<ITimeSeriesOutput>();

            Temperature.GLDAS gldasTemp = new Temperature.GLDAS();
            ITimeSeriesOutputFactory gtFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput gTempOutput = gtFactory.Initialize();
            ITimeSeriesInputFactory gtiFactory = new TimeSeriesInputFactory();
            ITimeSeriesInput gtiInput = gtiFactory.SetTimeSeriesInput(inpt, new List<string>() { "temperature" }, out errorMsg);
            gtiInput.Geometry.GeometryMetadata.Add("ETGLDAS", ".");
            ITimeSeriesOutput gldasTempOutput = gldasTemp.GetData(out errorMsg, gTempOutput, gtiInput);

            if (errorMsg != "")
            {
                Utilities.ErrorOutput err = new Utilities.ErrorOutput();
                return err.ReturnError(errorMsg);
            }
            inpt.DateTimeSpan.StartDate = inpt.DateTimeSpan.StartDate.AddHours(-6.0);

            Wind.GLDAS gldasWind = new Wind.GLDAS();
            ITimeSeriesOutputFactory gwFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput gWindOutput = gwFactory.Initialize();
            ITimeSeriesInputFactory gwiFactory = new TimeSeriesInputFactory();
            ITimeSeriesInput gwiInput = gwiFactory.SetTimeSeriesInput(inpt, new List<string>() { "wind" }, out errorMsg);
            ITimeSeriesOutput gldasWindOutput = gldasWind.GetData(out errorMsg, gWindOutput, gwiInput);
            outputList.Add(gldasWindOutput);

            if (errorMsg != "")
            {
                Utilities.ErrorOutput err = new Utilities.ErrorOutput();
                return err.ReturnError(errorMsg);
            }
            inpt.DateTimeSpan.StartDate = inpt.DateTimeSpan.StartDate.AddHours(-6.0);

            Humidity.GLDAS gldasHumid = new Humidity.GLDAS();
            ITimeSeriesOutputFactory ghFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput gHumidOutput = ghFactory.Initialize();
            ITimeSeriesInputFactory ghiFactory = new TimeSeriesInputFactory();
            inpt.Source = "gldas";
            ITimeSeriesInput ghiInput = ghiFactory.SetTimeSeriesInput(inpt, new List<string>() { "humidity" }, out errorMsg);
            ITimeSeriesOutput gldasHumidOutput = gldasHumid.GetData(out errorMsg, gHumidOutput, ghiInput);
            outputList.Add(gldasHumidOutput);

            if (errorMsg != "")
            {
                Utilities.ErrorOutput err = new Utilities.ErrorOutput();
                return err.ReturnError(errorMsg);
            }
            inpt.DateTimeSpan.StartDate = inpt.DateTimeSpan.StartDate.AddHours(-6.0);

            Radiation.GLDAS gldasRad = new Radiation.GLDAS();
            ITimeSeriesOutputFactory grFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput gRadOutput = grFactory.Initialize();
            ITimeSeriesInputFactory griFactory = new TimeSeriesInputFactory();
            ITimeSeriesInput griInput = griFactory.SetTimeSeriesInput(inpt, new List<string>() { "radiation" }, out errorMsg);
            ITimeSeriesOutput gldasRadOutput = gldasRad.GetData(out errorMsg, gRadOutput, griInput);
            outputList.Add(gldasRadOutput);

            if (errorMsg != "")
            {
                Utilities.ErrorOutput err = new Utilities.ErrorOutput();
                return err.ReturnError(errorMsg);
            }
            inpt.DateTimeSpan.StartDate = inpt.DateTimeSpan.StartDate.AddHours(-6.0);

            Pressure.GLDAS gpress = new Pressure.GLDAS();
            ITimeSeriesOutputFactory gpFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput gpressOutput = gpFactory.Initialize();
            inpt.Source = "gldas";
            ITimeSeriesInputFactory gpiFactory = new TimeSeriesInputFactory();
            ITimeSeriesInput gpInput = gpiFactory.SetTimeSeriesInput(inpt, new List<string>() { "surfacepressure" }, out errorMsg);
            ITimeSeriesOutput pressOutput = gpress.GetData(out errorMsg, gpressOutput, gpInput);
            outputList.Add(pressOutput);

            if (errorMsg != "")
            {
                Utilities.ErrorOutput err = new Utilities.ErrorOutput();
                return err.ReturnError(errorMsg);
            }

            foreach (ITimeSeriesOutput result in outputList)
            {
                gldasTempOutput = Utilities.Merger.MergeTimeSeries(gldasTempOutput, result);
                if (result.Metadata.Values.Contains("ERROR"))
                {
                    gldasTempOutput.Metadata.Add(result.DataSource.ToString() + " ERROR", "The service is unavailable or returned no valid data.");
                }
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
                double shmin = Convert.ToDouble(timeseries.Value[4]);
                double shmax = Convert.ToDouble(timeseries.Value[5]);
                double wind = Convert.ToDouble(timeseries.Value[3]);
                double solarRad = Convert.ToDouble(timeseries.Value[7]) * 0.0864;
                double pressure = Convert.ToDouble(timeseries.Value[8]) / 100; //Convert Pa to mbar
                double relHMax = 0.0;
                double relHMin = 0.0;
                double petPMD = 0.0;
                int jday = ++julianday;

                PenmanDailyMethod(tmin, tmax, tmean, jday, shmin, shmax, wind, solarRad, out relHMin, out relHMax, pressure,
                                          out petPMD, out errorMsg);

                //Setting order of all items
                /*timeseries.Value[0] = jday.ToString();
                timeseries.Value[1] = tmin.ToString("F2", CultureInfo.InstalledUICulture);
                timeseries.Value[2] = tmax.ToString("F2", CultureInfo.InstalledUICulture);
                timeseries.Value[3] = tmean.ToString("F2", CultureInfo.InstalledUICulture);
                timeseries.Value[4] = solarRad.ToString("F2", CultureInfo.InstalledUICulture);
                timeseries.Value[5] = wind.ToString("F2", CultureInfo.InstalledUICulture);
                timeseries.Value[6] = relHMin.ToString("F2", CultureInfo.InstalledUICulture);
                timeseries.Value[7] = relHMax.ToString("F2", CultureInfo.InstalledUICulture);
                timeseries.Value[8] = petPMD.ToString("F4", CultureInfo.InvariantCulture);*/
                timeseries.Value[0] = tmin.ToString("F2", CultureInfo.InstalledUICulture);
                timeseries.Value[1] = tmax.ToString("F2", CultureInfo.InstalledUICulture);
                timeseries.Value[2] = tmean.ToString("F2", CultureInfo.InstalledUICulture);
                timeseries.Value[3] = solarRad.ToString("F2", CultureInfo.InstalledUICulture);
                timeseries.Value[4] = wind.ToString("F2", CultureInfo.InstalledUICulture);
                timeseries.Value[5] = relHMin.ToString("F2", CultureInfo.InstalledUICulture);
                timeseries.Value[6] = relHMax.ToString("F2", CultureInfo.InstalledUICulture);
                timeseries.Value[7] = petPMD.ToString("F4", CultureInfo.InvariantCulture);
                timeseries.Value.RemoveAt(8);
            }
            gldasTempOutput.Dataset = "Evapotranspiration";
            gldasTempOutput.DataSource = "penmandaily";
            gldasTempOutput.Metadata = new Dictionary<string, string>()
                    {
                        { "elevation", elevation.ToString() },
                        { "latitude", latitude.ToString() },
                        { "longitude", longitude.ToString() },
                        { "albedo", albedo.ToString() },
                        { "request_time", DateTime.Now.ToString() },
                        { "column_1", "Date" },
                        { "column_2", "Minimum Temperature" },
                        { "column_3", "Maximum Temperature" },
                        { "column_4", "Mean Temperature" },
                        { "column_5", "Mean Solar Radiation" },
                        { "column_6", "Mean Wind Speed" },
                        { "column_7", "Minimum Relative Humidity" },
                        { "column_8", "Maximum Relative Humidity" },
                        { "column_9", "Potential Evapotranspiration" }
                    };
            return gldasTempOutput;
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