using System;
using System.IO;
using System.Configuration;
namespace Base
{
    public static class Config
    {
        public static string MYSqlIP { get; private set; }
        public static string MYSqlDB { get; private set; }
        public static string MYSqlUser { get; private set; }
        public static string MYSqlPass { get; private set; }
        public static string IP { get; private set; }
        public static int PORT { get; private set; }
        public static void INIT()
        {
            PORT = Convert.ToInt32(ConfigurationManager.AppSettings["PORT"]);
            IP = ConfigurationManager.AppSettings["IP"];
            MYSqlIP = ConfigurationManager.AppSettings["MYSQL_IP"];
            MYSqlDB = ConfigurationManager.AppSettings["MYSQL_DB"];
            MYSqlUser = ConfigurationManager.AppSettings["MYSQL_USER"];
            MYSqlPass = ConfigurationManager.AppSettings["MYSQL_PASSWORD"];
        }
    }
}
