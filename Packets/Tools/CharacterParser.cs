using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2NG.Tools
{

    public class CharactersInfo
    {
        /// <summary>
        /// Character Genders array. true = Female; false = Male
        /// </summary>
        public static readonly bool[] Genders = new bool[] {
            true, true, false, false, false, false, true
        };
    }

    public enum CharacterClass
    {
        Amazon,
        Sorceress,
        Necromancer,
        Paladin,
        Barbarian,
        Druid,
        Assassin,
        Invalid,
        NotApplicable,
        Any
    }
    [Flags]
    public enum CharacterFlags
    {
        /// <summary>
        /// None would normally mean a male softcore character on open non-ladder classic who never died...
        /// </summary>
        None = 0,
        /// <summary>
        /// Character has never joined a game
        /// </summary>
        Noob = 1,
        UNKNOWNx2 = 2,
        Hardcore = 4,

        UNKNOWNx40 = 0x40,
        UNKNOWNx10 = 0x10,  // Ghost ? (Died + Hardcore + Chicken...)
        Expansion = 0x20,
        NoNLadder = 0x8,

        Realm = 0x80,
        Female = 0x100,
    }
    /// <summary>
    /// All Bnet titles are 0x01 - 0x03. To match this list, add 4 if hardcore, 0x20 if expansion and 0x100 if female...
    /// Some expansion title are gender neutral; if title ends with "F", remove it to get proper title...
    /// </summary>
    public enum CharacterTitle
    {
        // Classic, male, softcore
        Nooblar = 0,
        Sir = 1,
        Lord = 2,
        Baron = 3,

        // Classic, female, softcore
        Nooblette = 0x100,
        Dame = 0x101,
        Lady = 0x102,
        Baroness = 0x103,

        // Classic, male, hardcore
        CourageousNooblar = 4,
        Count = 5,
        Duke = 6,
        King = 7,

        // Classic, female, hardcore
        CourageousNooblette = 0x104,
        Countess = 0x105,
        Duchess = 0x106,
        Queen = 0x107,

        // Expansion, male, softcore
        DoublePlusNooblar = 0x20,
        Slayer = 0x21,
        Champion = 0x22,
        Patriarch = 0x23,

        // Expansion, female, softcore
        DoublePlusNooblette = 0x120,
        SlayerF = 0x121,        // Slayer
        ChampionF = 0x122,      // Champion
        Matriarch = 0x123,

        // Expansion, male, hardcore
        NooblarWhoLikesChicken = 0x24,
        Destroyer = 0x25,
        Conquerer = 0x26,
        Guardian = 0x27,

        // Expansion, female, hardcore
        NoobletteWhoLikesChicken = 0x124,
        DestroyerF = 0x125, // Destroyer
        ConquererF = 0x125, // Conquerer
        GuardianF = 0x125,  // Guardian

        None = 0xFFFF
    }
    public class CharacterList
    {
        public readonly uint Requested;
        public readonly uint Total;
        public readonly uint Listed;
        public readonly CharacterInfo[] Characters;

        public CharacterList(byte[] data)
        {
            this.Requested = BitConverter.ToUInt16(data, 1);
            this.Total = BitConverter.ToUInt32(data, 3);
            this.Listed = BitConverter.ToUInt16(data, 7);

            this.Characters = new CharacterInfo[this.Listed];
            int index = 9;
            for (int i = 0; i < this.Listed && index < data.Length; i++)
            {
                this.Characters[i] = new CharacterInfo(
                    BitConverter.ToUInt32(data, index),
                    ByteConverter.GetNullString(data, (index += 4))
                );

                index += this.Characters[i].Name.Length + 1;

                this.Characters[i].ClientVersion = data[index];

                int bnc = data[index + 13] - 1;
                if (bnc < 0 || bnc > 6)
                    this.Characters[i].Class = CharacterClass.Invalid;
                else
                {
                    this.Characters[i].Class = (CharacterClass)bnc;
                    if (CharactersInfo.Genders[(int)this.Characters[i].Class])
                        this.Characters[i].Flags |= CharacterFlags.Female;
                }

                this.Characters[i].Level = data[index + 25];
                this.Characters[i].Flags |= (CharacterFlags)data[index + 26];

                int title;
                int act = (data[index + 27] & 0x3e) >> 1;
                if ((this.Characters[i].Flags & CharacterFlags.Expansion) == CharacterFlags.Expansion)
                {
                    title = act / 5;
                    act = act % 5;
                }
                else
                {
                    title = act / 4;
                    act = act % 4;
                }

                if (title == 3)
                    this.Characters[i].Act = 666;
                else
                    this.Characters[i].Act = act + 1;

                if ((this.Characters[i].Flags & CharacterFlags.Hardcore) == CharacterFlags.Hardcore)
                    title |= (int)CharacterFlags.Hardcore;
                if ((this.Characters[i].Flags & CharacterFlags.Expansion) == CharacterFlags.Expansion)
                    title |= (int)CharacterFlags.Expansion;
                if ((this.Characters[i].Flags & CharacterFlags.Female) == CharacterFlags.Female)
                    title |= (int)CharacterFlags.Female;

                this.Characters[i].Title = (CharacterTitle)title;

                //TODO: parse the graphics
                //BYTE Helmet = [2];
                //BYTE Weapon = [7];
                //BYTE Shield = [9];
                // etc...

                index = ByteConverter.GetBytePosition(data, 0, index) + 1;
            }
        }
    }

    public class CharacterInfo
    {
        const long TICKS_TILL_1970 = 621355968000000000; // ticks between 0001/01/01 and 1970/01/01

        public string Name;
        public CharacterClass Class;
        public int Level;
        public CharacterFlags Flags;
        public CharacterTitle Title;
        public int Act;
        public DateTime Expires;
        public int ClientVersion;

        public CharacterInfo(uint timestamp, string name)
        {
            // Convert unix UTC timestamp (seconds since 1970/01/01) to .NET local DateTime (ticks since 0001/01/01)
            this.Expires = new DateTime(TICKS_TILL_1970 + timestamp * 10000000L + (DateTime.Now.Ticks - DateTime.UtcNow.Ticks));
            this.Name = name;
        }

        public override string ToString()
        {
            return StringUtils.ToFormatedInfoString(this, false, ": ", ", ");
        }
    }
}
