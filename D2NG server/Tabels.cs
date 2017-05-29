using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace D2NG_server
{
    public static class Tabels
    {
        public static List<Account> accounts = new List<Account>();
        public static List<Cookies> cookies = new List<Cookies>();
        public static List<Games> games = new List<Games>();
        public static List<Channel> channels = new List<Channel>();
        public static List<string> IPBannes = new List<string>();
        public static List<Report> Reports = new List<Report>();
        public static bool CreateAccount(string Account, string Password, string Email)
        {
            
            Account account;
            if (Account.Contains("[") || Account.Contains("]") || Account.Contains(" ")) return false;
            lock (accounts)
                account = accounts.FirstOrDefault(a => a.AccountName == Account);
            if (account != null) return false;
            List<MySqlParameter> prams = new List<MySqlParameter>();
            var usern = new MySqlParameter("u", MySqlDbType.String); usern.Value = Account;
            var passwd = new MySqlParameter("p", MySqlDbType.String); passwd.Value = Password;
            var eml = new MySqlParameter("e", MySqlDbType.String); eml.Value = Email;
            prams.Add(usern); prams.Add(passwd); prams.Add(eml);
            var ID = Base.MYSQL.DoQuery("INSERT INTO accounts (username, password, email) VALUES(?u,?p,?e)", prams, 1);
            lock (accounts)
                accounts.Add(new Account((int)ID, Account, Password, Email));
            return true;
        }

        public static void CreateGame(string GameName, string Password, bool IsListed, string Descripton, D2NG.Tools.Difficulty diff, int maxPlayers, Cookies cookie)
        {
            lock (games)
                games.Add(new Games(GameName, Password, IsListed, Descripton, cookie.Relamd, diff, maxPlayers, cookie));
        }

        public static void Load()
        {
            GC.SuppressFinalize(accounts);
            GC.SuppressFinalize(cookies);
            GC.SuppressFinalize(games);
            GC.SuppressFinalize(channels);
            GC.SuppressFinalize(IPBannes);
            GC.SuppressFinalize(Reports);

            channels.Add(new Channel() { ID = 0, Name = "Global", Password = "" });
            Base.Config.INIT();
            List<MySqlParameter> prams = new List<MySqlParameter>();
            int Counter = 0;
            foreach(var account in Base.MYSQL.Select("SELECT * FROM accounts", prams, "accounts"))
            {
                prams.Clear();
                Counter++;
                var acc = new Account(account.GetInt("id"), account.GetString("username"), account.GetString("password"), account.GetString("email"), (D2NG.Tools.Rank)account.GetInt("rank"), UnixTimeStampToDateTime(account.GetInt("banned")), account.GetInt("bantype"));
                MySqlParameter UID = new MySqlParameter("t", MySqlDbType.Int32){ Value = acc.ID };
                prams.Add(UID);
                foreach (var friend in Base.MYSQL.Select("SELECT * FROM friends where (uid = ?t OR fid = ?t)", prams, "friends"))
                    acc.Friends.Add(new UserFriends(
                        (friend.GetInt("uid") == acc.ID ? friend.GetInt("fid") : friend.GetInt("uid")),
                        (friend.GetInt("type") == 0 || friend.GetInt("type") == 3 ? (D2NG.Tools.FriendType)friend.GetInt("type") :
                        (friend.GetInt("uid") == acc.ID ? D2NG.Tools.FriendType.Owner : D2NG.Tools.FriendType.Slave))));
                lock (accounts)
                    accounts.Add(acc);
            }
            Console.WriteLine("Total Accounts Loaded: {0}", Counter);
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
        public static double DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return (TimeZoneInfo.ConvertTimeToUtc(dateTime) -
                   new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds;
        }

    }
}
