using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Data.SQLite;
using System.Data;
using System.Text.RegularExpressions;

namespace Data.Simulate
{
    /// <summary>
    /// Curve Number base class.
    /// Classified as simulation data, simulation takes place on a different server so only a standard data call is made.
    /// </summary>
    public class CurveNumber
    {

        /// <summary>
        /// Get simulated curve number data.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public string Simulate(out string errorMsg, ITimeSeriesInput input)
        {
            errorMsg = "";

            string comID = getComID(out errorMsg, input);
            if (errorMsg.Contains("ERROR")) { return null; }

            string query = ConstructQuery(out errorMsg, input, comID);
            if (errorMsg.Contains("ERROR")) { return null; }

            string data = DownloadData(out errorMsg, query);
            if (errorMsg.Contains("ERROR")) { return null; }

            return data;
        }

        /// <summary>
        /// Constructs the url to retrieve curvenumber data.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        private string ConstructQuery(out string errorMsg, ITimeSeriesInput input, string com)
        {
            errorMsg = "";
            string sql = "SELECT DISTINCT CURVENUM FROM HUC12_PU_COMIDs_CONUS WHERE COMID = '" + com + "';";
            return sql;
        }

        /// <summary>
        /// Constructs the body of the post for retrieving curvenumber data.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private string getComID(out string errorMsg, ITimeSeriesInput input)
        {
            errorMsg = "";
            string url = "https://ofmpub.epa.gov/waters10/SpatialAssignment.Service?pGeometry=POINT(" + input.Geometry.Point.Longitude + "+" + input.Geometry.Point.Latitude + ")&pLayer=NHDPLUS_CATCHMENT&pSpatialSnap=TRUE&pReturnGeometry=TRUE";
            byte[] bytes = null;
            WebClient client = new WebClient();
            client.Credentials = CredentialCache.DefaultNetworkCredentials;
            int retries = 5;                                        // Max number of request retries
            try
            {
                while (retries > 0 && bytes == null)
                {
                    bytes = client.DownloadData(url);
                    retries -= 1;
                }
            }
            catch (System.Net.WebException ex)
            {
                errorMsg = "Error attempting to collection data from external server.";
                return null;
            }
            string str = Encoding.UTF8.GetString(bytes);
            string pattern = @"[0-9]{2,}";
            Match result = Regex.Match(str, pattern);
            if (result.Success)
            {
                str = result.Value;
            }
            else
            {
                errorMsg = "ERROR: Could not find valid geometry.";
            }
            return str;
        }

        /// <summary>
        /// Sends request for curvenumber timeseries data.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="url"></param>
        /// <param name="postBody"></param>
        /// <returns></returns>
        private string DownloadData(out string errorMsg, string query)//, byte[] postBody)
        {
            //Curve number will be stored in sql database so replace this method with SELECT statement  
            errorMsg = "";
            string data = "";
            
            //Create SQLite connection
            SQLiteConnection sqlite = new SQLiteConnection("Data Source=M:\\StreamHydrologyFiles\\NHDPlusV2Data\\database.sqlite");
            SQLiteDataAdapter ad;
            DataTable dt = new DataTable();
            
            try
            {
                SQLiteCommand cmd;
                sqlite.Open();  //Initiate connection to the db
                cmd = sqlite.CreateCommand();
                cmd.CommandText = query;  //set the passed query
                ad = new SQLiteDataAdapter(cmd);
                ad.Fill(dt); //fill the datasource
            }
            catch (SQLiteException ex)
            {
                return null;
            }
            sqlite.Close();
            data = dt.Rows[0][0].ToString();
            return data;
        }

        /// <summary>
        /// Parse data string and set to output object.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="dataset"></param>
        /// <param name="data"></param>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public ITimeSeriesOutput SetDataToOutput(out string errorMsg, string dataset, string data, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";
            //TODO: Format data to output object.
            return null; 
        }
    }
}
