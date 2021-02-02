using Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Streamflow
{
    public class ConstantVolume
    {

        public ITimeSeriesOutput<List<double>> GetData(out string errorMsg, ITimeSeriesInput input, string precipSource, string runoffSource, string boundarySource="", ITimeSeriesOutput<List<double>> streamTS=null)
        {

            ITimeSeriesOutput<List<double>> output = new TimeSeriesOutput<List<double>>();
            Utilities.MetaErrorOutput err = new Utilities.MetaErrorOutput();
            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            
            // Get precip data
            Precipitation.Precipitation precip = new Precipitation.Precipitation();
            precip.Input = iFactory.SetTimeSeriesInput(input, new List<string>() { "precipitation" }, out errorMsg);
            precip.Input.Source = precipSource;
            ITimeSeriesOutput<List<double>> precipOutput = precip.GetData(out errorMsg).ToListDouble();

            // Get runoff data
            SurfaceRunoff.SurfaceRunoff runoff = new SurfaceRunoff.SurfaceRunoff();
            runoff.Input = iFactory.SetTimeSeriesInput(input, new List<string>() { "surfacerunoff" }, out errorMsg);
            runoff.Input.Source = runoffSource;
            if (runoffSource == "cn")
            {
                runoff.Input.Geometry.GeometryMetadata.Add("precipSource", precipSource);
                runoff.Input.InputTimeSeries.Add("precipitation", (TimeSeriesOutput)precip.Output);
            }
            ITimeSeriesOutput<List<double>> runoffOutput = runoff.GetData(out errorMsg).ToListDouble();

            // Get baseflow data
            SubSurfaceFlow.SubSurfaceFlow subsurf = new SubSurfaceFlow.SubSurfaceFlow();
            subsurf.Input = iFactory.SetTimeSeriesInput(input, new List<string>() { "subsurfaceflow" }, out errorMsg);
            if (runoffSource == "cn")
            {
                subsurf.Input.Geometry.GeometryMetadata.Add("precipSource", precipSource);
                subsurf.Input.InputTimeSeries.Add("precipitation", (TimeSeriesOutput)precip.Output);
            }
            ITimeSeriesOutput<List<double>> subOutput = subsurf.GetData(out errorMsg).ToListDouble();        

            // Get incoming streamflow data, if required
            if (streamTS == null)
            {
                Streamflow sfBoundary = new Streamflow();
                sfBoundary.Input = iFactory.SetTimeSeriesInput(input, new List<string>() { "streamflow" }, out errorMsg);
                sfBoundary.Input.Source = boundarySource;
                streamTS = sfBoundary.GetData(out errorMsg);
            }

            if (errorMsg.Contains("ERROR"))
            {
                output.Metadata = err.ReturnError(errorMsg);
                errorMsg = "";
                return output;
            }
            return this.GetData(out errorMsg, input, precipOutput, runoffOutput, subOutput, streamTS);
        }

        public ITimeSeriesOutput<List<double>> GetData(out string errorMsg, ITimeSeriesInput input, ITimeSeriesOutput<List<double>> precipTS, ITimeSeriesOutput<List<double>> runoffTS, ITimeSeriesOutput<List<double>> baseflowTS, ITimeSeriesOutput<List<double>> streamTS)
        {





            double dsur = Convert.ToDouble(dtSurfaceRunoff.Rows[i][COMID].ToString());
            double dsub = Convert.ToDouble(dtSubSurfaceRunoff.Rows[i][COMID].ToString());
            double area = Convert.ToDouble(dtStreamNetwork.Rows[x]["AreaSqKM"]);
            dtStreamFlow.Rows[i][COMID] = ((dsub * area * 1000) + (dsur * area * 1000)) / 86400;//dsub + dsur;            m^3/day (1day/86400sec) => m^3/s



        }
    }
}
