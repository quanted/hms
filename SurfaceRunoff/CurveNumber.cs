using Data;
using System;
using System.Collections.Generic;
using System.Text;
using Precipitation;

namespace SurfaceRunoff
{
    /// <summary>
    /// SurfaceRunoff curve number class.
    /// </summary>
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

            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            // TODO: Add options for different precip inputs
            input.Source = "daymet";
            ITimeSeriesInput precipInput = iFactory.SetTimeSeriesInput(input, new List<string>() { "precipitation" }, out errorMsg);

            // Static test centroid point
            IPointCoordinate catchmentCentroid = new PointCoordinate()
            {
                Latitude = 46.69580547,
                Longitude = -69.36054766
            };
            precipInput.Geometry.Point = catchmentCentroid as PointCoordinate;
            input.Geometry.ComID = 718276;

            // Database call for centroid data with specified comid.
            // precipInput.Geometry.Point = GetCatchmentCentroid(out errorMsg, input.Geometry.ComID);

            ITimeSeriesOutput precipData = GetPrecipData(out errorMsg, precipInput, output);
            if (errorMsg.Contains("ERROR")) { return null; }

            Data.Simulate.CurveNumber cn = new Data.Simulate.CurveNumber();
            ITimeSeriesOutput cnOutput = cn.Simulate(out errorMsg, input, precipData);
            if (errorMsg.Contains("ERROR")) { return null; }

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

            //Remove dates from daymet that are not in date time span
            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput copy = oFactory.Initialize();
            foreach (var entry in precip.Output.Data)
            {
                DateTime date = Convert.ToDateTime(entry.Key.Substring(0, 10));
                if ((date.Date >= precip.Input.DateTimeSpan.StartDate.Date) && (date.Date <= precip.Input.DateTimeSpan.EndDate.Date))
                {
                    copy.Data.Add(entry.Key, entry.Value);
                }
                else
                {
                    continue;
                }
            }
            precip.Output = copy;

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
            IPointCoordinate centroid = new PointCoordinate()
            {
                Latitude = double.Parse(centroidDict["CentroidLatitude"]),
                Longitude = double.Parse(centroidDict["CentroidLongitude"])
            };
            return centroid as PointCoordinate;
        }

    }
}