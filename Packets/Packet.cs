using Newtonsoft.Json;
using System;
namespace Packets
{
    public class Packet
    {
        public enum Header : uint
        {
            Loggedin,
            Register,
            AddGame,
            RemoveGame,
            UpdateGame,
            ChatMsg,
            PlayerJoin,
            PlayerLeave,
            Loggout,
            CreateGame,
            RequestGameList,
            ReqestCharacter,
            Join,
            ISMOD,
            Ban,
            Premote,
            Report,
            RemoveReport,
            OnlineUsers,
            RemoveOUser,
            AddMod,
            KeepAlive,
            RemoveFriend,
            AddFriend,
            FriendStatus,
            UpdateFriendGame,
            JoinChannel,
            RequestChannelUsers,
            RequestFriends
        }
        public Header header;
        public dynamic Data;
        public Packet(Header header, dynamic data)
        {
            this.header = header;
            Data = data;
        }
        public string CreatePacket() { return JsonConvert.SerializeObject(this); }
    }

    public class GameListPacket
    {
        public enum Header : uint
        {
            Loggedin,
            Register,
            AddGame,
            RemoveGame,
            ChatMsg,
            PlayerJoin,
            PlayerLeave,
            Loggout,
            CreateGame,
            RequestGameList,
            ReqestCharacter,
            Join,
            ISMOD,
            Ban,
            Premote,
            Report,
            RemoveReport,
            KeepAlive,
            
        }
        public Header header;
        public D2NG.Tools.D2NGGames[] Data;
    }
}
