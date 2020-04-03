using System;
using System.Collections.Generic;
using Xunit;

namespace Data.Tests
{

    public class ITimeSeriesInputUnitTests
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
        [Fact]
        public void SetTimeSeriesInput()
        {
            string errorMsg = "";
            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            ITimeSeriesInput sampleInput = iFactory.SetTimeSeriesInput(validInput, new List<string>() { "precipitation" }, out errorMsg);
            Assert.Equal("", errorMsg);
        }

        /// <summary>
        /// Test source parameter
        /// </summary>
        [Theory]
        [InlineData("nldas", "")]
        [InlineData("wgen", "")]
        [InlineData("NLDAS", "")]
        [InlineData("", "ERROR: Source not found in input.")]
        [InlineData("123456", "ERROR: Source is not valid or not compatible with dataset. Dataset value(s): precipitation. Source value: 123456")]
        [InlineData("nldasas", "ERROR: Source is not valid or not compatible with dataset. Dataset value(s): precipitation. Source value: nldasas")]
        public void SetTimeSeriesInput_SourceTest(string source, string expected)
        {
            string errorMsg = "";
            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            TimeSeriesInput badSource = new TimeSeriesInput();
            badSource = validInput;
            badSource.Source = source;
            ITimeSeriesInput sampleInput = iFactory.SetTimeSeriesInput(badSource, new List<string>() { "precipitation" }, out errorMsg);
            Assert.Equal(expected, errorMsg);
        }

        /// <summary>
        /// Test dataset parameter
        /// </summary>
        [Theory]
        [InlineData("", "ERROR: Dataset is not valid. Dataset value(s): , ERROR: Unable to construct base url from the specified dataset and provided data source.")]
        [InlineData("precipitation", "")]
        [InlineData("asdfhlkajshd", "ERROR: Dataset is not valid. Dataset value(s): asdfhlkajshd, ERROR: Unable to construct base url from the specified dataset and provided data source.")]
        public void SetTimeSeriesInput_DatasetTest(string dataset, string expected)
        {
            string errorMsg = "";
            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            ITimeSeriesInput sampleInput = iFactory.SetTimeSeriesInput(validInput, new List<string>() { dataset }, out errorMsg);
            Assert.Equal(expected, errorMsg);
        }

        /// <summary>
        /// Test startdate parameter
        /// </summary>
        [Theory]
        [InlineData(2010, 10, 12, "")]
        [InlineData(0, 0, 0, "ERROR: Required 'StartDate' parameter was not found or is invalid.")]
        [InlineData(2016, 01, 01, "ERROR: Start date must be before end date.")]
        public void SetTimeSeriesInput_StartDateTest(int year, int month, int day, string expected)
        {
            string errorMsg = "";
            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            TimeSeriesInput dateTest = new TimeSeriesInput();
            dateTest = validInput;
            if (year == 0)
            {
                dateTest.DateTimeSpan.StartDate = DateTime.MinValue;
            }
            else
            {
                dateTest.DateTimeSpan.StartDate = new DateTime(year, month, day);
            }
            ITimeSeriesInput sampleInput = iFactory.SetTimeSeriesInput(dateTest, new List<string>() { "precipitation" }, out errorMsg);
            Assert.Equal(expected, errorMsg);
        }

        /// <summary>
        /// Test enddate parameter
        /// </summary>
        [Theory]
        [InlineData(2016, 12, 31, "")]
        [InlineData(0, 0, 0, "ERROR: Required 'EndDate' parameter was not found or is invalid.")]
        [InlineData(2010, 01, 01, "ERROR: Start date must be before end date.")]
        public void SetTimeSeriesInput_EndDateTest(int year, int month, int day, string expected)
        {
            string errorMsg = "";
            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            TimeSeriesInput dateTest = new TimeSeriesInput();
            dateTest = validInput;
            if (year == 0)
            {
                dateTest.DateTimeSpan.EndDate = DateTime.MinValue;
            }
            else
            {
                dateTest.DateTimeSpan.EndDate = new DateTime(year, month, day);
            }
            ITimeSeriesInput sampleInput = iFactory.SetTimeSeriesInput(dateTest, new List<string>() { "precipitation" }, out errorMsg);
            Assert.Equal(expected, errorMsg);
        }

        /// <summary>
        /// Test dateTimeSpan parameter
        /// </summary>
        [Fact]
        public void SetTimeSeriesInput_DateTimeSpanTest()
        {
            string errorMsg = "";
            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            TimeSeriesInput noSDate = new TimeSeriesInput();
            noSDate = validInput;
            noSDate.DateTimeSpan = null;
            ITimeSeriesInput sampleInput = iFactory.SetTimeSeriesInput(noSDate, new List<string>() { "precipitation" }, out errorMsg);
            Assert.Equal("ERROR: No DateTimeSpan found.", errorMsg);
        }

        /// <summary>
        /// Test geometry parameter
        /// </summary>
        [Fact]
        public void SetTimeSeriesInputTest_GeometryTest()
        {
            string errorMsg = "";
            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            TimeSeriesInput noGeom = new TimeSeriesInput();
            noGeom = validInput;
            noGeom.Geometry = null;
            ITimeSeriesInput sampleInput = iFactory.SetTimeSeriesInput(noGeom, new List<string>() { "precipitation" }, out errorMsg);
            Assert.Equal("ERROR: No input geometry was found.", errorMsg);
        }

        /// <summary>
        /// Test latitude/longitude parameters
        /// </summary>
        [Theory]
        [InlineData(33.925673, -83.355723, "")]
        [InlineData(100.0, -83.355723, "ERROR: Latitude value is not valid. Latitude must be between -90 and 90. Latitude: 100, ERROR: A valid COM ID was not found.")]
        [InlineData(33.925673, 200.0, "ERROR: Longitude value is not valid. Longitude must be between -180 and 180. Longitude: 200, ERROR: A valid COM ID was not found.")]
        public void SetTimeSeriesInput_LatLongTest(double latitude, double longitude, string expected)
        {
            string errorMsg = "";
            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            TimeSeriesInput latLongTest = new TimeSeriesInput();
            latLongTest = validInput;
            latLongTest.Geometry.Point.Latitude = latitude;
            latLongTest.Geometry.Point.Longitude = longitude;
            ITimeSeriesInput sampleInput = iFactory.SetTimeSeriesInput(latLongTest, new List<string>() { "precipitation" }, out errorMsg);
            Assert.Equal(expected, errorMsg);
        }
    }
}