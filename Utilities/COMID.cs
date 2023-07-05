using Data;
using System.Collections.Generic;
using System.IO;

namespace Utilities
{
    public class COMID
    {
        public static PointCoordinate GetCentroid(int comid, out string errorMsg)
        {
            errorMsg = "";
            string dbPath = Path.Combine(".", "App_Data", "catchments.sqlite");
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

        public static Dictionary<string, string> GetDbData(int comid, out string errorMsg)
        {
            errorMsg = "";
            string dbPath = Path.Combine(".", "App_Data", "catchments.sqlite");
            string query = "SELECT * FROM PlusFlow WHERE FROMCOMID = " + comid.ToString();
            Dictionary<string, string> dbData = Utilities.SQLite.GetData(dbPath, query);
            string query2 = "SELECT * FROM PlusFlowlineVAA WHERE ComID =" + comid.ToString();
            Dictionary<string, string> dbData2 = Utilities.SQLite.GetData(dbPath, query2);
            foreach (KeyValuePair<string, string> kv in dbData2)
            {
                if (!dbData.ContainsKey(kv.Key))
                {
                    dbData.Add(kv.Key, kv.Value);
                }
            }
            string query3 = "SELECT * FROM elevslope WHERE ComID =" + comid.ToString();
            Dictionary<string, string> dbData3 = Utilities.SQLite.GetData(dbPath, query3);
            foreach (KeyValuePair<string, string> kv in dbData3)
            {
                if (!dbData.ContainsKey(kv.Key))
                {
                    dbData.Add(kv.Key, kv.Value);
                }
            }
            string query4 = "SELECT * FROM HUC12_PU_COMIDs_CONUS WHERE COMID=" + comid.ToString();
            Dictionary<string, string> dbData4 = Utilities.SQLite.GetData(dbPath, query4);
            foreach (KeyValuePair<string, string> kv in dbData4)
            {
                if (!dbData.ContainsKey(kv.Key))
                {
                    dbData.Add(kv.Key, kv.Value);
                }
            }

            // Get and loop bankfull_conus
            string query5 = "SELECT * FROM bankfull_conus WHERE COMID=" + comid.ToString();
            Dictionary<string, string> dbData5 = Utilities.SQLite.GetData(dbPath, query5);
            foreach (KeyValuePair<string, string> kv in dbData5)
            {
                if (!dbData.ContainsKey(kv.Key))
                {
                    dbData.Add(kv.Key, kv.Value);
                }
            }

            return dbData;
        }

        public static Dictionary<string, object> GetGageID(int comid, out string errorMsg)
        {
            errorMsg = "";
            string dbPath = Path.Combine(".", "App_Data", "catchments.sqlite");
            string query = "SELECT * FROM GageInfo WHERE FLComID=" + comid.ToString();
            Dictionary<string, object> dbData = Utilities.SQLite.GetDataObject(dbPath, query);
            return dbData;
        }

        public static Dictionary<string, object> GetGageInfo(int gageID, out string errorMsg)
        {
            errorMsg = "";
            string dbPath = Path.Combine(".", "App_Data", "catchments.sqlite");
            string query = "SELECT * FROM GageInfo WHERE GAGEID=" + gageID.ToString();
            Dictionary<string, object> dbData = Utilities.SQLite.GetDataObject(dbPath, query);
            return dbData;
        }

    }
}
