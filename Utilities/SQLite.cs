using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.Sqlite;

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
            SqliteConnectionStringBuilder connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.DataSource = dbPath;

            using (SqliteConnection con = new SqliteConnection(connectionStringBuilder.ConnectionString))
            {
                con.Open();
                SqliteCommand com = con.CreateCommand();
                com.CommandText = query;
                using (SqliteDataReader reader = com.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            var k = reader.GetName(i);
                            var v = reader.GetString(i);

                            data.Add(k, v);
                        }
                    }
                }
                con.Close();
            }
            return data;
        }
        
    }
}
