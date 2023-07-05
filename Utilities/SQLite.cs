using Microsoft.Data.Sqlite;
using Serilog;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;

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
        public static SqliteConnection GetConnection(string dbPath)
        {
            // TODO: Dictionary object here is not sufficient for complete data retrieval from database.
            string cwd = Directory.GetCurrentDirectory();
            string absPath = "";
            if (!File.Exists(dbPath))
            {
                foreach (string p in cwd.Split(Path.DirectorySeparatorChar))
                {
                    absPath = Path.Combine(absPath, p);
                    if (p.Equals("hms"))
                    {
                        break;
                    }
                }

                absPath = Path.Combine(absPath, "Web.Services", dbPath);
            }
            else
            {
                absPath = dbPath;
            }

            SqliteConnectionStringBuilder connectionStringBuilder = new SqliteConnectionStringBuilder()
            {
                Mode = SqliteOpenMode.ReadOnly
            };
            connectionStringBuilder.DataSource = absPath;
            SqliteConnection con = new SqliteConnection(connectionStringBuilder.ConnectionString);
            return con;
        }

        /// <summary>
        /// Access sqlite database file, dbPath, and executes the query provided returning the query results
        /// </summary>
        /// <param name="dbPath">Path to the sqlite database</param>
        /// <param name="query">Query to be executed on sqlite database</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetData(string dbPath, string query)
        {
            if (query.Substring(0, 6).ToUpper() != "SELECT")
            {
                return new Dictionary<string, string>();
            }
            // TODO: Dictionary object here is not sufficient for complete data retrieval from database.
            string cwd = Directory.GetCurrentDirectory();
            string absPath = "";
            if (!File.Exists(dbPath))
            {
                foreach (string p in cwd.Split(Path.DirectorySeparatorChar))
                {
                    absPath = Path.Combine(absPath, p);
                    if (p.Equals("hms"))
                    {
                        break;
                    }
                }

                absPath = Path.Combine(absPath, "Web.Services", dbPath);
            }
            else
            {
                absPath = dbPath;
            }

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Dictionary<string, string> data = new Dictionary<string, string>();
            SqliteConnectionStringBuilder connectionStringBuilder = new SqliteConnectionStringBuilder()
            {
                Mode = SqliteOpenMode.ReadOnly
            };
            connectionStringBuilder.DataSource = absPath;
            try
            {
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
                                string v = null;
                                if (reader.IsDBNull(i))
                                {
                                    v = "NA";
                                }
                                else
                                {
                                    v = reader.GetValue(i).ToString();
                                }
                                if (!data.ContainsKey(k))
                                {
                                    data.Add(k, v);
                                }
                                else
                                {
                                    data[k] = data[k] + "," + v;
                                }
                                
                            }
                        }
                    }
                    con.Close();
                }
                stopwatch.Stop();
                Log.Information("SQLITE QUERY - Query Runtime: " + stopwatch.Elapsed.TotalSeconds.ToString() + " sec");
                return data;
            }
            catch (SqliteException ex)
            {
                Log.Warning(ex, "Error querying sqlite database.");
                return data;
            }
        }

        /// <summary>
        /// Access sqlite database file, dbPath, and executes the query provided returning the query results
        /// </summary>
        /// <param name="dbPath">Path to the sqlite database</param>
        /// <param name="query">Query to be executed on sqlite database</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetData(SqliteConnection dbConn, string query)
        {
            if (query.Substring(0, 6).ToUpper() != "SELECT")
            {
                return new Dictionary<string, string>();
            }

            Dictionary<string, string> data = new Dictionary<string, string>();
            try
            {
                if (dbConn.State == System.Data.ConnectionState.Closed)
                {
                    dbConn.Open();
                }
                SqliteCommand com = dbConn.CreateCommand();
                com.CommandText = query;
                using (SqliteDataReader reader = com.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            var k = reader.GetName(i);
                            string v = null;
                            if (reader.IsDBNull(i))
                            {
                                v = "NA";
                            }
                            else
                            {
                                v = reader.GetValue(i).ToString();
                            }
                            if (!data.ContainsKey(k))
                            {
                                data.Add(k, v);
                            }
                            else
                            {
                                data[k] = data[k] + "," + v;
                            }

                        }
                    }
                }
                return data;
            }
            catch (SqliteException ex)
            {
                Log.Warning(ex, "Error querying sqlite database.");
                return data;
            }
        }

        /// <summary>
        /// Access sqlite database file, dbPath, and executes the query provided returning the query results
        /// </summary>
        /// <param name="dbPath">Path to the sqlite database</param>
        /// <param name="query">Query to be executed on sqlite database</param>
        /// <returns></returns>
        public static Dictionary<string, object> GetDataObject(string dbPath, string query)
        {
            if (query.Substring(0, 6).ToUpper() != "SELECT")
            {
                return null;
            }
            string cwd = Directory.GetCurrentDirectory();
            string absPath = "";
            if (!File.Exists(dbPath))
            {
                foreach (string p in cwd.Split(Path.DirectorySeparatorChar))
                {
                    absPath = Path.Combine(absPath, p);
                    if (p.Equals("hms"))
                    {
                        break;
                    }
                }

                absPath = Path.Combine(absPath, "Web.Services", dbPath);
            }
            else
            {
                absPath = dbPath;
            }

            Dictionary<string, object> data = new Dictionary<string, object>();
            SQLiteConnectionStringBuilder connectionStringBuilder = new SQLiteConnectionStringBuilder();
            connectionStringBuilder.DataSource = absPath;
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(connectionStringBuilder.ConnectionString))
                {
                    con.Open();
                    SQLiteCommand com = con.CreateCommand();
                    com.CommandText = query;
                    using (SQLiteDataReader reader = com.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                var k = reader.GetName(i);
                                object v = "";

                                if (reader.IsDBNull(i))
                                {
                                    v = "NA";
                                }
                                else
                                {
                                    v = reader.GetValue(i);
                                }
                                if (!data.ContainsKey(k))
                                {
                                    data.Add(k, v);
                                }
                                else
                                {
                                    data[k] = data[k] + "," + v;
                                }
                            }
                        }
                    }
                    con.Close();
                }
                return data;
            }
            catch (SQLiteException ex)
            {
                Log.Warning(ex, "Error querying sqlite database.");
                return data;
            }
        }
    }
}

