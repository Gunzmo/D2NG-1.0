using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2NG_server
{
    public class Character : D2NG.Tools.CharacterInfo
    {
        public int ID;
        public DateTime Joined { private set; get; }
        public Character(D2NG.Tools.CharacterInfo character, int ID) : base(0, character.Name){
            this.ID = ID;
            Flags = character.Flags;
            Class = character.Class;
            Name = character.Name;
            Title = character.Title;
            Level = character.Level;
            Act = character.Act;
        }
        public void UpdateJoinStamp(DateTime Joined) { this.Joined = Joined; }
    }
}
