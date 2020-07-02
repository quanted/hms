using Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Utilities;

namespace WatershedDelineation
{

    public class DataDict
    {
        public Dictionary<string, string> data { get; set; }
        public Dictionary<string, string> metadata { get; set; }
    }
    /// <summary>
    /// Result structure of the json string retrieved from nwm.
    /// </summary>
    public class Result
    {
        public string id { get; set; }
        public string status { get; set; }
        public DataDict data; 
    }

    public class FlowRouting
    {
        public static DataSet calculateStreamFlows(string startDate, string endDate, DataTable dtStreamNetwork, List<string> lst, out List<string> validList, ITimeSeriesInput input, string streamAlgorithm, out string errorMsg)
        {
            //This function returns a dataset containing three tables
            errorMsg = "";
            Utilities.ErrorOutput err = new Utilities.ErrorOutput();
            DateTime startDateTime = Convert.ToDateTime(startDate);
            DateTime endDateTime = Convert.ToDateTime(endDate);

            //Make sure that dtStreamNetwork is sorted by COMID-SEQ column
            dtStreamNetwork.DefaultView.Sort = "[COMID_SEQ] ASC";

            //Create a Dataset with three tables: Surface Runoff, Sub-surfaceRunoff, StreamFlow.
            //Each table has the following columns: DateTime, one column for each COMID with COMID as the columnName
            DataSet ds = new DataSet();
            DataTable dtPrecip = new DataTable();
            DataTable dtSurfaceRunoff = new DataTable();
            DataTable dtSubSurfaceRunoff = new DataTable();
            DataTable dtStreamFlow = new DataTable();

            dtPrecip.Columns.Add("DateTime");
            dtSurfaceRunoff.Columns.Add("DateTime");
            dtSubSurfaceRunoff.Columns.Add("DateTime");
            dtStreamFlow.Columns.Add("DateTime");
            foreach (DataRow dr in dtStreamNetwork.Rows)
            {
                string tocom = dr["TOCOMID"].ToString();
                if (!dtSurfaceRunoff.Columns.Contains(tocom))
                {
                    dtPrecip.Columns.Add(tocom);
                    dtSurfaceRunoff.Columns.Add(tocom);
                    dtSubSurfaceRunoff.Columns.Add(tocom);
                    dtStreamFlow.Columns.Add(tocom);
                }
                else
                {
                    string test = tocom;
                    // Excluding COMID
                }
            }

            //Initialize these tables with 0s
            DataRow drPrecip = null;
            DataRow drSurfaceRunoff = null;
            DataRow drSubSurfaceRunoff = null;
            DataRow drStreamFlow = null;
            //int indx = 0;
            for (DateTime date = startDateTime; date <= endDateTime; date = date.AddDays(1))
            {
                drPrecip = dtPrecip.NewRow();
                drSurfaceRunoff = dtSurfaceRunoff.NewRow();
                drSubSurfaceRunoff = dtSubSurfaceRunoff.NewRow();
                drStreamFlow = dtStreamFlow.NewRow();

                foreach (DataColumn dc in dtStreamFlow.Columns)
                {
                    drPrecip[dc.ColumnName] = 0;
                    drSurfaceRunoff[dc.ColumnName] = 0;
                    drSubSurfaceRunoff[dc.ColumnName] = 0;
                    drStreamFlow[dc.ColumnName] = 0;
                }

                drPrecip["DateTime"] = date.ToShortDateString();
                drSurfaceRunoff["DateTime"] = date.ToShortDateString();
                drSubSurfaceRunoff["DateTime"] = date.ToShortDateString();
                drStreamFlow["DateTime"] = date.ToShortDateString();

                dtPrecip.Rows.Add(drPrecip);
                dtSurfaceRunoff.Rows.Add(drSurfaceRunoff);
                dtSubSurfaceRunoff.Rows.Add(drSubSurfaceRunoff);
                dtStreamFlow.Rows.Add(drStreamFlow);
                //indx++;
            }
            //Now add the tables to DataSet
            ds.Tables.Add(dtSurfaceRunoff);
            ds.Tables.Add(dtSubSurfaceRunoff);
            ds.Tables.Add(dtStreamFlow);
            ds.Tables.Add(dtPrecip);

            //Iterate through all streams and calculate flows
            string COMID = "";
            string fromCOMID = "";

            Dictionary<string, ITimeSeriesOutput> comSubResults = new Dictionary<string, ITimeSeriesOutput>();
            Dictionary<string, ITimeSeriesOutput> comSurfResults = new Dictionary<string, ITimeSeriesOutput>();
            Dictionary<string, ITimeSeriesOutput> comPrecipResults = new Dictionary<string, ITimeSeriesOutput>();

            Dictionary<string, SurfaceRunoff.SurfaceRunoff> surfaceFlow = new Dictionary<string, SurfaceRunoff.SurfaceRunoff>();
            Dictionary<string, SubSurfaceFlow.SubSurfaceFlow> subsurfaceFlow = new Dictionary<string, SubSurfaceFlow.SubSurfaceFlow>();
            Dictionary<string, Precipitation.Precipitation> precipitation = new Dictionary<string, Precipitation.Precipitation>();

            //Building list of valid centroids as many are null and will cause map errors if set to (0,0) or (null,null)
            validList = new List<string>();
            validList = lst;

            ITimeSeriesInputFactory inputFactory = new TimeSeriesInputFactory();
            input.Geometry.GeometryMetadata.Add("StreamFlowEndDate", input.DateTimeSpan.EndDate.ToString("MM/dd/yyyyHH:mm"));
            input.Geometry.GeometryMetadata.Add("StreamFlowStartDate", input.DateTimeSpan.StartDate.ToString("MM/dd/yyyyHH:mm"));

            foreach (string com in validList)
            {
                ITimeSeriesInput tsi = new TimeSeriesInput();
                tsi.Geometry = input.Geometry;
                tsi.DateTimeSpan = input.DateTimeSpan;
                tsi.Source = input.Source;
                tsi.TemporalResolution = "daily";
                TimeSeriesGeometry tsGeometry = new TimeSeriesGeometry();
                tsGeometry.Point = Utilities.COMID.GetCentroid(Convert.ToInt32(com), out errorMsg);
                tsGeometry.ComID = Convert.ToInt32(com);
                tsGeometry.GeometryMetadata = input.Geometry.GeometryMetadata;

                ITimeSeriesInput subIn = new TimeSeriesInput();
                subIn = tsi;
                subIn.Geometry = tsGeometry;
                subIn.Geometry.Point = Utilities.COMID.GetCentroid(Convert.ToInt32(com), out errorMsg);
                subIn.Geometry.ComID = Convert.ToInt32(com);
                SubSurfaceFlow.SubSurfaceFlow sub = new SubSurfaceFlow.SubSurfaceFlow();
                sub.Input = inputFactory.SetTimeSeriesInput(subIn, new List<string>() { "subsurfaceflow" }, out errorMsg);
                subsurfaceFlow.Add(com, sub);


                ITimeSeriesInput surfIn = new TimeSeriesInput();
                surfIn = tsi;
                surfIn.Geometry = tsGeometry;
                surfIn.Geometry.Point = Utilities.COMID.GetCentroid(Convert.ToInt32(com), out errorMsg);
                surfIn.Geometry.ComID = Convert.ToInt32(com);
                SurfaceRunoff.SurfaceRunoff runoff = new SurfaceRunoff.SurfaceRunoff();
                runoff.Input = inputFactory.SetTimeSeriesInput(surfIn, new List<string>() { "surfacerunoff" }, out errorMsg);
                surfaceFlow.Add(com, runoff);

                ITimeSeriesInput preIn = new TimeSeriesInput();
                preIn = tsi;
                preIn.Geometry = tsGeometry;
                preIn.Geometry.Point = Utilities.COMID.GetCentroid(Convert.ToInt32(com), out errorMsg);
                preIn.Geometry.ComID = Convert.ToInt32(com);
                preIn.Source = preIn.Geometry.GeometryMetadata["precipSource"];
                Precipitation.Precipitation precip = new Precipitation.Precipitation();
                precip.Input = inputFactory.SetTimeSeriesInput(preIn, new List<string>() { "precipitation" }, out errorMsg);
                precipitation.Add(com, precip);
            }

            object outputListLock = new object();
            var options = new ParallelOptions { MaxDegreeOfParallelism = -1 };

            List<string> precipError = new List<string>();
            Parallel.ForEach(precipitation, options, (KeyValuePair<string, Precipitation.Precipitation> preF) =>
            {
                string errorM = "";
                int retries = 4;
                while (retries > 0 && preF.Value.Output == null)
                {
                    preF.Value.GetData(out errorM);
                    Interlocked.Decrement(ref retries);//retries -= 1;
                }
                lock (outputListLock)
                {
                    precipError.Add(errorM);
                }
            });

            string missingComs = "";
            foreach (string com in validList)
            {
                if (precipitation[com].Output.GetType().Equals(typeof(TimeSeriesOutput)) || precipitation[com].Output.GetType().Equals(typeof(ITimeSeriesOutput)))
                {
                    surfaceFlow[com].Input.InputTimeSeries = new Dictionary<string, TimeSeriesOutput>();
                    surfaceFlow[com].Input.InputTimeSeries["precipitation"] = (TimeSeriesOutput)precipitation[com].Output;
                    subsurfaceFlow[com].Input.InputTimeSeries = new Dictionary<string, TimeSeriesOutput>();
                    subsurfaceFlow[com].Input.InputTimeSeries["precipitation"] = (TimeSeriesOutput)precipitation[com].Output;
                }
                else
                {
                    missingComs += com + ", ";
                }
            }


            List<string> surfaceError = new List<string>();
            Parallel.ForEach(surfaceFlow, options, (KeyValuePair<string, SurfaceRunoff.SurfaceRunoff> surF) =>
            {
                string errorM = "";
                //surF.Value.GetData(out errorM);
                int retries = 4;
                while (retries > 0 && surF.Value.Output == null)
                {
                    surF.Value.GetData(out errorM);
                    Interlocked.Decrement(ref retries); //retries -= 1;
                }
                lock (outputListLock)
                {
                    surfaceError.Add(errorM);
                }
            });

            foreach (string com in validList)
            {
                if (surfaceFlow[com].Output != null && (surfaceFlow[com].Output.GetType().Equals(typeof(TimeSeriesOutput)) || surfaceFlow[com].Output.GetType().Equals(typeof(ITimeSeriesOutput))))
                {
                    if (subsurfaceFlow[com].Input.InputTimeSeries == null)
                    {
                        subsurfaceFlow[com].Input.InputTimeSeries = new Dictionary<string, TimeSeriesOutput>();
                    }
                    subsurfaceFlow[com].Input.InputTimeSeries["surfacerunoff"] = (TimeSeriesOutput)surfaceFlow[com].Output;
                }
                
            }

            List<string> subsurfaceError = new List<string>();
            Parallel.ForEach(subsurfaceFlow, options, (KeyValuePair<string, SubSurfaceFlow.SubSurfaceFlow> subF) =>
            {
                string errorM = "";
                //subF.Value.GetData(out errorM);
                int retries = 4;
                while (retries > 0 && subF.Value.Output == null)
                {                    
                    subF.Value.GetData(out errorM);
                    Interlocked.Decrement(ref retries);//retries -= 1;
                }
                lock (outputListLock)
                {
                    subsurfaceError.Add(errorM);
                }
            });

            string flaskURL = Environment.GetEnvironmentVariable("FLASK_SERVER");
            string baseURL = "";
            if (flaskURL == null)
            {
                flaskURL = "http://localhost:7777";
            }

            if (!missingComs.Equals(""))
            {
                errorMsg += "Could not complete data requests for Com IDs: " + missingComs + "Their data values have been marked as invalid (-9999). ";
            }

            switch (streamAlgorithm)
            {
                case "changingvolume":
                    Debug.WriteLine("Flask Server URL: " + flaskURL);
                    baseURL = flaskURL + "/hms/hydrodynamic/constant_volume/?submodel=constant_volume&startDate=2019-05-01&endDate=2019-05-08&timestep=1&segments=1&boundary_flow=6";
                    break;
                case "kinematicwave":
                    errorMsg = "ERROR: Algorithm is not currently supported.";
                    break;
                case "nwm":
                    errorMsg = "ERROR: NWM stream flow data is currently disabled.";
                    break;
                    //for (int x = 0; x < dtStreamNetwork.Rows.Count; x++)
                    //{
                    //    COMID = dtStreamNetwork.Rows[x]["TOCOMID"].ToString();
                    //    fromCOMID = dtStreamNetwork.Rows[x]["FROMCOMID"].ToString();
                    //    DataRow[] drsFromCOMIDs = dtStreamNetwork.Select("TOCOMID = " + COMID);

                    //    List<string> fromCOMIDS = new List<string>();
                    //    foreach (DataRow dr2 in drsFromCOMIDs)
                    //    {
                    //        fromCOMIDS.Add(dr2["FROMCOMID"].ToString());
                    //    }
                                                
                    //    Debug.WriteLine("Flask Server URL: " + flaskURL);
                    //    baseURL = flaskURL + "/hms/nwm/data/?dataset=streamflow&comid=" + COMID + "&startDate=" + input.DateTimeSpan.StartDate.ToString("yyyy-MM-dd") + "&endDate=" + input.DateTimeSpan.EndDate.ToString("yyyy-MM-dd");
                    //    WatershedDelineation.NWM watermod = new WatershedDelineation.NWM();
                    //    string data = watermod.GetData(out errorMsg, baseURL);
                    //    //if (errorMsg.Contains("ERROR")) { break; }
                    //    Result result = JSON.Deserialize<Result>(data);

                    //    Dictionary<string, string> dailySF = new Dictionary<string, string>();
                    //    double sum = 0;
                    //    int ct = 0;
                    //    foreach (KeyValuePair<string, string> kvp in result.data.data)
                    //    {
                    //        sum += Convert.ToDouble(kvp.Value);
                    //        ct += 1;
                    //        if(ct == 24)
                    //        {
                    //            sum = sum / 24;
                    //            dailySF.Add(kvp.Key, sum.ToString());
                    //            ct = 0;
                    //            sum = 0;
                    //        }
                    //    }

                    //    for (int i = 0; i < dtStreamFlow.Rows.Count; i++)
                    //    {
                    //        DateTime datekey = Convert.ToDateTime(dtSubSurfaceRunoff.Rows[i]["DateTime"].ToString());
                    //        string date = datekey.ToString("yyyy-MM-dd") + " 00";
                    //        if (!validList.Contains(COMID))
                    //        {
                    //            continue;
                    //        }

                    //        if (subsurfaceFlow[COMID].Output == null || subsurfaceFlow[COMID].Output.Data.Count == 0)
                    //        {
                    //            dtSubSurfaceRunoff.Rows[i][COMID] = -9999;
                    //        }
                    //        else
                    //        {
                    //            //DateTime datekey = Convert.ToDateTime(dtSubSurfaceRunoff.Rows[i]["DateTime"].ToString());
                    //            //string date = datekey.ToString("yyyy-MM-dd") + " 00";
                    //            dtSubSurfaceRunoff.Rows[i][COMID] = subsurfaceFlow[COMID].Output.Data[date][0];
                    //        }

                    //        if (surfaceFlow[COMID].Output == null || surfaceFlow[COMID].Output.Data.Count == 0)
                    //        {
                    //            dtSurfaceRunoff.Rows[i][COMID] = -9999;
                    //        }
                    //        else
                    //        {
                    //            //DateTime datekey = Convert.ToDateTime(dtSurfaceRunoff.Rows[i]["DateTime"].ToString());
                    //            //string date = datekey.ToString("yyyy-MM-dd") + " 00";
                    //            dtSurfaceRunoff.Rows[i][COMID] = surfaceFlow[COMID].Output.Data[date][0];
                    //        }

                    //        if (precipitation[COMID].Output == null || precipitation[COMID].Output.Data.Count == 0)
                    //        {
                    //            dtPrecip.Rows[i][COMID] = -9999;
                    //        }
                    //        else
                    //        {
                    //            //DateTime datekey = Convert.ToDateTime(dtPrecip.Rows[i]["DateTime"].ToString());
                    //            //string date = datekey.ToString("yyyy-MM-dd") + " 00";
                    //            dtPrecip.Rows[i][COMID] = precipitation[COMID].Output.Data[date][0];
                    //        }

                            
                    //        //date = datekey.ToString("yyyy-MM-dd") + "T00:00:00";
                    //        date = datekey.ToString("yyyy-MM-dd") + "T23:00:00";

                    //        //Fill dtStreamFlow table by adding Surface and SubSurface flow from dtSurfaceRunoff and dtSubSurfaceRunoff tables.  We still need to add boundary condition flows
                    //        double dsur = Convert.ToDouble(dtSurfaceRunoff.Rows[i][COMID].ToString());
                    //        double dsub = Convert.ToDouble(dtSubSurfaceRunoff.Rows[i][COMID].ToString());
                    //        double area = Convert.ToDouble(dtStreamNetwork.Rows[x]["AreaSqKM"]);

                    //        if (result != null && dailySF.ContainsKey(date)) //if (result != null && result.data.data.ContainsKey(date))
                    //        {
                    //            dtStreamFlow.Rows[i][COMID] = Convert.ToDouble(dailySF[date]) / 35.314667;//Convert cubic feet / sec to meters / sec
                    //        }
                    //        else
                    //        {
                    //            dtStreamFlow.Rows[i][COMID] = (dsub * area / 1000) + (dsur * area / 1000);//dsub + dsur;
                    //        }                            
                            
                    //        /*Get stream flow time series for streams flowing into this COMID i.e. bondary condition.  Skip this step in the following three cases:
                    //        //  1. dr["FROMCOMID"].ToString()="0" in dtStreanNetwork table for this dr["TOCOMID"].ToString() i.e. it is head water stream
                    //        //  2. dr["FROMCOMID].ToString() = dr["TOCOMID"].ToString().  This can happen at the pour points
                    //        //  3. dr["FROMCOMID"].ToString() does not appear in the TOCOMID column of dtStreamNetwork table i.e. FROMCOMID is outside the network
                    //        //If multiple streams flow into this stream then add up stream flow time series of the inflow streams.
                    //        //There could be multiple bondary condition flows if multiple upstream streams flow into this COMID.            
                    //        foreach (string fromCom in fromCOMIDS)
                    //        {
                    //            if (fromCom == "0")//No boundary condition flows if the stream is a headwater (fromCOMID=0)
                    //            {
                    //                continue;
                    //            }
                    //            if (fromCom == COMID)//No boundary condition if fromCOMID=TOCOMID
                    //            {
                    //                continue;
                    //            }
                    //            DataRow[] drs = dtStreamNetwork.Select("TOCOMID = " + fromCom);
                    //            //No boundary condition if fromCOMID is not present in the streamNetwork table under TOCOMID column.  THis means that fromCOMID is outside our network.
                    //            if (drs == null || drs.Length == 0)
                    //            {
                    //                continue;
                    //            }
                    //            //Now add up all three time series: streams flow of streams inflowing into this stream, surface runoff, and sub-surface runoff
                    //            dtStreamFlow.Rows[i][COMID] = (Convert.ToDouble(dtStreamFlow.Rows[i][fromCom].ToString()) * area / 1000) + (Convert.ToDouble(dtStreamFlow.Rows[i][COMID].ToString()));// * area / 1000);
                    //        }*/
                    //    }
                    //}
                    //return ds;
                case "constantvolume":
                default:
                    for (int x = 0; x < dtStreamNetwork.Rows.Count; x++)
                    {
                        COMID = dtStreamNetwork.Rows[x]["TOCOMID"].ToString();
                        fromCOMID = dtStreamNetwork.Rows[x]["FROMCOMID"].ToString();
                        DataRow[] drsFromCOMIDs = dtStreamNetwork.Select("TOCOMID = " + COMID);

                        List<string> fromCOMIDS = new List<string>();
                        foreach (DataRow dr2 in drsFromCOMIDs)
                        {
                            fromCOMIDS.Add(dr2["FROMCOMID"].ToString());
                        }

                        for (int i = 0; i < dtStreamFlow.Rows.Count; i++)
                        {
                            if (!validList.Contains(COMID))
                            {
                                continue;
                            }

                            if (subsurfaceFlow[COMID].Output == null || subsurfaceFlow[COMID].Output.Data.Count == 0)
                            {
                                dtSubSurfaceRunoff.Rows[i][COMID] = -9999;
                            }
                            else
                            {
                                DateTime datekey = Convert.ToDateTime(dtSubSurfaceRunoff.Rows[i]["DateTime"].ToString());
                                string date = datekey.ToString("yyyy-MM-dd") + " 00";
                                dtSubSurfaceRunoff.Rows[i][COMID] = subsurfaceFlow[COMID].Output.Data[date][0];
                            }

                            if (surfaceFlow[COMID].Output == null || surfaceFlow[COMID].Output.Data.Count == 0)
                            {
                                dtSurfaceRunoff.Rows[i][COMID] = -9999;
                            }
                            else
                            {
                                DateTime datekey = Convert.ToDateTime(dtSurfaceRunoff.Rows[i]["DateTime"].ToString());
                                string date = datekey.ToString("yyyy-MM-dd") + " 00";
                                dtSurfaceRunoff.Rows[i][COMID] = surfaceFlow[COMID].Output.Data[date][0];
                            }

                            if (precipitation[COMID].Output == null || precipitation[COMID].Output.Data.Count == 0)
                            {
                                dtPrecip.Rows[i][COMID] = -9999;
                            }
                            else
                            {
                                DateTime datekey = Convert.ToDateTime(dtPrecip.Rows[i]["DateTime"].ToString());
                                string date = datekey.ToString("yyyy-MM-dd") + " 00";
                                dtPrecip.Rows[i][COMID] = precipitation[COMID].Output.Data[date][0];
                            }

                            //Fill dtStreamFlow table by adding Surface and SubSurface flow from dtSurfaceRunoff and dtSubSurfaceRunoff tables.  We still need to add boundary condition flows
                            double dsur = Convert.ToDouble(dtSurfaceRunoff.Rows[i][COMID].ToString());
                            double dsub = Convert.ToDouble(dtSubSurfaceRunoff.Rows[i][COMID].ToString());
                            double area = Convert.ToDouble(dtStreamNetwork.Rows[x]["AreaSqKM"]);
                            dtStreamFlow.Rows[i][COMID] = ((dsub * area * 1000) + (dsur * area * 1000)) / 86400;//dsub + dsur;            m^3/day (1day/86400sec) => m^3/s


                            //Get stream flow time series for streams flowing into this COMID i.e. bondary condition.  Skip this step in the following three cases:
                            //  1. dr["FROMCOMID"].ToString()="0" in dtStreanNetwork table for this dr["TOCOMID"].ToString() i.e. it is head water stream
                            //  2. dr["FROMCOMID].ToString() = dr["TOCOMID"].ToString().  This can happen at the pour points
                            //  3. dr["FROMCOMID"].ToString() does not appear in the TOCOMID column of dtStreamNetwork table i.e. FROMCOMID is outside the network
                            //If multiple streams flow into this stream then add up stream flow time series of the inflow streams.
                            //There could be multiple bondary condition flows if multiple upstream streams flow into this COMID.            
                            foreach (string fromCom in fromCOMIDS)
                            {
                                if (fromCom == "0")//No boundary condition flows if the stream is a headwater (fromCOMID=0)
                                {
                                    continue;
                                }
                                if (fromCom == COMID)//No boundary condition if fromCOMID=TOCOMID
                                {
                                    continue;
                                }
                                DataRow[] drs = dtStreamNetwork.Select("TOCOMID = " + fromCom);
                                //No boundary condition if fromCOMID is not present in the streamNetwork table under TOCOMID column.  THis means that fromCOMID is outside our network.
                                if (drs == null || drs.Length == 0)
                                {
                                    continue;
                                }
                                //Now add up all three time series: streams flow of streams inflowing into this stream, surface runoff, and sub-surface runoff
                                //dtStreamFlow.Rows[i][COMID] = (Convert.ToDouble(dtStreamFlow.Rows[i][fromCom].ToString()) * area / 1000) + (Convert.ToDouble(dtStreamFlow.Rows[i][COMID].ToString()));// * area / 1000);
                                dtStreamFlow.Rows[i][COMID] = Convert.ToDouble(dtStreamFlow.Rows[i][fromCom]) + Convert.ToDouble(dtStreamFlow.Rows[i][COMID]);
                            }
                        }
                    }
                    return ds;
            }
            
            return ds;
        }
        
        private static PointCoordinate GetCatchmentCentroid(out string errorMsg, int comid)
        {
            errorMsg = "";
            string dbPath = "./App_Data/catchments.sqlite";
            string query = "SELECT CentroidLatitude, CentroidLongitude FROM PlusFlowlineVAA WHERE ComID = " + comid.ToString();
            Dictionary<string, string> centroidDict = Utilities.SQLite.GetData(dbPath, query);
            if (centroidDict.Count == 0)
            {
                errorMsg = "ERROR: Unable to find catchment in database. ComID: " + comid.ToString();
                return null;
            }

            IPointCoordinate centroid = new PointCoordinate()
            {
                Latitude = double.Parse(centroidDict["CentroidLatitude"]),
                Longitude = double.Parse(centroidDict["CentroidLongitude"])
            };


            return centroid as PointCoordinate;
        }
    }
}