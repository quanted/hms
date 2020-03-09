using Data;
using SurfaceRunoff;
using System;
using System.Collections.Generic;
using System.Text;

namespace SubSurfaceFlow
{
    class CurveNumber
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

            ITimeSeriesOutput runoffData = new TimeSeriesOutput();
            bool getRunoff = true;
            if (input.Geometry.ComID == 0 || input.Geometry.ComID == -1)
            {
                // Validate comid
                input.Geometry.ComID = SurfaceRunoff.CurveNumber.GetComID(out errorMsg, input.Geometry.Point);
            }
            
            if ((input.Geometry.ComID != 0 || input.Geometry.ComID != -1) && input.Geometry.Point.Latitude == -9999)
            {
                input.Geometry.Point = Utilities.COMID.GetCentroid(input.Geometry.ComID, out errorMsg);
            }
            if (input.InputTimeSeries != null)
            {
                if (input.InputTimeSeries.ContainsKey("surfacerunoff"))
                {
                    runoffData.Data = input.InputTimeSeries["surfacerunoff"].Data;
                    getRunoff = false;
                }
            }
            if(getRunoff)
            {
                runoffData = GetRunoffData(out errorMsg, input, output);
            }
            if (errorMsg.Contains("ERROR")) { return null; }

            // TODO: Add a call to fetch the catchment from the point (lat,lng) values. Possibly EPA waters web service
            int comid = (string.IsNullOrWhiteSpace(input.Geometry.ComID.ToString())) ? 0 : input.Geometry.ComID;
            double bfPercent = GetCatchmentBaseflowPercent(out errorMsg, comid);

            ITimeSeriesOutput bfOutput = CalculateBaseflow(out errorMsg, bfPercent, input, runoffData);
            if (errorMsg.Contains("ERROR")) { return null; }

            bfOutput.DataSource = "curvenumber";
            bfOutput.Dataset = "subsurfaceflow";
            bfOutput.Metadata.Add("comid", input.Geometry.ComID.ToString());
            bfOutput.Metadata.Add("startdate", input.DateTimeSpan.StartDate.ToString());
            bfOutput.Metadata.Add("enddate", input.DateTimeSpan.EndDate.ToString());
            bfOutput.Metadata.Add("runoffSource", runoffData.DataSource);

            return bfOutput;
        }

        /// <summary>
        /// Gets runoff data.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        private ITimeSeriesOutput GetRunoffData(out string errorMsg, ITimeSeriesInput input, ITimeSeriesOutput output)
        {
            errorMsg = "";

            SurfaceRunoff.SurfaceRunoff runoff = new SurfaceRunoff.SurfaceRunoff();
            runoff.Input = input;
            runoff.Output = output;

            if (input.Geometry.GeometryMetadata.ContainsKey("runoffSource"))
            {
                switch (input.Geometry.GeometryMetadata["runoffSource"])
                {
                    case "nldas":
                        input.Source = "nldas";
                        input.TemporalResolution = "daily";
                        break;
                    case "gldas":
                        input.Source = "gldas";
                        input.TemporalResolution = "daily";
                        break;
                    case "curvenumber":
                    default:
                        input.Source = "curvenumber";
                        break;
                }
            }
            else
            {
                input.Source = "curvenumber";
            }
            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            ITimeSeriesInput tempInput = iFactory.SetTimeSeriesInput(input, new List<string>() { "surfacerunoff" }, out errorMsg);
            runoff.Input = tempInput;
            runoff.Output = runoff.GetData(out errorMsg);
            if (errorMsg.Contains("ERROR")) { return null; }
            return runoff.Output;
        }

        /// <summary>
        /// Get the catchments baseflow percent from a specified comid.
        /// Runs SQL query to sqlite database file.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="comid"></param>
        /// <returns></returns>
        private double GetCatchmentBaseflowPercent(out string errorMsg, int comid)
        {
            errorMsg = "";
            string dbPath = "./App_Data/hms_database.sqlite3";
            string query = "SELECT Percent FROM BaseFlow WHERE ComID = " + comid.ToString();
            Dictionary<string, string> results = Utilities.SQLite.GetData(dbPath, query);
            if (results.Count == 0)
            {
                errorMsg = "ERROR: Unable to find catchment in database. ComID: " + comid.ToString();
            }
            return double.Parse(results["Percent"]);
        }

        /// <summary>
        /// Calculate Baseflow from surface runoff data and streamcat catchment baseflow percentage value.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="percent"></param>
        /// <param name="input"></param>
        /// <param name="runoffData"></param>
        /// <returns></returns>
        private ITimeSeriesOutput CalculateBaseflow(out string errorMsg, double percent, ITimeSeriesInput input, ITimeSeriesOutput runoffData)
        {
            errorMsg = "";
            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput baseflowOutput = oFactory.Initialize();
            foreach (KeyValuePair<string, List<string>> dateValue in runoffData.Data)
            {
                string date = dateValue.Key;
                double rf = double.Parse(dateValue.Value[0]);
                double bf = ((rf)/(1-(percent/100))) - rf;

                List<string> d = new List<string>();
                d.Add(bf.ToString(input.DataValueFormat));
                baseflowOutput.Data.Add(date, d);
            }

            baseflowOutput.Metadata.Add("baseflowPercent", percent.ToString());
            return baseflowOutput;
        }
    }
}