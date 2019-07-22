using AQUATOXNutrientModel;
using Data;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
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

        private int headwaterFromCOMID;
        private ITimeSeriesOutput headwater;
        private List<NetworkCatchment> catchments;

        private ITimeSeriesOutput nceiPrecip;

        private Dictionary<int, NationalWaterModelData> nwmData;

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

            // Constructs default error output object containing error message.
            Utilities.ErrorOutput err = new Utilities.ErrorOutput();

            // Load static national water model data (start and end date determined by nwm data timeseries)
            this.LoadNWMData();

            // Demo static file
            string filePath = "App_Data\\NetworkConnectivityTable.xlsx";
            if (!File.Exists(filePath))
            {
                filePath = "App_Data/NetworkConnectivityTable.xlsx";
            }

            this.taskID = input.TaskID;

            // Step 1a: Load stream network connectivity table (App_Data/NetworkConnectivityTable.xlsx)
            Utilities.Logger.WriteToFile(input.TaskID, "Loading stream network connectivity table...");
            this.catchments = this.ReadXlsx(filePath);
            Utilities.Logger.WriteToFile(input.TaskID, "Successfully loaded Stream Network");

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

            Utilities.Logger.WriteToFile(input.TaskID, "Task: " + input.TaskID + " Completed");
            stpWatch.Stop();
            Utilities.Logger.WriteToFile(input.TaskID, "Time to complete task: " + (stpWatch.ElapsedMilliseconds / 1000).ToString() + " sec");
            return result;
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
            List<ITimeSeriesOutput> outputData = new List<ITimeSeriesOutput>();
            ITimeSeriesOutput bcFlow;
            ITimeSeriesOutput nwmFlow;
            ITimeSeriesOutput totalFlow;
            ITimeSeriesOutput result;
            dynamic aquatoxInput = this.SetAquatoxInput(catchment);
            if (catchment.FromCOMID == this.headwaterFromCOMID)
            {
                aquatoxInput.SV[0].LoadsRec.Loadings.ITSI.InputTimeSeries.input.Data = JObject.FromObject(this.bcAmmonia);
                aquatoxInput.SV[1].LoadsRec.Loadings.ITSI.InputTimeSeries.input.Data = JObject.FromObject(this.bcNitrate);
            }
            else
            {
                aquatoxInput.SV[0].LoadsRec.Loadings.ITSI.InputTimeSeries.input.Data = JObject.FromObject(this.completedAmmonia[catchment.FromCOMID].Data);
                aquatoxInput.SV[1].LoadsRec.Loadings.ITSI.InputTimeSeries.input.Data = JObject.FromObject(this.completedNitrate[catchment.FromCOMID].Data);
            }


            if (source.Equals("nwm"))
            {
                Utilities.Logger.WriteToFile(this.taskID, "Loading NWM data for catchment: " + catchment.COMID);
                // NWM source only loads nwm flow data, contaminant and Aquatox timeseries
                totalFlow = this.GetNWMData(catchment.COMID);
                outputData.Add(totalFlow);  // Column 1: NWM flow data
                Utilities.Logger.WriteToFile(this.taskID, "Successfully loaded NWM data.");

                Utilities.Logger.WriteToFile(this.taskID, "Generating contaminant loading for catchment: " + catchment.COMID);
                string contaminantInput = "{'startDate': '" + this.startDate.ToString("yyyy-MM-dd HH") + "', 'endDate': '" + this.endDate.ToString("yyyy-MM-dd HH") + "', 'temporalResolution': 'daily', 'min':'0', 'max':'5'}";
                ContaminantLoader.ContaminantLoader contaminant = new ContaminantLoader.ContaminantLoader("uniform", "json", contaminantInput);
                ITimeSeriesOutput contaminantData = contaminant.Result;
                Utilities.Logger.WriteToFile(this.taskID, "Successfully generated contaminant loading data");
                outputData.Add(contaminantData);    // Column 2: Contaminant Loading

                result = Utilities.Merger.MergeTimeSeries(outputData);
                result.Dataset = "wq_workflow";
                result.DataSource = source;
                result.Metadata = this.SetMetadata(result.Metadata, catchment);
                result.Metadata.Add("column_1", "total_flow");
                result.Metadata.Add("column_2", "contaminant_loading");
                result.Metadata.Add("column_3", "aquatox_ammonia");
                result.Metadata.Add("column_4", "aquatox_nitrate");
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

                // Step 7: Generate contaminant loading (uniform distribution)
                Utilities.Logger.WriteToFile(this.taskID, "Generating contaminant loading for catchment: " + catchment.COMID);
                string contaminantInput = "{'startDate': '" + this.startDate.ToString("yyyy-MM-dd HH") + "', 'endDate': '" + this.endDate.ToString("yyyy-MM-dd HH") + "', 'temporalResolution': 'daily', 'min':'0', 'max':'5'}";
                ContaminantLoader.ContaminantLoader contaminant = new ContaminantLoader.ContaminantLoader("uniform", "json", contaminantInput);
                ITimeSeriesOutput contaminantData = contaminant.Result;
                Utilities.Logger.WriteToFile(this.taskID, "Successfully generated contaminant loading data");
                outputData.Add(contaminantData);    // Column 6: Contaminant Loading

                result = Utilities.Merger.MergeTimeSeries(outputData);
                result.Dataset = "wq_workflow";
                result.DataSource = source;
                result.Metadata = this.SetMetadata(result.Metadata, catchment);
                result.Metadata.Add("column_1", "boundary_condition_flow");
                result.Metadata.Add("column_2", "surface_runoff");
                result.Metadata.Add("column_3", "baseflow");
                result.Metadata.Add("column_4", "precip");
                result.Metadata.Add("column_5", "total_flow");
                result.Metadata.Add("column_6", "contaminant_loading");
                result.Metadata.Add("column_7", "aquatox_ammonia");
                result.Metadata.Add("column_8", "aquatox_nitrate");
            }
            // Run Aquatox model
            string aquaTaskID = this.taskID + "-" + catchment.COMID.ToString() + "-aquatox";
            aquatoxInput.SV[3].LoadsRec.Alt_Loadings[0].ITSI.InputTimeSeries.input.Data = JObject.FromObject(totalFlow.Data);
            string aquatoxInputJson = JSON.Serialize(aquatoxInput);
            string errorMsg = "";
            AQTNutrientsModel AQTM = new AQTNutrientsModel(ref aquatoxInputJson, out errorMsg, true);
            string aquatoxOutput = AQTM.outputString;
            this.SetAquatoxOutputToObject(catchment.COMID, aquatoxOutput);
            Utilities.Logger.WriteToFile(this.taskID, "Dumping aquatox data for catchment: " + catchment.COMID);
            Utilities.MongoDB.DumpData(aquaTaskID, aquatoxOutput);
            Utilities.Logger.WriteToFile(this.taskID, "Aquatox data dumped into mongodb with taskID: " + aquaTaskID);
            this.completedAquatox.Add(aquaTaskID);
            result = Utilities.Merger.MergeTimeSeries(new List<ITimeSeriesOutput>() { result, this.completedAmmonia[catchment.COMID], this.completedNitrate[catchment.COMID] });

            string taskID = this.taskID + "-" + catchment.COMID.ToString();
            //Dumb result in database
            Utilities.Logger.WriteToFile(this.taskID, "Dumping collected data for catchment: " + catchment.COMID);
            Utilities.MongoDB.DumpData(taskID, JSON.Serialize(result));
            Utilities.Logger.WriteToFile(this.taskID, "Catchment data dumped into mongodb with taskID: " + taskID);

            return true;
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
                    data.Add(runoffData);
                    Utilities.Logger.WriteToFile(this.taskID, "Successfully downloaded runoff data for COMID: " + catchment);

                    SubSurfaceFlow.SubSurfaceFlow subsurface = new SubSurfaceFlow.SubSurfaceFlow();
                    subsurface.Input = input;
                    subsurface.Input.BaseURL = new List<string>() { TimeSeriesInputFactory.GetBaseURL(source, "subsurfaceflow") };
                    subsurface.Input.InputTimeSeries = new Dictionary<string, TimeSeriesOutput>();
                    if (source.Equals("curvenumber") || source.Equals("ncei"))
                    {
                        input.Source = "curvenumber";
                        subsurface.Input.InputTimeSeries.Add("surfacerunoff", data.ElementAt(0) as TimeSeriesOutput);
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
                    data.Add(subsurfaceflow);
                    Utilities.Logger.WriteToFile(this.taskID, "Successfully downloaded baseflow data for COMID: " + catchment);

                    ITimeSeriesOutput mergedData = Utilities.Merger.MergeTimeSeries(data.ElementAt(0), data.ElementAt(1));
                    this.completedContributingCatchments.Add(catchment, mergedData);
                    catchmentData.Add(mergedData);
                }
            }
            );
            Utilities.Logger.WriteToFile(this.taskID, "Summing up contributing flow for: " + current);
            return Utilities.Merger.SumTimeSeriesByColumn(catchmentData);
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
                input.Geometry.StationID = "GHCND:US1NCCM0006";
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
            string filePath = "App_Data\\CapeFearDailyFlow.csv";
            if (!File.Exists(filePath))
            {
                filePath = @"App_Data/CapeFearDailyFlow.csv";
            }

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
                    string flow = values[2].ToString();
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
                    file.Write(JSON.Serialize(output));
                }
            }
            catch (System.IO.IOException ex) {
                Thread.Sleep(1000);
                this.TestModeFileWrite(dataset, output);
            }
        }

        private ITimeSeriesOutput TestModeFileRead(string dataset)
        {
            string filePath = "App_Data\\" + dataset + "_testdata.json";
            ITimeSeriesOutput output = JSON.Deserialize<TimeSeriesOutput>(File.ReadAllText(filePath));
            return output;
        }

        private dynamic SetAquatoxInput(NetworkCatchment catchment)
        {
            dynamic input = this.LoadAquatoxInputFile();
            input.Location.Locale.SiteName = catchment.COMID.ToString();
            input.Location.Locale.SiteLength = catchment.Length;
            input.PSetup.FirstDay = this.startDate.ToString("yyyy-MM-dd") + "T00:00:00";
            input.PSetup.LastDay = this.endDate.ToString("yyyy-MM-dd") + "T00:00:00";

            return input;
        }

        private void LoadAquatoxOutput()
        {
            string filePath = "App_Data\\aquatox_nutrient_model_output.txt";
            try
            {
                string textString = System.IO.File.ReadAllText(filePath);
                dynamic output = JSON.Deserialize<dynamic>(textString);
                //Import ammonia sample output as the input for the boundary condition
                DateTime iDate = new DateTime(this.startDate.Year, this.startDate.Month, this.startDate.Day);
                foreach (var values in output.SV[0].output.Data)
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
                foreach (var values in output.SV[1].output.Data)
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

        private dynamic LoadAquatoxInputFile()
        {
            string filePath = "App_Data\\aquatox_nutrient_model_input.txt";
            try
            {
                string fileData = System.IO.File.ReadAllText(filePath);
                return JSON.Deserialize<dynamic>(fileData);
            }
            catch (Exception ex)
            {
                return "Error loading input configuration for aqautox nutrient model";
            }
        }

        private void SetAquatoxOutputToObject(int comid, string outputString)
        {
            dynamic output = JSON.Deserialize<dynamic>(outputString);
            ITimeSeriesOutputFactory oFactory = new TimeSeriesOutputFactory();
            ITimeSeriesOutput ammonia = oFactory.Initialize();
            ITimeSeriesOutput nitrate = oFactory.Initialize();
            ammonia.Data = output.SV[0].output["Data"].ToObject<Dictionary<string, List<string>>>();
            this.completedAmmonia.Add(comid, ammonia);
            nitrate.Data = output.SV[1].output["Data"].ToObject<Dictionary<string, List<string>>>();
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
