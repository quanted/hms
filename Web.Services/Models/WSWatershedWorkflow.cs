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
        private enum algorithms { constantvolume }//, changingvolume, kinematicwave }

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
            //errorMsg = (!Enum.TryParse(input.StreamHydrology, true, out algorithms sAlgos)) ? "ERROR: Algorithm is not currently supported." : "";
            //if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }
            errorMsg = (!Enum.TryParse(input.RunoffSource, true, out surfaceSources sSource)) ? "ERROR: 'Source' was not found or is invalid." : "";
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }
            errorMsg = (!Enum.TryParse(input.Geometry.GeometryMetadata["precipSource"], true, out precipSources pSource)) ? "ERROR: 'Source' was not found or is invalid." : "";
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            // SET Attributes to specific values until stack works
            input.TemporalResolution = "daily";

            //Stream Network Delineation
            List<string> lst = new List<string>();
            WatershedDelineation.StreamNetwork sn = new WatershedDelineation.StreamNetwork();
            DataTable dt = new DataTable();
            if (input.Geometry.ComID > 0)
            {
                dt = sn.prepareStreamNetworkForHUC(input.Geometry.ComID.ToString(), "com_id_num", out errorMsg, out lst);
            }
            else if (input.Geometry.HucID != null)
            {
                string len = input.Geometry.HucID.Length.ToString();
                dt = sn.prepareStreamNetworkForHUC(input.Geometry.HucID.ToString(), "huc_" + len + "_num", out errorMsg, out lst);
            }
            else
            {
                errorMsg = "ERROR: No valid geometry was found";
            }
            if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }

            //Getting Stream Flow data
            input.Source = input.RunoffSource;
            List<string> validList = new List<string>();
            DataSet ds = WatershedDelineation.FlowRouting.calculateStreamFlows(start.ToShortDateString(), end.ToShortDateString(), dt, lst, out validList, input, input.StreamHydrology, out errorMsg);
            lst = validList;

            Utilities.CatchmentAggregation cd = new Utilities.CatchmentAggregation();
            Utilities.GeometryData gd = null;
            if (input.Aggregation)
            {
                gd = cd.getData(input, lst, out errorMsg);
            }
            
            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput delinOutput = oFactory.Initialize();

            Web.Services.Controllers.WatershedWorkflowOutput totalOutput = new Web.Services.Controllers.WatershedWorkflowOutput();
            totalOutput.data = new Dictionary<int, Dictionary<string, ITimeSeriesOutput>>();
            int x = 1;
            Dictionary<string, string> meta = new Dictionary<string, string>();
            foreach (string com in lst)
            {
                //Setting all to ITimeSeriesOutput
                input.Source = input.Geometry.GeometryMetadata["precipSource"];
                ITimeSeriesOutput precipOutput = cd.getCatchmentAggregation(input, dtToITSOutput(ds.Tables[3], x), gd, input.Aggregation);//cd.getCatchmentAggregation(input, precipResult, gd, input.Aggregation);
                input.Source = input.RunoffSource;
                ITimeSeriesOutput surfOutput = cd.getCatchmentAggregation(input, dtToITSOutput(ds.Tables[0], x), gd, input.Aggregation); //dtToITSOutput(ds.Tables[0]); //cd.getCatchmentAggregation(input, surfResult, gd, input.Aggregation);
                input.Source = input.RunoffSource;
                ITimeSeriesOutput subOutput = cd.getCatchmentAggregation(input, dtToITSOutput(ds.Tables[1], x), gd, input.Aggregation);//dtToITSOutput(ds.Tables[1]);//cd.getCatchmentAggregation(input, subResult, gd, input.Aggregation);
                input.Source = input.StreamHydrology;
                ITimeSeriesOutput hydrologyOutput = cd.getCatchmentAggregation(input, dtToITSOutput(ds.Tables[2], x), gd, input.Aggregation);// dtToITSOutput(ds.Tables[2]);//cd.getCatchmentAggregation(input, dtToITSOutput(ds.Tables[2]), gd, input.Aggregation);

                Dictionary<string, ITimeSeriesOutput> timeSeriesDict = new Dictionary<string, ITimeSeriesOutput>();
                timeSeriesDict.Add("Precipitation", precipOutput);
                timeSeriesDict.Add("SurfaceRunoff", surfOutput);
                timeSeriesDict.Add("SubsurfaceRunoff", subOutput);
                timeSeriesDict.Add("StreamHydrology", hydrologyOutput);
                totalOutput.data.Add(Int32.Parse(com), timeSeriesDict);
                meta = precipOutput.Metadata;
                x++;
            }

            //Turn delineation table to ITimeseries
            totalOutput.table = new Dictionary<string, Dictionary<string, string>>();
            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                //List<string> lv = new List<string>();
                Dictionary<string, string> lv = new Dictionary<string, string>();
                int j = 0;
                string com = dr["COMID"].ToString();
                foreach (Object g in dr.ItemArray)
                {
                    lv.Add(dt.Columns[j++].ToString(), g.ToString());
                }
                if (totalOutput.table.ContainsKey(com))
                {
                    continue;
                }
                else
                {
                    totalOutput.table.Add(com, lv);
                }
            }

            //Adding delineation data to output
            totalOutput.Metadata = meta;
            totalOutput.metadata = new Dictionary<string, string>()
            {
                { "request_url", "api/workflow/watershed" },
                { "start_date", input.DateTimeSpan.StartDate.ToString() },
                { "end_date", input.DateTimeSpan.EndDate.ToString() },
                { "timestep", input.TemporalResolution.ToString() },
                { "errors", errorMsg },
                { "catchments", cd.listToString(lst, out errorMsg) },
                { "connectivity_table_source", "PlusFlowlineVAA" },
                { "NHDPlus_url" , "http://www.horizon-systems.com/nhdplus/NHDPlusV2_data.php"}
            };
            totalOutput.Dataset = "Precipitation, SurfaceRunoff, SubsurfaceRunoff, StreamHydrology";
            totalOutput.DataSource = input.Geometry.GeometryMetadata["precipSource"].ToString() + ", " + input.RunoffSource.ToString() + ", " + input.RunoffSource.ToString() + ", " + input.StreamHydrology.ToString();
            watch.Stop();
            string elapsed = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalMinutes.ToString();
            totalOutput.metadata.Add("Time_elapsed", elapsed);
            return totalOutput;
        }

        public ITimeSeriesOutput dtToITSOutput(DataTable dt, int x)
        {
            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput itimeoutput = oFactory.Initialize();
            foreach (DataRow dr in dt.Rows)
            {
                List<string> lv = new List<string>();
                lv.Add(dr[x].ToString());
                itimeoutput.Data.Add(dr[0].ToString(), lv);
            }

            itimeoutput.Metadata = new Dictionary<string, string>()
            {
                { "request_time", DateTime.Now.ToString() },
                { "column_1", "Date" },
                { "column_2", "Stream Flow" },
                { "units", "cubic meters" }
            };
            itimeoutput.Dataset = "Stream Flow";
            itimeoutput.DataSource = "curvenumber";
            return itimeoutput;
        }
    }
}