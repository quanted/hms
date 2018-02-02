using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

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
            timeSeries.Input = iFactory.SetTimeSeriesInput(validInput, new List<string>() { "PRECIP" }, out errorMsg);
        }

        // NLDAS public Methods to be tested

        // GetData(out string errorMsg, string dataset, ITimeSeriesInput componentInput)
        // AdjustForOffset(out string errorMsg, ITimeSeriesInput cInput)
        // SetDateToLocal(double offset, string dateHour, string dateFormat)
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
            timeSeries.Input.BaseURL = new List<string>{ "" };
            string data = nldas.GetData(out errorMsg, dataset, timeSeries.Input);
            Assert.AreEqual("ERROR: Unable to download requested nldas data. Invalid URI: The format of the URI could not be determined.", errorMsg);
        }


        [TestMethod]
        public void AdjustForOffsetValid()
        {
            BuildITimeSeries();
            NLDAS nldas = new NLDAS();
            string errorMsg = "";
            timeSeries.Input.TimeLocalized = true;
            string dataset = "Precipitation";
            timeSeries.Input.DateTimeSpan = NLDAS.AdjustForOffset(out errorMsg, timeSeries.Input) as DateTimeSpan;
            Assert.AreEqual("", errorMsg);
        }

        //TODO: AdjustForOffsetUnitTests, 1. Valid test, 2. Valid test for Offset -7 and timelocalized=true, 3. Valid test for Offset +7 and timelocalized=true, 4. Valid test with timelocalized=false, 5. Invalid timezone range
    }
}
