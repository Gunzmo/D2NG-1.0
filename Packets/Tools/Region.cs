using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Media.Imaging;

namespace D2NG.Tools
{
    public enum RealmStartupResult
    {
        Success = 0,
        NoBattleNetConnection = 0x0C,
        InvalidCDKey = 0x7E,    //TESTME: key in use? key banned? key invalid?
        TemporaryIPBan = 0x7F,  // "Your connection has been temporarily restricted from this realm. Please try to log in at another time"
    }
    public enum RealmLogonResult : uint
    {
        RealmUnavailable = 0x80000001,
        LogonFailed = 0x80000002,
    }
    public enum RealmCharacterActionResult
    {
        Success = 0,
        /// <summary>
        /// Character already exists or account already has maximum number of characters (currently 8)
        /// </summary>
        CharacterOverlap = 0x14,
        /// <summary>
        /// Character name is longer than 15 characters or contains illegal characters.
        /// </summary>
        InvalidCharacterName = 0x15,
        /// <summary>
        /// Invalid character name specified for action
        /// </summary>
        CharacterNotFound = 0x46,
        /// <summary>
        /// Invalid character name specified for character deletion
        /// </summary>
        CharacterDoesNotExist = 0x49,
        /// <summary>
        /// The action (logon, upgrade, etc. has failed for an unspecified reason)
        /// </summary>
        Failed = 0x7A,
        /// <summary>
        /// Cannot perform any action but delete on expired characters...
        /// </summary>
        CharacterExpired = 0x7B,
        /// <summary>
        /// When trying to upgrade the character
        /// </summary>
        CharacterAlreadyExpansion = 0x7C,
    }
    public enum CreateGameResult
    {
        Sucess = 0, // This does NOT automatically join the game - the client must also send packet RC 0x04
        InvalidGameName = 0x1E,
        GameAlreadyExists = 0x1F,
        DeadHardcoreCharacter = 0x6E,
    }
    public enum JoinGameResult
    {
        Sucess = 0, // Terminate the connection with the MCP and initiate with D2GS.
        PasswordIncorrect = 0x29,
        GameDoesNotExist = 0x2A,
        GameFull = 0x2B,
        LevelRequirementsNotMet = 0x2C, // You do not meet the level requirements for this game.
        DeadHardcoreCharacter = 0x6E,   // A dead hardcore character cannot join a game
        UnableToJoinHardcoreGame = 0x71,    // A non-hardcore character cannot join a game created by a Hardcore character
        UnableToJoinNightmareGame = 0x73,
        UnableToJoinHellGame = 0x74,
        UnableToJoinExpansionGame = 0x78,   // A non-expansion character cannot join a game created by an Expansion character.
        UnableToJoinClassicGame = 0x79, // A Expansion character cannot join a game created by a non-expansion character.
        UnableToJoinLadderGame = 0x7D   // A non-ladder character cannot join a game created by a Ladder character.
    }
    public enum FriendType
    {
        Mutal,
        Owner,
        Slave,
        Blocked
    }
    public enum ReportReason
    {
        Hacks,
        Spam,
        Scam
    }
    public enum MSGType
    {
        User,
        Channel,
    }
    public enum Region
    {
        Europe,
        USE,
        USW,
        Asia
    }
    [Flags]
    public enum Difficulty
    {
        Normal = 0,
        Nightmare = 1 << 0,
        Hell = 2 << 1,
        All = 4 << 2
    }
    public enum Rank
    {
        Guest,
        Member,
        Moderator,
        Administrator
    }
    public class D2NGGames
    {
        public string Gamename { get; set; }

        public int ID = 0;
        public List<string> Descriptions { get; set; }

        public Difficulty Difficulty { get; set; }

        public int Players{ get; set; }
        [JsonIgnore]
        public bool selected = false;
        public D2NGGames() { Players = 0; Descriptions = new List<string>(); }
    }

    public class Mods
    {
        public string AccountName;
        public int color;
        public Mods(string accountName, bool online)
        {
            AccountName = accountName;
            if (online)
                color = 0x0F0;
            else
                color = 0xF00;
        }
    }
}
