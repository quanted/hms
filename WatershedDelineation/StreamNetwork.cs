using System;
using System.Data;
using System.Data.SQLite;
using System.Collections.Generic;
using System.Text;

namespace WatershedDelineation
{
    public class StreamNetwork
    {        
        public DataTable prepareStreamNetworkForHUC(string HUCNumber, out string errorMsg, out List<string> lst)
        {
            DataTable dt = new DataTable();
            Utilities.CatchmentAggregation agg = new Utilities.CatchmentAggregation();
            lst = agg.prepareCOMID(HUCNumber, out errorMsg);
            dt = prepareStreamNetwork(lst, out errorMsg);
            return dt;
        }

        private static DataTable prepareStreamNetwork(List<string> lst, out string errorMsg)
        {
            errorMsg = "";
            DataTable dtPlusFlow = new DataTable();
            if (lst.Count <= 0)
            {
                errorMsg = "The list must contain at least one COMID";
                return dtPlusFlow;
            }
            StringBuilder sb = new StringBuilder();
            foreach (string comid in lst)
            {
                sb.Append("'");
                sb.Append(comid);
                sb.Append("',");
            }
            string comIDs = sb.ToString();
            int index = comIDs.LastIndexOf(",");
            if (index > 0)
            {
                comIDs.Remove(index, 1);
            }
            comIDs = comIDs.Remove(index, 1);

            dtPlusFlow = executeQuery("Select D.COMID AS COMID, A.FROMCOMID, A.TOCOMID, A.FROMHYDSEQ, A.TOHYDSEQ, A.DIRECTION, D.ReachCode, " +
                "B.SLOPE, D.AreaSqKM, D.StreamLeve, D.StreamOrde, D.ReachCode, D.LengthKM, " +
                "E.Q0001E*0.028316847 AS MeanAnnFlowM3PS, E.V0001E*0.3048 AS MeanAnnVelMPS " +
                "From PlusFlow A, elevslope B, PlusFlowlineVAA D, EROM_MA0001 E " +
                "Where A.TOCOMID = D.COMID And D.COMID IN (" + comIDs + ") And " +
                "B.COMID = D.COMID And D.COMID = E.COMID ORDER BY A.TOHYDSEQ DESC");

            dtPlusFlow.Columns.Add("COMMENTS");
            dtPlusFlow.AcceptChanges();
            foreach (DataRow row in dtPlusFlow.Rows)
            {
                string strFromComID = row["FROMCOMID"].ToString();
                string strToComID = row["TOCOMID"].ToString();
                if ((lst.Contains(strFromComID)) || (lst.Contains(strToComID)))
                {
                    continue;
                }
                row.Delete();
            }
            dtPlusFlow.AcceptChanges();
            string strComID = "";
            foreach (DataRow dr in dtPlusFlow.Rows)
            {
                strComID = dr["FROMCOMID"].ToString();
                if ((lst.Contains(strComID)) || (strComID == "0"))
                {
                    if (dr["DIRECTION"].ToString() == "712")
                        dr["COMMENTS"] = "Headwater";
                    else if (dr["DIRECTION"].ToString() == "714")
                        dr["COMMENTS"] = "Coastline";
                    else if (dr["DIRECTION"].ToString() == "713")
                        dr["COMMENTS"] = "Network end";
                    //else
                    {
                        DataRow[] drs = dtPlusFlow.Select("FROMCOMID='" + dr["TOCOMID"].ToString() + "'");
                        if ((drs == null) || (drs.Length == 0))
                        {
                            dr["COMMENTS"] = dr["COMMENTS"] + " " + "Pour point";
                        }
                    }
                    continue;
                }
                dr["COMMENTS"] = dr["COMMENTS"].ToString() + " Boundary Condition";
                DataRow[] drs1 = dtPlusFlow.Select("FROMCOMID='" + dr["TOCOMID"].ToString() + "'");
                if ((drs1 == null) || (drs1.Length == 0))
                {
                    dr["COMMENTS"] = dr["COMMENTS"] + " " + "Pour point";
                }
            }
            dtPlusFlow.Columns.Add("FROMCOMIDS");
            dtPlusFlow.Columns.Add("TOCOMIDS");
            string strTO = "";
            string strFROM = "";
            foreach (DataRow dr in dtPlusFlow.Rows)
            {
                strTO = "";
                strFROM = "";
                if (dr["FROMCOMID"].ToString() == "0")
                {
                    strTO = dr["TOCOMID"].ToString();
                }
                else
                {
                    DataRow[] drsTO = dtPlusFlow.Select("FROMCOMID='" + dr["FROMCOMID"].ToString() + "'");
                    foreach (DataRow dr1 in drsTO)
                    {
                        strTO += dr1["TOCOMID"].ToString() + ",";
                    }
                    if (strTO.Length > 0)
                        strTO = strTO.Remove(strTO.LastIndexOf(","));
                }
                dr["TOCOMIDS"] = strTO;

                DataRow[] drsFROM = dtPlusFlow.Select("TOCOMID='" + dr["TOCOMID"].ToString() + "'");
                foreach (DataRow dr1 in drsFROM)
                {
                    strFROM += dr1["FROMCOMID"].ToString() + ",";
                }
                if (strFROM.Length > 0)
                    strFROM = strFROM.Remove(strFROM.LastIndexOf(","));
                dr["FROMCOMIDS"] = strFROM;
            }

            DataView dv = dtPlusFlow.DefaultView;
            dv.Sort = "TOHYDSEQ DESC";
            dtPlusFlow = dv.ToTable();
            dtPlusFlow.Columns.Add("COMID_SEQ");
            if (dtPlusFlow.Rows.Count == 0)
            {
                //dataGridView1.DataSource = dtPlusFlow.DefaultView;
                return dtPlusFlow;
            }
            int seq = 1;
            dtPlusFlow.Rows[0]["COMID_SEQ"] = seq;
            if (dtPlusFlow.Rows.Count == 1)
            {
                //dataGridView1.DataSource = dtPlusFlow.DefaultView;
                return dtPlusFlow;
            }
            string prevToHydSeq = dtPlusFlow.Rows[0]["TOHYDSEQ"].ToString();
            for (int i = 1; i < dtPlusFlow.Rows.Count; i++)
            {
                if (dtPlusFlow.Rows[i]["TOHYDSEQ"].ToString() == prevToHydSeq)
                    dtPlusFlow.Rows[i]["COMID_SEQ"] = seq;
                else
                {
                    seq = seq + 1;
                    dtPlusFlow.Rows[i]["COMID_SEQ"] = seq;
                }
                prevToHydSeq = dtPlusFlow.Rows[i]["TOHYDSEQ"].ToString();
            }
            return dtPlusFlow;
        }
        private static DataTable executeQuery(string cmdText)
        {
            // create a new database connection:
            SQLiteConnection sqlite_conn = new SQLiteConnection("Data Source=database.sqlite;Version=3;");

            // open the connection:
            sqlite_conn.Open();
            SQLiteCommand sqlite_cmd = sqlite_conn.CreateCommand();

            // Let the SQLiteCommand object know our SQL-Query:
            sqlite_cmd.CommandText = cmdText;

            SQLiteDataAdapter da = new SQLiteDataAdapter(sqlite_cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            // Now lets execute the SQL ;-)
            //sqlite_cmd.ExecuteNonQuery();
            sqlite_conn.Close();
            return dt;
        }
    }
}
