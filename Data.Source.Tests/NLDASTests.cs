using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Data.Source;

namespace Data.Source.Tests
{
    [TestClass]
    public class NLDASTests
    {

        Precipitation.Precipitation timeSeries = new Precipitation.Precipitation();
        TimeSeriesInput validInput = new TimeSeriesInput()
        {
            Source = "nldas",
            DateTimeSpan = new DateTimeSpan()
            {
                StartDate = new DateTime(2015, 01, 01),
                EndDate = new DateTime(2015, 01, 08)
            },
            Geometry = new TimeSeriesGeometry()
            {
                Point = new PointCoordinate()
                {
                    Latitude = 33.925673,
                    Longitude = -83.355723
                },
                Timezone = new Timezone()
                {
                    Name = "EST",
                    Offset = -5,
                    DLS = true
                }
            }
        };

        private void BuildITimeSeries()
        {
            string errorMsg = "";
            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            timeSeries.Input = iFactory.SetTimeSeriesInput(validInput, new List<string>() { "precipitation" }, out errorMsg);
        }

        // NLDAS remaining public Methods to be implemented

        // ConstructURL(out string errorMsg, string dataset, ITimeSeriesInput cInput)
        // DetermineReturnCoordinates(out string errorMsg, IPointCoordinate point)
        // SetDataToOutput(out string errorMsg, string dataset, string data, ITimeSeriesOutput output, ITimeSeriesInput input)
        // CheckStatus(string dataset, ITimeSeriesInput testInput)


        [TestMethod]
        public void GetDataValidTest()
        {
            BuildITimeSeries();
            NLDAS nldas = new NLDAS();
            string errorMsg = "";

            string dataset = "Precipitation";
            string data = nldas.GetData(out errorMsg, dataset, timeSeries.Input);
            Assert.AreEqual("", errorMsg);
        }

        [TestMethod]
        public void GetDataNoBaseURL()
        {
            BuildITimeSeries();
            NLDAS nldas = new NLDAS();
            string errorMsg = "";

            string dataset = "Precipitation";
            timeSeries.Input.BaseURL = new List<string> { "" };
            string data = nldas.GetData(out errorMsg, dataset, timeSeries.Input);
            Assert.AreEqual("ERROR: Unable to download requested nldas data. Invalid URI: The format of the URI could not be determined.", errorMsg);
        }

        /// <summary>
        /// Valid test for AdjustForOffset, base
        /// </summary>
        [TestMethod]
        public void AdjustForOffsetValidTest1()
        {
            BuildITimeSeries();
            NLDAS nldas = new NLDAS();
            string errorMsg = "";
            timeSeries.Input.TimeLocalized = true;
            timeSeries.Input.DateTimeSpan = NLDAS.AdjustForOffset(out errorMsg, timeSeries.Input) as DateTimeSpan;
            Assert.AreEqual("", errorMsg);
        }

        /// <summary>
        /// Valid test for AdjustForOffset, TimeLocalized = true and Offset = -7
        /// </summary>
        [TestMethod]
        public void AdjustForOffsetValidTest2()
        {
            BuildITimeSeries();
            NLDAS nldas = new NLDAS();
            string errorMsg = "";
            timeSeries.Input.TimeLocalized = true;
            timeSeries.Input.Geometry.Timezone.Offset = -7;
            timeSeries.Input.DateTimeSpan = NLDAS.AdjustForOffset(out errorMsg, timeSeries.Input) as DateTimeSpan;
            string startDate = timeSeries.Input.DateTimeSpan.StartDate.ToString();
            string endDate = timeSeries.Input.DateTimeSpan.EndDate.ToString();
            Assert.AreEqual("1/1/2015 8:00:00 AM", startDate);
            Assert.AreEqual("1/9/2015 7:00:00 AM", endDate);
        }

        /// <summary>
        /// Valid test for AdjustForOffset, TimeLocalized = true and Offset = 7
        /// </summary>
        [TestMethod]
        public void AdjustForOffsetValidTest3()
        {
            BuildITimeSeries();
            NLDAS nldas = new NLDAS();
            string errorMsg = "";
            timeSeries.Input.TimeLocalized = true;
            timeSeries.Input.Geometry.Timezone.Offset = 7;
            timeSeries.Input.DateTimeSpan = NLDAS.AdjustForOffset(out errorMsg, timeSeries.Input) as DateTimeSpan;
            string startDate = timeSeries.Input.DateTimeSpan.StartDate.ToString();
            string endDate = timeSeries.Input.DateTimeSpan.EndDate.ToString();
            Assert.AreEqual("12/31/2014 5:00:00 PM", startDate);
            Assert.AreEqual("1/8/2015 5:00:00 PM", endDate);
        }

        /// <summary>
        /// Valid test for AdjustForOffset, TimeLocalized = false
        /// </summary>
        [TestMethod]
        public void AdjustForOffsetValidTest4()
        {
            BuildITimeSeries();
            NLDAS nldas = new NLDAS();
            string errorMsg = "";
            timeSeries.Input.TimeLocalized = false;
            timeSeries.Input.Geometry.Timezone.Offset = 7;
            timeSeries.Input.DateTimeSpan = NLDAS.AdjustForOffset(out errorMsg, timeSeries.Input) as DateTimeSpan;
            string startDate = timeSeries.Input.DateTimeSpan.StartDate.ToString();
            string endDate = timeSeries.Input.DateTimeSpan.EndDate.ToString();
            Assert.AreEqual("1/1/2015 1:00:00 AM", startDate);
            Assert.AreEqual("1/9/2015 12:00:00 AM", endDate);
        }

        /// <summary>
        /// Invalid test for AdjustForOffset, TimeLocalized = true, Offset = 24
        /// </summary>
        [TestMethod]
        public void AdjustForOffsetInvalidOffsetTest()
        {
            BuildITimeSeries();
            NLDAS nldas = new NLDAS();
            string errorMsg = "";
            timeSeries.Input.TimeLocalized = true;
            timeSeries.Input.Geometry.Timezone.Offset = 24;
            timeSeries.Input.DateTimeSpan = NLDAS.AdjustForOffset(out errorMsg, timeSeries.Input) as DateTimeSpan;
            string startDate = timeSeries.Input.DateTimeSpan.StartDate.ToString();
            string endDate = timeSeries.Input.DateTimeSpan.EndDate.ToString();
            Assert.AreEqual("12/31/2014 12:00:00 AM", startDate);
        }

        /// <summary>
        /// Valid test for SetDateToLocal, offset = -5
        /// </summary>
        [TestMethod]
        public void SetDateToLocalValidTest1()
        {
            BuildITimeSeries();
            string startDate = NLDAS.SetDateToLocal(
                -5,
                this.timeSeries.Input.DateTimeSpan.StartDate.ToString("yyyy-MM-dd HH"),
                this.timeSeries.Input.DateTimeSpan.DateTimeFormat);
            string expectedStartDate = "2014-12-31 19";
            Assert.AreEqual(expectedStartDate, startDate);
        }

        /// <summary>
        /// Valid test for SetDateToLocal, offset = 5
        /// </summary>
        [TestMethod]
        public void SetDateToLocalValidTest2()
        {
            BuildITimeSeries();
            string startDate = NLDAS.SetDateToLocal(
                5,
                this.timeSeries.Input.DateTimeSpan.StartDate.ToString("yyyy-MM-dd HH"),
                this.timeSeries.Input.DateTimeSpan.DateTimeFormat);
            string expectedStartDate = "2015-01-01 05";
            Assert.AreEqual(expectedStartDate, startDate);
        }

        /// <summary>
        /// Invalid test for SetDateToLocal, invalid DateTimeFormat
        /// </summary>
        [TestMethod]
        public void SetDateToLocalInvalidTest()
        {
            BuildITimeSeries();
            string startDate = NLDAS.SetDateToLocal(
                -5,
                this.timeSeries.Input.DateTimeSpan.StartDate.ToString("yyyy-MM-dd HH"),
                "yyyyyyy");
            Assert.AreEqual("0002014", startDate);
        }
    }
}