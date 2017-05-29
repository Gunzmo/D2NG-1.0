using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Threading;

namespace Base
{
    public static class MYSQL
    {
        #region Mysql Connection Structure
        private static string connectionString;
        static MYSQL()
        {
            Initialize();
        }

        private static void Initialize()
        {
            connectionString = "SERVER=" + Config.MYSqlIP + ";" + "DATABASE=" +
            Config.MYSqlDB + ";" + "UID=" + Config.MYSqlUser + ";" + "PASSWORD=" + Config.MYSqlPass + ";SslMode=Preferred;ConnectionLifeTime=300;";
        }
        #endregion 

        #region DoQuerys
        public static Int64 DoQuery(string query, List<MySqlParameter> prams, int Callback)
        {
            long ID = 0;

            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();
                using (MySqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = query;
                    foreach (MySqlParameter pram in prams)
                        cmd.Parameters.Add(pram);
                    try
                    {
                        cmd.ExecuteNonQuery();
                        ID = cmd.LastInsertedId;
                    }
                    catch { ID = -1; }
                }
                con.Close();
                return Convert.ToInt64(ID);
            }
        }
        public static bool DoQuery(string query, List<MySqlParameter> prams)
        {
            bool _check = false;
            using (MySqlConnection con = new MySqlConnection(connectionString))
            {
                con.Open();
                using (MySqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = query;
                    foreach (MySqlParameter pram in prams)
                        cmd.Parameters.Add(pram);
                    try
                    {
                        cmd.ExecuteNonQuery();
                        _check = true;
                    }
                    catch { _check = false; }
                }
                con.Close();
                return _check;
            }
        }
        #endregion

        #region MySql Selection/DoQuery Ulimited Colums
        /// <summary>
        /// Skapa dina egna efter project
        /// </summary>
        public class MYSQLDATALIST
        {
            List<mysqlDataItem> list = new List<mysqlDataItem>();
            private class mysqlDataItem
            {
                public string Tag;
                public object Value;

                public mysqlDataItem(object Value, string Tag)
                {
                    this.Tag = Tag;
                    this.Value = Value;
                }
            }

            public int TimeStamp { private set; get; }
            public string TableName { private set; get; }
            public int GetInt(string Name) { TimeStamp = DateTime.Now.Minute; try {
                    return Convert.ToInt32(list.FirstOrDefault(l => l.Tag == Name).Value); } catch { return 0; }
            }
            public double GetDouble(string Name) { TimeStamp = DateTime.Now.Minute; try { return (double)list.FirstOrDefault(l => l.Tag == Name).Value; } catch { return 0; } }
            public float GetFloat(string Name) { TimeStamp = DateTime.Now.Minute; try { return (float)list.FirstOrDefault(l => l.Tag == Name).Value; } catch { return 0f; } }
            public bool GetBool(string Name) { TimeStamp = DateTime.Now.Minute; try { return Convert.ToBoolean(list.FirstOrDefault(l => l.Tag == Name).Value);} catch { return false; } }
            public string GetString(string Name) { try{ var val = list.FirstOrDefault(l => l.Tag == Name); return (val.Value == DBNull.Value ? "" : (string)val.Value); } catch { return string.Empty; } }
            public T getObject<T>(string Name) { return (T)list.FirstOrDefault(l => l.Tag == Name).Value; }
            public T getJSONObject<T>(string Name) { return JsonConvert.DeserializeObject<T>((string)list.FirstOrDefault(l => l.Tag == Name).Value); }

            public void Add(object data, string ID) { list.Add(new mysqlDataItem(data, ID)); }
            public MYSQLDATALIST(string Tablename)
            {
                TableName = Tablename;
                TimeStamp = DateTime.Now.Minute;
            }
            public MYSQLDATALIST(){ TimeStamp = DateTime.Now.Minute; }
        }
        /// <summary>
        /// TableName only used if costum Rehash implanted
        /// </summary>
        /// <param name="query">Query string</param>
        /// <param name="prams">MySql Parameters in list</param>
        /// <param name="TableName">Table Name to Grabfrom</param>
        /// <returns>MYSQLDATALIST Filled With MySql Data</returns>
        public static List<MYSQLDATALIST> Select(string query, List<MySqlParameter> prams, string TableName = "")
        {
            var buffer = new List<MYSQLDATALIST>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (MySqlCommand cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = query;

                        foreach (MySqlParameter pram in prams)
                        {
                            cmd.Parameters.Add(pram);
                        }
                        using (MySqlDataReader dataReader = cmd.ExecuteReader())
                        {

                            while (dataReader.Read())
                            {
                                var data = new MYSQLDATALIST();
                                if (TableName != string.Empty) data = new MYSQLDATALIST(TableName);
                                for (int col = 0; col < dataReader.FieldCount; col++)
                                    data.Add(dataReader[col], dataReader.GetName(col).ToString());
                                buffer.Add(data);
                            }
                            dataReader.Close();
                        }
                    }

                    return buffer;
                }
                catch { connection.Close(); return buffer; }
            }

        }
        #endregion
    }
}
