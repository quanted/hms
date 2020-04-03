using Data;
using System;
using System.Collections.Generic;

namespace Solar
{
    /// <summary>
    /// Solar Calculator class based on excel spreadsheets from NOAA
    /// https://www.esrl.noaa.gov/gmd/grad/solcalc/calcdetails.html
    /// </summary>
    public class SolarCalculator : ITimeSeriesComponent
    {

        // -------------- Solar Calculator Variables -------------- //

        // TimeSeries Output variable 
        public ITimeSeriesOutput Output { get; set; }

        // TimeSeries Input variable
        public ITimeSeriesInput Input { get; set; }

        /// <summary>
        /// Calculator model: 'Day' or 'Year'
        /// </summary>
        public string Model;

        /// <summary>
        /// LocalTime variable for when model='year'
        /// </summary>
        public string LocalTime;

        /// <summary>
        /// Default Solar Calculator constructor
        /// </summary>
        public SolarCalculator() { }

        /// <summary>
        /// Get the solar calculator data
        /// </summary>
        public void GetCalculatorData()
        {
            string errorMsg = "";

            // If the timezone information is not provided, the tz details are retrieved and set to the geometry.timezone varaible.
            if (this.Input.Geometry.Timezone.Offset == 0)
            {
                this.Input.Geometry.Timezone = Utilities.Time.GetTimezone(out errorMsg, this.Input.Geometry.Point) as Timezone;
                return;
            }

            ITimeSeriesOutputFactory iFactory = new TimeSeriesOutputFactory();
            this.Output = iFactory.Initialize();

            // Add defaults to metadata
            this.Output.Metadata.Add("latitude", this.Input.Geometry.Point.Latitude.ToString());
            this.Output.Metadata.Add("longitude", this.Input.Geometry.Point.Longitude.ToString());
            this.Output.Metadata.Add("timezone", this.Input.Geometry.Timezone.Offset.ToString());
            this.Output.Metadata.Add("startDate", this.Input.DateTimeSpan.StartDate.ToString());

            if (this.Model.Equals("year"))
            {
                string[] columns = new string[] {
                    "Time (hrs past local midnight)", "Julian Day", "Julian Century", "Geom Mean Long Sun (deg)", "Geom Mean Anom Sun (deg)",
                    "Eccent Earth Orbit", "Sun Eq of Ctr", "Sun True Long (deg)", "Sun True Anom (deg)", "Sun Rad Vector (AUs)", "Sun App Long (deg)",
                    "Mean Obliq Ecliptic (deg)", "Obliq Corr (deg)", "Sun Rt Ascen (deg)", "Sun Declin (deg)", "vary", "Eq of Time (minutes)",
                    "HA Sunrise (deg)", "Solar Noon (LST)", "Sunrise Time (LST)", "Sunset Time (LST)", "Sunlight Duration (minutes)", "True Solar Time (min)",
                    "Hour Angle (deg)", "Solar Zenith Angle (deg)", "Solar Elevation Angle (deg)", "Approx Atmospheric Refraction (deg)",
                    "Solar Elevation corrected for atm refraction (deg)", "Solar Azimuth Angle (deg cw from N)"
                };
                this.Output.Metadata.Add("endDate", this.Input.DateTimeSpan.EndDate.ToString());
                this.Output.Metadata.Add("columns", String.Join(", ", columns));
                RunYearCalculation();
                this.Output.Metadata.Add("localTime", this.LocalTime);
                this.Output.Dataset = "Solar Calculator Year";
            }
            else
            {
                string[] columns = new string[] {
                    "Julian Day", "Julian Century", "Geom Mean Long Sun (deg)", "Geom Mean Anom Sun (deg)",
                    "Eccent Earth Orbit", "Sun Eq of Ctr", "Sun True Long (deg)", "Sun True Anom (deg)", "Sun Rad Vector (AUs)", "Sun App Long (deg)",
                    "Mean Obliq Ecliptic (deg)", "Obliq Corr (deg)", "Sun Rt Ascen (deg)", "Sun Declin (deg)", "vary", "Eq of Time (minutes)",
                    "HA Sunrise (deg)", "Solar Noon (LST)", "Sunrise Time (LST)", "Sunset Time (LST)", "Sunlight Duration (minutes)", "True Solar Time (min)",
                    "Hour Angle (deg)", "Solar Zenith Angle (deg)", "Solar Elevation Angle (deg)", "Approx Atmospheric Refraction (deg)",
                    "Solar Elevation corrected for atm refraction (deg)", "Solar Azimuth Angle (deg cw from N)"
                };
                this.Output.Metadata.Add("endDate", this.Input.DateTimeSpan.EndDate.ToString());
                this.Output.Metadata.Add("columns", String.Join(", ", columns));
                RunDayCalculation();
                this.Output.Dataset = "Solar Calculator Day";
            }

            this.Output.DataSource = "NOAA/NASA";
            this.Output.Metadata.Add("noaa_url", "https://www.esrl.noaa.gov/gmd/grad/solcalc/calcdetails.html");
            this.Output.Metadata.Add("noaa_web_app", "https://www.esrl.noaa.gov/gmd/grad/solcalc/");
            this.Output.Metadata.Add("description", "The calculations in the NOAA Sunrise/Sunset and Solar Position Calculators are based on equations from Astronomical Algorithms, " +
                "by Jean Meeus. The sunrise and sunset results are theoretically accurate to within a minute for locations between +/- 72° latitude, and within 10 minutes outside of " +
                "those latitudes. However, due to variations in atmospheric composition, temperature, pressure and conditions, observed values may vary from calculations.");
            this.Output.Metadata.Add("dataForLitigation", "The NOAA Solar Calculator is for research and recreational use only. NOAA cannot certify or authenticate sunrise, " +
                "sunset or solar position data. The U.S. Government does not collect observations of astronomical data, and due to atmospheric conditions our calculated " +
                "results may vary significantly from actual observed values."); 
        }

        /// <summary>
        /// Run year calculations for solar calculator
        /// </summary>
        private void RunYearCalculation()
        {
            Dictionary<string, List<string>> solarData = new Dictionary<string, List<string>>();
            DateTime currentDate = new DateTime();
            
            currentDate = this.Input.DateTimeSpan.StartDate;

            DateTime end = new DateTime();
            end = this.Input.DateTimeSpan.EndDate.AddDays(1);

            TimeSpan hour = TimeSpan.Parse(this.LocalTime);
            currentDate = currentDate.Add(hour);
            while (!(currentDate.CompareTo(end) > 0))
            {
                List<string> data = new List<string>();
                data.Add(this.LocalTime);

                // Julian Day double value (F)
                double julianDay = GetJulianDay(currentDate.ToOADate(), this.Input.Geometry.Timezone.Offset);
                data.Add(Math.Round(julianDay, 2).ToString());

                // Julian Century double value (G)
                double julianCentury = GetJulianCentury(julianDay);
                data.Add(Math.Round(julianCentury, 8).ToString());

                // Geom Mean Long Sun [deg] (I)
                double gmls = GetGeomMeanLongSun(julianCentury);
                data.Add(Math.Round(gmls, 5).ToString());

                // Geom Mean Anom Sun [deg] (J)
                double gmas = GetGeomMeanAnomSun(julianCentury);
                data.Add(Math.Round(gmas, 3).ToString());

                // Eccent Earth Orbit (K)
                double eeo = GetEccentEarthOrbit(julianCentury);
                data.Add(Math.Round(eeo, 6).ToString());

                // Sun Eq of Ctr (L)
                double seoc = GetSunEqOfCtr(julianCentury, gmas);
                data.Add(Math.Round(seoc, 6).ToString());

                // Sun True Long [deg] (M)
                double stl = GetSunTrueLong(gmls, seoc);
                data.Add(Math.Round(stl, 4).ToString());

                // Sun True Anom [deg] (N)
                double sta = GetSunTrueAnom(gmas, seoc);
                data.Add(Math.Round(sta, 3).ToString());

                // Sun Rad Vector [AUs] (O)
                double srv = GetSunRadVector(eeo, sta);
                data.Add(Math.Round(srv, 6).ToString());

                // Sun App Long [deg] (P)
                double sal = GetSunAppLong(julianCentury, stl);
                data.Add(Math.Round(sal, 4).ToString());

                // Mean Obliq Ecliptic [deg] (Q)
                double moe = GetMeanObliqEcliptic(julianCentury);
                data.Add(Math.Round(moe, 5).ToString());

                // Obliq Corr [deg] (R)
                double oc = GetObliqCorr(julianCentury, moe);
                data.Add(Math.Round(oc, 5).ToString());

                // Sun Rt Ascen [deg] (S)
                double sra = GetSunRtAscen(sal, oc);
                data.Add(Math.Round(sra, 4).ToString());

                // Sun Declin [deg] (T)
                double sd = GetSunDeclin(sal, oc);
                data.Add(Math.Round(sd, 4).ToString());

                // vary (U)
                double vary = GetVary(oc);
                data.Add(Math.Round(vary, 6).ToString());

                // Eq of Time [minutes] (V)
                double eot = GetEqOfTime(gmls, gmas, eeo, vary);
                data.Add(Math.Round(eot, 6).ToString());

                // HA Sunrise [deg] (W)
                double has = GetHASunrise(this.Input.Geometry.Point.Latitude, sd);
                data.Add(Math.Round(has, 6).ToString());

                // Solar Noon [LST] (X)
                double sn = GetSolarNoon(this.Input.Geometry.Point.Longitude, this.Input.Geometry.Timezone.Offset, eot);
                TimeSpan snTS = TimeSpan.FromDays(sn);
                data.Add(snTS.ToString());
                
                // Sunrise Time [LST] (Y)
                double srt = GetSunriseTime(has, sn);
                TimeSpan srtTS = TimeSpan.FromDays(srt);
                data.Add(srtTS.ToString());

                // Sunset Time [LST] (Z)
                double sst = GetSunsetTime(has, sn);
                TimeSpan sstTS = TimeSpan.FromDays(sst);
                data.Add(sstTS.ToString());

                // Sunlight Duration [minutes] (AA)
                double sld = GetSunlightDuration(has);
                data.Add(Math.Round(sld, 5).ToString());

                // True Solar Time [minutes] (AB)
                double test = hour.TotalDays;
                double tst = GetTrueSolarTime(this.Input.Geometry.Point.Longitude, this.Input.Geometry.Timezone.Offset, hour.TotalDays, eot);
                data.Add(Math.Round(tst, 4).ToString());

                // Hour Angle [deg] (AC)
                double ha = GetHourAngle(tst);
                data.Add(Math.Round(ha, 5).ToString());

                // Solar Zenith Angle [deg] (AD)
                double sza = GetSolarZenithAngle(this.Input.Geometry.Point.Latitude, sd, ha);
                data.Add(Math.Round(sza, 5).ToString());

                // Solar Elevation Angle [deg] (AE)
                double sea = GetSolarElevationAngle(sza);
                data.Add(Math.Round(sea, 5).ToString());

                // Approx Atmospheric Refraction [deg] (AF)             
                double aar = GetApproxAtmRefraction(sea);
                data.Add(Math.Round(aar, 6).ToString());

                // Solar Elevation corrected for atmospheric refraction [deg] (AG)
                double secr = GetSolarElevationCorrected(sea, aar);
                data.Add(Math.Round(secr, 6).ToString());

                // Solar Azimuth Angle [deg cw from N] (AH)
                double saa = GetSolarAzimuthAngle(this.Input.Geometry.Point.Latitude, ha, sza, sd);
                data.Add(Math.Round(saa, 4).ToString());

                solarData.Add(currentDate.ToString("MM/dd/yyyy"), data);
                currentDate = currentDate.AddDays(1);
            }
            this.Output.Data = solarData;
        }

        /// <summary>
        /// Run day calculations for solar calculator
        /// </summary>
        private void RunDayCalculation()
        {
            Dictionary<string, List<string>> solarData = new Dictionary<string, List<string>>();
            DateTime currentDate = new DateTime();

            currentDate = this.Input.DateTimeSpan.StartDate;
            DateTime endDate = currentDate.AddDays(1);
            currentDate = currentDate.AddMinutes(6);

            while (currentDate.CompareTo(endDate) <= 0)
            {
                List<string> data = new List<string>();

                // Julian Day double value (F)
                double julianDay = GetJulianDay(currentDate.ToOADate(), this.Input.Geometry.Timezone.Offset);
                data.Add(Math.Round(julianDay, 2).ToString());

                // Julian Century double value (G)
                double julianCentury = GetJulianCentury(julianDay);
                data.Add(Math.Round(julianCentury, 8).ToString());

                // Geom Mean Long Sun [deg] (I)
                double gmls = GetGeomMeanLongSun(julianCentury);
                data.Add(Math.Round(gmls, 5).ToString());

                // Geom Mean Anom Sun [deg] (J)
                double gmas = GetGeomMeanAnomSun(julianCentury);
                data.Add(Math.Round(gmas, 3).ToString());

                // Eccent Earth Orbit (K)
                double eeo = GetEccentEarthOrbit(julianCentury);
                data.Add(Math.Round(eeo, 6).ToString());

                // Sun Eq of Ctr (L)
                double seoc = GetSunEqOfCtr(julianCentury, gmas);
                data.Add(Math.Round(seoc, 6).ToString());

                // Sun True Long [deg] (M)
                double stl = GetSunTrueLong(gmls, seoc);
                data.Add(Math.Round(stl, 5).ToString());

                // Sun True Anom [deg] (N)
                double sta = GetSunTrueAnom(gmas, seoc);
                data.Add(Math.Round(sta, 5).ToString());

                // Sun Rad Vector [AUs] (O)
                double srv = GetSunRadVector(eeo, sta);
                data.Add(Math.Round(srv, 6).ToString());

                // Sun App Long [deg] (P)
                double sal = GetSunAppLong(julianCentury, stl);
                data.Add(Math.Round(sal, 4).ToString());

                // Mean Obliq Ecliptic [deg] (Q)
                double moe = GetMeanObliqEcliptic(julianCentury);
                data.Add(Math.Round(moe, 5).ToString());

                // Obliq Corr [deg] (R)
                double oc = GetObliqCorr(julianCentury, moe);
                data.Add(Math.Round(oc, 5).ToString());

                // Sun Rt Ascen [deg] (S)
                double sra = GetSunRtAscen(sal, oc);
                data.Add(Math.Round(sra, 4).ToString());

                // Sun Declin [deg] (T)
                double sd = GetSunDeclin(sal, oc);
                data.Add(Math.Round(sd, 5).ToString());

                // vary (U)
                double vary = GetVary(oc);
                data.Add(Math.Round(vary, 6).ToString());

                // Eq of Time [minutes] (V)
                double eot = GetEqOfTime(gmls, gmas, eeo, vary);
                data.Add(Math.Round(eot, 6).ToString());

                // HA Sunrise [deg] (W)
                double has = GetHASunrise(this.Input.Geometry.Point.Latitude, sd);
                data.Add(Math.Round(has, 6).ToString());

                // Solar Noon [LST] (X)
                double sn = GetSolarNoon(this.Input.Geometry.Point.Longitude, this.Input.Geometry.Timezone.Offset, eot);
                TimeSpan snTS = TimeSpan.FromDays(sn);
                data.Add(snTS.ToString());

                // Sunrise Time [LST] (Y)
                double srt = GetSunriseTime(has, sn);
                TimeSpan srtTS = TimeSpan.FromDays(srt);
                data.Add(srtTS.ToString());

                // Sunset Time [LST] (Z)
                double sst = GetSunsetTime(has, sn);
                TimeSpan sstTS = TimeSpan.FromDays(sst);
                data.Add(sstTS.ToString());

                // Sunlight Duration [minutes] (AA)
                double sld = GetSunlightDuration(has);
                data.Add(Math.Round(sld, 5).ToString());

                // True Solar Time [minutes] (AB)
                double tst = GetTrueSolarTime(this.Input.Geometry.Point.Longitude, this.Input.Geometry.Timezone.Offset, currentDate.TimeOfDay.TotalDays, eot);
                data.Add(Math.Round(tst, 4).ToString());

                // Hour Angle [deg] (AC)
                double ha = GetHourAngle(tst);
                data.Add(Math.Round(ha, 5).ToString());

                // Solar Zenith Angle [deg] (AD)
                double sza = GetSolarZenithAngle(this.Input.Geometry.Point.Latitude, sd, ha);
                data.Add(Math.Round(sza, 5).ToString());

                // Solar Elevation Angle [deg] (AE)
                double sea = GetSolarElevationAngle(sza);
                data.Add(Math.Round(sea, 5).ToString());

                // Approx Atmospheric Refraction [deg] (AF)             
                double aar = GetApproxAtmRefraction(sea);
                data.Add(Math.Round(aar, 6).ToString());

                // Solar Elevation corrected for atmospheric refraction [deg] (AG)
                double secr = GetSolarElevationCorrected(sea, aar);
                data.Add(Math.Round(secr, 6).ToString());

                // Solar Azimuth Angle [deg cw from N] (AH)
                double saa = GetSolarAzimuthAngle(this.Input.Geometry.Point.Latitude, ha, sza, sd);
                data.Add(Math.Round(saa, 4).ToString());

                solarData.Add(currentDate.ToString("MM/dd/yyyy HH:mm:ss"), data);
                currentDate = currentDate.AddMinutes(6);
            }
            this.Output.Data = solarData;
        }

        /// <summary>
        /// Convert double degree to radian
        /// </summary>
        /// <param name="degree"></param>
        /// <returns></returns>
        private static double ToRadian(double degree)
        {
            return (degree * Math.PI) / 180.0;
        }

        /// <summary>
        /// Convert double radian to degree
        /// </summary>
        /// <param name="radian"></param>
        /// <returns></returns>
        private static double ToDegree(double radian)
        {
            return (radian * 180) / Math.PI;
        }

        // ---------- Value Functions ---------- //

        /// <summary>
        /// Get julian day as double
        /// </summary>
        /// <param name="oaDate">Date as OADate</param>
        /// <param name="offset">Timezone offset</param>
        /// <returns></returns>
        public double GetJulianDay(double oaDate, double offset)
        {
            return oaDate + 2415018.5 - (offset / 24);
        }

        /// <summary>
        /// Get julian century as double
        /// </summary>
        /// <param name="julianDay">julian day</param>
        /// <returns></returns>
        public double GetJulianCentury(double julianDay)
        {
            return (julianDay - 2451545.0) / 36525.0;
        }

        /// <summary>
        /// Get geom mean long sun value (deg)
        /// </summary>
        /// <param name="julianCentury">Julian century</param>
        /// <returns></returns>
        public double GetGeomMeanLongSun(double julianCentury)
        {
            return (280.46646 + julianCentury * (36000.76983 + julianCentury * 0.0003032)) % 360.0;
        }

        /// <summary>
        /// Get geom mean anon sun value (deg)
        /// </summary>
        /// <param name="julianCentury">Julian century</param>
        /// <returns></returns>
        public double GetGeomMeanAnomSun(double julianCentury)
        {
            return 357.52911 + julianCentury * (35999.05029 - 0.0001537 * julianCentury);
        }

        /// <summary>
        /// Get eccent earth orbit
        /// </summary>
        /// <param name="julianCentury">Julian century</param>
        /// <returns></returns>
        public double GetEccentEarthOrbit(double julianCentury)
        {
            return 0.016708634 - julianCentury * (0.000042037 + 0.0000001267 * julianCentury);
        }

        /// <summary>
        /// Get sun eq of ctr
        /// </summary>
        /// <param name="julianCentury">julian century</param>
        /// <param name="gmas">geom mean anom sun</param>
        /// <returns></returns>
        public double GetSunEqOfCtr(double julianCentury, double gmas)
        {
            return Math.Sin(ToRadian(gmas)) * (1.914602 - julianCentury * (0.004817 + 0.000014 * julianCentury)) +
                    Math.Sin(2 * ToRadian(gmas)) * (0.019993 - 0.000101 * julianCentury) + Math.Sin(ToRadian(3.0 * gmas)) * 0.000289;
        }

        /// <summary>
        /// Get sun true long (deg)
        /// </summary>
        /// <param name="gmls">geom mean long sun</param>
        /// <param name="seoc">sun eq of ctr</param>
        /// <returns></returns>
        public double GetSunTrueLong(double gmls, double seoc)
        {
            return gmls + seoc;
        }

        /// <summary>
        /// Get sun true anom (deg)
        /// </summary>
        /// <param name="gmas">geom mean anom sun</param>
        /// <param name="seoc">sun eq of ctr</param>
        /// <returns></returns>
        public double GetSunTrueAnom(double gmas, double seoc)
        {
            return gmas + seoc;
        }

        /// <summary>
        /// Get sun rad vector (AUs)
        /// </summary>
        /// <param name="eeo">eccent earth orbit</param>
        /// <param name="sta">sun true anom</param>
        /// <returns></returns>
        public double GetSunRadVector(double eeo, double sta)
        {
            return (1.000001018 * (1 - eeo * eeo)) / (1 + eeo * Math.Cos(ToRadian(sta)));
        }

        /// <summary>
        /// Get sun app long (deg)
        /// </summary>
        /// <param name="julieanCentury">julian century</param>
        /// <param name="stl">sun true long</param>
        /// <returns></returns>
        public double GetSunAppLong(double julianCentury, double stl)
        {
            return stl - 0.00569 - 0.00478 * Math.Sin(ToRadian(125.04 - 1934.136 * julianCentury));
        }

        /// <summary>
        /// Get mean obliq ecliptic
        /// </summary>
        /// <param name="julianCentury">julian century</param>
        /// <returns></returns>
        public double GetMeanObliqEcliptic(double julianCentury)
        {
            return 23 + (26 + ((21.448 - julianCentury * (46.815 + julianCentury * (0.00059 - julianCentury * 0.001813)))) / 60) / 60;
        }

        /// <summary>
        /// Get obliq corr (deg)
        /// </summary>
        /// <param name="julianCentury">julian century</param>
        /// <param name="moe">mean obliq ecliptic</param>
        /// <returns></returns>
        public double GetObliqCorr(double julianCentury, double moe)
        {
            return moe + 0.00256 * Math.Cos(ToRadian(125.04 - 1934.136 * julianCentury));
        }

        /// <summary>
        /// Get sun rt ascen (deg)
        /// </summary>
        /// <param name="sal">sun app long</param>
        /// <param name="oc">obliq corr</param>
        /// <returns></returns>
        public double GetSunRtAscen(double sal, double oc)
        {
            return ToDegree(Math.Atan2((Math.Cos(ToRadian(oc)) * Math.Sin(ToRadian(sal))), Math.Cos(ToRadian(sal))));
        }

        /// <summary>
        /// Get sun declin (deg)
        /// </summary>
        /// <param name="sal"></param>
        /// <param name="oc"></param>
        /// <returns></returns>
        public double GetSunDeclin(double sal, double oc)
        {
            return ToDegree(Math.Asin(Math.Sin(ToRadian(oc)) * Math.Sin(ToRadian(sal))));
        }

        /// <summary>
        /// Get vary value
        /// </summary>
        /// <param name="oc">obliq corr</param>
        /// <returns></returns>
        public double GetVary(double oc)
        {
            return Math.Tan(ToRadian(oc / 2)) * Math.Tan(ToRadian(oc / 2));
        }

        /// <summary>
        /// Get eq of time (minutes)
        /// </summary>
        /// <param name="gmls">geom mean long sun</param>
        /// <param name="gmas">geom mean anom sun</param>
        /// <param name="eeo">ecent earth orbit</param>
        /// <param name="vary">vary</param>
        /// <returns></returns>
        public double GetEqOfTime(double gmls, double gmas, double eeo, double vary)
        {
            return 4.0 * ToDegree((vary * Math.Sin(2.0 * ToRadian(gmls))) - (2.0 * eeo * Math.Sin(ToRadian(gmas))) + (4.0 * eeo * vary * Math.Sin(ToRadian(gmas))
                    * Math.Cos(2.0 * ToRadian(gmls))) - (0.5 * vary * vary * Math.Sin(4.0 * ToRadian(gmls))) - (1.25 * eeo * eeo * Math.Sin(2 * ToRadian(gmas))));
        }

        /// <summary>
        /// Get HA sunrise (deg)
        /// </summary>
        /// <param name="latitude">latitude</param>
        /// <param name="sd">sun declin</param>
        /// <returns></returns>
        public double GetHASunrise(double latitude, double sd)
        {
            return ToDegree(Math.Acos(Math.Cos(ToRadian(90.833)) / (Math.Cos(ToRadian(latitude)) 
                * Math.Cos(ToRadian(sd))) - Math.Tan(ToRadian(latitude)) * Math.Tan(ToRadian(sd))));
        }

        /// <summary>
        /// Get solar noon (LST)
        /// </summary>
        /// <param name="longitude">longitude</param>
        /// <param name="offset">offset</param>
        /// <param name="eot">eq of time</param>
        /// <returns></returns>
        public double GetSolarNoon(double longitude, double offset, double eot)
        {
            return (720 - 4 * longitude - eot + offset * 60) / 1440;
        }

        /// <summary>
        /// Get sunrise time (LST)
        /// </summary>
        /// <param name="has">HA sunrise</param>
        /// <param name="sn">solar noon</param>
        /// <returns></returns>
        public double GetSunriseTime(double has, double sn)
        {
            return (sn * 1440 - has * 4) / 1440;
        }

        /// <summary>
        /// Get sunset time (LST)
        /// </summary>
        /// <param name="has">HA sunrise</param>
        /// <param name="sn">solar noon</param>
        /// <returns></returns>
        public double GetSunsetTime(double has, double sn)
        {
            return (sn * 1440 + has * 4) / 1440;
        }

        /// <summary>
        /// Get solar duation (min)
        /// </summary>
        /// <param name="has">HA sunrise</param>
        /// <returns></returns>
        public double GetSunlightDuration(double has)
        {
            return 8 * has;
        }

        /// <summary>
        /// Get true solar time (min)
        /// </summary>
        /// <param name="longitude">longitude</param>
        /// <param name="offset">timezone offset</param>
        /// <param name="hours">hours expressed as whole/fractional value of day</param>
        /// <param name="eot">Eq of Time</param>
        /// <returns></returns>
        public double GetTrueSolarTime(double longitude, double offset, double hours, double eot)
        {
            double n = ((hours * 1440.0) + eot + (4.0 * longitude) - (60.0 * offset));
            if(n < 0)
            {
                return (1440 - Math.Abs(n)) % 1440;
            }
            else
            {
                return n % 1440;
            }
        }

        /// <summary>
        /// Get hour angle (deg)
        /// </summary>
        /// <param name="tst">true solar time</param>
        /// <returns></returns>
        public double GetHourAngle(double tst)
        {
            return (tst / 4 < 0) ? tst / 4.0 + 180 : tst / 4.0 - 180;
        }

        /// <summary>
        /// Get solar zenith angle (deg)
        /// </summary>
        /// <param name="latitude">latitude</param>
        /// <param name="sd">sunlight duration</param>
        /// <param name="ha">hour angle</param>
        /// <returns></returns>
        public double GetSolarZenithAngle(double latitude, double sd, double ha)
        {
            return ToDegree(Math.Acos(Math.Sin(ToRadian(latitude)) * Math.Sin(ToRadian(sd)) +
                    Math.Cos(ToRadian(latitude)) * Math.Cos(ToRadian(sd)) * Math.Cos(ToRadian(ha))));
        }

        /// <summary>
        /// Get solar elevation angle (deg)
        /// </summary>
        /// <param name="sza">solar zenith angle</param>
        /// <returns></returns>
        public double GetSolarElevationAngle(double sza)
        {
            return 90 - sza;
        }

        /// <summary>
        /// Get approximate atmospheric refraction (deg)
        /// </summary>
        /// <param name="sea">solar elevation angle</param>
        /// <returns></returns>
        public double GetApproxAtmRefraction(double sea)
        {
            double aar = 0.0;
            if (sea > 85)
            {
                aar = 0.0;
            }
            else
            {
                if (sea > 5)
                {
                    aar = 58.1 / Math.Tan(ToRadian(sea)) - 0.07 / Math.Pow(Math.Tan(ToRadian(sea)), 3) + 0.000086 / Math.Pow(Math.Tan(ToRadian(sea)), 5);
                }
                else
                {
                    if (sea > -0.575)
                    {
                        aar = 1735 + sea * (-518.2 + sea * (103.4 + sea * (-12.79 + sea * 0.711)));
                    }
                    else
                    {
                        aar = -20.772 / Math.Tan(ToRadian(sea));
                    }
                }
            }
            return aar / 3600;
        }

        /// <summary>
        /// Get solar elevation corrected for atmospheric refraction (deg)
        /// </summary>
        /// <param name="sea">solar elevation angle</param>
        /// <param name="aar">approximate atmospheric refraction</param>
        /// <returns></returns>
        public double GetSolarElevationCorrected(double sea, double aar)
        {
            return sea + aar;
        }

        /// <summary>
        /// Get Solar Asimuth Angle (deg cw from N)
        /// </summary>
        /// <param name="latitude">latitude</param>
        /// <param name="ha">hour angle</param>
        /// <param name="sza">solar zenith angle</param>
        /// <param name="sd">sunlight duration</param>
        /// <returns></returns>
        public double GetSolarAzimuthAngle(double latitude, double ha, double sza, double sd)
        {
            if (ha > 0)
            {
                double n = ToDegree(Math.Acos(
                    (Math.Sin(ToRadian(latitude)) * Math.Cos(ToRadian(sza)) - Math.Sin(ToRadian(sd)))
                    / (Math.Cos(ToRadian(latitude)) * Math.Sin(ToRadian(sza)))));
                if (n >= 0)
                {
                    return n + 180 % 360.0;
                }
                else
                {
                    return (360.0 - Math.Abs(n + 180)) % 360.0;
                }
            }
            else
            {
                double n = (540 - ToDegree(Math.Acos(
                    ((Math.Sin(ToRadian(latitude)) * Math.Cos(ToRadian(sza))) - Math.Sin(ToRadian(sd)))
                    / (Math.Cos(ToRadian(latitude)) * Math.Sin(ToRadian(sza))))));
                if (n >= 0) {
                    return n % 360.0;
                }
                else
                {
                    return (360.0 - Math.Abs(n)) % 360.0;
                }
            }
        }
    }
}
