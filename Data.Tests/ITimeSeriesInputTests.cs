using Microsoft.VisualStudio.TestTools.UnitTesting;
using Data;
using System;
using System.Collections.Generic;

namespace Data.Tests
{
    [TestClass]
    public class ITimeSeriesInputTests
    {
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

        /// <summary>
        /// Valid input object provided, no resulting error expected.
        /// </summary>
        [TestMethod]
        public void SetValidTimeSeriesInputTest()
        {
            string errorMsg = "";
            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            ITimeSeriesInput sampleInput = iFactory.SetTimeSeriesInput(validInput, new List<string>() { "PRECIP" }, out errorMsg);
            Assert.AreEqual("", errorMsg);
        }

        /// <summary>
        /// Invalid source provided, error expected indicating issue.
        /// </summary>
        [TestMethod]
        public void SetTimeSeriesInputTestInvalidSource()
        {
            string errorMsg = "";
            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            TimeSeriesInput badSource = new TimeSeriesInput();
            badSource = validInput;
            badSource.Source = "badSource";
            ITimeSeriesInput sampleInput = iFactory.SetTimeSeriesInput(badSource, new List<string>() { "PRECIP" }, out errorMsg);
            Assert.AreEqual("ERROR: Provided source is not valid. Unable to construct base url.", errorMsg);
        }

        /// <summary>
        /// No source provided, error expected indicating issue.
        /// </summary>
        [TestMethod]
        public void SetTimeSeriesInputTestNoSource()
        {
            string errorMsg = "";
            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            TimeSeriesInput badSource = new TimeSeriesInput();
            badSource = validInput;
            badSource.Source = "";
            ITimeSeriesInput sampleInput = iFactory.SetTimeSeriesInput(badSource, new List<string>() { "PRECIP" }, out errorMsg);
            string expectedError = "ERROR: Required 'Source' parameter was not found or is invalid., ERROR: Provided source is not valid. Unable to construct base url.";
            Assert.AreEqual(expectedError, errorMsg);
        }

        /// <summary>
        /// Invalid Dataset provided, error expected indicating issue. Error for no dataset is identical as an invalid dataset.
        /// </summary>
        [TestMethod]
        public void SetTimeSeriesInputTestInvalidDataset()
        {
            string errorMsg = "";
            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            ITimeSeriesInput sampleInput = iFactory.SetTimeSeriesInput(validInput, new List<string>() { "BadDataset" }, out errorMsg);
            Assert.AreEqual("ERROR: Unable to construct base url from the specified dataset and provided data source.", errorMsg);
        }

        /// <summary>
        /// Invalid Date range provided, error expected indicating issue.
        /// </summary>
        [TestMethod]
        public void SetTimeSeriesInputTestInvalidDate()
        {
            string errorMsg = "";
            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            TimeSeriesInput badDate = new TimeSeriesInput();
            badDate = validInput;
            badDate.DateTimeSpan.StartDate = new DateTime(2015, 01, 10);
            ITimeSeriesInput sampleInput = iFactory.SetTimeSeriesInput(badDate, new List<string>() { "PRECIP" }, out errorMsg);
            Assert.AreEqual("ERROR: Start date must be before end date.", errorMsg);
        }

        /// <summary>
        /// No date provided, error expected indicating issue.
        /// </summary>
        [TestMethod]
        public void SetTimeSeriesInputTestNoDate()
        {
            string errorMsg = "";
            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            TimeSeriesInput noSDate = new TimeSeriesInput();
            noSDate = validInput;
            noSDate.DateTimeSpan = null;
            ITimeSeriesInput sampleInput = iFactory.SetTimeSeriesInput(noSDate, new List<string>() { "PRECIP" }, out errorMsg);
            Assert.AreEqual("ERROR: DateTimeSpan object is null. DateTimeSpan, with a StartDate and EndDate, is required.", errorMsg);
        }

        /// <summary>
        /// No geometry provided, error expected indicating issue.
        /// </summary>
        [TestMethod]
        public void SetTimeSeriesInputTestNoGeometry()
        {
            string errorMsg = "";
            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            TimeSeriesInput noGeom = new TimeSeriesInput();
            noGeom = validInput;
            noGeom.Geometry = null;
            ITimeSeriesInput sampleInput = iFactory.SetTimeSeriesInput(noGeom, new List<string>() { "PRECIP" }, out errorMsg);
            Assert.AreEqual("ERROR: No geometry values found in the provided parameters.", errorMsg);
        }

        /// <summary>
        /// No point provided, error expected indicating issue.
        /// </summary>
        [TestMethod]
        public void SetTimeSeriesInputTestNoPoint()
        {
            string errorMsg = "";
            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            TimeSeriesInput noPoint = new TimeSeriesInput();
            noPoint = validInput;
            noPoint.Geometry.Point = null;
            ITimeSeriesInput sampleInput = iFactory.SetTimeSeriesInput(noPoint, new List<string>() { "PRECIP" }, out errorMsg);
            string expectedError = "ERROR: No geometry values found in the provided parameters., ERROR: Latitude or Longitude value is not a valid coordinate.";
            Assert.AreEqual(expectedError, errorMsg);
        }

        /// <summary>
        /// Invalid Latitude provided, error expected indicating issue.
        /// </summary>
        [TestMethod]
        public void SetTimeSeriesInputTestInvalidLatitude()
        {
            string errorMsg = "";
            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            TimeSeriesInput invLat = new TimeSeriesInput();
            invLat = validInput;
            invLat.Geometry.Point.Latitude = 100;
            ITimeSeriesInput sampleInput = iFactory.SetTimeSeriesInput(invLat, new List<string>() { "PRECIP" }, out errorMsg);
            Assert.AreEqual("ERROR: Latitude or Longitude value is not a valid coordinate.", errorMsg);
        }

        /// <summary>
        /// Invalid Longitude provided, error expected indicating issue.
        /// </summary>
        [TestMethod]
        public void SetTimeSeriesInputTestInvalidLongitude()
        {
            string errorMsg = "";
            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            TimeSeriesInput invLon = new TimeSeriesInput();
            invLon = validInput;
            invLon.Geometry.Point.Latitude = 200;
            ITimeSeriesInput sampleInput = iFactory.SetTimeSeriesInput(invLon, new List<string>() { "PRECIP" }, out errorMsg);
            Assert.AreEqual("ERROR: Latitude or Longitude value is not a valid coordinate.", errorMsg);
        }

    }
}
