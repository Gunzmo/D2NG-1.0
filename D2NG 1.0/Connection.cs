using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alchemy;
using Alchemy.Classes;
using D2NG.Tools;
using Newtonsoft.Json;
using Packets;
using System.Threading;
using System.IO;

namespace D2NG
{
    public class Connection
    {
        WebSocketClient Client;


        #region Base
        public bool Connected { private set; get; }
        public bool Logged { private set; get; }
        public Connection()
        {
            Connected = false;
            Logged = false;
        }
        public void Connect()
        {
            var IP = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"/realmd.ip");
            Client = new WebSocketClient(IP) { OnReceive = ReceivePacket, OnConnected = OnConnected, OnDisconnect = Disconnected };
            Client.Connect();
        }
        private void Disconnected(UserContext context) {Connected = false; Logged = false; D2NGConnect(Connected); }
        private void OnConnected(UserContext context) {Connected = true; D2NGConnect(Connected); }
        private void ReceivePacket(UserContext context)
        {
            var json = context.DataFrame.ToString();
            if (json == "7" || json == "\r\n/ HTTP/1" || json == "404") return;
            Packet obj = JsonConvert.DeserializeObject<Packet>(json);

            switch (obj.header)
            {
                case Packet.Header.Loggedin:
                    LoginCallback((bool)obj.Data);
                    Client.Send(new Packet(Packet.Header.ReqestCharacter, UI.D2NGMainWindow.SelectedCharacter).CreatePacket());
                    Client.Send(new Packet(Packet.Header.RequestChannelUsers, null).CreatePacket());
                    Client.Send(new Packet(Packet.Header.RequestFriends, null).CreatePacket());
                    return;
                case Packet.Header.Loggout:
                    LogoutCallback();
                    return;
                case Packet.Header.AddGame:
                    GameListPacket GamePacket = JsonConvert.DeserializeObject<GameListPacket>(json);
                    AddGame(GamePacket.Data);
                    return;
                case Packet.Header.RemoveGame:
                    RemoveGame((int)obj.Data);
                    return;
                case Packet.Header.ChatMsg:
                    Message((string)obj.Data[0], (int)obj.Data[1]);
                    return;
                case Packet.Header.UpdateGame:
                    var desc = (Newtonsoft.Json.Linq.JArray)obj.Data[1];
                    UpdateGame((int)obj.Data[0], desc.ToObject<string[]>(), (int)obj.Data[2]);
                    return;
                case Packet.Header.ReqestCharacter:
                    Client.Send(new Packet(Packet.Header.ReqestCharacter, UI.D2NGMainWindow.SelectedCharacter).CreatePacket());
                    return;
                case Packet.Header.Join:
                    JoinGame((string)obj.Data[0], (string)obj.Data[1]);
                    UI.D2NGMainWindow.ISD2NG = true;
                    return;
                case Packet.Header.Register:
                    RegisterC((bool)obj.Data);
                    return;
                case Packet.Header.ISMOD:
                    ISMOD((bool)obj.Data);
                    return;
                case Packet.Header.OnlineUsers:
                    OnlineUser((string)obj.Data);
                    return;
                case Packet.Header.RemoveOUser:
                    RemoveOnlineUser((string)obj.Data);
                    return;
                case Packet.Header.AddMod:
                    AddMod((string)obj.Data[0], (bool)obj.Data[1]);
                    return;
                case Packet.Header.RemoveReport:
                    return;
                case Packet.Header.AddFriend:
                    AddFriendCall(new D2NG.UI.InGame.FriendItem((int)obj.Data[0], (string)obj.Data[1], (string)obj.Data[2], (bool)obj.Data[3], (CharacterClass)obj.Data[4], (FriendType)obj.Data[5]));
                    return;
                case Packet.Header.FriendStatus:
                    FriendOnlineStatus((int)obj.Data[0], (bool)obj.Data[1]);
                    return;
                case Packet.Header.UpdateFriendGame:
                    FriendGame((int)obj.Data[0], (int)obj.Data[1], (string)obj.Data[2]);
                    return;
                case Packet.Header.RequestChannelUsers:
                    RequestChannelUsers(obj.Data);
                    break;
                    }
        }
        #endregion

        #region Callbacks

        public delegate void _RequestChannelUsers(string AccountName);
        public event _RequestChannelUsers RequestChannelUsers;

        public delegate void _AddMod(string AccountName, bool Status);
        public event _AddMod AddMod;

        public delegate void _AddOnlineUsers(string AccountName);
        public event _AddOnlineUsers OnlineUser;

        public delegate void _RemoveOnlineUsers(string AccountName);
        public event _RemoveOnlineUsers RemoveOnlineUser;

        public delegate void _FriendOnlineStatus(int ID, bool Status);
        public event _FriendOnlineStatus FriendOnlineStatus;

        public delegate void _FriendGame(int ID, int GameID, string GameName);
        public event _FriendGame FriendGame;

        public delegate void _AddFriend(D2NG.UI.InGame.FriendItem Friend);
        public event _AddFriend AddFriendCall;

        public delegate void _Login(bool Status);
        public event _Login LoginCallback;

        public delegate void _Logout();
        public event _Logout LogoutCallback;

        public delegate void _Register(bool registerd);
        public event _Register RegisterC;

        public delegate void _AddGame(Tools.D2NGGames[] Data);
        public event _AddGame AddGame;

        public delegate void _RemoveGame(int ID);
        public event _RemoveGame RemoveGame;

        public delegate void _Connected(bool Status);
        public event _Connected D2NGConnect;

        public delegate void _Message(string Message, int ID);
        public event _Message Message;

        public delegate void _UpdateGame(int ID, string[] Descriptions, int Players);
        public event _UpdateGame UpdateGame;

        public delegate void _JoinGame(string GameName, string Password);
        public event _JoinGame JoinGame;

        public delegate void _ISMOD(bool Admin);
        public event _ISMOD ISMOD;

        #endregion

        #region Invokes
        internal void RemoveFriend(int ID)
            { if (Connected) Client.Send(new Packet(Packet.Header.RemoveFriend, ID).CreatePacket()); }
        internal void AddFriend(string FriendName)
            { if (Connected) Client.Send(new Packet(Packet.Header.AddFriend, FriendName).CreatePacket()); }
        internal void Report(string username, ReportReason reason)
            { if (Connected) Client.Send(new Packet(Packet.Header.Report, new dynamic[] { username, reason }).CreatePacket()); }
        internal void RemoveReport(int report)
            { if (Connected) Client.Send(new Packet(Packet.Header.RemoveReport, report).CreatePacket()); }
        internal void SendJoin(int ID)
            { if (Connected) Client.Send(new Packet(Packet.Header.Join, ID).CreatePacket()); }
        internal void SendJoin()
            { if (Connected) Client.Send(new Packet(Packet.Header.PlayerJoin, null).CreatePacket()); }
        internal void SendLeave()
            { if (Connected) Client.Send(new Packet(Packet.Header.PlayerLeave, null).CreatePacket());}
        internal void CreateGame(string GameName, string Password, bool IsListed, string Description, int MaxPlayers, D2NG.Tools.Difficulty Diff)
            { if(Connected) Client.Send(new Packet(Packet.Header.AddGame, new dynamic[] { GameName, Password, IsListed, Description, MaxPlayers, Diff}).CreatePacket()); }
        internal void RequestGames(Tools.Difficulty Diff)
            { if (Connected) Client.Send(new Packet(Packet.Header.RequestGameList, Diff).CreatePacket()); }
        internal void Login(string Account, string Password, Tools.Region Realm)
            { if (Connected) Client.Send(new Packet(Packet.Header.Loggedin, new dynamic[] {Account, Tools.Crypto.md5Hashing(Password), Realm}).CreatePacket()); }
        internal void Loggout()
            { if (Connected) Client.Send(new Packet(Packet.Header.Loggout, null).CreatePacket()); }
        internal void Register(string Account, string Password, string Email)
            { if (Connected) Client.Send(new Packet(Packet.Header.Register, new dynamic[] { Account, Tools.Crypto.md5Hashing(Password), Email }).CreatePacket()); }
        internal void UpdateCharacter()
            { if(Connected) Client.Send(new Packet(Packet.Header.ReqestCharacter, UI.D2NGMainWindow.SelectedCharacter).CreatePacket()); }
        internal void BanUser(string Account, DateTime BanTime, bool IP)
            { if (Connected) Client.Send(new Packet(Packet.Header.Ban, new dynamic[] { Account, BanTime, IP }).CreatePacket()); }
        internal void PremoteUser(string Account, int Status)
            { if (Connected) Client.Send(new Packet(Packet.Header.Ban, new dynamic[] { Account, Status }).CreatePacket()); }
        internal void SendMSG(string msg)
            { if (Connected) Client.Send(new Packet(Packet.Header.ChatMsg, msg).CreatePacket()); }
        #endregion

    }
}
