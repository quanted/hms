using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Data.Source.Tests
{
    [TestClass]
    public class NLDAS
    {
        /// <summary>
        /// Gets basic default TimeSeries object, containing only required fields.
        /// </summary>
        /// <returns></returns>
        public ITimeSeriesInput GetTimeSeriesObject()
        {
            ITimeSeriesInput validInput = new TimeSeriesInput();
            validInput.DateTimeSpan = new DateTimeSpan()
            {
                StartDate = new DateTime(2010, 1, 1),
                EndDate = new DateTime(2010, 1, 2)
            };
            validInput.Geometry = new TimeSeriesGeometry()
            {
                Point = new PointCoordinate()
                {
                    Latitude = 33,
                    Longitude = -83,
                },
            };
            validInput.Source = "nldas";
            return validInput;
        }

        /// <summary>
        /// NLDAS GetData valid unit test, using basic default TimeSeries object.
        /// Will always fail due to Application["url_list"] not being loaded for unit tests, will need a workaround.
        /// </summary>
        [TestMethod]
        public void GetDataValid()
        {
            string errorMsg = "";
            string dataset = "PRECIP";
            ITimeSeriesInput input = GetTimeSeriesObject();
            Data.Source.NLDAS nldas = new Source.NLDAS();
            string data = nldas.GetData(out errorMsg, dataset, input);
            string containsData = "";
            try
            {
                containsData = data.Substring(data.IndexOf("Data"), 4);
            }
            finally
            {
                Assert.AreEqual("Data", containsData);
            }
        }

        /// <summary>
        /// NLDAS DetermineReturnCoordinates valid unit test, using input Point(33, -83).
        /// </summary>
        [TestMethod]
        public void DetermineReturnCoordinatesValid()
        {
            Data.Source.NLDAS nldas = new Source.NLDAS();
            string errorMsg = "";
            // List of coordinates that contains a central point and points around the bounds of the nldas boundary.
            List<IPointCoordinate> points = new List<IPointCoordinate>();
            points.Add(new PointCoordinate() { Latitude = 33, Longitude = -83 });
            points.Add(new PointCoordinate() { Latitude = 25, Longitude = -124 });
            points.Add(new PointCoordinate() { Latitude = 52, Longitude = -124 });
            points.Add(new PointCoordinate() { Latitude = 25, Longitude = -64 });
            points.Add(new PointCoordinate() { Latitude = 52, Longitude = -64 });

            List<double[]> expected = new List<double[]>();
            expected.Add(new double[2] { 33.0625, -82.9375 });
            expected.Add(new double[2] { 24.9375, -123.9375 });
            expected.Add(new double[2] { 52.0625, -123.9375 });
            expected.Add(new double[2] { 24.9375, -63.9375 });
            expected.Add(new double[2] { 52.0625, -63.9375 });

            for (int i = 0; i < points.Count; i++)
            {
                double[] result = nldas.DetermineReturnCoordinates(out errorMsg, points[i]);
                Assert.AreEqual(expected[i][0], result[0]);
                Assert.AreEqual(expected[i][1], result[1]);
            }
        }
    }
}
