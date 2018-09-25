using Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace WatershedDelineation
{
    public class FlowRouting
    {
        public static DataSet calculateStreamFlows(string startDate, string endDate, DataTable dtStreamNetwork, ITimeSeriesOutput surface, ITimeSeriesOutput subsurface, out string errorMsg)
        {
            //This function returns a dataset containing three tables
            errorMsg = "";
            DateTime startDateTime = Convert.ToDateTime(startDate);
            DateTime endDateTime = Convert.ToDateTime(endDate);

            //Make sure that dtStreamNetwork is sorted by COMID-SEQ column
            dtStreamNetwork.DefaultView.Sort = "[COMID_SEQ] ASC";

            //Create a Dataset with three tables: Surface Runoff, Sub-surfaceRunoff, StreamFlow.
            //Each table has the following columns: DateTime, one column for each COMID with COMID as the columnName
            DataSet ds = new DataSet();
            DataTable dtSurfaceRunoff = new DataTable();
            DataTable dtSubSurfaceRunoff = new DataTable();
            DataTable dtStreamFlow = new DataTable();

            dtSurfaceRunoff.Columns.Add("DateTime");
            dtSubSurfaceRunoff.Columns.Add("DateTime");
            dtStreamFlow.Columns.Add("DateTime");
            foreach (DataRow dr in dtStreamNetwork.Rows)
            {
                string tocom = dr["TOCOMID"].ToString();
                if (!dtSurfaceRunoff.Columns.Contains(tocom))
                {
                    dtSurfaceRunoff.Columns.Add(tocom);
                    dtSubSurfaceRunoff.Columns.Add(tocom);
                    dtStreamFlow.Columns.Add(tocom);
                }
            }

            //Initialize these tables with 0s
            DataRow drSurfaceRunoff = null;
            DataRow drSubSurfaceRunoff = null;
            DataRow drStreamFlow = null;
            int indx = 0;
            for (DateTime date = startDateTime; date <= endDateTime; date = date.AddDays(1))
            {
                drSurfaceRunoff = dtSurfaceRunoff.NewRow();
                drSubSurfaceRunoff = dtSubSurfaceRunoff.NewRow();
                drStreamFlow = dtStreamFlow.NewRow();

                foreach (DataColumn dc in dtStreamFlow.Columns)
                {
                    drSurfaceRunoff[dc.ColumnName] = 0;
                    drSubSurfaceRunoff[dc.ColumnName] = 0;
                    drStreamFlow[dc.ColumnName] = 0;
                }

                drSurfaceRunoff["DateTime"] = date.ToShortDateString();
                drSubSurfaceRunoff["DateTime"] = date.ToShortDateString();
                drStreamFlow["DateTime"] = date.ToShortDateString();

                dtSurfaceRunoff.Rows.Add(drSurfaceRunoff);
                dtSubSurfaceRunoff.Rows.Add(drSubSurfaceRunoff);
                dtStreamFlow.Rows.Add(drStreamFlow);
                indx++;
            }
            //Now add the tables to DataSet
            ds.Tables.Add(dtSurfaceRunoff);
            ds.Tables.Add(dtSubSurfaceRunoff);
            ds.Tables.Add(dtStreamFlow);

            //Iterate through all streams and calculate flows
            string COMID = "";
            string fromCOMID = "";
            for (int i = 0; i < indx - 1; i++) //for (int i = 0; i < dtStreamNetwork.Rows.Count; i++)
            {
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

                    int j = 0;
                    foreach (var pair in surface.Data)
                    {
                        if (j >= dtSurfaceRunoff.Rows.Count)
                        {
                            break;
                        }
                        DataRow dr = dtSurfaceRunoff.Rows[j++];
                        DateTime datekey = DateTime.ParseExact(pair.Key.ToString(), "yyyy-MM-dd HH", null);
                        string st = dr["DateTime"].ToString();// + " 00";
                        if (datekey.ToShortDateString() == st)
                        {
                            dr[COMID] = pair.Value[0];
                        }
                    }
                    j = 0;
                    foreach (var pair in subsurface.Data)
                    {
                        if (j >= dtSubSurfaceRunoff.Rows.Count)
                        {
                            break;
                        }
                        DataRow dr = dtSubSurfaceRunoff.Rows[j++];
                        DateTime datekey = DateTime.ParseExact(pair.Key.ToString(), "yyyy-MM-dd HH", null);
                        string st = dr["DateTime"].ToString();// + " 00";
                        if (datekey.ToShortDateString() == st)
                        {
                            dr[COMID] = pair.Value[0];
                        }
                    }

                    /*for(int j = 0; j < dtSurfaceRunoff.Rows.Count; j++)
                    {
                        DataRow surfdr = dtSurfaceRunoff.Rows[j];
                        DataRow subdr = dtSubSurfaceRunoff.Rows[j];
                        DateTime datekey = DateTime.ParseExact(surface.Data.ElementAt(j).Key.ToString(), "yyyy-MM-dd HH", null);
                        string st = surfdr["DateTime"].ToString();// + " 00";
                        if (datekey.ToShortDateString() == st)
                        {
                            surfdr[COMID] = surface.Data.ElementAt(j).Value[0];
                            subdr[COMID] = subsurface.Data.ElementAt(j).Value[0];
                        }
                    }*/

                    //Fill dtStreamFlow table by adding Surface and SubSurface flow from dtSurfaceRunoff and dtSubSurfaceRunoff tables.  We still need to add boundary condition flows
                    double dsur = Convert.ToDouble(dtSurfaceRunoff.Rows[i][COMID].ToString());
                    double dsub = Convert.ToDouble(dtSubSurfaceRunoff.Rows[i][COMID].ToString());
                    dtStreamFlow.Rows[i][COMID] = dsub + dsur;

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
                        dtStreamFlow.Rows[i][COMID] = Convert.ToDouble(dtStreamFlow.Rows[i][fromCom].ToString()) + Convert.ToDouble(dtStreamFlow.Rows[i][COMID].ToString());
                    }
                }
            }

            return ds;
        }
    }
}