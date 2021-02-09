using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Streamflow
{
    public class KinematicWave
    {

        public ITimeSeriesOutput<List<double>> GetData(out string errorMsg, ITimeSeriesInput input, string precipSource, string runoffSource, string boundarySource="", ITimeSeriesOutput<List<double>> streamTS=null, double constantLoading=0.0, ITimeSeriesOutput<List<double>> loadings=null)
        {

            ITimeSeriesOutput<List<double>> output = new TimeSeriesOutput<List<double>>();
            Utilities.MetaErrorOutput err = new Utilities.MetaErrorOutput();
            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();

            string tempSource = input.Source;

            input.Geometry.Point = Utilities.COMID.GetCentroid(input.Geometry.ComID, out errorMsg);
            if (errorMsg.Contains("ERROR")) { return null; }

            // Get precip data
            Precipitation.Precipitation precip = new Precipitation.Precipitation();
            input.Source = precipSource;
            precip.Input = iFactory.SetTimeSeriesInput(input, new List<string>() { "precipitation" }, out errorMsg);
            ITimeSeriesOutput<List<double>> precipOutput = precip.GetData(out errorMsg).ToListDouble();

            // Get runoff data
            SurfaceRunoff.SurfaceRunoff runoff = new SurfaceRunoff.SurfaceRunoff();
            input.Source = runoffSource;
            runoff.Input = iFactory.SetTimeSeriesInput(input, new List<string>() { "surfacerunoff" }, out errorMsg);
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
            if (streamTS == null && boundarySource != null)
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
            input.Source = tempSource;
            return this.GetData(out errorMsg, input, precipOutput, runoffOutput, subOutput, streamTS, constantLoading, loadings);
        }

        public ITimeSeriesOutput<List<double>> GetData(out string errorMsg, ITimeSeriesInput input, ITimeSeriesOutput<List<double>> precipTS, ITimeSeriesOutput<List<double>> runoffTS, ITimeSeriesOutput<List<double>> baseflowTS, ITimeSeriesOutput<List<double>> streamTS, double constantLoading = 0.0, ITimeSeriesOutput<List<double>> loadings = null)
        {
            ITimeSeriesOutputFactory<List<double>> oFactory = new TimeSeriesOutputFactory<List<double>>();
            ITimeSeriesOutput<List<double>> streamOut = oFactory.Initialize();
            streamOut.DataSource = "kinematicwave";
            streamOut.Dataset = "streamflow";
            streamOut.Metadata.Add("precip_source", precipTS.DataSource);
            streamOut.Metadata.Add("runoff_source", runoffTS.DataSource);
            streamOut.Metadata.Add("baseflow_source", baseflowTS.DataSource);

            Dictionary<string, string> metadata = Utilities.COMID.GetDbData(input.Geometry.ComID, out errorMsg);
            double area = Convert.ToDouble(metadata["AreaSqKM"]);

            foreach (KeyValuePair<string, List<double>> keyValue in precipTS.Data)
            {
                List<double> values = new List<double>();
                values.Add(keyValue.Value[0]);
                double surface = runoffTS.Data[keyValue.Key][0];
                values.Add(surface);
                double baseflow = baseflowTS.Data[keyValue.Key][0];
                values.Add(baseflow);
                double inputFlow = 0.0;
                if (streamTS != null) {
                    inputFlow = streamTS.Data[keyValue.Key][0];
                }
                values.Add(inputFlow);
                double loading = constantLoading;
                if (loadings != null)
                {
                    if (loadings.Data.ContainsKey(keyValue.Key))
                    {
                        loading = loadings.Data[keyValue.Key][0];
                    }
                }
                // Assuming loading and inputFlow are in m^3/sec, added to converted (baseflow + surfaceflow)          
                double outputFlow = loading + inputFlow + (((baseflow * area * 1000) + (surface * area * 1000)) / 86400); // m^3 / day(1day/86400sec) => m^3/s
                values.Add(outputFlow);
                streamOut.Data.Add(keyValue.Key, values);
            }

            streamOut.Metadata.Add("column_1", "datetime");
            streamOut.Metadata.Add("column_2", "precipitation");
            streamOut.Metadata.Add("column_3", "runoff");
            streamOut.Metadata.Add("column_4", "baseflow");
            streamOut.Metadata.Add("column_5", "streamflow");
            streamOut.Metadata = this.CombineMetadata("catchment", streamOut.Metadata, metadata);
            streamOut.Metadata = this.CombineMetadata("precipitation", streamOut.Metadata, precipTS.Metadata);
            streamOut.Metadata = this.CombineMetadata("runoff", streamOut.Metadata, runoffTS.Metadata);
            streamOut.Metadata = this.CombineMetadata("baseflow", streamOut.Metadata, baseflowTS.Metadata);
            if (streamTS != null)
            {
                streamOut.Metadata = this.CombineMetadata("inputflow", streamOut.Metadata, streamTS.Metadata);
            }
            if (loadings != null)
            {
                streamOut.Metadata = this.CombineMetadata("loading", streamOut.Metadata, loadings.Metadata);
            }
            streamOut.Metadata.Add("streamflow_units", "m^3/sec");

            return streamOut;
        }

        private Dictionary<string, string> CombineMetadata(string dataset, Dictionary<string, string> metadata, Dictionary<string, string> datasetMeta)
        {
            
            foreach(KeyValuePair<string, string> keyValue in datasetMeta)
            {
                if (keyValue.Key.Contains("column") || keyValue.Key.Contains("tz_offset") || keyValue.Key.Contains("timezone"))
                {
                    continue;
                }
                string key = dataset + "_" + keyValue.Key;
                if (!metadata.ContainsKey(key))
                {
                    metadata.Add(key, keyValue.Value);
                }
            }
            return metadata;
        }
    }
}
