using Data;
using System.Collections.Generic;

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
            if (errorMsg.Contains("ERROR")) { return null; }


            ITimeSeriesOutput bfOutput = CalculateBaseflow(out errorMsg, bfPercent, input, runoffData);
            if (errorMsg.Contains("ERROR")) { return null; }

            bfOutput.DataSource = "Curve Number";
            bfOutput.Dataset = "SubSurfaceFlow";
            bfOutput.Metadata.Add("comid", input.Geometry.ComID.ToString());
            bfOutput.Metadata.Add("startdate", input.DateTimeSpan.StartDate.ToString());
            bfOutput.Metadata.Add("enddate", input.DateTimeSpan.EndDate.ToString());
            bfOutput.Metadata.Add("runoffSource", runoffData.DataSource);
            bfOutput.Metadata = this.MergeDictionaries(bfOutput.Metadata, runoffData.Metadata);
            bfOutput.Metadata["column_2"] = "Subsurface Flow";

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
                return -1.0;
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

        /// <summary>
        /// Copies all of the key value pairs in dict2 into dict1, duplicates are ignored
        /// </summary>
        /// <param name="dict1"></param>
        /// <param name="dict2"></param>
        /// <returns></returns>
        private Dictionary<string, string> MergeDictionaries(Dictionary<string, string> dict1, Dictionary<string, string> dict2)
        {
            if(dict2 == null) {
                return dict1;
            }
            else if(dict1 == null)
            {
                return dict2;
            }
            foreach (KeyValuePair<string, string> kv in dict2)
            {
                if (!dict1.ContainsKey(kv.Key))
                {
                    dict1.Add(kv.Key, kv.Value);
                }
            }
            return dict1;
        }
    }
}