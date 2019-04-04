using Data;
using System;
using System.Collections.Generic;
using System.Text;
using Precipitation;
using System.Net;
using System.IO;
using System.Threading;

namespace SurfaceRunoff
{
    /// <summary>
    /// SurfaceRunoff curve number class.
    /// </summary>
    public class CurveNumber
    {

        /// <summary>
        /// GetData function for curvenumber.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public ITimeSeriesOutput GetData(out string errorMsg, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";
            ITimeSeriesOutput precipData;
            if (!CheckForInputTimeseries(out errorMsg, input))
            {
                ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
                string tempSource = input.Source;
                string precipSource = (input.Geometry.GeometryMetadata.ContainsKey("precipSource")) ? input.Source : "daymet";
                input.Source = precipSource;
                ITimeSeriesInput precipInput = iFactory.SetTimeSeriesInput(input, new List<string>() { "precipitation" }, out errorMsg);

                if (input.Geometry.ComID == 0)
                {
                    // Validate comid
                    input.Geometry.ComID = GetComID(out errorMsg, input.Geometry.Point);
                }
                else
                {
                    // Database call for centroid data with specified comid.
                    precipInput.Geometry.Point = GetCatchmentCentroid(out errorMsg, input.Geometry.ComID);
                    if (errorMsg.Contains("ERROR")) { return null; }
                }
                                                                               
                precipInput.TemporalResolution = "daily";
                precipData = GetPrecipData(out errorMsg, precipInput, output);
                if (errorMsg.Contains("ERROR")) { return null; }
                input.Source = tempSource;
            }
            else
            {
                precipData = input.InputTimeSeries["precipitation"];
            }

            Data.Simulate.CurveNumber cn = new Data.Simulate.CurveNumber();
            ITimeSeriesOutput cnOutput = cn.Simulate(out errorMsg, input, precipData);
            if (errorMsg.Contains("ERROR")) { return null; }

            cnOutput.Metadata.Add("comid", input.Geometry.ComID.ToString());
            cnOutput.Metadata.Add("startdate", input.DateTimeSpan.StartDate.ToString());
            cnOutput.Metadata.Add("enddate", input.DateTimeSpan.EndDate.ToString());
            cnOutput.Metadata.Add("precipSource", precipData.DataSource);
            cnOutput.Metadata.Add("column_1", "Date");
            cnOutput.Metadata.Add("column_2", "Surface Runoff");


            return cnOutput;
        }

        /// <summary>
        /// Gets precipitation data.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        private ITimeSeriesOutput GetPrecipData(out string errorMsg, ITimeSeriesInput input, ITimeSeriesOutput output)
        {
            errorMsg = "";

            Precipitation.Precipitation precip = new Precipitation.Precipitation();
            precip.Input = input;
            precip.Output = output;

            if (input.Geometry.GeometryMetadata.ContainsKey("precipSource"))
            {
                switch (input.Geometry.GeometryMetadata["precipSource"])
                {
                    case "nldas":
                        input.Source = "nldas";
                        input.TemporalResolution = "daily";
                        break;
                    case "gldas":
                        input.Source = "gldas";
                        input.TemporalResolution = "daily";
                        break;
                    case "daymet":
                    default:
                        input.Source = "daymet";
                        break;
                }
            }
            else
            {
                input.Source = "daymet";
            }
            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            ITimeSeriesInput tempInput = iFactory.SetTimeSeriesInput(input, new List<string>() { "precipitation" }, out errorMsg);
            precip.Input = tempInput;
            precip.Output = precip.GetData(out errorMsg);
            if (errorMsg.Contains("ERROR")) { return null; }
            return precip.Output;
        }

        /// <summary>
        /// Get the catchment centroid from a specified comid.
        /// Runs SQL query to sqlite database file.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="comid"></param>
        /// <returns></returns>
        private PointCoordinate GetCatchmentCentroid(out string errorMsg, int comid)
        {
            errorMsg = "";
            string dbPath = "./App_Data/catchments.sqlite";
            string query = "SELECT CentroidLatitude, CentroidLongitude FROM PlusFlowlineVAA WHERE ComID = " + comid.ToString();
            Dictionary<string, string> centroidDict = Utilities.SQLite.GetData(dbPath, query);
            if (centroidDict.Count == 0)
            {
                errorMsg = "ERROR: Unable to find catchment in database. ComID: " + comid.ToString();
                return null;
            }
            IPointCoordinate centroid = new PointCoordinate()
            {
                Latitude = double.Parse(centroidDict["CentroidLatitude"]),
                Longitude = double.Parse(centroidDict["CentroidLongitude"])
            };
            return centroid as PointCoordinate;
        }

        /// <summary>
        /// Get the comid from a specified lat/long.
        /// Runs SQL query to sqlite database file.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="comid"></param>
        /// <returns></returns>
        private int GetComID(out string errorMsg, PointCoordinate point)
        {
            errorMsg = "";
            int com = 0;
            string url = "https://ofmpub.epa.gov/waters10/SpatialAssignment.Service?pGeometry=POINT(" + point.Longitude + " " + point.Latitude + ")&pLayer=NHDPLUS_CATCHMENT&pSpatialSnap=TRUE&pReturnGeometry=FALSE";
            WebClient myWC = new WebClient();
            string data = "";
            try
            {
                int retries = 5;                                        // Max number of request retries
                string status = "";                                     // response status code
                string jobID = "";
                while (retries > 0 && !status.Contains("OK"))
                {
                    WebRequest wr = WebRequest.Create(url);
                    HttpWebResponse response = (HttpWebResponse)wr.GetResponse();
                    status = response.StatusCode.ToString();
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    data = reader.ReadToEnd();
                    reader.Close();
                    response.Close();
                    retries -= 1;
                    if (!status.Contains("OK"))
                    {
                        Thread.Sleep(100);
                    }
                }
            }
            catch (Exception ex)
            {
                errorMsg = "ERROR: Could not find ComID for the given coordinates." + ex.Message;
            }
            string expr = "(?:\"assignment_value\":\")[0-9]+(?:\")";
            System.Text.RegularExpressions.Match reg = System.Text.RegularExpressions.Regex.Match(data, expr);
            com = int.Parse(reg.Value.Split(":")[1].Replace("\"", ""));
            return com;
        }


        /// <summary>
        /// Check for valid ITimeSeriesOutput in the input object
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private bool CheckForInputTimeseries(out string errorMsg, ITimeSeriesInput input)
        {
            errorMsg = "";
            bool validTSInput = false;
            if (input.InputTimeSeries != null)
            {
                foreach(var e in input.InputTimeSeries)
                {
                    string dataset = e.Key;
                    ITimeSeriesOutput o = e.Value;
                    if (dataset.ToLower().Equals("precipitation"))
                    {
                        // Assuming input TimeSeries has a temporal resolution of 1 day
                        int inputDays = (input.DateTimeSpan.EndDate - input.DateTimeSpan.StartDate).Days;
                        int inputTSDays = o.Data.Keys.Count;
                        if (inputDays == inputTSDays)
                        {
                            validTSInput = true;
                        }
                    }
                }
            }
            return validTSInput;
        }
    }
}