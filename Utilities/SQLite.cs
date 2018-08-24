using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;

namespace Utilities
{
    public class SQLite
    {

        /// <summary>
        /// Access sqlite database file, dbPath, and executes the query provided returning the query results
        /// </summary>
        /// <param name="dbPath">Path to the sqlite database</param>
        /// <param name="query">Query to be executed on sqlite database</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetData(string dbPath, string query)
        {
            if (query.Substring(0,6).ToUpper() != "SELECT")
            {
                return null;
            }
            // TODO: Dictionary object here is not sufficient for complete data retrieval from database.
            Dictionary<string, string> data = new Dictionary<string, string>();
            using (System.Data.SQLite.SQLiteConnection con = new System.Data.SQLite.SQLiteConnection(dbPath))
            {
                using (System.Data.SQLite.SQLiteCommand com = new System.Data.SQLite.SQLiteCommand(con))
                {
                    con.Open();                             
                    com.CommandText = query;                
                    com.ExecuteNonQuery();                
                    using (System.Data.SQLite.SQLiteDataReader reader = com.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            data.Add(reader["key"].ToString(), reader["value"].ToString());
                        }
                    }
                    con.Close();
                }
            }
            return data;
        }
        
    }
}
