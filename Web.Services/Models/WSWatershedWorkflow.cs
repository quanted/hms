using Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Web;
using Web.Services.Controllers;

namespace Web.Services.Models
{
    /// <summary>
    /// HMS Web Serivce WorkFlow Model
    /// </summary>
    public class WSWatershedWorkFlow
    {
        private enum surfaceSources { nldas, gldas, curvenumber }
        private enum subSources { nldas, gldas }
        private enum precipSources { nldas, gldas, ncdc, daymet, wgen, prism }

        /// <summary>
        /// Gets workflow data.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ITimeSeriesOutput> GetWorkFlowData(WatershedWorkflowInput input)
        {
            DateTime start = input.DateTimeSpan.StartDate;
            DateTime end = input.DateTimeSpan.EndDate;
            
            var watch = System.Diagnostics.Stopwatch.StartNew();
            string errorMsg = "";

            // Constructs default error output object containing error message.
            Utilities.ErrorOutput err = new Utilities.ErrorOutput();

            // Validate all sources.
            errorMsg = (!Enum.TryParse(input.RunoffSource, true, out surfaceSources sSource)) ? "ERROR: 'Source' was not found or is invalid." : "";
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }
            errorMsg = (!Enum.TryParse(input.PrecipSource, true, out precipSources pSource)) ? "ERROR: 'Source' was not found or is invalid." : "";
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            // Precipitation object
            // Validate precipitation sources.
            input.Source = input.PrecipSource;
            input.DateTimeSpan.StartDate = start;
            input.DateTimeSpan.EndDate = end;
            errorMsg = (!Enum.TryParse(input.Source, true, out precipSources preSource)) ? "ERROR: 'Source' was not found or is invalid." : "";
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }
            Precipitation.Precipitation precip = new Precipitation.Precipitation();
            // ITimeSeriesInputFactory object used to validate and initialize all variables of the input object.
            ITimeSeriesInputFactory precipiFactory = new TimeSeriesInputFactory();
            precip.Input = precipiFactory.SetTimeSeriesInput(input, new List<string>() { "precipitation" }, out errorMsg);
            // If error occurs in input validation and setup, errorMsg is added to metadata of an empty object.
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }
            if (precip.Input.Source.Contains("ncdc"))
            {
                precip.Input.Geometry.GeometryMetadata["token"] = (precip.Input.Geometry.GeometryMetadata.ContainsKey("token")) ? precip.Input.Geometry.GeometryMetadata["token"] : "RUYNSTvfSvtosAoakBSpgxcHASBxazzP";
            }
            // Gets the Precipitation data.
            ITimeSeriesOutput precipResult = precip.GetData(out errorMsg);
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            // SubSurfaceFlow object
            SubSurfaceFlow.SubSurfaceFlow sub = new SubSurfaceFlow.SubSurfaceFlow();
            input.Source = input.RunoffSource == "curvenumber" ? "nldas" : input.RunoffSource;//Subflow source should be same as surface. If surface uses curve number (not available to subflow), default to nldas for sub.
            string subSource = input.Source;
            input.DateTimeSpan.StartDate = start;
            input.DateTimeSpan.EndDate = end;
            // ITimeSeriesInputFactory object used to validate and initialize all variables of the input object.
            ITimeSeriesInputFactory subiFactory = new TimeSeriesInputFactory();
            sub.Input = subiFactory.SetTimeSeriesInput(input, new List<string>() { "subsurfaceflow" }, out errorMsg);
            // If error occurs in input validation and setup, errorMsg is added to metadata of an empty object.
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }
            // Gets the SubSurfaceFlow data.
            ITimeSeriesOutput subResult = sub.GetData(out errorMsg);
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            // SurfaceRunoff object
            SurfaceRunoff.SurfaceRunoff runoff = new SurfaceRunoff.SurfaceRunoff();
            input.DateTimeSpan.StartDate = start;
            input.DateTimeSpan.EndDate = end;
            input.Source = input.RunoffSource;
            // ITimeSeriesInputFactory object used to validate and initialize all variables of the input object.
            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            runoff.Input = iFactory.SetTimeSeriesInput(input, new List<string>() { "surfacerunoff" }, out errorMsg);
            // If error occurs in input validation and setup, errorMsg is added to metadata of an empty object.
            //if (errorMsg.Contains("ERROR") && input.Source != "curvenumber") { return err.ReturnError(errorMsg); }
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }
            // Gets the SurfaceRunoff data.
            ITimeSeriesOutput surfResult = runoff.GetData(out errorMsg);
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            //Stream Network Delineation
            List<string> lst = new List<string>();
            WatershedDelineation.StreamNetwork sn = new WatershedDelineation.StreamNetwork();
            string gtype = "";
            if (input.Geometry.GeometryMetadata.ContainsKey("huc_8_num"))
            {
                gtype = "huc_8_num";
            }
            else if (input.Geometry.GeometryMetadata.ContainsKey("huc_12_num"))
            {
                gtype = "huc_12_num";
            }
            else if (input.Geometry.GeometryMetadata.ContainsKey("com_id_list"))
            {
                gtype = "com_id_list";
            }
            else if (input.Geometry.ComID > 0 || input.Geometry.GeometryMetadata.ContainsKey("com_id_num"))
            {
                gtype = "com_id_num";
            }
            input.Geometry.GeometryMetadata.Add("GeometryType", gtype);
            DataTable dt = sn.prepareStreamNetworkForHUC(input.Geometry.GeometryMetadata[gtype].ToString(), gtype, out errorMsg, out lst);
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            //Getting Stream Flow data
            DataSet ds = WatershedDelineation.FlowRouting.calculateStreamFlows(start.ToShortDateString(), end.ToShortDateString(), dt, subResult, surfResult, out errorMsg);


            Utilities.CatchmentAggregation cd = new Utilities.CatchmentAggregation();
            Utilities.GeometryData gd = null;
            // if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }
            if (input.Aggregation)
            {
                gd = cd.getData(input, lst, out errorMsg);
            }
            input.Source = input.PrecipSource;
            ITimeSeriesOutput precipOutput = cd.getCatchmentAggregation(input, precipResult, gd, input.Aggregation);
            input.Source = input.RunoffSource;
            ITimeSeriesOutput surfOutput = cd.getCatchmentAggregation(input, surfResult, gd, input.Aggregation);
            input.Source = subSource;
            ITimeSeriesOutput subOutput = cd.getCatchmentAggregation(input, subResult, gd, input.Aggregation);
            input.Source = input.StreamHydrology;
            ITimeSeriesOutput hydrologyOutput = dtToITSOutput(ds.Tables[2]);//cd.getCatchmentAggregation(input, dtToITSOutput(ds.Tables[2]), gd, input.Aggregation);

            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput delinOutput = oFactory.Initialize();

            Web.Services.Controllers.WatershedWorkflowOutput totalOutput = new Web.Services.Controllers.WatershedWorkflowOutput();
            totalOutput.data = new Dictionary<int, Dictionary<string, ITimeSeriesOutput>>();
            foreach (string com in lst)
            {
                Dictionary<string, ITimeSeriesOutput> timeSeriesDict = new Dictionary<string, ITimeSeriesOutput>();
                timeSeriesDict.Add("Precipitation", precipOutput);
                timeSeriesDict.Add("SurfaceRunoff", surfOutput);
                timeSeriesDict.Add("SubsurfaceRunoff", subOutput);
                timeSeriesDict.Add("StreamHydrology", hydrologyOutput);
                totalOutput.data.Add(Int32.Parse(com), timeSeriesDict);
            }

            //Turn delineation table to ITimeseries
            totalOutput.table = new List<Dictionary<string, string>>();
            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                //List<string> lv = new List<string>();
                Dictionary<string, string> lv = new Dictionary<string, string>();
                int j = 0;
                foreach (Object g in dr.ItemArray)
                {
                    lv.Add(dt.Columns[j++].ToString(), g.ToString());
                }
                totalOutput.table.Add(lv);
            }

            //Adding delineation data to output
            totalOutput.Metadata = precipOutput.Metadata;
            totalOutput.metadata = new Dictionary<string, string>()
            {
                { "request_url", "api/workflow/watershed" },
                { "geometry_input", gtype },
                { "geometry_value", input.Geometry.GeometryMetadata[gtype].ToString() },
                { "start_date", input.DateTimeSpan.StartDate.ToString() },
                { "end_date", input.DateTimeSpan.EndDate.ToString() },
                { "timestep", input.TemporalResolution.ToString() },
                { "catchments", cd.listToString(lst, out errorMsg) },
                { "connectivity_table_source", "PlusFlowlineVAA" },
                { "NHDPlus_url" , "http://www.horizon-systems.com/nhdplus/NHDPlusV2_data.php"}
            };
            totalOutput.Dataset = "Precipitation, SurfaceRunoff, SubsurfaceRunoff, StreamHydrology";
            totalOutput.DataSource = input.PrecipSource.ToString() + ", " + input.RunoffSource.ToString() + ", " + subOutput.DataSource.ToString() + ", " + input.StreamHydrology.ToString();
            watch.Stop();
            string elapsed = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalMinutes.ToString();
            totalOutput.metadata.Add("Time_elapsed", elapsed);
            return totalOutput;
        }

        public ITimeSeriesOutput dtToITSOutput (DataTable dt)
        {
            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput itimeoutput = oFactory.Initialize();
            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                List<string> lv = new List<string>();
                lv.Add(dr[1].ToString());
                itimeoutput.Data.Add(dr[0].ToString(), lv);
                i++;
            }
            
            itimeoutput.Metadata = new Dictionary<string, string>()
            {
                { "request_time", DateTime.Now.ToString() },
                { "column_1", "Date" },
                { "column_2", "Stream Flow" },
                { "units", "mm" }
            };
            itimeoutput.Dataset = "Stream Flow";
            itimeoutput.DataSource = "curvenumber";
            return itimeoutput;
        }
    }
}