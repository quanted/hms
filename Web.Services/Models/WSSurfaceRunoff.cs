using Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Services.Models
{
    /// <summary>
    /// HMS Web Service SurfaceRunoff Model
    /// </summary>
    public class WSSurfaceRunoff
    {

        private enum RunoffSources { nldas, gldas, curvenumber }

        /// <summary>
        /// Gets SurfaceRunoff data using the given TimeSeriesInput parameters.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ITimeSeriesOutput> GetSurfaceRunoff(ITimeSeriesInput input)
        {
            string errorMsg = "";

            // Constructs default error output object containing error message.
            Utilities.ErrorOutput err = new Utilities.ErrorOutput();

            // SurfaceRunoff object
            SurfaceRunoff.SurfaceRunoff runoff = new SurfaceRunoff.SurfaceRunoff();

            // ITimeSeriesInputFactory object used to validate and initialize all variables of the input object.
            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            runoff.Input = iFactory.SetTimeSeriesInput(input, new List<string>() { "surfacerunoff" }, out errorMsg);

            // If error occurs in input validation and setup, errorMsg is added to metadata of an empty object.
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            ITimeSeriesOutput result = null;

            // Catchment, specified by comID, spatially weighted average
            //if (input.Geometry.Point == null)
            //{
            //    if (input.Geometry.ComID > 0)
            //    {
            //        result = this.SpatiallyWeightedAverage(input);
            //    }
            //    else
            //    {
            //        return err.ReturnError("ERROR: No valid point or catchment comID found in inputs.");
            //    }
            //}
            //else
            //{
            //    // Gets the SurfaceRunoff data.
            //    result = runoff.GetData(out errorMsg);
            //}
            // Gets the SurfaceRunoff data.
            result = runoff.GetData(out errorMsg);
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            // Get generic statistics
            result = Utilities.Statistics.GetStatistics(out errorMsg, runoff.Input, result);

            return result;

        }

        /// <summary>
        /// Calculates spatially weighted average for surface runoff over a specified catchment comID
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private ITimeSeriesOutput SpatiallyWeightedAverage(ITimeSeriesInput input)
        {
            string errorMsg = "";

            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput aggregated = oFactory.Initialize();
            ITimeSeriesOutput merged = oFactory.Initialize();
            ITimeSeriesOutput tests = oFactory.Initialize();

            //Spatial weighted average aggregation
            Utilities.CatchmentAggregation cd = new Utilities.CatchmentAggregation();
            Utilities.GeometryData gd = null;
            gd = cd.getData(input, new List<string> { input.Geometry.ComID.ToString() }, out errorMsg);

            List<ITimeSeriesOutput> results = new List<ITimeSeriesOutput>();
            for(int i = 0; i < gd.geometry.ElementAt(0).Value.points.Count; i++)
            {
                ITimeSeriesInput point_input = input.Clone(new List<string>() {"surfacerunoff"});
                point_input.Geometry = new TimeSeriesGeometry();
                point_input.Geometry.Point = new PointCoordinate()
                {
                    Latitude = gd.geometry.ElementAt(0).Value.points[i].latitude,
                    Longitude = gd.geometry.ElementAt(0).Value.points[i].longitude
                };
                SurfaceRunoff.SurfaceRunoff runoff = new SurfaceRunoff.SurfaceRunoff();
                ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
                runoff.Input = iFactory.SetTimeSeriesInput(point_input, new List<string>() { "surfacerunoff" }, out errorMsg);
                ITimeSeriesOutput output = runoff.GetData(out errorMsg);

                merged = Utilities.Merger.MergeTimeSeries(output, merged);
                results.Add(output);
                //merged.Metadata.Add("p_area_" + i, gd.geometry.ElementAt(0).Value.points[i].percentArea.ToString());
            }

            aggregated = cd.getCatchmentAggregation(input, merged, gd, true);

            aggregated.Metadata.Add("comID", input.Geometry.ComID.ToString());
            aggregated.Metadata["column_2"] = "runoff";
            aggregated.Metadata.Remove("column_3");
            aggregated.Metadata.Remove("column_5");
            //foreach (ITimeSeriesOutput o in results)
            //{
            //    aggregated = Utilities.Merger.MergeTimeSeries(aggregated, o);
            //}
            return aggregated;
        }
    }
}