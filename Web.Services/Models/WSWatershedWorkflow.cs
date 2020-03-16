using Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Threading.Tasks;
using Web.Services.Controllers;

namespace Web.Services.Models
{

    /// <summary>
    /// HMS Web Serivce WorkFlow Model
    /// </summary>
    public class WSWatershedWorkFlow
    {
        private enum surfaceSources { nldas, gldas, nwm, curvenumber }
        private enum subSources { nldas, gldas }
        private enum precipSources { nldas, gldas, ncei, daymet, wgen, prism, nwm }
        private enum algorithms { constantvolume }//, changingvolume, kinematicwave }

        /// <summary>
        /// Gets workflow data.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<WatershedWorkflowOutput> GetWorkFlowData(WatershedWorkflowInput input)
        {
            WatershedWorkflowOutput tempOut = new WatershedWorkflowOutput();
            DateTime start = input.DateTimeSpan.StartDate;
            DateTime end = input.DateTimeSpan.EndDate;

            var watch = System.Diagnostics.Stopwatch.StartNew();
            string errorMsg = "";

            // Constructs default error output object containing error message.
            Utilities.MetaErrorOutput err = new Utilities.MetaErrorOutput();

            // Validate all sources.
            //errorMsg = (!Enum.TryParse(input.StreamHydrology, true, out algorithms sAlgos)) ? "ERROR: Algorithm is not currently supported." : "";
            //if (errorMsg.Contains("ERROR")) { return err.ReturnError(errorMsg); }
            errorMsg = (!Enum.TryParse(input.RunoffSource, true, out surfaceSources sSource)) ? "ERROR: 'Source' was not found or is invalid." : "";
            if (errorMsg.Contains("ERROR"))
            {
                tempOut.Metadata = err.ReturnError(errorMsg);
                return tempOut;
            }
            errorMsg = (!Enum.TryParse(input.Geometry.GeometryMetadata["precipSource"], true, out precipSources pSource)) ? "ERROR: 'Source' was not found or is invalid." : "";
            if (errorMsg.Contains("ERROR"))
            {
                tempOut.Metadata = err.ReturnError(errorMsg);
                return tempOut;
            }
            // SET Attributes to specific values until stack works
            input.TemporalResolution = "daily";
            if(input.RunoffSource == "nldas" || input.RunoffSource == "gldas")
            {
                input.Geometry.GeometryMetadata["precipSource"] = input.RunoffSource;
            }

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
                if(input.Geometry.HucID.Length < 12)
                {
                    tempOut.Metadata = err.ReturnError("ERROR: HUC type invalid or currently unsupported. Provided level: " + len);
                    return tempOut;
                }
                dt = sn.prepareStreamNetworkForHUC(input.Geometry.HucID.ToString(), "huc_" + len + "_num", out errorMsg, out lst);
            }
            else
            {
                errorMsg = "ERROR: No valid geometry was found";
            }
            if (errorMsg.Contains("ERROR"))
            {
                tempOut.Metadata = err.ReturnError(errorMsg);
                return tempOut;
            }
            //NCEI station attributes
            if (input.Geometry.GeometryMetadata["precipSource"] == "ncei")
            {                
                input.Geometry.GeometryMetadata["token"] = "RUYNSTvfSvtosAoakBSpgxcHASBxazzP";
                input.Geometry.GeometryMetadata["stationID"] = nceiStation(out errorMsg, input);  
            }
            if (errorMsg.Contains("ERROR"))
            {
                tempOut.Metadata = err.ReturnError(errorMsg);
                return tempOut;
            }

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
            /*totalOutput.data = new Dictionary<int, Dictionary<string, ITimeSeriesOutput>>();*/
            Dictionary<int, Dictionary<string, ITimeSeriesOutput>> data = new Dictionary<int, Dictionary<string, ITimeSeriesOutput>>();
            int x = 1;
            Dictionary<string, string> meta = new Dictionary<string, string>();
            foreach (string com in lst)
            {
                //Setting all to ITimeSeriesOutput
                input.Source = input.Geometry.GeometryMetadata["precipSource"];
                ITimeSeriesOutput precipOutput = cd.getCatchmentAggregation(input, dtToITSOutput(ds.Tables[3], x, input.Source), gd, input.Aggregation);//cd.getCatchmentAggregation(input, precipResult, gd, input.Aggregation);
                input.Source = input.RunoffSource;
                ITimeSeriesOutput surfOutput = cd.getCatchmentAggregation(input, dtToITSOutput(ds.Tables[0], x, input.Source), gd, input.Aggregation); //dtToITSOutput(ds.Tables[0]); //cd.getCatchmentAggregation(input, surfResult, gd, input.Aggregation);
                input.Source = input.RunoffSource;
                ITimeSeriesOutput subOutput = cd.getCatchmentAggregation(input, dtToITSOutput(ds.Tables[1], x, input.Source), gd, input.Aggregation);//dtToITSOutput(ds.Tables[1]);//cd.getCatchmentAggregation(input, subResult, gd, input.Aggregation);
                input.Source = input.StreamHydrology;
                ITimeSeriesOutput hydrologyOutput = cd.getCatchmentAggregation(input, dtToITSOutput(ds.Tables[2], x, input.Source), gd, input.Aggregation);// dtToITSOutput(ds.Tables[2]);//cd.getCatchmentAggregation(input, dtToITSOutput(ds.Tables[2]), gd, input.Aggregation);

                Dictionary<string, ITimeSeriesOutput> timeSeriesDict = new Dictionary<string, ITimeSeriesOutput>();
                timeSeriesDict.Add("Precipitation", precipOutput);
                timeSeriesDict.Add("SurfaceRunoff", surfOutput);
                timeSeriesDict.Add("SubsurfaceRunoff", subOutput);
                timeSeriesDict.Add("StreamHydrology", hydrologyOutput);
                data.Add(Int32.Parse(com), timeSeriesDict);
                meta = precipOutput.Metadata;
                x++;
            }

            //Turn delineation table to ITimeseries
            /*totalOutput.table = new Dictionary<string, Dictionary<string, string>>();*/
            Dictionary<string, Dictionary<string, string>> table = new Dictionary<string, Dictionary<string, string>>();

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
                if (table.ContainsKey(com))
                {
                    continue;
                }
                else
                {
                    table.Add(com, lv);
                }
            }

            //Adding delineation data to output
            totalOutput.Metadata = meta;
            totalOutput.Metadata = new Dictionary<string, string>()
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
            //totalOutput.Dataset = "Precipitation, SurfaceRunoff, SubsurfaceRunoff, StreamHydrology";
            //totalOutput.DataSource = input.Geometry.GeometryMetadata["precipSource"].ToString() + ", " + input.RunoffSource.ToString() + ", " + input.RunoffSource.ToString() + ", " + input.StreamHydrology.ToString();
            watch.Stop();
            string elapsed = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalMinutes.ToString();
            //totalOutput.Metadata.Add("Time_elapsed", elapsed);

            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true
            };

            WatershedWorkflowOutput wOutput = new WatershedWorkflowOutput();
            wOutput.Dataset = "Precipitation, SurfaceRunoff, SubsurfaceRunoff, StreamHydrology";
            wOutput.DataSource = input.Geometry.GeometryMetadata["precipSource"].ToString() + ", " + input.RunoffSource.ToString() + ", " + input.RunoffSource.ToString() + ", " + input.StreamHydrology.ToString();
            wOutput.Metadata = totalOutput.Metadata;
            wOutput.Metadata.Add("Time_elapsed", elapsed);
            wOutput.Data = new Dictionary<string, Dictionary<string, ITimeSeriesOutput>>();
            foreach (KeyValuePair<int, Dictionary<string, ITimeSeriesOutput>> kv in data)
            {
                wOutput.Data.Add(kv.Key.ToString(), kv.Value);
            }
            wOutput.Table = table;
            
            //totalOutput.Data = new Dictionary<string, List<string>>();
            //foreach(KeyValuePair<int, Dictionary<string, ITimeSeriesOutput>> kv in data)
            //{
            //    string dataString = JsonSerializer.Serialize(kv.Value, options);
            //    totalOutput.Data[kv.Key.ToString()] = new List<string> { dataString };
            //
            //totalOutput.table = new Dictionary<string, string>();
            //foreach (KeyValuePair<string, Dictionary<string, string>> kv in table)
            //{
            //    string tableString = JsonSerializer.Serialize(table, options);
            //    totalOutput.table[kv.Key] = tableString;
            //}

            return wOutput;
        }

        public ITimeSeriesOutput dtToITSOutput(DataTable dt, int x, string column2)
        {
            column2 = (column2 == null) ? "Stream Flow" : column2;
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
                { "column_2", column2 },
                { "units", "cubic meters" }
            };
            itimeoutput.Dataset = "Stream Flow";
            itimeoutput.DataSource = "curvenumber";
            return itimeoutput;
        }


        private string nceiStation(out string errorMsg, ITimeSeriesInput input)
        {
            errorMsg = "";
            string dbPath = "./App_Data/catchments.sqlite";
            string query = "";
            //NCEI station is required. If only a ComID is provided, use catchment centroid coordinates to find nearest NCEI station.
            if (input.Geometry.HucID != null)
            {
                //NEED TO CHANGE THESE QUERIES TO ONLY ADD ONE POINT
                if(input.Geometry.HucID.Length == 8)
                {
                    query = "SELECT CentroidLatitude, CentroidLongitude FROM PlusFlowlineVAA WHERE ComID in (SELECT COMID From HUC12_PU_COMIDs_CONUS Where HUC12 LIKE '" + input.Geometry.HucID.ToString() + "%') AND CentroidLatitude IS NOT NULL AND CentroidLongitude IS NOT NULL";
                }
                else if (input.Geometry.HucID.Length == 12)
                {
                    query = "SELECT CentroidLatitude, CentroidLongitude FROM PlusFlowlineVAA WHERE ComID in (SELECT COMID From HUC12_PU_COMIDs_CONUS Where HUC12='" + input.Geometry.HucID.ToString() + "') AND CentroidLatitude IS NOT NULL AND CentroidLongitude IS NOT NULL";
                }
            }            
            if (input.Geometry.ComID > 0)
            {
                query = "SELECT CentroidLatitude, CentroidLongitude FROM PlusFlowlineVAA WHERE ComID = " + input.Geometry.ComID.ToString();        
            }

            Dictionary<string, string> centroidDict = Utilities.SQLite.GetData(dbPath, query);
            if (centroidDict.Count == 0)
            {
                errorMsg = "ERROR: Unable to find catchment in database.";
                return null;
            }

            IPointCoordinate centroid = new PointCoordinate()
            {
                Latitude = double.Parse(centroidDict["CentroidLatitude"]),
                Longitude = double.Parse(centroidDict["CentroidLongitude"])
            };

            string nceiQuery = "/hms/gis/ncdc/stations/?latitude=" + centroid.Latitude.ToString() + "&longitude=" + centroid.Longitude.ToString() + "&geometry=point&startDate=" + input.DateTimeSpan.StartDate.ToString("yyyy-MM-dd") + "&endDate=" + input.DateTimeSpan.EndDate.ToString("yyyy-MM-dd") + "&crs=4326";

            //Using FLASK NCDC webservice            
            Utilities.NCEIResult result = Utilities.WebAPI.RequestData<Utilities.NCEIResult>(nceiQuery).Result;
            //Set NCEI station to closest station regardless of type
            double coverage = 0.0;
            double distance = 100000.0;
            bool stationFound = false;
            foreach (Utilities.NCEIStations details in result.data)//Opt for closest GHCND station, if any
            {
                if (details.id.Contains("GHCND"))
                {
                    if (details.data.datacoverage > coverage)
                    {
                        input.Geometry.StationID = details.id.ToString();
                        coverage = details.data.datacoverage;
                        distance = details.distance;
                        stationFound = true;
                    }
                    else if (details.data.datacoverage == coverage && details.distance < distance)
                    {
                        input.Geometry.StationID = details.id.ToString();
                        coverage = details.data.datacoverage;
                        distance = details.distance;
                        stationFound = true;
                    }
                }
            }
            if (!stationFound)
            {
                errorMsg = "ERROR: Unable to find a valid GHCND station.";
            }
            return input.Geometry.StationID;
        }
    }
}