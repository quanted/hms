using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Net;
using System.Globalization;
using Data;
using System.Threading;
using System.IO;

//http://ldas.gsfc.nasa.gov/faq/#Subset
//ftp://hydro1.sci.gsfc.nasa.gov/data/s4pa/NLDAS/NLDAS_FORA0125_H.002/1980/004/NLDAS_FORA0125_H.A19800104.0000.002.grb.xml
//ftp://hydro1.sci.gsfc.nasa.gov/data/s4pa/NLDAS/
//http://ldas.gsfc.nasa.gov/nldas/NLDAS1forcing.php
//https://www.eol.ucar.edu/projects/ceop/dm/documents/refdata_report/eqns.html
//http://disc.sci.gsfc.nasa.gov/hydrology/data-rods-time-series-data

namespace Evapotranspiration
{
    public class NLDAS2
    {
        protected string _urlBasePF = @"http://hydro1.sci.gsfc.nasa.gov/daac-bin/access/timeseries.cgi?variable=NLDAS:NLDAS_FORA0125_H.002:";
        protected string _type = "&type=asc2";
        protected string _parameter = "";
        protected string _source = "";
        protected string _latitude = "";
        protected string _longitude = "";
        protected string _startDate = "";
        protected string _endDate = "";
        protected int _NLDASX = 0;
        protected int _NLDASY = 0;
        protected int _TimeZoneOffsetFromGMT = 0;
        double pWestmostGridCenter = -124.9375;
        double pSouthmostGridCenter = 25.0625;
        double pDegreesPerGridCell = 0.1250;
        public enum parameter
        {
            PrecipitationTotal_kgPerM2, Temperature2mAboveGround_K, SpecificHumidity2mAboveGround_KgPerKg,
            WindSpeedZonal10mAboveGround_MPerS, WindSpeedMeridional10mAboveGround_MPerS,
            ShortWaveRadiationFluxDownwards_WPerM2, LongWaveRadiationFluxDownwards_WPerM2,
            PotentialEvaporationHourlyTotal_KgPerM2, SurfacePressure_Pa
        };

        public NLDAS2(double latitude, double longitude, string startDate, string endDate)
        {
            _source = "nldas";
            _parameter = "TMP2m";
            _latitude = latitude.ToString();
            _longitude = longitude.ToString();
            _startDate = startDate;
            _endDate = endDate;
            cleanStartAndEndDates();
            _NLDASX = Convert.ToInt32((longitude - pWestmostGridCenter) / pDegreesPerGridCell);
            _NLDASY = Convert.ToInt32((latitude - pSouthmostGridCenter) / pDegreesPerGridCell);
        }
        public NLDAS2(parameter param, double latitude, double longitude, string startDate, string endDate)
        {
            _source = "nldas";
            _parameter = getNLDASParameterName(param);
            _latitude = latitude.ToString();
            _longitude = longitude.ToString();
            _startDate = startDate;
            _endDate = endDate;
            cleanStartAndEndDates();
            _NLDASX = Convert.ToInt32((longitude - pWestmostGridCenter) / pDegreesPerGridCell);
            _NLDASY = Convert.ToInt32((latitude - pSouthmostGridCenter) / pDegreesPerGridCell);
        }
        public NLDAS2(string source, double latitude, double longitude, string startDate, string endDate)
        {
            _source = source;
            _parameter = "Tair_f_inst";
            _latitude = latitude.ToString();
            _longitude = longitude.ToString();
            _startDate = startDate;
            _endDate = endDate;
            cleanStartAndEndDates();
            _NLDASX = Convert.ToInt32((longitude - pWestmostGridCenter) / pDegreesPerGridCell);
            _NLDASY = Convert.ToInt32((latitude - pSouthmostGridCenter) / pDegreesPerGridCell);
            switch (source)
            {
                case "nldas":
                    _urlBasePF = @"http://hydro1.sci.gsfc.nasa.gov/daac-bin/access/timeseries.cgi?variable=NLDAS:NLDAS_FORA0125_H.002:";
                    break;
                case "gldas":
                    _urlBasePF = @"http://hydro1.gesdisc.eosdis.nasa.gov/daac-bin/access/timeseries.cgi?variable=GLDAS2:GLDAS_NOAH025_3H_v2.1:";
                    break;
                case "daymet":
                case "ncdc":
                case "wgen":
                case "prism":
                default:
                    _urlBasePF = @"http://hydro1.sci.gsfc.nasa.gov/daac-bin/access/timeseries.cgi?variable=NLDAS:NLDAS_FORA0125_H.002:";
                    break;
            }
        }

        public DataTable getData1(int timeZoneOffsetFromGMT_Hours, out string errorMsg)
        {
            errorMsg = "";
            DataTable dt = new DataTable();

            if (timeZoneOffsetFromGMT_Hours > 0)
            {
                errorMsg = "Invalid Time Zone offset from GMT.  The offset must be a negative integer for USA.";
                return dt;
            }

            timeZoneOffsetFromGMT_Hours = Math.Abs(timeZoneOffsetFromGMT_Hours);

            // Get table with the dates and the temperature data.
            dt = getDatesTemperatures(timeZoneOffsetFromGMT_Hours, out errorMsg);

            return dt;
        }

        public DataTable getData2(int timeZoneOffsetFromGMT_Hours, out string errorMsg)
        {
            errorMsg = "";
            string errorMsg1 = "";
            DataTable dt = new DataTable();
            DataTable dtSR = new DataTable();


            if (timeZoneOffsetFromGMT_Hours > 0)
            {
                errorMsg = "Invalid Time Zone offset from GMT.  The offset must be a negative integer for USA.";
                return dt;
            }

            timeZoneOffsetFromGMT_Hours = Math.Abs(timeZoneOffsetFromGMT_Hours);

            // Get table with the dates and the temperature data.
            dt = getDatesTemperatures(timeZoneOffsetFromGMT_Hours, out errorMsg1);

            if ((dt == null) || (dt.Rows.Count <= 0) || (errorMsg1 != ""))
            {
                errorMsg = "Temperature data for specified dates are not available." + errorMsg1;
                return dt;
            }

            // Get table with solar radiation data.  
            dtSR = getSolarRad(timeZoneOffsetFromGMT_Hours, out errorMsg1);

            if ((dtSR == null) || (dtSR.Rows.Count <= 0) || (errorMsg1 != ""))
            {
                errorMsg = "Solar radiation data for specified dates are not available." + errorMsg1;
                return dt;
            }

            dt.Columns.Add("SolarRadMean_MJm2day");

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i]["SolarRadMean_MJm2day"] = dtSR.Rows[i]["SolarRadMean_MJm2day"];
            }

            return dt;
        }

        public DataTable getData3(int timeZoneOffsetFromGMT_Hours, out string errorMsg)
        {
            errorMsg = "";
            string errorMsg1 = "";
            DataTable dt = new DataTable();
            DataTable dtSR = new DataTable();
            DataTable dtSH = new DataTable();

            if (timeZoneOffsetFromGMT_Hours > 0)
            {
                errorMsg = "Invalid Time Zone offset from GMT.  The offset must be a negative integer for USA.";
                return dt;
            }


            timeZoneOffsetFromGMT_Hours = Math.Abs(timeZoneOffsetFromGMT_Hours);

            // Get table with the dates and the temperature data.
            dt = getDatesTemperatures(timeZoneOffsetFromGMT_Hours, out errorMsg1);

            if ((dt == null) || (dt.Rows.Count <= 0) || (errorMsg1 != ""))
            {
                errorMsg = "Temperature data for specified dates are not available." + errorMsg1;
                return dt;
            }

            // Get table with specific humidity data.  
            dtSH = getSpecificHumidity(timeZoneOffsetFromGMT_Hours, out errorMsg1);

            if ((dtSH == null) || (dtSH.Rows.Count <= 0) || (errorMsg1 != ""))
            {
                errorMsg = "Specific humidity data for specified dates are not available." + errorMsg1;
                return dt;
            }

            // Get table with solar radiation data.  
            dtSR = getSolarRad(timeZoneOffsetFromGMT_Hours, out errorMsg1);

            if ((dtSR == null) || (dtSR.Rows.Count <= 0) || (errorMsg1 != ""))
            {
                errorMsg = "Solar radiation data for specified dates are not available." + errorMsg1;
                return dt;
            }

            dt.Columns.Add("SolarRadMean_MJm2day");
            dt.Columns.Add("SHmin");
            dt.Columns.Add("SHmax");

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i]["SolarRadMean_MJm2day"] = dtSR.Rows[i]["SolarRadMean_MJm2day"];
                dt.Rows[i]["SHmin"] = dtSH.Rows[i]["SHmin"];
                dt.Rows[i]["SHmax"] = dtSH.Rows[i]["SHmax"];
            }

            return dt;
        }

        public DataTable getData4(int timeZoneOffsetFromGMT_Hours, out string errorMsg)
        {
            errorMsg = "";
            string errorMsg1 = "";
            DataTable dt = new DataTable();
            DataTable dtSR = new DataTable();
            DataTable dtSH = new DataTable();
            DataTable dtWS = new DataTable();


            if (timeZoneOffsetFromGMT_Hours > 0)
            {
                errorMsg = "Invalid Time Zone offset from GMT.  The offset must be a negative integer for USA.";
                return dt;
            }


            timeZoneOffsetFromGMT_Hours = Math.Abs(timeZoneOffsetFromGMT_Hours);

            // Get table with the dates and the temperature data.
            dt = getDatesTemperatures(timeZoneOffsetFromGMT_Hours, out errorMsg1);

            if ((dt == null) || (dt.Rows.Count <= 0) || (errorMsg1 != ""))
            {
                errorMsg = "Temperature data for specified dates are not available." + errorMsg1;
                return dt;
            }

            // Get table with specific humidity data.  
            dtSH = getSpecificHumidity(timeZoneOffsetFromGMT_Hours, out errorMsg1);

            if ((dtSH == null) || (dtSH.Rows.Count <= 0) || (errorMsg1 != ""))
            {
                errorMsg = "Specific humidity data for specified dates are not available." + errorMsg1;
                return dt;
            }

            // Get table with wind speed data.  
            dtWS = getWindSpeed(timeZoneOffsetFromGMT_Hours, out errorMsg1);

            if ((dtWS == null) || (dtWS.Rows.Count <= 0) || (errorMsg1 != ""))
            {
                errorMsg = "Wind speed data for specified dates are not available." + errorMsg1;
                return dt;
            }

            // Get table with solar radiation data.  
            dtSR = getSolarRad(timeZoneOffsetFromGMT_Hours, out errorMsg1);

            if ((dtSR == null) || (dtSR.Rows.Count <= 0) || (errorMsg1 != ""))
            {
                errorMsg = "Solar radiation data for specified dates are not available." + errorMsg1;
                return dt;
            }

            dt.Columns.Add("SolarRadMean_MJm2day");
            dt.Columns.Add("SHmin");
            dt.Columns.Add("SHmax");
            dt.Columns.Add("WindSpeedMean_m/s");

            /*List<int> count = new List<int>(){ dtSR.Rows.Count, dtSH.Rows.Count, dtWS.Rows.Count };
            int rowcount = count.Min();
            if( > rowcount)
            {
                dt.Rows.
            }*/

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i]["SolarRadMean_MJm2day"] = dtSR.Rows[i]["SolarRadMean_MJm2day"];
                dt.Rows[i]["SHmin"] = dtSH.Rows[i]["SHmin"];
                dt.Rows[i]["SHmax"] = dtSH.Rows[i]["SHmax"];
                dt.Rows[i]["WindSpeedMean_m/s"] = dtWS.Rows[i]["WindSpeedMean_m/s"];
            }

            return dt;
        }

        public DataTable getDataHourly(int timeZoneOffsetFromGMT_Hours, bool flagHSPF, out string errorMsg)
        {
            double wsu, wsv, windSpeed;

            DateTime startDate = Convert.ToDateTime(_startDate);
            //startDate = startDate.AddDays(-1.0);
            _startDate = startDate.Year.ToString() + "-" + startDate.Month.ToString() + "-" + startDate.Day.ToString();
            DateTime endDate = Convert.ToDateTime(_endDate);
            endDate = endDate.AddDays(1.0);
            _endDate = endDate.Year.ToString() + "-" + endDate.Month.ToString() + "-" + endDate.Day.ToString();
            cleanStartAndEndDates();

            errorMsg = "";

            DataTable dt = new DataTable();
            DataTable dtHourly = new DataTable();
            DataTable dtSR = new DataTable();
            DataTable dtSH = new DataTable();
            DataTable dtWU = new DataTable();
            DataTable dtWV = new DataTable();
            DataTable dtPrep = new DataTable();
            DataTable dtPE = new DataTable();

            if (timeZoneOffsetFromGMT_Hours > 0)
            {
                errorMsg = "Invalid Time Zone offset from GMT.  The offset must be a negative integer for USA.";
                return dt;
            }

            timeZoneOffsetFromGMT_Hours = Math.Abs(timeZoneOffsetFromGMT_Hours);
            if (_source == "nldas")
            {
                startDate = startDate.AddDays(-1.0);
                _startDate = startDate.Year.ToString() + "-" + startDate.Month.ToString() + "-" + startDate.Day.ToString();
                // Get temperature and dates.
                _parameter = getNLDASParameterName(parameter.Temperature2mAboveGround_K);
                dt = getData(out errorMsg);

                // Get Solar Radiation data.  
                _parameter = getNLDASParameterName(parameter.ShortWaveRadiationFluxDownwards_WPerM2);
                dtSR = getData(out errorMsg);

                // Get specific humidity.  
                _parameter = getNLDASParameterName(parameter.SpecificHumidity2mAboveGround_KgPerKg);
                dtSH = getData(out errorMsg);

                // Get wind data.  
                _parameter = getNLDASParameterName(parameter.WindSpeedZonal10mAboveGround_MPerS);
                dtWU = getData(out errorMsg);

                _parameter = getNLDASParameterName(parameter.WindSpeedMeridional10mAboveGround_MPerS);
                dtWV = getData(out errorMsg);
            }
            else
            {
                if (Convert.ToDateTime(_startDate).Year > 2010 || Convert.ToDateTime(_endDate).Year > 2010)
                {
                    errorMsg = "No GLDAS data is available for this time frame.";
                    return dt;
                    /*_source = "nldas";
                    startDate = startDate.AddDays(-1.0);
                    _startDate = startDate.Year.ToString() + "-" + startDate.Month.ToString() + "-" + startDate.Day.ToString();
                    _urlBasePF = @"http://hydro1.sci.gsfc.nasa.gov/daac-bin/access/timeseries.cgi?variable=NLDAS:NLDAS_FORA0125_H.002:";*/
                }
                // Get temperature and dates.
                _parameter = getNLDASParameterName(parameter.Temperature2mAboveGround_K);
                dt = getData(out errorMsg);

                // Get specific humidity.  
                _parameter = getNLDASParameterName(parameter.SpecificHumidity2mAboveGround_KgPerKg);
                dtSH = getData(out errorMsg);

                // Get wind data.  
                _parameter = getNLDASParameterName(parameter.WindSpeedZonal10mAboveGround_MPerS);
                dtWU = getData(out errorMsg);

                _parameter = getNLDASParameterName(parameter.WindSpeedMeridional10mAboveGround_MPerS);
                dtWV = getData(out errorMsg);

                // Get Solar Radiation data.  
                _parameter = getNLDASParameterName(parameter.ShortWaveRadiationFluxDownwards_WPerM2);
                dtSR = getData(out errorMsg);
            }


            if (flagHSPF)
            {
                // Get precipitation.
                _parameter = getNLDASParameterName(parameter.PrecipitationTotal_kgPerM2);
                dtPrep = getData(out errorMsg);

                // Get potential evaporation.
                _parameter = getNLDASParameterName(parameter.PotentialEvaporationHourlyTotal_KgPerM2);
                dtPE = getData(out errorMsg);
            }

            if (errorMsg != "")
            {
                errorMsg = "ERROR: Unable to download data";
                return dt;//null;
            }

            for (int i = 0; i < timeZoneOffsetFromGMT_Hours; i++)
            {
                dt.Rows[0].Delete();
                dtSR.Rows[0].Delete();
                dtSH.Rows[0].Delete();
                dtWU.Rows[0].Delete();
                dtWV.Rows[0].Delete();
                if (flagHSPF)
                {
                    dtPrep.Rows[0].Delete();
                    dtPE.Rows[0].Delete();
                }
            }


            foreach (DataRow dr in dt.Rows)
            {
                dr["Value"] = Convert.ToDouble(dr["Value"].ToString()) - 273.15; // Convert Kelvin to Celsius
            }

            dtHourly.Columns.Add("DateHour");
            dtHourly.Columns.Add("Julian_Day");
            dtHourly.Columns.Add("THourly_C");
            dtHourly.Columns.Add("SolarRad_MJm2day");
            dtHourly.Columns.Add("SH_Hourly");
            dtHourly.Columns.Add("WindSpeed_m/s");
            if (flagHSPF)
            {
                dtHourly.Columns.Add("Precipitation_Hourly");
                dtHourly.Columns.Add("Potential_Evaporation");
            }

            string date = "", datej = "";
            int startYear;
            double solarRad;
            double precipHrly;
            double potEvap;
            double elapsedJulianDate;
            int julianDayOfYear;
            int hour;
            int remainder;
            int remaindermax = 24;
            int remaindermin = 23;
            if (_source == "gldas")
            {
                remaindermax = 8;
                remaindermin = 7;
            }
            DataRow dr1 = null;
            List<Double> listdt = new List<double>();
            List<String> listDate = new List<String>();
            List<int> listHour = new List<int>();
            List<Double> listSR = new List<double>();
            List<Double> listSH = new List<double>();
            List<Double> listWU = new List<double>();
            List<Double> listWV = new List<double>();
            List<Double> listPrep = new List<double>();
            List<Double> listPE = new List<double>();


            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Math.DivRem(i, remaindermax, out remainder);

                if (remainder == 0)
                {
                    date = dt.Rows[i]["Date"].ToString();
                    listdt = new List<double>();
                    listDate = new List<String>();
                    listHour = new List<int>();
                    listSR = new List<double>();
                    listSH = new List<double>();
                    listWU = new List<double>();
                    listWV = new List<double>();
                    if (flagHSPF)
                    {
                        listPrep = new List<double>();
                        listPE = new List<double>();
                    }
                }

                dt.Rows[i]["Date"] = date;
                dt.Rows[i]["Hour"] = remainder;
                listDate.Add(Convert.ToString(date));
                listHour.Add(Convert.ToInt32(remainder));
                listdt.Add(Convert.ToDouble(dt.Rows[i]["Value"].ToString()));
                listSR.Add(Convert.ToDouble(dtSR.Rows[i]["Value"].ToString()));
                listSH.Add(Convert.ToDouble(dtSH.Rows[i]["Value"].ToString()));
                listWU.Add(Convert.ToDouble(dtWU.Rows[i]["Value"].ToString()));
                listWV.Add(Convert.ToDouble(dtWV.Rows[i]["Value"].ToString()));
                if (flagHSPF)
                {
                    listPrep.Add(Convert.ToDouble(dtPrep.Rows[i]["Value"].ToString()));
                    listPE.Add(Convert.ToDouble(dtPE.Rows[i]["Value"].ToString()));
                }

                if (remainder == remaindermin)
                {
                    for (int j = 0; j < remaindermax; j++)
                    {
                        dr1 = dtHourly.NewRow();
                        hour = listHour[j];
                        date = listDate[j] + " " + hour.ToString("00") + ":00";
                        dr1["DateHour"] = date;

                        datej = listDate[j].ToString();
                        startDate = Convert.ToDateTime(datej);
                        startYear = startDate.Year - 1;
                        elapsedJulianDate = Convert.ToDateTime(startYear.ToString() + "-12-31").ToOADate();
                        julianDayOfYear = Convert.ToInt32(Convert.ToDateTime(datej).ToOADate() - elapsedJulianDate);
                        dr1["Julian_Day"] = julianDayOfYear;

                        dt.Rows[i]["Date"] = listdt[j];
                        solarRad = listSR[j] * 0.0864;
                        dr1["SolarRad_MJm2day"] = solarRad.ToString("F2", CultureInfo.InvariantCulture);
                        dr1["THourly_C"] = listdt[j].ToString("F2", CultureInfo.InvariantCulture);
                        dr1["SH_Hourly"] = listSH[j].ToString("F6", CultureInfo.InvariantCulture);
                        wsu = listWU[j];
                        wsv = listWV[j];
                        windSpeed = Math.Sqrt(wsu * wsu + wsv * wsv);
                        dr1["WindSpeed_m/s"] = windSpeed.ToString("F2", CultureInfo.InvariantCulture);

                        if (flagHSPF)
                        {
                            // Convert hourly precipitation and potential evaporation from mm/hr to in/hr.
                            precipHrly = listPrep[j] / 25.4;
                            potEvap = listPE[j] / 25.4;
                            dr1["Precipitation_Hourly"] = precipHrly.ToString("F3", CultureInfo.InvariantCulture);
                            dr1["Potential_Evaporation"] = potEvap.ToString("F6", CultureInfo.InvariantCulture);
                        }

                        dtHourly.Rows.Add(dr1);
                    }

                }

            }
            return dtHourly;
        }

        public DataTable getDatesTemperatures(int timeZoneOffsetFromGMT_Hours, out string errorMsg)
        {
            DateTime startDate = Convert.ToDateTime(_startDate);
            int remaindermax = 8;
            int remaindermin = 7;
            if (_source == "nldas")
            {
                remaindermax = 24;
                remaindermin = 23;
                startDate = startDate.AddDays(-1.0);
            }


            _startDate = startDate.Year.ToString() + "-" + startDate.Month.ToString() + "-" + startDate.Day.ToString();
            DateTime endDate = Convert.ToDateTime(_endDate);
            endDate = endDate.AddDays(1.0);
            _endDate = endDate.Year.ToString() + "-" + endDate.Month.ToString() + "-" + endDate.Day.ToString();
            cleanStartAndEndDates();
            errorMsg = "";
            DataTable dt = new DataTable();

            _parameter = getNLDASParameterName(parameter.Temperature2mAboveGround_K);
            dt = getData(out errorMsg);

            if ((dt == null) || (dt.Rows.Count <= 0))
            {
                errorMsg = "Data for specified dates not available.";
                return dt;
            }

            foreach (DataRow dr in dt.Rows)
            {
                dr["Value"] = Convert.ToDouble(dr["Value"].ToString()) - 273.15; //Convert Kelvin to Celsius
            } //foreach

            if (timeZoneOffsetFromGMT_Hours == 0) //No need to adjust time series for time zone offset
            {
                return dt;
            }

            for (int i = 0; i < timeZoneOffsetFromGMT_Hours; i++)
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
            startDate = Convert.ToDateTime(_startDate);
            int startYear = startDate.Year - 1;
            double elapsedJulianDate = Convert.ToDateTime(startYear.ToString() + "-12-31").ToOADate();
            int julianDayOfYear = 0;
            List<Double> list = new List<double>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Math.DivRem(i, remaindermax, out remainder);           //24?
                if (remainder == 0)
                {
                    date = dt.Rows[i]["Date"].ToString();
                    dr1 = dtMinMax.NewRow();
                    list = new List<double>();

                }
                dt.Rows[i]["Date"] = date;
                dt.Rows[i]["Hour"] = remainder;
                list.Add(Convert.ToDouble(dt.Rows[i]["Value"].ToString()));

                if (remainder == remaindermin)                         //23?
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
            return dtMinMax;
        }

        public DataTable getSolarRad(int timeZoneOffsetFromGMT_Hours, out string errorMsg)
        {
            errorMsg = "";
            int remaindermax = 8;
            int remaindermin = 7;

            DataTable dt = new DataTable();
            DataTable dtSR = new DataTable();
            
            /*if (_source == "gldas" && (Convert.ToDateTime(_startDate).Year > 2010 || Convert.ToDateTime(_endDate).AddDays(-1.0).Year > 2010))
            {
                errorMsg = "No GLDAS data is available for this time frame.";
                return dtSR;
                //_source = "nldas";
                //DateTime startDate = Convert.ToDateTime(_startDate);
                //startDate = startDate.AddDays(-1.0);
                //_startDate = startDate.Year.ToString() + "-" + startDate.Month.ToString() + "-" + startDate.Day.ToString();
                //_urlBasePF = @"http://hydro1.sci.gsfc.nasa.gov/daac-bin/access/timeseries.cgi?variable=NLDAS:NLDAS_FORA0125_H.002:";
            }*/

            if (_source == "nldas")
            {
                remaindermax = 24;
                remaindermin = 23;
            }

            // Get Solar Radiation data.  
            _parameter = getNLDASParameterName(parameter.ShortWaveRadiationFluxDownwards_WPerM2);
            dtSR = getData(out errorMsg);

            if ((dtSR == null) || (dtSR.Rows.Count <= 0) || (errorMsg != ""))
            {
                errorMsg = "Solar radiation data not available." + errorMsg;
                return dt;
            }

            for (int i = 0; i < timeZoneOffsetFromGMT_Hours; i++)
            {
                dtSR.Rows[0].Delete();
            }

            DataTable dtMinMax = new DataTable();
            dtMinMax.Columns.Add("SolarRadMean_MJm2day");
            int remainder = 0;

            double meanSR = 0;
            DataRow dr1 = null;
            List<Double> listSR = new List<double>();

            for (int i = 0; i < dtSR.Rows.Count; i++)
            {
                Math.DivRem(i, remaindermax, out remainder);
                if (remainder == 0)
                {
                    dr1 = dtMinMax.NewRow();
                    listSR = new List<double>();
                }

                listSR.Add(Convert.ToDouble(dtSR.Rows[i]["Value"].ToString()));

                if (remainder == remaindermin)
                {
                    meanSR = listSR.Average();
                    meanSR = meanSR * 0.0864;  // Convert units from W/s to MJ/(m^2 day)
                    dr1["SolarRadMean_MJm2day"] = meanSR.ToString("F2", CultureInfo.InvariantCulture);
                    dtMinMax.Rows.Add(dr1);
                }
            }

            return dtMinMax;
        }


        public DataTable getSpecificHumidity(int timeZoneOffsetFromGMT_Hours, out string errorMsg)
        {
            errorMsg = "";
            int remaindermax = 8;
            int remaindermin = 7;

            if (_source == "nldas")
            {
                remaindermax = 24;
                remaindermin = 23;
            }
            DataTable dt = new DataTable();
            DataTable dtSH = new DataTable();

            //Get specific humidity.  
            _parameter = getNLDASParameterName(parameter.SpecificHumidity2mAboveGround_KgPerKg);
            dtSH = getData(out errorMsg);

            if ((dtSH == null) || (dtSH.Rows.Count <= 0) || (errorMsg != ""))
            {
                errorMsg = "Specific humidity data not available." + errorMsg;
                return dt;
            }

            for (int i = 0; i < timeZoneOffsetFromGMT_Hours; i++)
            {
                dtSH.Rows[0].Delete();
            }

            DataTable dtMinMax = new DataTable();
            dtMinMax.Columns.Add("SHmin");
            dtMinMax.Columns.Add("SHmax");

            int remainder = 0;
            DataRow dr1 = null;
            List<Double> listSH = new List<double>();

            for (int i = 0; i < dtSH.Rows.Count; i++)
            {
                Math.DivRem(i, remaindermax, out remainder);
                if (remainder == 0)
                {
                    dr1 = dtMinMax.NewRow();
                    listSH = new List<double>();
                }

                listSH.Add(Convert.ToDouble(dtSH.Rows[i]["Value"].ToString()));

                if (remainder == remaindermin)
                {
                    dr1["SHmin"] = listSH.Min().ToString("F6", CultureInfo.InvariantCulture);
                    dr1["SHmax"] = listSH.Max().ToString("F6", CultureInfo.InvariantCulture);
                    dtMinMax.Rows.Add(dr1);
                }
            }

            return dtMinMax;
        }

        public DataTable getWindSpeed(int timeZoneOffsetFromGMT_Hours, out string errorMsg)
        {
            errorMsg = "";
            int remaindermax = 8;
            int remaindermin = 7;

            if (_source == "nldas")
            {
                remaindermax = 24;
                remaindermin = 23;
            }

            DataTable dt = new DataTable();
            DataTable dtWU = new DataTable();
            DataTable dtWV = new DataTable();

            // Get wind data.  
            _parameter = getNLDASParameterName(parameter.WindSpeedZonal10mAboveGround_MPerS);
            dtWU = getData(out errorMsg);

            if ((dtWU == null) || (dtWU.Rows.Count <= 0) || (errorMsg != ""))
            {
                errorMsg = "Wind speed data not available." + errorMsg;
                return dt;
            }

            _parameter = getNLDASParameterName(parameter.WindSpeedMeridional10mAboveGround_MPerS);
            dtWV = getData(out errorMsg);

            if ((dtWV == null) || (dtWV.Rows.Count <= 0) || (errorMsg != ""))
            {
                errorMsg = "Wind speed data not available." + errorMsg;
                return dt;
            }

            for (int i = 0; i < timeZoneOffsetFromGMT_Hours; i++)
            {
                dtWU.Rows[0].Delete();
                dtWV.Rows[0].Delete();
            }

            DataTable dtMinMax = new DataTable();
            dtMinMax.Columns.Add("WindSpeedMean_m/s");

            int remainder = 0;
            DataRow dr1 = null;
            List<Double> listWS = new List<double>();
            double wsu, wsv, windS;
            double meanWS = 0;

            for (int i = 0; i < dtWU.Rows.Count; i++)
            {
                Math.DivRem(i, remaindermax, out remainder);
                if (remainder == 0)
                {
                    dr1 = dtMinMax.NewRow();
                    listWS = new List<double>();
                }

                wsu = Convert.ToDouble(dtWU.Rows[i]["Value"].ToString());
                wsv = Convert.ToDouble(dtWV.Rows[i]["Value"].ToString());
                windS = Math.Sqrt(wsu * wsu + wsv * wsv);
                listWS.Add(windS);

                if (remainder == remaindermin)
                {
                    meanWS = listWS.Average();
                    dr1["WindSpeedMean_m/s"] = meanWS.ToString("F2", CultureInfo.InvariantCulture);
                    dtMinMax.Rows.Add(dr1);
                }
            }

            return dtMinMax;
        }

        protected string getNLDASParameterName(parameter param)
        {
            //Parameter reference from http://ldas.gsfc.nasa.gov/nldas/NLDAS1forcing.php
            string NLDASParam = "";
            if (_source == "nldas")
            {
                switch (param)
                {
                    case parameter.PrecipitationTotal_kgPerM2:
                        NLDASParam = "APCPsfc"; //kg per m2
                        break;
                    case parameter.SpecificHumidity2mAboveGround_KgPerKg:
                        NLDASParam = "SPFH2m"; //kg per kg
                        break;
                    case parameter.Temperature2mAboveGround_K:
                        NLDASParam = "TMP2m";  //Kelvin
                        break;
                    case parameter.WindSpeedZonal10mAboveGround_MPerS:
                        NLDASParam = "UGRD10m"; //m per s
                        break;
                    case parameter.WindSpeedMeridional10mAboveGround_MPerS:
                        NLDASParam = "VGRD10m"; //m per s
                        break;
                    case parameter.ShortWaveRadiationFluxDownwards_WPerM2:
                        NLDASParam = "DSWRFsfc";
                        break;
                    case parameter.LongWaveRadiationFluxDownwards_WPerM2:
                        NLDASParam = "DLWRFsfc";
                        break;
                    case parameter.PotentialEvaporationHourlyTotal_KgPerM2:
                        NLDASParam = "PEVAPsfc"; //kg per m2
                        break;
                    case parameter.SurfacePressure_Pa:
                        NLDASParam = "PRESsfc"; //pascal
                        break;
                        throw new System.Exception("Unknow parameter.");
                }
            }
            else if (_source == "gldas")
            {
                switch (param)
                {
                    case parameter.PrecipitationTotal_kgPerM2:
                        NLDASParam = "Rainf_f_tavg"; //kg per m2
                        break;
                    case parameter.PotentialEvaporationHourlyTotal_KgPerM2:
                        NLDASParam = "PotEvap_tavg"; //kg per m2
                        _urlBasePF = @"http://hydro1.gesdisc.eosdis.nasa.gov/daac-bin/access/timeseries.cgi?variable=GLDAS2:GLDAS_NOAH025_3H_v2.0:";
                        break;
                    case parameter.SpecificHumidity2mAboveGround_KgPerKg:
                        NLDASParam = "Qair_f_inst"; //kg per kg
                        break;
                    case parameter.Temperature2mAboveGround_K:
                        NLDASParam = "Tair_f_inst";  //Kelvin
                        break;
                    case parameter.WindSpeedZonal10mAboveGround_MPerS:
                        NLDASParam = "Wind_f_inst"; //m per s
                        break;
                    case parameter.WindSpeedMeridional10mAboveGround_MPerS:
                        NLDASParam = "Wind_f_inst"; //m per s
                        break;
                    case parameter.ShortWaveRadiationFluxDownwards_WPerM2:
                        NLDASParam = "SWdown_f_tavg";
                        _urlBasePF = @"http://hydro1.gesdisc.eosdis.nasa.gov/daac-bin/access/timeseries.cgi?variable=GLDAS2:GLDAS_NOAH025_3H_v2.0:";
                        break;
                    case parameter.LongWaveRadiationFluxDownwards_WPerM2:
                        NLDASParam = "LWdown_f_tavg";
                        _urlBasePF = @"http://hydro1.gesdisc.eosdis.nasa.gov/daac-bin/access/timeseries.cgi?variable=GLDAS2:GLDAS_NOAH025_3H_v2.0:";
                        break;
                    case parameter.SurfacePressure_Pa:
                        NLDASParam = "PRESsfc"; //pascal
                        break;
                        throw new System.Exception("Unknow parameter.");
                }
            }

            return NLDASParam;
        }

        public DataTable getData(out string errorMsg)
        {
            errorMsg = "";
            DataTable dt = new DataTable();
            dt.Columns.Add("Date");
            dt.Columns.Add("Hour");
            dt.Columns.Add("Value");
            string url = "";
            switch (_source)
            {
                case "nldas":
                    url = _urlBasePF + _parameter + "&startDate=" + _startDate + "T24" + "&endDate=" + _endDate + "T24" +
                         "&location=NLDAS:X" + _NLDASX.ToString() + "-Y" + _NLDASY.ToString() + _type; // "X298" + "-" + "Y152" + _type;
                    break;
                case "gldas":
                    url = _urlBasePF + _parameter + "&startDate=" + _startDate + "T06" + "&endDate=" + _endDate + "T23" +
                         "&location=GEOM:POINT" + @"%28" + _longitude + @",%20" + _latitude + @"%29" + _type; // "X298" + "-" + "Y152" + _type;
                    break;
                case "wgen":
                    break;
                case "prism":
                default:
                    url = _urlBasePF + _parameter + "&startDate=" + _startDate + "T24" + "&endDate=" + _endDate + "T24" +
                         "&location=NLDAS:X" + _NLDASX.ToString() + "-Y" + _NLDASY.ToString() + _type; // "X298" + "-" + "Y152" + _type;
                    break;

            }
            // Response status message
            byte[] bytes = null;
            int retries = 10;                                        // Max number of request retries
            try
            {
                while (retries > 0 && bytes == null)
                {
                    WebClient client = new WebClient();
                    client.Credentials = CredentialCache.DefaultNetworkCredentials;
                    bytes = client.DownloadData(url);
                    retries -= 1;
                    if (bytes == null)
                    {
                        Thread.Sleep(100);
                    }
                }
            }
            catch(System.Net.WebException ex)
            {
                if(ex.Message.Contains("404"))
                {
                    return getData(out errorMsg);
                }
                errorMsg = "Error attempting to collection data from external server.";
                return null;
            }
            if (bytes != null)
            {
                string str = Encoding.UTF8.GetString(bytes);
                char[] delimiterChars = { '\n' };
                string[] lines = str.Split(delimiterChars);
                string cleanLine = "";
                bool blnDataStart = false;
                string[] values = null;
                DataRow dr = null;
                foreach (string line in lines)
                {
                    cleanLine = line.Trim();
                    if (cleanLine.Contains("Date&Time"))
                    {
                        blnDataStart = true;
                        continue;
                    }
                    if ((cleanLine.Contains("Date&Time")) || (cleanLine.Contains("MEAN")) || (cleanLine == ""))
                    {
                        continue;
                    }
                    if (blnDataStart == true)
                    {
                        values = System.Text.RegularExpressions.Regex.Split(cleanLine, @"\s\s+");
                        if (values != null && _source != "nldas")
                        {
                            dr = dt.NewRow();
                            string[] delimiter = { "T", "\t", " ", ":00:00" };
                            string[] dateAndTime = values[0].Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                            dr["Date"] = dateAndTime[0];
                            dr["Hour"] = Convert.ToInt32(dateAndTime[1].Replace("Z", ""));
                            //dr["Date"] = values[0];
                            dr["Value"] = Decimal.Parse(dateAndTime[2], System.Globalization.NumberStyles.Any);
                            dt.Rows.Add(dr);
                        }
                        else if (values != null && _source == "nldas")
                        {
                            dr = dt.NewRow();
                            char[] delimiter = { ' ' };
                            string[] dateAndTime = values[0].Split(delimiter);
                            dr["Date"] = dateAndTime[0];
                            dr["Hour"] = Convert.ToInt32(dateAndTime[1].Replace("Z", ""));
                            //dr["Date"] = values[0];
                            if ((values.Count() > 0) && (values[1] != ""))
                            {
                                dr["Value"] = Decimal.Parse(values[1], System.Globalization.NumberStyles.Any);
                            }
                            dt.Rows.Add(dr);
                        }
                    }
                }
            }
            return dt;
        }

        public double getAnnualPrecipitation()
        {
            double aprecip;
            double valuep;
            string _parameter1 = "APCPsfc";
            string _startDate1 = "1981-01-01";
            string _endDate1 = "2010-12-31";
            string url = "";
            List<Double> list = new List<double>();
            if (_source == "nldas")
            {
                url = _urlBasePF + _parameter1 + "&startDate=" + _startDate1 + "T00" + "&endDate=" + _endDate1 + "T23" +
                         "&location=NLDAS:X" + _NLDASX.ToString() + "-Y" + _NLDASY.ToString() + _type;
            }
            else if (_source == "gldas")
            {
                url = _urlBasePF + "Rainf_f_tavg" + "&location=GEOM:POINT" + @"%28" + _longitude + @",%20" + _latitude + @"%29" +
                        "&startDate=" + _startDate + "T06" + "&endDate=" + _endDate + "T23" + _type; // "X298" + "-" + "Y152" + _type;
            }
            byte[] bytes = null;
            int retries = 5;
            string status = "";
            while (retries > 0 && !status.Contains("OK"))
            {
                WebRequest wr = WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)wr.GetResponse();
                status = response.StatusCode.ToString();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                bytes = Encoding.ASCII.GetBytes(reader.ReadToEnd());
                reader.Close();
                response.Close();
                retries -= 1;
                if (!status.Contains("OK"))
                {
                    Thread.Sleep(100);
                }
            }
            //WebClient client = new WebClient();
            //client.Credentials = CredentialCache.DefaultNetworkCredentials;
            //byte[] bytes = client.DownloadData(url);
            if (bytes != null)
            {
                string str = Encoding.UTF8.GetString(bytes);
                char[] delimiterChars = { '\n' };
                string[] lines = str.Split(delimiterChars);
                string cleanLine = "";
                bool blnDataStart = false;
                string[] values = null;

                foreach (string line in lines)
                {
                    cleanLine = line.Trim();
                    if (cleanLine.Contains("Date&Time"))
                    {
                        blnDataStart = true;
                    }
                    if ((cleanLine.Contains("Date&Time")) || (cleanLine.Contains("MEAN")) || (cleanLine == ""))
                    {
                        continue;
                    }
                    if (blnDataStart == true)
                    {
                        values = System.Text.RegularExpressions.Regex.Split(cleanLine, @"\s\s+");
                        if (values != null && _source == "nldas")
                        {
                            char[] delimiter = { ' ' };
                            string[] dateAndTime = values[0].Split(delimiter);
                            if ((values.Count() > 0) && (values[1] != ""))
                            {
                                valuep = (Double)Decimal.Parse(values[1], System.Globalization.NumberStyles.Any);
                                list.Add(valuep);
                            }
                        }
                        else if (values != null && _source != "nldas")
                        {
                            string[] delimiter = { "T", "\t", " ", ":00:00" };
                            string[] dateAndTime = values[0].Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                            valuep = (Double)Decimal.Parse(dateAndTime[2], System.Globalization.NumberStyles.Any);
                            list.Add(valuep * 60 * 60);//gldas precip is in kg/m^2/s rather than kg/m^2 hourly
                        }
                    }
                }
            }

            aprecip = list.Sum() / 30.0;
            return aprecip;
        }

        public double calculateDewPointTemp(double temp_degreeC, double specificHumidity)
        {
            double dewPointTemp_degreeC = 0;
            double es = 6.112 * Math.Exp((17.67 * temp_degreeC) / (temp_degreeC + 243.5));
            return dewPointTemp_degreeC;
        }

        protected void cleanStartAndEndDates()
        {
            DateTime dStartDate = Convert.ToDateTime(_startDate);
            DateTime dEndDate = Convert.ToDateTime(_endDate);
            string year = "";
            string month = "";
            string day = "";
            if (dStartDate.Year < 1979)
            {
                _startDate = "1979-01-01";
            }
            dEndDate = Convert.ToDateTime(_endDate);
            if (dEndDate.Month < 10)
                month = "0" + dEndDate.Month.ToString();
            else
                month = dEndDate.Month.ToString();
            if (dEndDate.Day < 10)
                day = "0" + dEndDate.Day.ToString();
            else
                day = dEndDate.Day.ToString();
            _endDate = dEndDate.Year.ToString() + "-" + month + "-" + day;

            dStartDate = Convert.ToDateTime(_startDate);
            if (dStartDate.Month < 10)
                month = "0" + dStartDate.Month.ToString();
            else
                month = dStartDate.Month.ToString();
            if (dStartDate.Day < 10)
                day = "0" + dStartDate.Day.ToString();
            else
                day = dStartDate.Day.ToString();
            _startDate = dStartDate.Year.ToString() + "-" + month + "-" + day;
        }
    }
}