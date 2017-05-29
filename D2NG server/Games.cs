using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace D2NG_server
{
    public class GamePacket
    {

    }
    public class Games
    {
        public bool Ladder { private set; get; }
        public bool Hardcore { private set; get; }
        public bool Expantion { private set; get; }
        public D2NG.Tools.Difficulty difficulty { private set; get; }
        public D2NG.Tools.Region Region { private set; get; }
        public int GameOwner = 0;
        public List<Character> characters = new List<Character>();
        public int MaxPlayers = 0;
        public int ID = 0;
        public string GameName { private set; get; }
        public string Password { private set; get; }
        public string Description { private set; get; }
        public bool IsListed { private set; get; }
        public Games(string GameName, string Password, bool IsListed, string Description, D2NG.Tools.Region Region, D2NG.Tools.Difficulty difficulty, int MaxPlayers, Cookies cookie)
        {
            ID = Tabels.games.Count + 1;
            GameOwner = cookie.Character.ID;
            Ladder = ((cookie.Character.Flags & D2NG.Tools.CharacterFlags.NoNLadder) != D2NG.Tools.CharacterFlags.NoNLadder);
            Hardcore = ((cookie.Character.Flags & D2NG.Tools.CharacterFlags.Hardcore) == D2NG.Tools.CharacterFlags.Hardcore);
            Expantion = ((cookie.Character.Flags & D2NG.Tools.CharacterFlags.Expansion) == D2NG.Tools.CharacterFlags.Expansion);
            this.MaxPlayers = (MaxPlayers > 8 ? 8 : MaxPlayers);
            this.Region = Region;
            this.difficulty = difficulty;
            this.GameName = GameName;
            this.Password = Password;
            this.Description = Description;
            this.IsListed = IsListed;
            characters.Add(cookie.Character);
            cookie.SetGameID(ID);
            
        }
        public void UpdateVisability(Cookies cookie, bool Listed) { if(cookie.Character.ID == GameOwner) IsListed = Listed; }
        public void Join(Cookies Cookie)
        {
            if (characters.Count == MaxPlayers) return;
            lock (characters)
                characters.Add(Cookie.Character);
            Cookie.SetGameID(ID);
            Cookie.AccountBinder.Joins++;
            Cookie.Character.UpdateJoinStamp(DateTime.Now);
            var Game = Program.GenerateGame(this);
            lock (Tabels.cookies)
                foreach (var c in Tabels.cookies)
                    c.context.Send(JsonConvert.SerializeObject(new Packets.Packet(Packets.Packet.Header.UpdateGame, new dynamic[] { ID, Game.Descriptions, Game.Players })));
            if (Cookie.context != null)
            Cookie.context.Send(new Packets.Packet(Packets.Packet.Header.Join, new dynamic[] {GameName, Password}).CreatePacket());
            
        }
        public void Leave(Cookies Cookie)
        {
            var car = characters.FirstOrDefault(c => c.ID == Cookie.Character.ID);
            if (car == null) return;
            lock (characters)
                characters.Remove(car);
            if (GameOwner == car.ID)
            {
                var Char = characters.OrderBy(c => c.Joined).FirstOrDefault();
                if (Char != null)
                    GameOwner = Char.ID;
            }
            Cookie.SetGameID(0);
            var Game = Program.GenerateGame(this);
            lock (Tabels.cookies)
                foreach (var c in Tabels.cookies)
                    c.context.Send(JsonConvert.SerializeObject(new Packets.Packet(Packets.Packet.Header.UpdateGame, new dynamic[] { ID, Game.Descriptions, Game.Players })));
        }
        
    }
}
