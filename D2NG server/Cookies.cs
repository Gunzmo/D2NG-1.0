using Alchemy.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2NG_server
{
    public class Cookies
    {
        public string CookieStrID { private set; get; } 
        public int CookieID { private set; get; }
        public int AccountID { private set; get; }
        public int ChannelID = 0;
        public Character Character { get; private set; }
        public D2NG.Tools.Region Relamd = D2NG.Tools.Region.Europe;
        public UserContext context;
        public bool InGame { get { return (GameID > 0); } }
        public int GameID { private set; get; }
        public int JoinRequest { set; get; }
        public Account AccountBinder { get { return Tabels.accounts.FirstOrDefault(a => a.ID == AccountID); } }
        public D2NG.Tools.Rank Rank { get
            {
                if (AccountBinder == null)
                    return D2NG.Tools.Rank.Guest;
                else
                    return AccountBinder.Rank;
            }
        }
        public Cookies(int ID, UserContext context)
        {
            CookieID = ID;
            this.context = context;
            CookieStrID = D2NG.Tools.Crypto.GenKey;
            GameID = 0;
        }
        public void SetGameID(int ID) { GameID = ID; JoinRequest = 0; }
        public void SetCharacter(D2NG.Tools.CharacterInfo Char) {
            Character = new Character(Char, (Character == null ? Program.CharacterIDCounter : Character.ID));
        }
        public bool Login(string AccountName, string Password)
        {
            var account = Tabels.accounts.FirstOrDefault(a => a.AccountName.ToLower() == AccountName.ToLower() && a.Password == Password);
            if (account != null){
                    if (account.CookieLink != null){
                        account.CookieLink.Logout();
                        account.CookieLink.context.Send(new Packets.Packet(Packets.Packet.Header.Loggout, true).CreatePacket());
                    }
                    AccountID = account.ID;
                }
            return account != null;
        }
        public bool Logout()
        {
            AccountID = 0;
            return true;
        }

    }
}
