﻿using Data;
using System.Collections.Generic;
using System.IO;

namespace Utilities
{
    public class COMID
    {
        public static PointCoordinate GetCentroid(int comid, out string errorMsg)
        {
            errorMsg = "";
            //string dbPath = "./App_Data/catchments.sqlite";
            string dbPath = Path.Combine(".", "App_Data", "catchments.sqlite");
            string query = "SELECT CentroidLatitude, CentroidLongitude FROM PlusFlowlineVAA WHERE ComID = " + comid.ToString();
            Dictionary<string, string> centroidDict = Utilities.SQLite.GetData(dbPath, query);
            if (centroidDict.Count == 0)
            {
                errorMsg = "ERROR: Unable to find catchment in database. ComID: " + comid.ToString();
                return null;
                //TESTING
                //return new PointCoordinate()
                //{
                //    Latitude = 33.9264,
                //    Longitude = -83.356
                //};
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
