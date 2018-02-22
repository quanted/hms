using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Solar.Tests
{
    [TestClass]
    public class SolarCalculatorTests
    {

        [TestMethod]
        public void GetJulianDayTest1()
        {
            Solar.SolarCalculator solCalc = new SolarCalculator();
            DateTime date = new DateTime(2010, 06, 21);

            double jd = solCalc.GetJulianDay(date.ToOADate(), -6);
            Assert.AreEqual(2455368.75, Math.Round(jd, 2));
        }

        [TestMethod]
        public void GetJulianDayTest2()
        {
            Solar.SolarCalculator solCalc = new SolarCalculator();
            DateTime date = new DateTime(2010, 12, 10, 12, 0 , 0);

            double jd = solCalc.GetJulianDay(date.ToOADate(), -7);
            Assert.AreEqual(2455541.29, Math.Round(jd, 2));
        }

        [TestMethod]
        public void GetJulianDayTest3()
        {
            Solar.SolarCalculator solCalc = new SolarCalculator();
            DateTime date = new DateTime(2010, 06, 21, 21, 54, 00);

            double jd = solCalc.GetJulianDay(date.ToOADate(), -6);
            Assert.AreEqual(2455369.66, Math.Round(jd, 2));
        }

        [TestMethod]
        public void GetJulianCentury1()
        {
            Solar.SolarCalculator solCalc = new SolarCalculator();
            double julianDay = solCalc.GetJulianDay(new DateTime(2010, 02, 13, 12, 0, 0).ToOADate(), -7);
            double jc = solCalc.GetJulianCentury(julianDay);
            Assert.AreEqual(0.10119895, Math.Round(jc, 8));
        }

        [TestMethod]
        public void GetJulianCentury2()
        {
            Solar.SolarCalculator solCalc = new SolarCalculator();
            double julianDay = solCalc.GetJulianDay(new DateTime(2010, 06, 21, 16, 0, 0).ToOADate(), -6);
            double jc = solCalc.GetJulianCentury(julianDay);
            Assert.AreEqual(0.10470682, Math.Round(jc, 8));
        }

        [TestMethod]
        public void GetGMLSTest1()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double julianCentury = 0.10119895049052735;
            double gmls = solCalc.GetGeomMeanLongSun(julianCentury);
            Assert.AreEqual(323.7065868, Math.Round(gmls, 7));
        }

        [TestMethod]
        public void GetGMLSTest2()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double julianCentury = 0.10470682181154035;
            double gmls = solCalc.GetGeomMeanLongSun(julianCentury);
            Assert.AreEqual(89.99265499, Math.Round(gmls, 8));
        }

        [TestMethod]
        public void GetGMASTest1()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double julianCentury = 0.10119895049052735;
            double gmas = solCalc.GetGeomMeanAnomSun(julianCentury);
            Assert.AreEqual(4000.595216, Math.Round(gmas, 6));
        }

        [TestMethod]
        public void GetGMASTest2()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double julianCentury = 0.10470682181154035;
            double gmas = solCalc.GetGeomMeanAnomSun(julianCentury);
            Assert.AreEqual(4126.875252, Math.Round(gmas, 6));
        }

        [TestMethod]
        public void GetEEOTest1()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double julianCentury = 0.10119895049052735;
            double eeo = solCalc.GetEccentEarthOrbit(julianCentury);
            Assert.AreEqual(0.016704379, Math.Round(eeo, 9));
        }

        [TestMethod]
        public void GetEEOTest2()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double julianCentury = 0.10470682181154035;
            double eeo = solCalc.GetEccentEarthOrbit(julianCentury);
            Assert.AreEqual(0.016704231, Math.Round(eeo, 9));
        }

        [TestMethod]
        public void GetSEOCTest1()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double julianCentury = 0.10119895;
            double gmas = 4000.595216;
            double seoc = solCalc.GetSunEqOfCtr(julianCentury, gmas);
            Assert.AreEqual(1.2655276, Math.Round(seoc, 7));
        }

        [TestMethod]
        public void GetSEOCTest2()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double julianCentury = 0.10470682;
            double gmas = 4126.875252;
            double seoc = solCalc.GetSunEqOfCtr(julianCentury, gmas);
            Assert.AreEqual(0.4259834, Math.Round(seoc, 7));
        }

        [TestMethod]
        public void GetSTLTest1()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double gmls = 323.7065868;
            double seoc = 1.265527642;
            double stl = solCalc.GetSunTrueLong(gmls, seoc);
            Assert.AreEqual(324.9721144, Math.Round(stl, 7));
        }

        [TestMethod]
        public void GetSTLTest2()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double gmls = 89.99265499;
            double seoc = 0.425983383;
            double stl = solCalc.GetSunTrueLong(gmls, seoc);
            Assert.AreEqual(90.4186384, Math.Round(stl, 7));
        }

        [TestMethod]
        public void GetSTATest1()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double gmas = 4000.595216;
            double seoc = 1.265527642;
            double sta = solCalc.GetSunTrueAnom(gmas, seoc);
            Assert.AreEqual(4001.860744, Math.Round(sta, 6));
        }

        [TestMethod]
        public void GetSTATest2()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double gmas = 4126.875252;
            double seoc = 0.425983383;
            double sta = solCalc.GetSunTrueAnom(gmas, seoc);
            Assert.AreEqual(4127.30124, Math.Round(sta, 5));
        }

        [TestMethod]
        public void GetSRVTest1()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double eeo = 0.016704379;
            double sta = 4001.860744;
            double srv = solCalc.GetSunRadVector(eeo, sta);
            Assert.AreEqual(0.98743737, Math.Round(srv, 8));
        }

        [TestMethod]
        public void GetSRVTest2()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double eeo = 0.016704231;
            double sta = 4127.301236;
            double srv = solCalc.GetSunRadVector(eeo, sta);
            Assert.AreEqual(1.016282961, Math.Round(srv, 9));
        }

        [TestMethod]
        public void GetSALTest1()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double julianCentury = 0.10119895;
            double stl = 324.9721144;
            double sal = solCalc.GetSunAppLong(julianCentury, stl);
            Assert.AreEqual(324.9709356, Math.Round(sal, 7));
        }

        [TestMethod]
        public void GetSALTest2()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double julianCentury = 0.10470682;
            double stl = 90.41863838;
            double sal = solCalc.GetSunAppLong(julianCentury, stl);
            Assert.AreEqual(90.41761466, Math.Round(sal, 8));
        }

        [TestMethod]
        public void GetMOETest1()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double julianCentury = 0.10119895;
            double moe = solCalc.GetMeanObliqEcliptic(julianCentury);
            Assert.AreEqual(23.4379751, Math.Round(moe, 7));
        }

        [TestMethod]
        public void GetMOETest2()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double julianCentury = 0.10470682;
            double moe = solCalc.GetMeanObliqEcliptic(julianCentury);
            Assert.AreEqual(23.43792948, Math.Round(moe, 8));
        }

        [TestMethod]
        public void GetOCTest1()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double julianCentury = 0.10119895;
            double moe = 23.4379751;
            double oc = solCalc.GetObliqCorr(julianCentury, moe);
            Assert.AreEqual(23.43882153, Math.Round(oc, 8));
        }

        [TestMethod]
        public void GetOCTest2()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double julianCentury = 0.10470682;
            double moe = 23.43792948;
            double oc = solCalc.GetObliqCorr(julianCentury, moe);
            Assert.AreEqual(23.43848456, Math.Round(oc, 8));
        }

        [TestMethod]
        public void GetSRATest1()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double sal = 324.9709356;
            double oc = 23.43882153;
            double sra = solCalc.GetSunRtAscen(sal, oc);
            Assert.AreEqual(-32.746043, Math.Round(sra, 6));
        }

        [TestMethod]
        public void GetSRATest2()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double sal = 90.41761466;
            double oc = 23.43848456;
            double sra = solCalc.GetSunRtAscen(sal, oc);
            Assert.AreEqual(90.45517045, Math.Round(sra, 8));
        }

        [TestMethod]
        public void GetSDTest1()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double sal = 324.9709356;
            double oc = 23.43882153;
            double sd = solCalc.GetSunDeclin(sal, oc);
            Assert.AreEqual(-13.1979801, Math.Round(sd, 7));
        }

        [TestMethod]
        public void GetSDTest2()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double sal = 90.41761466;
            double oc = 23.43848456;
            double sd = solCalc.GetSunDeclin(sal, oc);
            Assert.AreEqual(23.43782475, Math.Round(sd, 8));
        }

        [TestMethod]
        public void GetVaryTest1()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double oc = 23.43882153;
            double vary = solCalc.GetVary(oc);
            Assert.AreEqual(0.04303276, Math.Round(vary, 8));
        }

        [TestMethod]
        public void GetVaryTest2()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double oc = 23.43848456;
            double vary = solCalc.GetVary(oc);
            Assert.AreEqual(0.04303148, Math.Round(vary, 8));
        }

        [TestMethod]
        public void GetEqOfTimeTest1()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double gmls = 323.7065868;
            double gmas = 4000.595216;
            double eeo = 0.016704379;
            double vary = 0.043032756;
            double eot = solCalc.GetEqOfTime(gmls, gmas, eeo, vary);
            Assert.AreEqual(-14.2222, Math.Round(eot, 6));
        }

        [TestMethod]
        public void GetEqOfTimeTest2()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double gmls = 89.99265499;
            double gmas = 4126.875252;
            double eeo = 0.016704231;
            double vary = 0.043031483;
            double eot = solCalc.GetEqOfTime(gmls, gmas, eeo, vary);
            Assert.AreEqual(-1.850250, Math.Round(eot, 6));
        }

        [TestMethod]
        public void GetHASunriseTest1()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double latitude = 40.0;
            double sd = -13.19798007;
            double has = solCalc.GetHASunrise(latitude, sd);
            Assert.AreEqual(79.7883504, Math.Round(has, 7));
        }

        [TestMethod]
        public void GetHASunriseTest2()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double latitude = 40.0;
            double sd = 23.43782475;
            double has = solCalc.GetHASunrise(latitude, sd);
            Assert.AreEqual(112.609815, Math.Round(has, 6));
        }

        [TestMethod]
        public void GetSolarNoonTest1()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double longitude = -105.0;
            double offset = -7.0;
            double eot = -14.22219967;
            double sn = solCalc.GetSolarNoon(longitude, offset, eot);
            Assert.AreEqual(TimeSpan.FromDays(0.50987652754861112), TimeSpan.FromDays(sn));
        }

        [TestMethod]
        public void GetSolarNoonTest2()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double longitude = -105.0;
            double offset = -6.0;
            double eot = -1.850250206;
            double sn = solCalc.GetSolarNoon(longitude, offset, eot);
            Assert.AreEqual(TimeSpan.FromDays(0.54295156264305555), TimeSpan.FromDays(sn));
        }

        [TestMethod]
        public void GetSunriseTimeTest1()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double has = 79.78835039;
            double sn = 0.50987652754861112;
            double srt = solCalc.GetSunriseTime(has, sn);
            Assert.AreEqual(TimeSpan.FromDays(0.28824222090972224), TimeSpan.FromDays(srt));
        }

        [TestMethod]
        public void GetSunriseTimeTest2()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double has = 112.6098153;
            double sn = 0.54295156264305555;
            double srt = solCalc.GetSunriseTime(has, sn);
            Assert.AreEqual(TimeSpan.FromDays(0.23014652014305553), TimeSpan.FromDays(srt));
        }

        [TestMethod]
        public void GetSunsetTimeTest1()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double has = 79.78835039;
            double sn = 0.50987652754861112;
            double sst = solCalc.GetSunsetTime(has, sn);
            Assert.AreEqual(TimeSpan.FromDays(0.73151083418750007), TimeSpan.FromDays(sst));
        }

        [TestMethod]
        public void GetSunsetTimeTest2()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double has = 112.6098153;
            double sn = 0.54295156264305555;
            double sst = solCalc.GetSunsetTime(has, sn);
            Assert.AreEqual(TimeSpan.FromDays(0.85575660514305552), TimeSpan.FromDays(sst));
        }

        [TestMethod]
        public void GetSunlightDurationTest1()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double has = 79.78835039;
            double sd = solCalc.GetSunlightDuration(has);
            Assert.AreEqual(638.3068, Math.Round(sd, 5));
        }

        [TestMethod]
        public void GetSunlightDurationTest2()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double has = 112.6098153;
            double sd = solCalc.GetSunlightDuration(has);
            Assert.AreEqual(900.87852, Math.Round(sd, 5));
        }

        [TestMethod]
        public void GetTrueSolarTimeTest1()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double longitude = -105.00;
            double offset = -7.0;
            double hours = new TimeSpan(12, 0, 0).TotalDays;
            double eot = -14.22219967;
            double tst = solCalc.GetTrueSolarTime(longitude, offset, hours, eot);
            Assert.AreEqual(705.7778003, Math.Round(tst, 7));
        }

        [TestMethod]
        public void GetTrueSolarTimeTest2()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double longitude = -105.00;
            double offset = -6.0;
            double hours = new TimeSpan(16, 0, 0).TotalDays;
            double eot = -1.850250206;
            double tst = solCalc.GetTrueSolarTime(longitude, offset, hours, eot);
            Assert.AreEqual(898.1497498, Math.Round(tst, 7));
        }

        [TestMethod]
        public void GetHourAngleTest1()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double tst = 705.7778003;
            double ha = solCalc.GetHourAngle(tst);
            Assert.AreEqual(-3.5555499, Math.Round(ha, 7));
        }

        [TestMethod]
        public void GetHourAngleTest2()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double tst = 898.1497498;
            double ha = solCalc.GetHourAngle(tst);
            Assert.AreEqual(44.53743745, Math.Round(ha, 8));
        }

        [TestMethod]
        public void GetSolarZenithAngleTest1()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double latitude = 40.0;
            double sd = -13.19798007;
            double ha = -3.555549918;
            double sza = solCalc.GetSolarZenithAngle(latitude, sd, ha);
            Assert.AreEqual(53.30063602, Math.Round(sza, 8));
        }

        [TestMethod]
        public void GetSolarZenithAngleTest2()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double latitude = 40.0;
            double sd = 23.43782475;
            double ha = 44.53743745;
            double sza = solCalc.GetSolarZenithAngle(latitude, sd, ha);
            Assert.AreEqual(40.83025006, Math.Round(sza, 8));
        }

        [TestMethod]
        public void GetSolarElevationAngleTest1()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double sza = 53.30063602;
            double sea = solCalc.GetSolarElevationAngle(sza);
            Assert.AreEqual(36.69936398, Math.Round(sea, 8));
        }

        [TestMethod]
        public void GetSolarElevationAngleTest2()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double sza = 40.83025006;
            double sea = solCalc.GetSolarElevationAngle(sza);
            Assert.AreEqual(49.16974994, Math.Round(sea, 8));
        }

        [TestMethod]
        public void GetApproxAtmRefractionTest1()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double sea = 36.69936398;
            double aar = solCalc.GetApproxAtmRefraction(sea);
            Assert.AreEqual(0.021605629, Math.Round(aar, 9));
        }

        [TestMethod]
        public void GetApproxAtmRefractionTest2()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double sea = 49.16974994;
            double aar = solCalc.GetApproxAtmRefraction(sea);
            Assert.AreEqual(0.013933057, Math.Round(aar, 9));
        }

        [TestMethod]
        public void GetSolarElevationCorrectedTest1()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double sea = 36.69936398;
            double aar = 0.021605629;
            double sec = solCalc.GetSolarElevationCorrected(sea, aar);
            Assert.AreEqual(36.7209696, Math.Round(sec, 7));
        }

        [TestMethod]
        public void GetSolarElevationCorrectedTest2()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double sea = 49.16974994;
            double aar = 0.013933057;
            double sec = solCalc.GetSolarElevationCorrected(sea, aar);
            Assert.AreEqual(49.183683, Math.Round(sec, 8));
        }

        [TestMethod]
        public void GetSolarAzimuthAngleTest1()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double latitude = 40.0;
            double ha = -3.555549918;
            double sza = 53.30063602;
            double sd = -13.19798007;
            double saa = solCalc.GetSolarAzimuthAngle(latitude, ha, sza, sd);
            Assert.AreEqual(175.68125446, Math.Round(saa, 8));
        }

        [TestMethod]
        public void GetSolarAzimuthAngleTest2()
        {
            SolarCalculator solCalc = new SolarCalculator();
            double latitude = 40.0;
            double ha = 44.53743745;
            double sza = 40.83025006;
            double sd = 23.43782475;
            double saa = solCalc.GetSolarAzimuthAngle(latitude, ha, sza, sd);
            Assert.AreEqual(259.80956393, Math.Round(saa, 8));
        }
    }
}
