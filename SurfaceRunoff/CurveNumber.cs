using Data;
using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Threading;
using Serilog;
using System.Linq;

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
                string precipSource = (input.Geometry.GeometryMetadata.ContainsKey("precipSource")) ? input.Geometry.GeometryMetadata["precipSource"] : "daymet";
                input.Source = precipSource;
                ITimeSeriesInput precipInput = iFactory.SetTimeSeriesInput(input, new List<string>() { "precipitation" }, out errorMsg);

                if (input.Geometry.ComID == 0 || input.Geometry.ComID == -1)
                {
                    // Validate comid
                    input.Geometry.ComID = GetComID(out errorMsg, input.Geometry.Point);
                }
                else
                {
                    // Database call for centroid data with specified comid.
                    precipInput.Geometry.Point = Utilities.COMID.GetCentroid(input.Geometry.ComID, out errorMsg);
                    if (errorMsg.Contains("ERROR")) { return null; }
                }
                if(input.Source == "ncei" && input.Geometry.StationID == null)
                {
                    Log.Warning("Error for Surface Runoff request using Curve Number with NCEI precipitation data source, missing stationID.");
                    return null;
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

            if (precipData.Data.GetType().Equals(typeof(Utilities.ErrorOutput)) || precipData.Data.Count <= 0)
            {
                errorMsg = "ERROR: Could not obtain valid precipitation data.";
                return null;
            }

            Data.Simulate.CurveNumber cn = new Data.Simulate.CurveNumber();
            ITimeSeriesOutput cnOutput = cn.Simulate(out errorMsg, input, precipData);
            if (errorMsg.Contains("ERROR")) { return null; }
            if (input.TemporalResolution == "monthly") 
            {
                cnOutput.Data = MonthlyAggregatedSum(out errorMsg, 1.0, cnOutput, input);
            }

            cnOutput.Metadata.Add("comid", input.Geometry.ComID.ToString());
            cnOutput.Metadata.Add("startdate", input.DateTimeSpan.StartDate.ToString());
            cnOutput.Metadata.Add("enddate", input.DateTimeSpan.EndDate.ToString());
            cnOutput.Metadata.Add("precipSource", precipData.DataSource);
            cnOutput.Metadata.Add("column_1", "Date");
            cnOutput.Metadata.Add("column_2", "Surface Runoff");
            cnOutput.Metadata = this.MergeDictionaries(cnOutput.Metadata, precipData.Metadata);

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
            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            ITimeSeriesInput tempInput = iFactory.SetTimeSeriesInput(input, new List<string>() { "precipitation" }, out errorMsg);
            precip.Input = tempInput;
            precip.Output = precip.GetData(out errorMsg);
            if (errorMsg.Contains("ERROR")) { return null; }
            return precip.Output;
        }

        /// <summary>
        /// Get the comid from a specified lat/long.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="comid"></param>
        /// <returns></returns>
        public static int GetComID(out string errorMsg, PointCoordinate point)
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
                        int inputDays = (input.DateTimeSpan.EndDate.Date - input.DateTimeSpan.StartDate.Date).Days + 1; //int inputDays = (input.DateTimeSpan.EndDate - input.DateTimeSpan.StartDate).Days;
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

        /// <summary>
        /// Copies all of the key value pairs in dict2 into dict1, duplicates are ignored
        /// </summary>
        /// <param name="dict1"></param>
        /// <param name="dict2"></param>
        /// <returns></returns>
        private Dictionary<string, string> MergeDictionaries(Dictionary<string, string> dict1, Dictionary<string, string> dict2)
        {
            if (dict2 == null)
            {
                return dict1;
            }
            else if (dict1 == null)
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

        /// <summary>
        /// Monthly aggregated sums for SurfaceRunoff data.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public static Dictionary<string, List<string>> MonthlyAggregatedSum(out string errorMsg, double modifier, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";

            DateTime iDate = new DateTime();
            double sum = 0.0;

            // Unit conversion coefficient
            double unit = (input.Units.Contains("imperial")) ? 0.0393701 : 1.0;

            string dateString0 = output.Data.Keys.ElementAt(0).ToString().Substring(0, output.Data.Keys.ElementAt(0).ToString().Length - 1) + ":00:00";
            DateTime.TryParse(dateString0, out iDate);

            Dictionary<string, List<string>> tempData = new Dictionary<string, List<string>>();
            for (int i = 0; i < output.Data.Count; i++)
            {
                DateTime date = new DateTime();
                string dateString = output.Data.Keys.ElementAt(i).ToString().Substring(0, output.Data.Keys.ElementAt(i).ToString().Length - 1) + ":00:00";
                DateTime.TryParse(dateString, out date);
                if (date.Month != iDate.Month || i == output.Data.Count - 1)
                {
                    tempData.Add(iDate.ToString(input.DateTimeSpan.DateTimeFormat), new List<string>() { (modifier * unit * sum).ToString(input.DataValueFormat) });
                    iDate = date;
                    sum = Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0]);
                }
                else
                {
                    sum += Convert.ToDouble(output.Data[output.Data.Keys.ElementAt(i)][0]);
                }
            }
            return tempData;
        }
    }
}