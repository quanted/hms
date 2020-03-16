using AQUATOXNutrientModel;
using Data;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Utilities;
using Web.Services.Controllers;

namespace Web.Services.Models
{
    /// <summary>
    /// HMS Web Service Water Quality Model
    /// </summary>
    public class WSWaterQuality
    {
        private bool testMode = false;

        // Static Inputs
        private DateTime startDate = DateTime.Today.AddDays(-40);
        private DateTime endDate = DateTime.Today.AddDays(-8);

        private string taskID;
        private bool metric = true;

        private int headwaterFromCOMID;
        private ITimeSeriesOutput headwater;
        private List<NetworkCatchment> catchments;
        private double maxStreamLength = 0;

        private int minNitrate = 1000;
        private int maxNitrate = 10000;
        private int minAmmonia = 100000;
        private int maxAmmonia = 750000;

        private ITimeSeriesOutput nceiPrecip;
        private int seed = 42;
        private Random rnd;

        private Dictionary<int, NationalWaterModelData> nwmData;
        private Dictionary<int, List<double>> lateralFlow;

        private Dictionary<string, List<string>> bcAmmonia = new Dictionary<string, List<string>>();
        private Dictionary<string, List<string>> bcNitrate = new Dictionary<string, List<string>>();
        private Dictionary<int, ITimeSeriesOutput> completedAmmonia = new Dictionary<int, ITimeSeriesOutput>();
        private Dictionary<int, ITimeSeriesOutput> completedNitrate = new Dictionary<int, ITimeSeriesOutput>();
        private List<string> completedAquatox = new List<string>();

        // Contains a dictionary of complete flow catchment outputs (flow column)
        private Dictionary<int, ITimeSeriesOutput> completeFlows = new Dictionary<int, ITimeSeriesOutput>();
        // Contains a dictionary of all completed contributing outputs (surface flow and sub surface flow columns)
        private Dictionary<int, ITimeSeriesOutput> completedContributingCatchments = new Dictionary<int, ITimeSeriesOutput>();

        /// <summary>
        /// Gets water quality data using the given TimeSeriesInput parameters.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ITimeSeriesOutput> GetWaterQualityData(WaterQualityInput input)
        {
            string errorMsg = "";
            DateTime now = DateTime.Now;
            var stpWatch = System.Diagnostics.Stopwatch.StartNew();
            Utilities.Logger.WriteToFile(input.TaskID, new List<string>(){
                "--- Starting HMS WQ Workflow ---",
                "taskID: " + input.TaskID,
                "souce: " + input.DataSource,
                "timestamp: " + now.ToString(),
                "--------------------------------"
            });
            this.minNitrate = input.MinNitrate < 0 ? input.MinNitrate : this.minNitrate;
            this.minAmmonia = input.MinAmmonia < 0 ? input.MinAmmonia : this.minAmmonia;
            this.maxNitrate = input.MaxNitrate < 0 ? input.MaxNitrate : this.maxNitrate;
            this.maxAmmonia = input.MaxAmmonia < 0 ? input.MaxAmmonia : this.maxAmmonia;


            // Constructs default error output object containing error message.
            Utilities.ErrorOutput err = new Utilities.ErrorOutput();

            // Load static national water model data (start and end date determined by nwm data timeseries)
            this.LoadNWMData();
            this.rnd = new Random(this.seed);
            // Demo static file
            string filePath = File.Exists("App_Data\\NetworkConnectivityTable.xlsx") ? "App_Data\\NetworkConnectivityTable.xlsx" : "App_Data/NetworkConnectivityTable.xlsx";

            this.taskID = input.TaskID;

            // Step 1a: Load stream network connectivity table (App_Data/NetworkConnectivityTable.xlsx)
            Utilities.Logger.WriteToFile(input.TaskID, "Loading stream network connectivity table...");
            this.catchments = this.ReadXlsx(filePath);
            Utilities.Logger.WriteToFile(input.TaskID, "Successfully loaded Stream Network");

            this.maxStreamLength = this.catchments.Select(x => x.Length).Max();

            this.lateralFlow = this.CalculateNWMLateralFlows();

            // Step 2: Download flow data for the headwater COMID (first row in table) using WaterShedDelineation.NWM
            Utilities.Logger.WriteToFile(input.TaskID, "Headwater COMID: " + catchments.ElementAt(0).FromCOMID);
            Utilities.Logger.WriteToFile(input.TaskID, "Downloading NWM Data for headwater boundary condition...");
            if (this.testMode && this.TestModeFileCheck("nwm"))
            {
                this.headwater = this.TestModeFileRead("nwm");
            }
            else
            {
                this.headwater = this.GetNWMData(catchments.ElementAt(0).FromCOMID);
                if (this.testMode)
                {
                    this.TestModeFileWrite("nwm", this.headwater);
                }
            }

            Utilities.Logger.WriteToFile(input.TaskID, new List<string>(){
                "Successfully downloaded NWM Data for headwater boundary condition",
                ""
            });

            this.headwaterFromCOMID = catchments.ElementAt(0).FromCOMID;
            this.LoadAquatoxOutput();

            List<string> successfulCOMIDS = new List<string>();
            foreach (NetworkCatchment catchment in catchments)
            {
                Utilities.Logger.WriteToFile(input.TaskID, "Collecting data for catchment COMID: " + catchment.COMID);
                bool s = this.GetDataByCOMID(catchment, input.DataSource);
                if (s) { successfulCOMIDS.Add(this.taskID + "-" + catchment.COMID.ToString()); }
                Utilities.Logger.WriteToFile(input.TaskID, new List<string>(){
                    "Successfully collected data for catchment COMID: " + catchment.COMID,
                    ""
                });
            }
            Utilities.Logger.WriteToFile(input.TaskID, "Successfully collected stream network data");

            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput result = oFactory.Initialize();
            result.Dataset = "Water Quality Workflow";
            result.DataSource = input.DataSource;
            result.Data.Add("taskIDs", successfulCOMIDS);
            result.Data.Add("aquatoxTaskIDs", this.completedAquatox);
            result.Metadata.Add("catchmentCount", catchments.Count.ToString());
            result.Metadata.Add("startDate", this.startDate.ToString("yyyy-MM-dd"));
            result.Metadata.Add("endDate", this.endDate.ToString("yyyy-MM-dd"));
            result.Metadata.Add("streamStart", catchments.ElementAt(0).COMID.ToString());
            result.Metadata.Add("streamEnd", catchments.ElementAt(catchments.Count - 1).COMID.ToString());
            result.Metadata.Add("minAmmonia", this.minAmmonia.ToString());
            result.Metadata.Add("maxAmmonia", this.maxAmmonia.ToString());
            result.Metadata.Add("minNitrate", this.minNitrate.ToString());
            result.Metadata.Add("maxNitrate", this.maxNitrate.ToString());

            Utilities.Logger.WriteToFile(input.TaskID, "Task: " + input.TaskID + " Completed");
            stpWatch.Stop();
            Utilities.Logger.WriteToFile(input.TaskID, "Time to complete task: " + (stpWatch.ElapsedMilliseconds / 1000).ToString() + " sec");
            return result;
        }

        private Dictionary<int, List<double>> CalculateNWMLateralFlows()
        {
            Dictionary<int, List<double>> lateral = new Dictionary<int, List<double>>();
            foreach (NetworkCatchment catchment in catchments)
            {
                List<double> values = new List<double>();
                foreach(KeyValuePair<string, List<string>> kv in this.nwmData[catchment.COMID].Timeseries)
                {
                    double diff = Math.Abs(double.Parse(kv.Value[0]) - double.Parse(this.nwmData[catchment.FromCOMID].Timeseries[kv.Key][0]));
                    values.Add(diff);
                }
                lateral.Add(catchment.COMID, values);
            }
            return lateral;
        }

        private ITimeSeriesOutput GetNWMData(int catchment)
        {
            string errorMsg = "";

            // Commented out method uses HMS flask NWM endpoint to retrieve data, remote data server currently shutdown
            //WatershedDelineation.NWM nwm = new WatershedDelineation.NWM();
            //string flaskURL = (Environment.GetEnvironmentVariable("FLASK_SERVER") != null) ? Environment.GetEnvironmentVariable("FLASK_SERVER") : "http://localhost:7777";
            //string nwmUrl = flaskURL + "/hms/nwm/data/?dataset=streamflow&comid=" + catchment.ToString() + "&startDate=" + this.startDate.ToString("yyyy-MM-dd") + "&endDate=" + this.endDate.ToString("yyyy-MM-dd");
            //Utilities.Logger.WriteToFile(this.taskID, "Getting NWM data with URL: " + nwmUrl);
            //string data = nwm.GetData(out errorMsg, nwmUrl);
            //dynamic jsonData = JSON.Deserialize<dynamic>(data);

            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput nwmData = oFactory.Initialize();
            nwmData.Dataset = "streamflow";
            nwmData.DataSource = "National Water Model";
            Dictionary<string, List<string>> timeseries;

            //timeseries = JSON.Deserialize<Dictionary<string, List<string>>>(JSON.Serialize(jsonData.data));
            timeseries = this.nwmData[catchment].Timeseries;


            if (this.testMode && timeseries == null)
            {
                string contaminantInput = "{'startDate': '" + this.startDate.ToString("yyyy-MM-dd HH") + "', 'endDate': '" + this.endDate.ToString("yyyy-MM-dd HH") + "', 'temporalResolution': 'daily', 'min':'5000', 'max':'6000'}";
                ContaminantLoader.ContaminantLoader contaminant = new ContaminantLoader.ContaminantLoader("uniform", "json", contaminantInput);
                nwmData.Data = contaminant.Result.Data;
            }
            else
            {
                nwmData.Data = timeseries;
            }
            return nwmData;
        }

        private bool GetDataByCOMID(NetworkCatchment catchment, string source)
        {
            string errorMsg = "";

            List<ITimeSeriesOutput> outputData = new List<ITimeSeriesOutput>();
            ITimeSeriesOutput bcFlow;
            ITimeSeriesOutput nwmFlow;
            ITimeSeriesOutput totalFlow;
            ITimeSeriesOutput result;

            // rectangle estimate 
            double estimateVolume = (catchment.Length * 1000) * (400 * 0.3048) * (40 * 0.3048);         // Length converted from km to m, width ~= 400ft converted to m, depth ~= 40ft converted to m

            AQTNutrientsModel aquatoxSim = this.SetAquatoxInput(catchment);

            Utilities.Logger.WriteToFile(this.taskID, "Generating ammonia loading for catchment: " + catchment.COMID);
            ITimeSeriesOutput ammoniaLoadingData = this.FlowRatioForLoading(catchment, this.maxAmmonia, this.minAmmonia);
            Utilities.Logger.WriteToFile(this.taskID, "Successfully generated ammonia loading data");

            Utilities.Logger.WriteToFile(this.taskID, "Generating nitrate loading for catchment: " + catchment.COMID);
            ITimeSeriesOutput nitrateLoadingData = this.FlowRatioForLoading(catchment, this.maxNitrate, this.minNitrate);
            Utilities.Logger.WriteToFile(this.taskID, "Successfully generated ammonia loading data");

            if (catchment.FromCOMID == this.headwaterFromCOMID)
            {
                aquatoxSim.AQSim.AQTSeg.SV[0].LoadsRec.Loadings.ITSI.InputTimeSeries["input"].Data = this.bcAmmonia;
                aquatoxSim.AQSim.AQTSeg.SV[1].LoadsRec.Loadings.ITSI.InputTimeSeries["input"].Data = this.bcNitrate;
            }
            else
            {
                aquatoxSim.AQSim.AQTSeg.SV[0].LoadsRec.Loadings.ITSI.InputTimeSeries["input"].Data = this.completedAmmonia[catchment.FromCOMID].Data;
                aquatoxSim.AQSim.AQTSeg.SV[1].LoadsRec.Loadings.ITSI.InputTimeSeries["input"].Data = this.completedNitrate[catchment.FromCOMID].Data;
            }
            if (source.Equals("nwm"))
            {
                Utilities.Logger.WriteToFile(this.taskID, "Loading NWM data for catchment: " + catchment.COMID);
                // NWM source only loads nwm flow data, contaminant and Aquatox timeseries
                totalFlow = this.GetNWMData(catchment.COMID);
                outputData.Add(totalFlow);  // Column 1: NWM flow data
                Utilities.Logger.WriteToFile(this.taskID, "Successfully loaded NWM data.");

                outputData.Add(ammoniaLoadingData);    // Column 2: Contaminant Loading
                outputData.Add(nitrateLoadingData);

                result = Utilities.Merger.MergeTimeSeries(outputData);
                result.Dataset = "wq_workflow";
                result.DataSource = source;
                result.Metadata = this.SetMetadata(result.Metadata, catchment);
                result.Metadata.Add("column_1", "total_flow");
                result.Metadata.Add("column_2", "ammonia_loading");
                result.Metadata.Add("column_3", "nitrate_loading");
                result.Metadata.Add("column_4", "ammonia_concentration");
                result.Metadata.Add("column_5", "nitrate_concentration");
                result.Metadata.Add("column_1_units", (this.metric) ? "m3/day" : "f3/day");
                result.Metadata.Add("column_2_units", "mg day");
                result.Metadata.Add("column_3_units", "mg day");
                result.Metadata.Add("column_4_units", "mg/L day");
                result.Metadata.Add("column_5_units", "mg/L day");
            }
            else
            {
                // Step 2b: Use downloaded NWM data for headwater boundary condition
                if (catchment.FromCOMID == this.headwaterFromCOMID)
                {
                    bcFlow = this.headwater;
                    nwmFlow = this.headwater;
                }
                // Step 2c: Read in flow data from previously calculated COMID flow
                else
                {   // Download NWM streamflow data for COMID for comparison
                    if (this.testMode)              // In testMode, reuse initail headwater data.
                    {
                        nwmFlow = this.headwater;
                    }
                    else
                    {
                        nwmFlow = this.GetNWMData(catchment.COMID);
                    }
                    if (this.completeFlows.ContainsKey(catchment.FromCOMID))
                    {
                        bcFlow = this.completeFlows[catchment.FromCOMID];
                    }
                    else
                    {
                        // If the boundary condition flow has not previously been calculated, set equal to NWM flow
                        bcFlow = nwmFlow;
                        // if not nwm data, set to zero
                    }
                }
                outputData.Add(bcFlow);             // Column 1: Boundary Condition Flow
                Utilities.Logger.WriteToFile(this.taskID, "Getting precipitation data for catchment: " + catchment.COMID);
                ITimeSeriesOutput precip = this.GetPrecipData(source, catchment.COMID);
                Utilities.Logger.WriteToFile(this.taskID, "Successfully retrieved precipitation data");

                //outputData.Add(nwmFlow);            // Column 2: NWM Flow (will be the same for the first catchment)
                Utilities.Logger.WriteToFile(this.taskID, "Getting contributing runoff/baseflow data for catchment: " + catchment.COMID);
                ITimeSeriesOutput contributing = this.GetContributingFlow(catchment.COMID, catchment.ContributingCOMIDs, source, precip);
                Utilities.Logger.WriteToFile(this.taskID, "Successfully retrieved runoff/baseflow data");
               
                outputData.Add(contributing);       // Column 2/3: Surface Runoff and Subsurface flow (from all contributing catchments)
                outputData.Add(precip);             // Column 4: Precipitation of current catchment.

                // Step 6: Calculate total flow ( step2 + step4 + step5 )
                Utilities.Logger.WriteToFile(this.taskID, "Calculating total flow data for catchment: " + catchment.COMID);
                totalFlow = Utilities.Merger.SumTimeSeriesByRow("total flow", source, new List<ITimeSeriesOutput>() { bcFlow, contributing });
                Utilities.Logger.WriteToFile(this.taskID, "Successfully calculated total flow data");
                outputData.Add(totalFlow);          // Column 5: Total Flow
                this.completeFlows.Add(catchment.COMID, totalFlow);

                outputData.Add(ammoniaLoadingData);
                outputData.Add(nitrateLoadingData);

                result = Utilities.Merger.MergeTimeSeries(outputData);
                result.Dataset = "wq_workflow";
                result.DataSource = source;
                result.Metadata = this.SetMetadata(result.Metadata, catchment);
                result.Metadata.Add("column_1", "boundary_condition_flow");
                result.Metadata.Add("column_2", "total_surface_runoff");
                result.Metadata.Add("column_3", "total_baseflow");
                result.Metadata.Add("column_4", "total_precip");
                result.Metadata.Add("column_5", "total_flow");
                result.Metadata.Add("column_6", "ammonia_loading");
                result.Metadata.Add("column_7", "nitrate_loading");
                result.Metadata.Add("column_8", "ammonia_concentration");
                result.Metadata.Add("column_9", "nitrate_concentration");
                result.Metadata.Add("column_1_units", (this.metric) ? "m3/day" : "f3/day");
                result.Metadata.Add("column_2_units", "m3/day");
                result.Metadata.Add("column_3_units", "m3/day");
                result.Metadata.Add("column_4_units", "mm/day");
                result.Metadata.Add("column_5_units", "m3/day");
                result.Metadata.Add("column_6_units", "mg day");
                result.Metadata.Add("column_7_units", "mg day");
                result.Metadata.Add("column_8_units", "mg/L day");
                result.Metadata.Add("column_9_units", "mg/L day");
            }

            foreach (KeyValuePair<string, List<string>> kv in ammoniaLoadingData.Data)
            {
                string dtValues = kv.Key.Split(" ")[0];
                DateTime dt = Convert.ToDateTime(dtValues);
                aquatoxSim.AQSim.AQTSeg.SV[0].LoadsRec.Alt_Loadings[2].list.Add(dt, Convert.ToDouble(kv.Value[0]));
            }
            aquatoxSim.AQSim.AQTSeg.SV[0].LoadsRec.Alt_Loadings[2].UseConstant = false;
            foreach (KeyValuePair<string, List<string>> kv in nitrateLoadingData.Data)
            {
                string dtValues = kv.Key.Split(" ")[0];
                DateTime dt = Convert.ToDateTime(dtValues);
                aquatoxSim.AQSim.AQTSeg.SV[1].LoadsRec.Alt_Loadings[2].list.Add(dt, Convert.ToDouble(kv.Value[0]));
            }
            aquatoxSim.AQSim.AQTSeg.SV[1].LoadsRec.Alt_Loadings[2].UseConstant = false;

            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true
            };

            // Run Aquatox model
            string aquaTaskID = this.taskID + "-" + catchment.COMID.ToString() + "-aquatox";
            aquatoxSim.AQSim.AQTSeg.SV[3].LoadsRec.Alt_Loadings[0].ITSI.InputTimeSeries["input"].Data = totalFlow.Data;
            aquatoxSim.AQSim.AQTSeg.SV[3].InitialCond = Convert.ToInt32(estimateVolume);
            aquatoxSim.AQSim.Integrate();
            this.SetAquatoxOutputToObject(catchment.COMID, aquatoxSim);
            Utilities.Logger.WriteToFile(this.taskID, "Dumping aquatox data for catchment: " + catchment.COMID);
            string aqtOutput = "";
            aqtOutput = aquatoxSim.AQSim.ExportJSON(ref aqtOutput);
            Utilities.MongoDB.DumpData(aquaTaskID, aqtOutput);
            //Utilities.Logger.WriteToFile(catchment.COMID + "-aquatox", aquatoxOutput);
            Utilities.Logger.WriteToFile(this.taskID, "Aquatox data dumped into mongodb with taskID: " + aquaTaskID);
            this.completedAquatox.Add(aquaTaskID);
            result = Utilities.Merger.MergeTimeSeries(new List<ITimeSeriesOutput>() { result, this.completedAmmonia[catchment.COMID], this.completedNitrate[catchment.COMID] });
            result.Metadata.Add("estimated_volume", estimateVolume.ToString());
            result.Dataset = "water_quality_workflow";
            result.DataSource = source;

            string taskID = this.taskID + "-" + catchment.COMID.ToString();
            //Dumb result in database
            Utilities.Logger.WriteToFile(this.taskID, "Dumping collected data for catchment: " + catchment.COMID);
            Utilities.MongoDB.DumpData(taskID, JSON.Serialize(result));
            Utilities.Logger.WriteToFile(this.taskID, "Catchment data dumped into mongodb with taskID: " + taskID);

            return true;
        }

        private ITimeSeriesOutput FlowRatioForLoading(NetworkCatchment catchment, double maxLoad, double minLoad)
        {

            double minFlow = this.lateralFlow.Select(x => x.Value.Min()).Min();
            double maxFlow = this.lateralFlow.Select(x => x.Value.Max()).Max();
            //minFlow = (minFlow < 100) ? 100 : minFlow;

            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput output = oFactory.Initialize();
            int i = 0;
            foreach(KeyValuePair<string, List<string>> kv in this.nwmData[catchment.COMID].Timeseries)
            {
                double iFlow = this.lateralFlow[catchment.COMID][i];
                double ratio = (iFlow - minFlow) / (maxFlow - minFlow);
                ratio = (ratio < 0) ? 0 : ratio;
                double load = (maxLoad - minLoad) * ratio + minLoad;
                load = (iFlow == 0) ? 0.0 : load;
                List<string> valueList = new List<string>() { { load.ToString() } };
                output.Data.Add(kv.Key, valueList);
                i += 1;
            }
            return output;
        }

        private Dictionary<string, string> SetMetadata(Dictionary<string, string> current, NetworkCatchment catchment)
        {
            current.Add("COMID", catchment.COMID.ToString());
            current.Add("FromCOMID", catchment.FromCOMID.ToString());
            current.Add("ToCOMID", catchment.ToCOMID.ToString());
            current.Add("ContributingCOMIDs", String.Join(", ", catchment.ContributingCOMIDs));
            current.Add("Length(km)", catchment.Length.ToString());
            current.Add("ReachCode", catchment.ReachCode.ToString());
            current.Add("HydroSeq", catchment.HydroSeq.ToString());

            return current;
        }
        private ITimeSeriesOutput GetContributingFlow(int current, List<int> catchments, string source, ITimeSeriesOutput precipData)
        {
            string errorMsg = "";
            List<ITimeSeriesOutput> catchmentData = new List<ITimeSeriesOutput>();
            // Get SurfaceFlow for each catchment in catchments
            // Calculate Subsurface flow for each catchment in catchment
            // Sum all surfaceflow timestep values
            // Sum all subsurface flow timestep values
            // Merge surfaceflow timeseries and subsurfaceflow timeseries, return result

            // Add current catchment COMID to contributing catchments.
            catchments.Add(current);

            // Steps 4/5
            var pOptions = new ParallelOptions()
            {
                MaxDegreeOfParallelism = 6
            };
            Parallel.ForEach(catchments, pOptions, (catchment) =>
            //foreach (int catchment in catchments)
            {
                if (this.completedContributingCatchments.ContainsKey(catchment))
                {
                    Utilities.Logger.WriteToFile(this.taskID, "Using existing data for COMID: " + catchment);
                    catchmentData.Add(this.completedContributingCatchments[catchment]);
                }
                else
                {
                    Thread.Sleep(500);
                    PointCoordinate point = Utilities.COMID.GetCentroid(catchment, out errorMsg);
                    if (point == null)
                    {
                        point = Utilities.COMID.GetCentroid(current, out errorMsg);
                        if (point == null)
                        {
                            point = Utilities.COMID.GetCentroid(this.catchments.Where(x => x.COMID == current).First().FromCOMID, out errorMsg);
                        }
                        if (point == null)
                        {
                            point = Utilities.COMID.GetCentroid(this.catchments.ElementAt(0).COMID, out errorMsg);
                        }
                        else
                        {
                            Utilities.Logger.WriteToFile(this.taskID, "Unable to get centroid for: " + catchment);
                        }
                    }
                    ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
                    ITimeSeriesInput input = iFactory.Initialize();
                    input.DateTimeSpan.StartDate = this.startDate;
                    input.DateTimeSpan.EndDate = this.endDate;
                    input.Source = source;
                    input.Geometry.Point = point;
                    input.Geometry.ComID = catchment;
                    Timezone tz = new Timezone()
                    {
                        Offset = -5,
                        Name = "East Coast"
                    };
                    input.Geometry.Timezone = tz;
                    input.TemporalResolution = "daily";
                    if (source.Equals("ncei"))
                    {
                        input.Source = "curvenumber";
                        input.InputTimeSeries = new Dictionary<string, TimeSeriesOutput>()
                        {
                            { "precipitation", precipData as TimeSeriesOutput }
                        };
                    }
                    List<ITimeSeriesOutput> data = new List<ITimeSeriesOutput>();
                    SurfaceRunoff.SurfaceRunoff runoff = new SurfaceRunoff.SurfaceRunoff();
                    runoff.Input = input;

                    runoff.Input.BaseURL.Add(TimeSeriesInputFactory.GetBaseURL(source, "surfacerunoff"));
                    Utilities.Logger.WriteToFile(this.taskID, "Downloading runoff data for COMID: " + catchment);
                    ITimeSeriesOutput runoffData = null;
                    if (this.testMode && this.TestModeFileCheck("surfacerunoff"))
                    {
                        runoffData = this.TestModeFileRead("surfacerunoff");
                    }
                    else
                    {
                        bool runoffB = true;
                        int runoffI = 5;
                        while (runoffB && runoffI != 0) {
                            runoffData = runoff.GetData(out errorMsg);
                            if(runoffData != null)
                            {
                                runoffB = false;
                            }
                            else if(errorMsg.Contains("ComID"))
                            {
                                if (runoff.Input.Geometry.ComID == current) {
                                    runoff.Input.Geometry.ComID = this.GetPreviousCOMID(current);
                                    input.Geometry.ComID = this.GetPreviousCOMID(current);
                                }
                                else
                                {
                                    runoff.Input.Geometry.ComID = current;
                                    input.Geometry.ComID = current;
                                }
                            }
                            else
                            {
                                Thread.Sleep(500);
                            }
                            runoffI -= 1;
                        }
                        if (this.testMode)
                        {
                            this.TestModeFileWrite("surfacerunoff", runoffData);
                        }
                    }
                    Utilities.Logger.WriteToFile(this.taskID, "Successfully downloaded runoff data for COMID: " + catchment);

                    SubSurfaceFlow.SubSurfaceFlow subsurface = new SubSurfaceFlow.SubSurfaceFlow();
                    subsurface.Input = input;
                    subsurface.Input.BaseURL = new List<string>() { TimeSeriesInputFactory.GetBaseURL(source, "subsurfaceflow") };
                    subsurface.Input.InputTimeSeries = new Dictionary<string, TimeSeriesOutput>();
                    if (source.Equals("curvenumber") || source.Equals("ncei"))
                    {
                        input.Source = "curvenumber";
                        subsurface.Input.InputTimeSeries.Add("surfacerunoff", runoffData as TimeSeriesOutput);
                    }
                    Utilities.Logger.WriteToFile(this.taskID, "Downloading baseflow data for COMID: " + catchment);
                    ITimeSeriesOutput subsurfaceflow = null;
                    if (this.testMode && this.TestModeFileCheck("subsurfaceflow"))
                    {
                        subsurfaceflow = this.TestModeFileRead("subsurfaceflow");
                    }
                    else
                    {
                        bool baseflowB = true;
                        int baseflowI = 5;
                        while (baseflowB && baseflowI != 0)
                        {
                            subsurfaceflow = subsurface.GetData(out errorMsg);
                            if (subsurfaceflow != null)
                            {
                                baseflowB = false;
                            }
                            else
                            {
                                Thread.Sleep(500);
                            }
                            baseflowI -= 1;
                        }
                        if (this.testMode)
                        {
                            this.TestModeFileWrite("subsurfaceflow", subsurfaceflow);
                        }
                    }
                    string catchmentDBFile = File.Exists("App_Data\\catchments.sqlite") ? "App_Data\\catchments.sqlite" : "App_Data/catchments.sqlite";
                    string areaQuery = "SELECT * FROM PlusFlowlineVAA WHERE ComID = " + catchment;
                    Dictionary<string, string> catchmentDetails = Utilities.SQLite.GetData(catchmentDBFile, areaQuery);

                    double catchmentArea = catchmentDetails.ContainsKey("AreaSqKM") ? double.Parse(catchmentDetails["AreaSqKM"]) : 1;    // If key doesn't exist set area 1km^2
                    runoffData = Utilities.Merger.ModifyTimeSeries(runoffData, catchmentArea * 1000);  // Unit conversion from mm/day to m^3/day, 1000m/1km * 1000m/1km
                    subsurfaceflow = Utilities.Merger.ModifyTimeSeries(subsurfaceflow, catchmentArea * 1000);  // Unit conversion from mm/day to m^3/day, 1m/1000mm * 1000m/1km * 1000m/1km

                    data.Add(runoffData);
                    data.Add(subsurfaceflow);
                    Utilities.Logger.WriteToFile(this.taskID, "Successfully downloaded baseflow data for COMID: " + catchment);

                    ITimeSeriesOutput mergedData = Utilities.Merger.MergeTimeSeries(data.ElementAt(0), data.ElementAt(1));
                    this.completedContributingCatchments.Add(catchment, mergedData);
                    catchmentData.Add(mergedData);
                }
            }
            );
            Utilities.Logger.WriteToFile(this.taskID, "Summing up contributing flow for: " + current);

            return Utilities.Merger.SumTimeSeriesByColumn(catchmentData, 1.0);
        }

        private int GetPreviousCOMID(int current)
        {
            return this.catchments.Where(x => x.COMID == current).First().FromCOMID;
        }

        private ITimeSeriesOutput GetPrecipData(string source, int catchment)
        {
            string errorMsg = "";

            ITimeSeriesInputFactory iFactory = new TimeSeriesInputFactory();
            ITimeSeriesInput input = iFactory.Initialize();
            if (source.Equals("ncei") && this.nceiPrecip == null)
            {
                input.Geometry.StationID = "GHCND:US1NCBD0002";
            }
            else if (source.Equals("ncei"))
            {
                return this.nceiPrecip;
            }
            else
            {
                PointCoordinate point = Utilities.COMID.GetCentroid(catchment, out errorMsg);
                if(point == null)
                {
                    point = Utilities.COMID.GetCentroid(this.catchments.Where(x => x.COMID == catchment).First().FromCOMID, out errorMsg);
                }
                input.Geometry.Point = point;
            }
            Timezone tz = new Timezone()
            {
                Offset = -5,
                Name = "East Coast"
            };
            input.Geometry.Timezone = tz;
            input.DateTimeSpan.StartDate = this.startDate;
            input.DateTimeSpan.EndDate = this.endDate;
            input.Source = source;
            input.TemporalResolution = "daily";
            Precipitation.Precipitation precip = new Precipitation.Precipitation();
            precip.Input = input;
            precip.Input.BaseURL.Add(TimeSeriesInputFactory.GetBaseURL(source, "precipitation"));
            ITimeSeriesOutput data = null;
            if (this.testMode && this.TestModeFileCheck("precipitation"))
            {
                return this.TestModeFileRead("precipitation");
            }
            else
            {
                data = precip.GetData(out errorMsg);
                if (this.testMode)
                {
                    this.TestModeFileWrite("precipitation", data);
                }
            }
            if (source.Equals("ncei"))
            {
                this.nceiPrecip = data;
            }
            return data;
        }

        private List<NetworkCatchment> ReadXlsx(string filePath)
        {
            List<NetworkCatchment> catchments = new List<NetworkCatchment>();
            using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(filePath, false))
            {
                WorkbookPart bkPart = spreadsheetDocument.WorkbookPart;
                SharedStringTablePart sTablePart = bkPart.GetPartsOfType<SharedStringTablePart>().First();
                SharedStringTable sTable = sTablePart.SharedStringTable;

                WorksheetPart shPart = bkPart.WorksheetParts.FirstOrDefault();
                SheetData data = shPart.Worksheet.Elements<SheetData>().FirstOrDefault();
                bool first = true;
                foreach (Row r in data)
                {
                    if (first)
                    {
                        first = false;
                        continue;
                    }
                    List<string> row = new List<string>();
                    foreach (Cell cell in r)
                    {
                        if (cell.CellReference.InnerText.Contains("E") && row.Count < 4)
                        {
                            row.Add("");
                        }
                        else if ((cell.DataType != null) && (cell.DataType == CellValues.SharedString))
                        {
                            int ssid = int.Parse(cell.CellValue.Text);
                            string strV = sTable.ChildElements[ssid].InnerText;
                            row.Add(strV);
                        }
                        else if (cell.CellValue == null)
                        {
                            row.Add("");
                        }
                        else
                        {
                            row.Add(cell.CellValue.Text);
                        }
                    }
                    if (row.Count == 9)
                    {
                        row.Add("");
                    }
                    catchments.Add(new NetworkCatchment(row.ElementAt(0), row.ElementAt(1), row.ElementAt(2), row.ElementAt(3),
                        row.ElementAt(4), row.ElementAt(5), row.ElementAt(6), row.ElementAt(7), row.ElementAt(8), row.ElementAt(9)));
                }
            }
            return catchments;
        }

        private void LoadNWMData()
        {
            this.nwmData = new Dictionary<int, NationalWaterModelData>();
            string filePath = File.Exists("App_Data\\CapeFearDailyFlow.csv") ? "App_Data\\CapeFearDailyFlow.csv" : "App_Data/CapeFearDailyFlow.csv";

            double conversionFactor = (this.metric) ? 0.0283168 : 1.0;          // Cubic feet to cubic meters
            conversionFactor = conversionFactor * (3600 * 24);                  // per second to per day
            Dictionary<int, Dictionary<string, double>> tsData = new Dictionary<int, Dictionary<string, double>>();
            using (var reader = new StreamReader(filePath))
            {
                bool first = true;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (first)
                    {
                        first = false;
                        continue;
                    }
                    var values = line.Split(",");
                    string dateStr = values[0] + " 00";
                    int comid = int.Parse(values[1]);
                    string flow = (double.Parse(values[2]) * conversionFactor).ToString("E3");
                    if (this.nwmData.ContainsKey(comid))
                    {
                        this.nwmData[comid].Timeseries.Add(dateStr, new List<string>() { flow });
                    }
                    else
                    {
                        NationalWaterModelData data = new NationalWaterModelData();
                        data.COMID = comid;
                        data.Timeseries = new Dictionary<string, List<string>>() { { dateStr, new List<string>() { flow } } };
                        this.nwmData.Add(comid, data);
                    }
 
                }
            }
            this.startDate = DateTime.ParseExact(this.nwmData.First().Value.Timeseries.Keys.First(), "yyyy-MM-dd HH", CultureInfo.InvariantCulture);
            this.endDate = DateTime.ParseExact(this.nwmData.First().Value.Timeseries.Keys.Last(), "yyyy-MM-dd HH", CultureInfo.InvariantCulture);
        }

        private bool TestModeFileCheck(string dataset)
        {
            string filePath = "App_Data\\" + dataset + "_testdata.json";
            return File.Exists(filePath);
        }

        private void TestModeFileWrite(string dataset, ITimeSeriesOutput output)
        {
            string filePath = "App_Data\\" + dataset + "_testdata.json";
            try
            {
                using (System.IO.StreamWriter file =
                    new System.IO.StreamWriter(filePath))
                {
                    file.Write(JsonSerializer.Serialize(output));
                }
            }
            catch (System.IO.IOException ex) {
                Thread.Sleep(1000);
                this.TestModeFileWrite(dataset, output);
            }
        }

        private ITimeSeriesOutput TestModeFileRead(string dataset)
        {
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true
            };
            string filePath = "App_Data\\" + dataset + "_testdata.json";
            ITimeSeriesOutput output = JsonSerializer.Deserialize<TimeSeriesOutput>(File.ReadAllText(filePath), options);
            return output;
        }

        private AQTNutrientsModel SetAquatoxInput(NetworkCatchment catchment)
        {
            AQTNutrientsModel input = this.LoadAquatoxInputFile();
            
            input.AQSim.AQTSeg.Location.Locale.SiteName = catchment.COMID.ToString();
            input.AQSim.AQTSeg.Location.Locale.SiteLength = catchment.Length;
            input.AQSim.AQTSeg.PSetup.FirstDay = this.startDate;
            input.AQSim.AQTSeg.PSetup.LastDay = this.endDate;

            return input;
        }

        private void LoadAquatoxOutput()
        {
            string filePath = File.Exists("App_Data\\aquatox_nutrient_model_output.txt") ? "App_Data\\aquatox_nutrient_model_output.txt" : "App_Data/aquatox_nutrient_model_output.txt";

            try
            {
                string errorMsg = "";
                string textString = System.IO.File.ReadAllText(filePath);
                AQTNutrientsModel output = new AQTNutrientsModel(ref textString,out errorMsg, false);
                //Import ammonia sample output as the input for the boundary condition
                DateTime iDate = new DateTime(this.startDate.Year, this.startDate.Month, this.startDate.Day);
                foreach (var values in output.AQSim.AQTSeg.SV[0].output.Data)
                {
                    if (iDate <= this.endDate)
                    {
                        string v = (values.Value[0]).ToString();
                        this.bcAmmonia.Add(iDate.ToString("yyyy-MM-dd HH"), new List<string>() { v });
                    }
                    iDate = iDate.AddDays(1);
                }
                //Import nitrate sample output as the input for the boundary condition
                iDate = new DateTime(this.startDate.Year, this.startDate.Month, this.startDate.Day);
                foreach (var values in output.AQSim.AQTSeg.SV[1].output.Data)
                {
                    if (iDate <= this.endDate)
                    {
                        this.bcNitrate.Add(iDate.ToString("yyyy-MM-dd HH"), new List<string>() { values.Value[0].ToString() });
                    }
                    iDate = iDate.AddDays(1);
                }

            }
            catch (Exception ex)
            {
                Utilities.Logger.WriteToFile(this.taskID, "Error loading input configuration for aqautox nutrient model. " + ex.Message);
            }
        }

        private AQTNutrientsModel LoadAquatoxInputFile()
        {
            string filePath = File.Exists("App_Data\\aquatox_nutrient_model_input.txt") ? "App_Data\\aquatox_nutrient_model_input.txt" : "App_Data/aquatox_nutrient_model_input.txt";
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true
            };
            try
            {
                string fileData = System.IO.File.ReadAllText(filePath);
                string errorMsg = "";
                AQTNutrientsModel model = new AQTNutrientsModel(ref fileData, out errorMsg, false);
                return model;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error loading input configuration for aqautox nutrient model. Error: " + ex.Message);
                return null;
            }
        }

        private void SetAquatoxOutputToObject(int comid, AQTNutrientsModel model)
        {
            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput ammonia = oFactory.Initialize();
            ITimeSeriesOutput nitrate = oFactory.Initialize();
            ammonia.Data = model.AQSim.AQTSeg.SV[0].output.Data;
            this.completedAmmonia.Add(comid, ammonia);
            nitrate.Data = model.AQSim.AQTSeg.SV[1].output.Data;
            this.completedNitrate.Add(comid, nitrate);
        }
    }

    public class NetworkCatchment
    {
        public int COMID;
        public int FromCOMID;
        public int ToCOMID;
        public List<int> ContributingCOMIDs;
        public long ReachCode;
        public double FromMeas;
        public double ToMeas;
        public int HydroSeq;
        public double Length;
        public string Comments;

        public NetworkCatchment(string comid, string from, string to, string contrib, string reach, string fromM, string toM, string seq, string len, string comm)
        {
            this.COMID = int.Parse(comid);
            this.FromCOMID = int.Parse(from);
            this.ToCOMID = (to == "NULL") ? -1 : int.Parse(to);
            this.ContributingCOMIDs = (contrib == "") ? new List<int>() : contrib.Split(',').Select(s => int.Parse(s)).ToList();
            this.ReachCode = long.Parse(reach);
            this.FromMeas = double.Parse(fromM);
            this.ToMeas = double.Parse(toM);
            this.HydroSeq = int.Parse(seq);
            this.Length = double.Parse(len);
            this.Comments = comm;
        }
    }

    public class NationalWaterModelData
    {
        public int COMID;
        public Dictionary<string, List<string>> Timeseries;

        public NationalWaterModelData() { }

    }
}
