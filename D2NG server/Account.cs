using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2NG_server
{
    public class Account
    {
        public string Password { private set; get; }
        public string Email { private set; get; }
        public List<UserFriends> Friends = new List<UserFriends>();
        public Cookies CookieLink { get { return Tabels.cookies.FirstOrDefault(c => ID == c.AccountID); } }
        public DateTime BanTime;
        public int Banned = 0;
        public int Joins = 0;
        public int Requests = 0;
        public bool Flagged { get { return (Joins - Requests >= 10); } }
        public string AccountName { private set; get; }
        public int ID { private set; get; }
        public D2NG.Tools.Rank Rank;
        public Account(int ID, string AccountName, string Password, string Email, D2NG.Tools.Rank Rank = D2NG.Tools.Rank.Member, DateTime? BanTime = null, int? Banned = 0)
        {
            this.ID = ID;
            this.AccountName = AccountName;
            this.Password = Password;
            this.Email = Email;
            this.Rank = Rank;
            if(Banned.HasValue)
                this.Banned = Banned.Value;
            if(BanTime.HasValue)
                this.BanTime = BanTime.Value;
        }
    }

    public class UserFriends
    {
      
        public int userID;
        public D2NG.Tools.FriendType type;
        public Account userLink { get { return Tabels.accounts.FirstOrDefault(u => u.ID == userID); } }
        public UserFriends(int u, D2NG.Tools.FriendType t)
        {
            userID = u;
            type = t;
        }


    }

}
