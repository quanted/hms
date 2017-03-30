using Microsoft.VisualStudio.TestTools.UnitTesting;
using HMSGDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMSGDAL.Tests
{
    [TestClass()]
    public class HMSGDALTests
    {

        //Valid test coordinates and cooresponding gmt offset.
        double[] test_latitude = new double[] { 52.0, 42.0, 38.0, -17.0, 28.0, 30.0, -25.0, -42.0, -23.0, 33.0, 51.0, 63 };
        double[] test_longitude = new double[] { 0.0, 13.0, 23.0, 46.0, 77.0, 104.0, 131.0, 173.0, -46.0, -81.0, -115.0, -151.0 };
        double[] test_gmtOffset = new double[] { 0.00, 1.00, 2.00, 3.00, 5.50, 8.00, 9.50, 12.0, -3.00, -5.00, -7.00, -9.00 };

        /// <summary>
        /// Executes a unit test on the GetGMTOffset method, using the test double arrays for inputs and expected outputs. All results are expected to be valid.
        /// </summary>
        [TestMethod()]
        public void GetGMTOffsetSerialTest()
        {
            string errorMsg = "";
            HMSTimeSeries.HMSTimeSeries ts = new HMSTimeSeries.HMSTimeSeries();
            HMSGDAL testGDAL = new HMSGDAL();
            double results;
            for (int i = 0; i < test_gmtOffset.Length; i++)
            {
                results = testGDAL.GetGMTOffset(out errorMsg, test_latitude[i], test_longitude[i], ts);
                Assert.AreEqual(test_gmtOffset[i], results);
            }
        }

        //Invalid test coordinates
        double[] invalid_latitude = new double[] { 181.0, 0.0, 181.0, -181.0, 0.0, -181.0 };
        double[] invalid_longitude = new double[] { 0.0, 181.0, 181.0, 0.0, -181.0, -181.0 };

        /// <summary>
        /// Executes a unit test on the GetGMTOffset method, using the invalid test arrays. All results expected to be invalid.
        /// </summary>
        [TestMethod()]
        public void GetGMTOffsetInvalidCoordinates()
        {
            string errorMsg = "";
            HMSTimeSeries.HMSTimeSeries ts = new HMSTimeSeries.HMSTimeSeries();
            HMSGDAL testGDAL = new HMSGDAL();
            string result = "";
            for (int i = 0; i < invalid_latitude.Length; i++)
            {
                double results = testGDAL.GetGMTOffset(out errorMsg, invalid_latitude[i], invalid_longitude[i], ts);
                if (i == 1 || i == 4) { result = "ERROR: Invalid longitude value."; }
                else { result = "ERROR: Invalid latitude value."; }
                Assert.AreEqual(result, errorMsg);
            }
        }

        //Valid date transform values
        string[] test_initial_dates = new string[] { "2000-01-01 00", "1999-12-31 23", "2000-01-01 10", "1999-12-31 22", "2000-01-01 09", "2000-01-01 12", "2000-01-01 00", "2000-01-01 03", "2000-01-01 02", "2000-01-01 08", "1999-12-31 10"};
        double[] test_dates_offsets = new double[] { 0.0, 1.0, 3.0, 6.0, 9.0, 12.0, -1.0, -3.0, -6.0, -9.0, -12.0 };
        string[] test_final_dates = new string[] { "2000-01-01 00Z", "2000-01-01 00Z", "2000-01-01 13Z", "2000-01-01 04Z", "2000-01-01 18Z", "2000-01-02 00Z", "1999-12-31 23Z", "2000-01-01 00Z", "1999-12-31 20Z", "1999-12-31 23Z", "1999-12-30 22Z"};

        /// <summary>
        /// Executes a unit test on SetDateToLocal method, using valid date arrays. All results expected to be valid..
        /// </summary>
        [TestMethod()]
        public void SetDateToLocalSerialTest()
        {
            string errorMsg = "";
            HMSGDAL testGDAL = new HMSGDAL();
            string results = "";
            for (int i = 0; i < test_final_dates.Length; i++)
            {
                results = testGDAL.SetDateToLocal(out errorMsg, test_initial_dates[i], test_dates_offsets[i]);
                Assert.AreEqual(test_final_dates[i], results);
            }
        }

        //Invalid offset transform values
        double[] invalid_offset = new double[] { 13.0, -13.0 };
        //Invalid date transform values
        string[] invalid_date = new string[] { "2001-13-01 00", "0001-01-01 00", "2001-01-32 00", "2001-01-01 25", "notadate", "12345678" };

        /// <summary>
        /// Executes a unit test on SetDateToLocal method, using invalid offset array, followed by invalid dates. All results expected to be invalid.
        /// </summary>
        [TestMethod()]
        public void SetDateToLocalInvalidDates()
        {
            string errorMsg = "";
            HMSGDAL testGDAL = new HMSGDAL();
            string results = "";
            for (int i = 0; i < invalid_offset.Length; i++) {
                results = testGDAL.SetDateToLocal(out errorMsg, "2001-01-01 00", invalid_offset[i]);
                Assert.AreEqual("ERROR: Invalid offset.", errorMsg);
            }
            for (int i = 0; i < invalid_offset.Length; i++)
            {
                results = testGDAL.SetDateToLocal(out errorMsg, invalid_date[i], 1.0);
                Assert.AreEqual("ERROR: Invalid date provided, unable to convert to local date.", errorMsg);
            }
        }

        //Expected values for dates using the test_dates_offsets array on 2000-01-01 00, with start=true.
        DateTime[] valid_datetime_S = new DateTime[]
        {
            new DateTime(2000, 01, 01, 00, 00, 00),
            new DateTime(1999, 12, 31, 23, 00, 00),
            new DateTime(1999, 12, 31, 21, 00, 00),
            new DateTime(1999, 12, 31, 18, 00, 00),
            new DateTime(1999, 12, 31, 15, 00, 00),
            new DateTime(1999, 12, 31, 12, 00, 00),
            new DateTime(2000, 01, 01, 01, 00, 00),
            new DateTime(2000, 01, 01, 03, 00, 00),
            new DateTime(2000, 01, 01, 06, 00, 00),
            new DateTime(2000, 01, 01, 09, 00, 00),
            new DateTime(2000, 01, 01, 12, 00, 00)

        };

        //Expected values for dates using the test_dates_offsets array on 2000-01-01 00, with start=false.
        DateTime[] valid_datetime_F = new DateTime[]
        {
            new DateTime(2000, 01, 01, 23, 00, 00),
            new DateTime(2000, 01, 01, 22, 00, 00),
            new DateTime(2000, 01, 01, 20, 00, 00),
            new DateTime(2000, 01, 01, 17, 00, 00),
            new DateTime(2000, 01, 01, 14, 00, 00),
            new DateTime(2000, 01, 01, 11, 00, 00),
            new DateTime(2000, 01, 02, 00, 00, 00),
            new DateTime(2000, 01, 02, 02, 00, 00),
            new DateTime(2000, 01, 02, 05, 00, 00),
            new DateTime(2000, 01, 02, 08, 00, 00),
            new DateTime(2000, 01, 02, 11, 00, 00)
        };

        /// <summary>
        /// Executes a unit test on AdjustDateByOffset method, tests output DateTime objects using the test_dates_offsets for start=true and start=false.All results expected to be valid.
        /// </summary>
        [TestMethod()]
        public void AdjustDateByOffsetSerialTest()
        {
            string errorMsg = "";
            DateTime result = new DateTime();
            DateTime testDate = new DateTime(2000, 01, 01, 00, 00, 00);
            HMSGDAL testGDAL = new HMSGDAL();
            for (int i = 0; i < valid_datetime_S.Length; i++)           //checking DateTime values from offset list for start=true
            {
                result = testGDAL.AdjustDateByOffset(out errorMsg, test_dates_offsets[i], testDate, true);
                Assert.AreEqual(valid_datetime_S[i], result);
            }
            for (int i = 0; i < valid_datetime_F.Length; i++)           //checking DateTime values from offset list for start=false
            {
                result = testGDAL.AdjustDateByOffset(out errorMsg, test_dates_offsets[i], testDate, false);
                Assert.AreEqual(valid_datetime_F[i], result);
            }
        } 

        /// <summary>
        /// Executes a unit test on ReturnCentroid method using a sample catchment shapefile.
        /// </summary>
        [TestMethod()]
        public void ReturnCentroidTestResult()
        {
            string errorMsg = "";
            string shapefileName = String.Concat(AppDomain.CurrentDomain.BaseDirectory, @"\Sample Catchment\cat");
            HMSGDAL testGDAL = new HMSGDAL();
            double[] result = testGDAL.ReturnCentroid(out errorMsg, shapefileName);
            string expected = "40.6026199171982, -109.577803299712";                            // Expected results
            Assert.AreEqual(expected, result[0].ToString() + ", " + result[1].ToString());
        }

        /// <summary>
        /// Executes a unit test on ReturnCentroid method using an invalid catchment shapefile path. Exception expected.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(NullReferenceException))]
        public void ReturnCentroidTestInvalidPath()
        {
            string errorMsg = "";
            string shapefileName = String.Concat(AppDomain.CurrentDomain.BaseDirectory, @"\Folder that does not exist\");
            HMSGDAL testGDAL = new HMSGDAL();
            double[] result = testGDAL.ReturnCentroid(out errorMsg, shapefileName);
        }

        /// <summary>
        /// Executes a unit test on CellAreaInShapefile method testing for coordinate count and area precentages. Results expected to be valid.
        /// </summary>
        [TestMethod()]
        public void CellAreaInShapefileValidResults()
        {
            string errorMsg = "";
            HMSGDAL gdalTest = new HMSGDAL();
            HMSLDAS.HMSLDAS gldas = new HMSLDAS.HMSLDAS();
            string shapefileName = String.Concat(AppDomain.CurrentDomain.BaseDirectory, @"\Sample Catchment\cat");
            bool sourceNLDAS = true;
            double[] center = gldas.DetermineReturnCoordinates(out errorMsg, gdalTest.ReturnCentroid(out errorMsg, shapefileName), sourceNLDAS);
            gdalTest.CellAreaInShapefileByGrid(out errorMsg, center, 0.1250);
            Assert.AreEqual(10, gdalTest.coordinatesInShapefile.Count);
            Assert.AreEqual(1.0, gdalTest.areaPrecentInGeometry[0]);
            Assert.AreEqual(0.02467, Math.Round(gdalTest.areaPrecentInGeometry[8], 5));
            gdalTest.CellAreaInShapefileByGrid(out errorMsg, center, 0.250);
            Assert.AreEqual(6, gdalTest.coordinatesInShapefile.Count);
            Assert.AreEqual(1.0, gdalTest.areaPrecentInGeometry[0]);
            Assert.AreEqual(0.37095, Math.Round(gdalTest.areaPrecentInGeometry[2], 5));
        }

        /// <summary>
        /// Executes a unit test on ReturnCentroid method with coordinate not inside shapefile, result is count=0, and with cellwidths of negative values. 
        /// </summary>
        [TestMethod()]
        public void CellAreaInShapefileInValidResults()
        {
            string errorMsg = "";
            HMSGDAL gdalTest = new HMSGDAL();
            HMSLDAS.HMSLDAS gldas = new HMSLDAS.HMSLDAS();
            string shapefileName = String.Concat(AppDomain.CurrentDomain.BaseDirectory, @"\Sample Catchment\cat");
            double[] center = new double[] { 0.0, 0.0 };
            gldas.DetermineReturnCoordinates(out errorMsg, gdalTest.ReturnCentroid(out errorMsg, shapefileName), true);
            gdalTest.CellAreaInShapefileByGrid(out errorMsg, center, 0.1250);
            Assert.AreEqual(0, gdalTest.coordinatesInShapefile.Count);
            gdalTest.CellAreaInShapefileByGrid(out errorMsg, center, -0.1250);
            Assert.AreEqual(0, gdalTest.coordinatesInShapefile.Count);
        }

    }
}