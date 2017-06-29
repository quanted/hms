using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Data;
using System.IO;
using Web.Services.Controllers;
using System.Collections.Generic;
using Precipitation;

namespace UnitTests
{
    [TestClass]
    public class NLDAS
    {

        [TestMethod]
        public void GetDataTest()
        {
            ITimeSeriesInput input = ConstructTestInput();
            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput output = oFactory.Initialize();
            Precipitation.NLDAS nldas = new Precipitation.NLDAS();
            output = nldas.GetData(out string errorMsg, output, input);
            Assert.AreEqual("", "1");

        }

        /// <summary>
        /// Unit conversion test, default metric (mm) to imperial (in) conversion of precip data.
        /// </summary>
        [TestMethod]
        public void UnitConversionTest()
        {
            ITimeSeriesInput input = ConstructTestInput();
            ITimeSeriesOutput output = ConstructTestOutput(input);

            Dictionary<string, List<string>> convertedData = Precipitation.NLDAS.UnitConversion(out string errorMsg, 1.0, output, input);
            Assert.AreEqual("5.028E-002", convertedData["2015-01-01 21"][0]);
            Assert.AreEqual("3.340E-002", convertedData["2015-01-03 18"][0]);
            Assert.AreEqual("0.000E+000", convertedData["2014-12-31 19"][0]);
        }

        /// <summary>
        /// Daily aggregation test of nldas precip data.
        /// </summary>
        [TestMethod]
        public void DailyAggregationTest()
        {
            ITimeSeriesInput input = ConstructTestInput();
            ITimeSeriesOutput output = ConstructTestOutput(input);

            Dictionary<string, List<string>> dailyValues = Precipitation.NLDAS.DailyAggregatedSum(out string errorMsg, 1.0, output, input);
            Assert.AreEqual("3.085E+000", dailyValues["2015-01-01 00"][0]);
            Assert.AreEqual("0.000E+000", dailyValues["2015-01-10 00"][0]);
            Assert.AreEqual("0.000E+000", dailyValues["2015-01-21 00"][0]);
        }

        /// <summary>
        /// Weekly aggregation test of nldas precip data.
        /// </summary>
        [TestMethod]
        public void WeeklyAggregationTest()
        {
            ITimeSeriesInput input = ConstructTestInput();
            ITimeSeriesOutput output = ConstructTestOutput(input);

            Dictionary<string, List<string>> weeklyValues = Precipitation.NLDAS.WeeklyAggregatedSum(out string errorMsg, 1.0, output, input);
            Assert.AreEqual("5.739E+000", weeklyValues["2015-01-07 01"][0]);
            Assert.AreEqual("2.911E-001", weeklyValues["2015-01-14 01"][0]);
            Assert.AreEqual("2.784E+001", weeklyValues["2015-01-21 01"][0]);
        }

        /// <summary>
        /// Monthly aggregation test of nldas precip data.
        /// </summary>
        [TestMethod]
        public void MonthlyAggregationTest()
        {
            ITimeSeriesInput input = ConstructTestInput();
            ITimeSeriesOutput output = ConstructTestOutput(input);

            Dictionary<string, List<string>> monthlyValues = Precipitation.NLDAS.MonthlyAggregatedSum(out string errorMsg, 1.0, output, input);
            Assert.AreEqual("1.035E+002", monthlyValues["2015-01-01 00"][0]);
            Assert.AreEqual("1.107E+002", monthlyValues["2015-02-01 00"][0]);
            Assert.AreEqual("1.025E+002", monthlyValues["2015-03-01 00"][0]);
        }

        /// <summary>
        /// Constructs sample input object for unit tests
        /// </summary>
        /// <returns></returns>
        private ITimeSeriesInput ConstructTestInput()
        {
            PrecipitationInput sampleInput = new PrecipitationInput()
            {
                Source = "nldas",
                DateTimeSpan = new DateTimeSpan()
                {
                    StartDate = new DateTime(2015, 01, 01),
                    EndDate = new DateTime(2015, 01, 08),
                    DateTimeFormat = "yyyy-MM-dd HH"
                },
                Geometry = new TimeSeriesGeometry()
                {
                    Description = "EPA Athens Office",
                    Point = new PointCoordinate()
                    {
                        Latitude = 33.925673,
                        Longitude = -83.355723
                    },
                    GeometryMetadata = new Dictionary<string, string>()
                    {
                        { "City", "Athens" },
                        { "State", "Georgia"},
                        { "Country", "United States" }
                    },
                    Timezone = new Timezone()
                    {
                        Name = "EST",
                        Offset = -5,
                        DLS = false
                    }
                },
                DataValueFormat = "E3",
                TemporalResolution = "default",
                TimeLocalized = true,
                Units = "default",
                OutputFormat = "json"
            };
            return sampleInput;
        }

        /// <summary>
        /// Constructs sample output object for unit tests
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private ITimeSeriesOutput ConstructTestOutput(ITimeSeriesInput input)
        {
            string sampleOutputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "precipSample.txt");
            string data = "";
            using( StreamReader sr = new StreamReader(sampleOutputPath, System.Text.Encoding.UTF8))
            {
                data = sr.ReadToEnd();
            }

            Data.Source.NLDAS nldas = new Data.Source.NLDAS();
            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput output = oFactory.Initialize();

            output = nldas.SetDataToOutput(out string errorMsg, "Precipitation", data, output, input);
            return output;
        }

    }
}
