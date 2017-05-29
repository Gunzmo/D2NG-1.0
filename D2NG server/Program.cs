using System.Linq;
using Alchemy.Classes;
using Alchemy;
using System.Net;
using Newtonsoft.Json;
using Packets;
using System.Collections.Generic;
using System;
using D2NG.Tools;
using System.Threading;

namespace D2NG_server
{
    public class Report
    {
        public int ID;
        public string Reporter;
        public string Reported;
        public ReportReason Reason;
        public Report(string Reporter, string Reported, ReportReason Reason)
        {
            ID = Tabels.Reports.Count + 1;
            this.Reported = Reported;
            this.Reporter = Reporter;
            this.Reason = Reason;
        }
        
    }
    class Program
    {
        static int CharacterID = 0;
        public static int CharacterIDCounter
        {
            get
            {
                CharacterID++;
                return CharacterID;
            }
        }
        static WebSocketServer server;
        static void Main(string[] args)
        {
            Tabels.Load();
            Console.Title = "Diablo II Next Generaton Server";
            server = new WebSocketServer(8081, IPAddress.Any)
            {
                OnReceive = OnReceive,
                OnConnected = OnConnect,
                OnDisconnect = OnDisconnect,
            };
            server.Start();
            Console.WriteLine("D2NG IS Running at: {0}", IPAddress.Any + ":8081");
            System.Diagnostics.Process.GetCurrentProcess().WaitForExit();
        }
        static bool ISIPBANNED(UserContext context) { return Tabels.IPBannes.Contains(context.ClientAddress.ToString().Split(':')[0]); }
        private static void OnReceive(UserContext context)
        {
            Cookies cookie;
            lock (Tabels.cookies)
                cookie = Tabels.cookies.FirstOrDefault(c => c.context.ClientAddress == context.ClientAddress);
            if (cookie == null) return;
            if (ISIPBANNED(cookie.context))
                return;
            if (cookie.AccountBinder != null)
            {
                if (cookie.AccountBinder.Banned == 1)
                    return;
                if (cookie.AccountBinder.Banned == 2)
                {
                    lock (Tabels.IPBannes)
                        Tabels.IPBannes.Add(cookie.context.ClientAddress.ToString().Split(':')[0]);
                    lock (Tabels.cookies)
                        Tabels.cookies.Remove(cookie);
                    return;

                }
            }
            try
            {
                var json = context.DataFrame.ToString();
                if (json == "7" || json == "\r\n/ HTTP/1") return;
                Packet obj = JsonConvert.DeserializeObject<Packet>(json);
                switch (obj.header)
                {
                    #region Ban
                    case Packet.Header.Ban:
                        if (cookie.Rank <= Rank.Member) return;
                        Account acc;
                        lock (Tabels.accounts)
                            acc = Tabels.accounts.FirstOrDefault(a => a.AccountName == (string)obj.Data[0]);
                        if (acc == null) return;
                        if (acc.Rank == Rank.Administrator || acc.Rank == Rank.Moderator && (bool)obj.Data[2]) return;
                        acc.Banned = ((bool)obj.Data[2] ? 2 : 1);
                        acc.BanTime = (DateTime)obj.Data[1];
                        if (acc.CookieLink != null)
                            acc.CookieLink.context.Send(new Packet(Packet.Header.Loggout, null).CreatePacket());
                        var report = Tabels.Reports.FirstOrDefault(r => r.Reported == (string)obj.Data[0]);
                        if (report == null) return;
                        cookie.context.Send(new Packet(Packet.Header.RemoveReport, report.ID).CreatePacket());
                        return;
                    #endregion
                    #region Premote
                    case Packet.Header.Premote:
                        if (cookie.Rank != Rank.Administrator) return;
                        Account ace;
                        lock (Tabels.accounts)
                            ace = Tabels.accounts.FirstOrDefault(ac => ac.AccountName == (string)obj.Data[0]);
                        if (ace.AccountName == "Gunzmo") return;
                        ace.Rank = (Rank)obj.Data[1];
                        return;
                    #endregion
                    #region Report
                    case Packet.Header.Report:
                        if (cookie.Rank < Rank.Member) return;
                        Account Acco;
                        lock (Tabels.accounts)
                            Acco = Tabels.accounts.FirstOrDefault(A => A.AccountName == obj.Data[0]);
                        if (Acco == null) return;
                        if (Acco.Rank == Rank.Administrator) return;
                        lock (Tabels.Reports)
                            Tabels.Reports.Add(new Report(cookie.AccountBinder.AccountName, Acco.AccountName, (ReportReason)obj.Data[1]));
                        return;
                    #endregion
                    #region Remove Report
                    case Packet.Header.RemoveReport:
                        if (cookie.Rank <= Rank.Member) return;
                        Report rep;
                        lock (Tabels.Reports)
                            rep = Tabels.Reports.FirstOrDefault(r => r.ID == (int)obj.Data);
                        if (rep == null) return;
                        lock (Tabels.Reports)
                            Tabels.Reports.Remove(rep);
                        cookie.context.Send(new Packet(Packet.Header.RemoveReport, rep.ID).CreatePacket());
                        return;
                    #endregion
                    #region Add Friend
                    case Packet.Header.AddFriend:
                        if (cookie.Rank < Rank.Member) return;
                        if (((string)obj.Data).ToLower() == cookie.AccountBinder.AccountName.ToLower()) return;
                        Account HeAccount;
                        lock (Tabels.accounts)
                            HeAccount = Tabels.accounts.FirstOrDefault(accu => accu.AccountName.ToLower() == ((string)obj.Data).ToLower());
                        if (HeAccount == null) return;
                        UserFriends HeFriends;
                        lock (HeAccount.Friends)
                            HeFriends = HeAccount.Friends.FirstOrDefault(f => f.userID == cookie.AccountID);
                        if (HeFriends == null)
                        {
                            lock (HeAccount.Friends)
                                HeAccount.Friends.Add(new UserFriends(cookie.AccountID, FriendType.Slave));
                            lock (cookie.AccountBinder.Friends)
                                cookie.AccountBinder.Friends.Add(new UserFriends(HeAccount.ID, FriendType.Owner));
                            if (HeAccount.CookieLink != null)
                                HeAccount.CookieLink.context.Send(new Packet(Packet.Header.AddFriend, new dynamic[] { cookie.AccountID, cookie.AccountBinder.AccountName, "", false, CharacterClass.Any, FriendType.Slave }).CreatePacket());
                            cookie.context.Send(new Packet(Packet.Header.AddFriend, new dynamic[] { HeAccount.ID, HeAccount.AccountName, "", false, CharacterClass.Any, FriendType.Owner }).CreatePacket());
                            var uid = new MySql.Data.MySqlClient.MySqlParameter("uid", MySql.Data.MySqlClient.MySqlDbType.Int32) { Value = cookie.AccountID };
                            var fid = new MySql.Data.MySqlClient.MySqlParameter("fid", MySql.Data.MySqlClient.MySqlDbType.Int32) { Value = HeAccount.ID };
                            var prams = new List<MySql.Data.MySqlClient.MySqlParameter>() { uid, fid };
                            Base.MYSQL.DoQuery("INSERT INTO friends (uid, fid, type) VALUE(?uid, ?fid, 1)", prams);
                        }
                        else
                        {
                            if (HeFriends.type == FriendType.Mutal) return;
                            UserFriends myFriendList;
                            lock(cookie.AccountBinder.Friends)
                                myFriendList = cookie.AccountBinder.Friends.FirstOrDefault(fu => fu.userID == HeAccount.ID);
                            if (myFriendList.type == FriendType.Mutal) return;

                            if (myFriendList.type == FriendType.Slave)
                            {
                                myFriendList.type = FriendType.Mutal;
                                HeFriends.type = FriendType.Mutal;

                                if (HeAccount.CookieLink != null)
                                {
                                    dynamic[] Data1 = new dynamic[6];
                                    Data1[0] = cookie.AccountID;
                                    Data1[1] = cookie.AccountBinder.AccountName;
                                    Data1[3] = (cookie.AccountBinder.CookieLink != null);
                                    if (cookie.Character != null)
                                    {
                                        Data1[2] = cookie.Character.Name;
                                        Data1[4] = cookie.Character.Class;
                                    }else
                                    {
                                        Data1[2] = "";
                                        Data1[4] = CharacterClass.Any;
                                    }
                                    Data1[5] = HeFriends.type;
                                    HeAccount.CookieLink.context.Send(new Packet(Packet.Header.AddFriend, Data1).CreatePacket());
                                }
                               
                                dynamic[] Data = new dynamic[6];
                                Data[0] = myFriendList.userID;
                                Data[1] = myFriendList.userLink.AccountName;
                                Data[3] = (myFriendList.userLink.CookieLink != null);
                                if (myFriendList.userLink.CookieLink != null)
                                {
                                    if (myFriendList.userLink.CookieLink.Character != null)
                                    {
                                        Data[2] = myFriendList.userLink.CookieLink.Character.Name;
                                        Data[4] = myFriendList.userLink.CookieLink.Character.Class;
                                    }
                                }
                                else
                                {
                                    Data[2] = "";
                                    Data[4] = CharacterClass.Any;
                                }
                                Data[5] = myFriendList.type;
                                context.Send(new Packet(Packet.Header.AddFriend, Data).CreatePacket());


                                var uid1 = new MySql.Data.MySqlClient.MySqlParameter("uid", MySql.Data.MySqlClient.MySqlDbType.Int32) { Value = cookie.AccountID };
                                var fid1 = new MySql.Data.MySqlClient.MySqlParameter("fid", MySql.Data.MySqlClient.MySqlDbType.Int32) { Value = myFriendList.userID };
                                var prams1 = new List<MySql.Data.MySqlClient.MySqlParameter>() { uid1, fid1 };
                                Base.MYSQL.DoQuery("UPDATE friends SET type = 0 WHERE (uid = ?uid AND fid = ?fid) OR (uid = ?fid AND fid = ?uid)", prams1);
                            }
                        }
                        return;
                    #endregion
                    #region Remove Friend
                    case Packet.Header.RemoveFriend:
                            if (cookie.Rank < Rank.Member) return;
                            var mefri = cookie.AccountBinder.Friends.FirstOrDefault(fa => fa.userID == (int)obj.Data);
                            var hefri = mefri.userLink.Friends.FirstOrDefault(fa => fa.userID == cookie.AccountID);
                            if (mefri == null || hefri == null) return;
                            lock (cookie.AccountBinder.Friends)
                                cookie.AccountBinder.Friends.Remove(mefri);
                            lock (hefri.userLink.Friends)
                                hefri.userLink.Friends.Remove(mefri);
                            var fid2 = new MySql.Data.MySqlClient.MySqlParameter("fid", MySql.Data.MySqlClient.MySqlDbType.Int32) { Value = mefri.userID };
                            var uid2 = new MySql.Data.MySqlClient.MySqlParameter("uid", MySql.Data.MySqlClient.MySqlDbType.Int32) { Value = hefri.userID };
                            Base.MYSQL.DoQuery("DELETE FROM friends where (fid = ?fid and uid = ?uid) OR (fid = ?uid and uid = ?fid)", new List<MySql.Data.MySqlClient.MySqlParameter>() { fid2, uid2 });
                            return;
                    #endregion
                    #region Send Message
                    case Packet.Header.ChatMsg:
                            var MSG = (string)obj.Data;
                            if (MSG.Substring(0, 2).ToLower() == "/c")
                            {
                                var ChannelSelect = MSG.Split(' ');
                                Channel channel;
                                lock (Tabels.channels)
                                    channel = Tabels.channels.FirstOrDefault(c => c.Name.ToLower() == ChannelSelect[1].ToLower());
                                if (channel == null)
                                {
                                    var newChannel = new Channel() { ID = Tabels.channels.Count + 1, Name = ChannelSelect[1], Password = (ChannelSelect.Length == 3 ? ChannelSelect[2] : "") };
                                    newChannel.Join(cookie, (ChannelSelect.Length == 3 ? ChannelSelect[2] : ""));
                                    lock (Tabels.channels)
                                        Tabels.channels.Add(newChannel);
                                return;
                                }
                                else  if (channel.Join(cookie, (ChannelSelect.Length == 3 ? ChannelSelect[2] : "")))
                                    cookie.context.Send(new Packet(Packet.Header.JoinChannel, channel.Name).CreatePacket());
                                else
                                    cookie.context.Send(new Packet(Packet.Header.ChatMsg, new dynamic[] { "Invalid Password!", 0 }).CreatePacket());
                                return;
                            }
                            if (MSG.Substring(0, 2).ToLower() == "/w")
                            {
                                var WisperMSG = MSG.Split(' ');
                                string WispMSG = string.Empty;
                                for (int i = 2; i < WisperMSG.Length; i++)
                                    WispMSG += WisperMSG[i] + " ";
                                Account acce;
                                lock (Tabels.accounts)
                                    acce = Tabels.accounts.FirstOrDefault(Accu => Accu.AccountName.ToLower() == WisperMSG[1].ToLower());
                                if (acce == null)
                                {
                                    cookie.context.Send(new Packet(Packet.Header.ChatMsg, new dynamic[] { "User Does Not Exist!", 0 }).CreatePacket());
                                    return;
                                }
                                else if (acce.CookieLink == null)
                                {
                                    cookie.context.Send(new Packet(Packet.Header.ChatMsg, new dynamic[] { acce.AccountName + " Is not Online!", 0 }).CreatePacket());
                                    return;
                                }
                                else
                                {
                                    WispMSG = "[" + cookie.AccountBinder.AccountName + "]: " + WispMSG;
                                    cookie.context.Send(new Packet(Packet.Header.ChatMsg, new dynamic[] { WispMSG, cookie.AccountID }).CreatePacket());
                                    acce.CookieLink.context.Send(new Packet(Packet.Header.ChatMsg, new dynamic[] {  WispMSG, cookie.AccountID }).CreatePacket());
                                }
                                return;
                            }
                            if(MSG.Substring(0, 2).ToLower() == "/l")
                            {
                                if (cookie.ChannelID == 0)
                            { 
                                cookie.context.Send(new Packet(Packet.Header.ChatMsg, new dynamic[] { "You have not Joined a Channel!", 0 }).CreatePacket());
                                return;
                            }
                            else
                                {
                                    var chunnle = Tabels.channels.FirstOrDefault(c => c.ID == cookie.ChannelID);
                                    if (chunnle != null)
                                    {
                                        cookie.context.Send(new Packet(Packet.Header.ChatMsg, new dynamic[] { "You have Left the Channel: " + chunnle.ID + "!", 0 }).CreatePacket());
                                        Cookies[] Kakor;
                                        lock (Tabels.cookies)
                                            Kakor = Tabels.cookies.Where(c => c.ChannelID == cookie.ChannelID).ToArray();
                                        cookie.ChannelID = 0;
                                        foreach (var Kakerino in Kakor)
                                            Kakerino.context.Send(new Packet(Packet.Header.ChatMsg, new dynamic[] { cookie.AccountBinder.AccountName, -1 }).CreatePacket());
                                        if (Kakor.Length == 0)
                                            lock (Tabels.channels)
                                                Tabels.channels.Remove(chunnle);
                                    }
                                }
                            }
                            Cookies[] cookies;
                            lock (Tabels.cookies)
                                cookies = Tabels.cookies.Where(c => c.ChannelID == cookie.ChannelID).ToArray();
                            foreach (var kaka in cookies)
                                kaka.context.Send(new Packet(Packet.Header.ChatMsg, new dynamic[] { "[" + cookie.AccountBinder.AccountName + "]: " + obj.Data, (kaka.ChannelID == 0 ? -1 : -2) }).CreatePacket());
                                
                        return;
                        #endregion
                    #region Create Game
                        case Packet.Header.AddGame:
                        if (cookie.Rank < Rank.Member) return;
                        Tabels.CreateGame((string)obj.Data[0], (string)obj.Data[1], (bool)obj.Data[2], (string)obj.Data[3], (D2NG.Tools.Difficulty)obj.Data[4], (int)obj.Data[5], cookie);
                        return;
                    #endregion
                    #region Login
                    case Packet.Header.Loggedin:
                        if(cookie.Login((string)obj.Data[0], (string)obj.Data[1]))
                        {
                            cookie.context.Send(new Packet(Packet.Header.Loggedin, true).CreatePacket());
                            cookie.Relamd = (D2NG.Tools.Region)obj.Data[2];
                            if (cookie.Rank >= Rank.Moderator)
                            {
                                cookie.context.Send(new Packet(Packet.Header.ISMOD, (cookie.AccountBinder.Rank == D2NG.Tools.Rank.Administrator)).CreatePacket());
                                if(cookie.Rank == Rank.Administrator)
                                {
                                    lock(Tabels.cookies)
                                    foreach (var Cookie in Tabels.cookies.Where(c => c.AccountID != 0))
                                        cookie.context.Send(new Packet(Packet.Header.OnlineUsers, Cookie.AccountBinder.AccountName).CreatePacket());
                                    lock(Tabels.accounts)
                                        foreach(var account in Tabels.accounts.Where(a => a.Rank >= Rank.Moderator))
                                            cookie.context.Send(new Packet(Packet.Header.AddMod, new dynamic[] { account.AccountName, (account.CookieLink != null) }).CreatePacket());
                                }
                            }
                            cookie.context.Send(new Packet(Packet.Header.ReqestCharacter, null).CreatePacket());
                            foreach (var cookiieess in Tabels.cookies.Where(c => c.ChannelID == cookie.ChannelID))
                            { 
                                if (cookiieess.AccountBinder == null) continue;
                                cookiieess.context.Send(new Packet(Packet.Header.RequestChannelUsers, cookie.AccountBinder.AccountName).CreatePacket());
                            }

                        }
                        return;
                    case Packet.Header.RequestFriends:
                        foreach (var frund in cookie.AccountBinder.Friends)
                        {
                            dynamic[] Data = new dynamic[6];
                            Data[0] = frund.userID;
                            Data[1] = frund.userLink.AccountName;
                            Data[3] = (frund.userLink.CookieLink != null);
                            if (frund.userLink.CookieLink != null)
                            {
                                if (frund.userLink.CookieLink.Character != null)
                                {
                                    Data[2] = frund.userLink.CookieLink.Character.Name;
                                    Data[4] = frund.userLink.CookieLink.Character.Class;
                                }
                            }
                            else
                            {
                                Data[2] = "";
                                Data[4] = CharacterClass.Any;
                            }
                            Data[5] = frund.type;
                            cookie.context.Send(new Packet(Packet.Header.AddFriend, Data).CreatePacket());
                            if (frund.userLink.CookieLink != null)
                                frund.userLink.CookieLink.context.Send(new Packet(Packet.Header.FriendStatus, new dynamic[] { cookie.AccountID, true }).CreatePacket());
                        }
                       
                        return;
                    #endregion

                    #region ChannelUsers
                    case Packet.Header.RequestChannelUsers:
                            foreach (var account in Tabels.accounts.Where(a => a.CookieLink != null && a.CookieLink.ChannelID == cookie.ChannelID))
                                cookie.context.Send(new Packet(Packet.Header.RequestChannelUsers, account.AccountName).CreatePacket());
                    break;
                    #endregion
                    #region Logout
                    case Packet.Header.Loggout:
                        if (cookie.Rank < Rank.Member) return;
                        cookie.Logout();
                        cookie.context.Send(new Packet(Packet.Header.Loggout, true).CreatePacket());
                        lock (Tabels.cookies)
                            foreach (var Admin in Tabels.cookies.Where(c => c.Rank == Rank.Administrator))
                                Admin.context.Send(new Packet(Packet.Header.RemoveOUser, cookie.AccountBinder.AccountName).CreatePacket());
                            foreach (var frund in cookie.AccountBinder.Friends.Where(f => f.type == D2NG.Tools.FriendType.Mutal && f.userLink != null))
                                frund.userLink.CookieLink.context.Send(new Packet(Packet.Header.FriendStatus, new dynamic[] { cookie.AccountID, false }).CreatePacket());
                    return;
                    #endregion
                    #region Register
                    case Packet.Header.Register:
                        bool Registerd = Tabels.CreateAccount((string)obj.Data[0], (string)obj.Data[1], (string)obj.Data[2]);
                        cookie.context.Send(new Packet(Packet.Header.Register, Registerd).CreatePacket());
                        return;
                    #endregion
                    #region Join
                    case Packet.Header.Join:
                        if (cookie.Rank < Rank.Member) return;
                        cookie.AccountBinder.Requests++;
                        Games gamee;
                        lock(Tabels.games)
                            gamee = Tabels.games.FirstOrDefault(g => g.ID == (int)obj.Data[0]);
                        cookie.JoinRequest = (int)obj.Data[0];
                        if (gamee == null) return;
                        cookie.context.Send(new Packet(Packet.Header.Join, new dynamic[] { gamee.GameName, gamee.Password }).CreatePacket());
                        break;
                    #endregion
                    #region Player Join
                    case Packet.Header.PlayerJoin:
                        if (cookie.Rank < Rank.Member) return;
                        Games game;
                        lock(Tabels.games)
                            game = Tabels.games.FirstOrDefault(g => g.ID == cookie.JoinRequest);
                        if (game == null) return;
                        game.Join(cookie);
                        foreach (var f in cookie.AccountBinder.Friends.Where(f => f.userLink.CookieLink != null && f.type == FriendType.Mutal))
                            f.userLink.CookieLink.context.Send(new Packet(Packet.Header.UpdateFriendGame, new dynamic[] { cookie.AccountID, game.ID, "In Game:" + game.GameName}).CreatePacket());
                    return;
                    #endregion
                    #region Player Leave
                    case Packet.Header.PlayerLeave:
                        if (cookie.Rank < Rank.Member) return;
                        Games games;
                        lock(Tabels.games)
                            games = Tabels.games.FirstOrDefault(g => g.ID == cookie.GameID);
                        if (games == null) return;
                        games.Leave(cookie);
                        if (games.characters.Count == 0)
                        {
                            lock(Tabels.cookies)
                                foreach (var c in Tabels.cookies.Where(c => c.Rank >= Rank.Member && c.context != null))
                                    c.context.Send(new Packet(Packet.Header.RemoveGame, games.ID).CreatePacket());
                            lock (Tabels.games)
                                Tabels.games.Remove(games);
                        }
                        foreach (var f in cookie.AccountBinder.Friends.Where(f => f.userLink.CookieLink != null && f.type == FriendType.Mutal))
                            f.userLink.CookieLink.context.Send(new Packet(Packet.Header.UpdateFriendGame, new dynamic[] { cookie.AccountID, 0, "In Lobby" }).CreatePacket());
                    return;
                    #endregion
                    #region Request Game List
                    case Packet.Header.RequestGameList:
                        if (cookie.Rank < Rank.Member) return;
                        RequestGameList(cookie, (Difficulty)obj.Data);
                        return;
                    #endregion
                    #region Request Character
                    case Packet.Header.ReqestCharacter:
                        if (cookie.Rank < Rank.Member) return;
                        cookie.SetCharacter(new D2NG.Tools.CharacterInfo(0, (string)obj.Data["Name"])
                        {
                            Class = (D2NG.Tools.CharacterClass)obj.Data["Class"],
                            Level = (int)obj.Data["Level"],
                            Flags = (D2NG.Tools.CharacterFlags)obj.Data["Flags"],
                            Title = (D2NG.Tools.CharacterTitle)obj.Data["Title"],
                            Act = (int)obj.Data["Act"],
                        });
                        return;
                    #endregion
                }
            }
            catch { context.Send("404"); }
            
        }
        public static void RequestGameList(Cookies Cookie, D2NG.Tools.Difficulty Difficulty)
        {

            List<D2NGGames> gameList = new List<D2NGGames>();
            lock(Tabels.games)
                foreach (var game in Tabels.games.Where(G =>
                     G.IsListed &&
                     G.characters.Count < G.MaxPlayers &&
                     (Difficulty == D2NG.Tools.Difficulty.All ? (G.difficulty & Difficulty) == Difficulty : G.difficulty == Difficulty) &&
                     G.Expantion == ((Cookie.Character.Flags & D2NG.Tools.CharacterFlags.Expansion) == D2NG.Tools.CharacterFlags.Expansion) &&
                     G.Hardcore == ((Cookie.Character.Flags & D2NG.Tools.CharacterFlags.Hardcore) == D2NG.Tools.CharacterFlags.Hardcore) &&
                     G.Ladder == ((Cookie.Character.Flags & D2NG.Tools.CharacterFlags.NoNLadder) != D2NG.Tools.CharacterFlags.NoNLadder)
                    )) gameList.Add(GenerateGame(game));

            Cookie.context.Send(new Packet(Packet.Header.AddGame, gameList).CreatePacket());
        }
        public static D2NGGames GenerateGame(Games game)
        {
            var GAME = new D2NG.Tools.D2NGGames()
            {
                Difficulty = game.difficulty,
                Gamename = game.GameName,
                Players = game.characters.Count
            };
            GAME.Descriptions.Add("Descriptions:" + game.Description);
            GAME.Descriptions.Add("Characters:");
            for (int i = 0; i < game.characters.Count; i++)
            {
                
                GAME.Descriptions.Add(
                    (game.characters[i].Title != CharacterTitle.Nooblar || game.characters[i].Title != CharacterTitle.Nooblette || game.characters[i].Title != CharacterTitle.CourageousNooblar ||
                    game.characters[i].Title != CharacterTitle.CourageousNooblette|| game.characters[i].Title != CharacterTitle.DoublePlusNooblar || game.characters[i].Title != CharacterTitle.DoublePlusNooblette 
                    || game.characters[i].Title != CharacterTitle.NooblarWhoLikesChicken || game.characters[i].Title != CharacterTitle.NoobletteWhoLikesChicken 
                    ? "" : game.characters[i].Title.ToString() + " ")
                     + game.characters[i].Class.ToString() + ": " + game.characters[i].Name + " [" + game.characters[i].Level + "]");
            }
            return GAME;
        }
        private static void OnConnect(UserContext context)
        {
            if (ISIPBANNED(context))
                return;
            lock (Tabels.cookies) Tabels.cookies.Add(new Cookies(Tabels.cookies.Count + 1, context));
        }
        private static void OnDisconnect(UserContext context)
        {
            lock (Tabels.cookies) Tabels.cookies.Remove(Tabels.cookies.FirstOrDefault(c => c.context.ClientAddress == context.ClientAddress));
        }
    }
}
