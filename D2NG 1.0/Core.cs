using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WhiteMagic;
using D2NG.Pointers;
using D2NG.Breakpoints;

using System.Windows;
using WhiteMagic.WinAPI;
using D2NG.Tools;
using Microsoft.Win32;
using System.Windows.Interop;

namespace D2NG
{
    public static class MsecToNow
    {
        public static long MSecToNow(this DateTime date)
        {
            return (DateTime.Now.Ticks - date.Ticks) / TimeSpan.TicksPerMillisecond;
        }
    }
    
    public class Core : IDisposable
    {
        /// <summary>
        /// Credits
        /// casualcat for Pointers
        /// r57shell for alot of help
        /// Zakamurite for Dirty Deeds / Information on WhiteMagic And some Copypaste (MSecToNow, RECT for ingame window possiton, MyTimer(Was lazy))
        /// E.T.@BH for D2Smells Character List structure And some String as well some BitConverter code
        /// Gunzmo for making D2NG and hosting server
        /// </summary>
        public Process Process { get { return Game; } }
        Process Game;
        public ProcessDebugger Debugger { get { return PD; } }
        ProcessDebugger PD;
        MyTimer looper = new MyTimer();
        public string Path;
        public delegate void _Callback(int type, dynamic data);
        public event _Callback Callback;
        public bool InGame { get { return (_InGame == 1 ? true : false); } }
        IntPtr MainHandler;
        public Core(Process Game, Action Callback, IntPtr MainHandler)
        {
            this.Game = Game;
            if (!MagicHelpers.SetDebugPrivileges())
            {
                MessageBox.Show("Run As Administrator!");
                Callback();
                Dispose();
                return;
            }
            this.MainHandler = MainHandler;
        }
        public void Start()
        {
            PD = new ProcessDebugger(Game.Id);
            PD.Run();
            while (!PD.WaitForComeUp(50) && DateTime.Now.MSecToNow() < 5000) { }
            looper.Interval = 10;
            looper.Tick += Looper_Tick;
            looper.Start();
            AddInjections();
            RegistryKey myKey = Registry.LocalMachine.OpenSubKey("HKEY_CURRENT_USER\\Software\\Blizzard Entertainment\\Diablo II", true);
            if (myKey != null)
            {
                myKey.SetValue("LVL_REST", 0x29a, RegistryValueKind.DWord);
                myKey.Close();
            }
            Game.WaitForInputIdle(1000);
            int style = WinAPI.GetWindowLong(Game.MainWindowHandle, WinAPI.GWL_STYLE);
            style = style & ~WinAPI.WS_CAPTION & ~D2NG.Tools.WinAPI.WS_THICKFRAME;
            WinAPI.SetWindowLong(Game.MainWindowHandle, WinAPI.GWL_STYLE, style);
            if (Game.Responding)
            {
                Callback(7, true);
                InvokeMouseClick(0, 0);
                D2NG_1._0.MainWindow.GameHandler = Game.MainWindowHandle;
                HwndSource source = (HwndSource)HwndSource.FromVisual(((D2NG_1._0.MainWindow)Application.Current.MainWindow).GameLocation);
                WinAPI.SetParent(Game.MainWindowHandle, source.Handle);
                Point relativePoint = ((D2NG_1._0.MainWindow)Application.Current.MainWindow).GameLocation.TransformToAncestor(Application.Current.MainWindow).Transform(new Point(0, 0));
                WinAPI.MoveWindow(Game.MainWindowHandle, (int)relativePoint.X, (int)relativePoint.Y, (int)((D2NG_1._0.MainWindow)Application.Current.MainWindow).GameLocation.ActualWidth, (int)((D2NG_1._0.MainWindow)Application.Current.MainWindow).GameLocation.ActualHeight, false);
                Callback(7, false);
            }
        }

        internal void Register(string Account, string Password, string Email, Region Region)
        {
            SelectRealm(Region);
            new Thread(() => {
                while (!ConnectedToBnet) { }
                InvokeMouseClick(400, 330);
                InvokeMouseClick(400, 330);
                Thread.Sleep(500);
                InvokeKeyPressAsString(Account);
                Thread.Sleep(100);
                InvokeMouseClick(400, 390);
                InvokeMouseClick(400, 390);
                Thread.Sleep(500);
                InvokeKeyPressAsString(Password);
                Thread.Sleep(100);
                InvokeMouseClick(400, 450);
            }).Start();
        }

        byte MouseVisability = 0;
        byte _InGame;
        byte Screen;

        string gamename;
        string password;
        string descripton;
        string MaxPlayers;
        bool ConnectedToBnet = false;
        bool IsListed = true;
        Difficulty Difficulty { get; set; }
        void Looper_Tick(object sender, EventArgs e)
        {
            if (!PD.IsDebugging) return;
            if (MouseVisability != PD.Read<byte>(gameEXE.MouseVisible))
            {
                PD.Write<byte>(gameEXE.MouseVisible, 0x1);
                MouseVisability = PD.Read<byte>(gameEXE.MouseVisible);
            }
            if (Screen != PD.Read<byte>(gameEXE.GameLocationPtr))
            { 
                Screen = PD.Read<byte>(gameEXE.GameLocationPtr);
                if (Screen == 14)
                    ConnectedToBnet = true;
                else
                    ConnectedToBnet = false;
                if (Screen == 25)
                    Callback(13, null);
            }
            if (_InGame != PD.Read<byte>(gameEXE.InGamePTR))
            {
                _InGame = PD.Read<byte>(gameEXE.InGamePTR);
                if(Screen == 0 && !InGame)
                {
                    Callback(10, null);
                }
                if (Screen == 0 && InGame)
                    if (UI.D2NGMainWindow.ISD2NG)
                        if (CreateJoin)
                            Callback(5, new dynamic[] { true, gamename, password, IsListed, descripton, MaxPlayers, Difficulty });
                        else
                            Callback(9, null);
                    else
                        Callback(5, new dynamic[] { true });
                else if (Screen == 0 && _InGame == 0)
                    Callback(5, new dynamic[] { false });
            }
            Callback(8, null);

        }

        #region Packets
        public void SendPacket(string ppacket)
        {
            byte[] packet = StringToByteArray(ppacket);
            using (var suspender = new ProcessSuspender(PD))
            {
                var addr = PD.AllocateBytes(packet);
                PD.Call(gameEXE.SendPacket,
                    MagicConvention.StdCall,
                    packet.Length, 1, addr);
                PD.FreeMemory(addr);
            }
        }
        public void ReceivePacket(string ppacket)
        {
            byte[] packet = StringToByteArray(ppacket);
            using (var suspender = new ProcessSuspender(PD))
            {
                var addr = PD.AllocateBytes(packet);
                PD.Call(gameEXE.ReceivePacket,
                    MagicConvention.FastCall,
                    addr, packet.Length);
                PD.FreeMemory(addr);
            }
        }

        #endregion

        #region MapHack
        protected uint LoadAct(UnitAny player)
        {
            using (var suspender = new ProcessSuspender(PD))
            {
                var expCharFlag = PD.Read<byte>(new IntPtr(0x3A284C));
                var act = PD.Read<Act>(new IntPtr(player.dwAct));
                //(act, mapSeed, expCharFlag, 0, Difficulty, a3, actLevelStartId, LoadAct_1, LoadAct_3);

                return PD.Call<uint>(gameEXE.LoadAct,
                    MagicConvention.StdCall,
                    player.dwAct,
                    act.dwMapSeed,
                    1,
                    0,
                    (int)Difficulty,
                    0,
                    (uint)m_ActLevels[player.pAct],
                    PD.GetAddress(gameEXE.LoadAct_1),
                    PD.GetAddress(gameEXE.LoadAct_3));
            }
        }
        protected static uint[] m_ActLevels = new uint[]
        {
             1, 40, 75, 103, 109, 137
        };
        public void RevealMap()
        {

            var Unit = PD.Call<UnitAny>(gameEXE.PlayerUnit, MagicConvention.StdCall);
            var pAct = LoadAct(Unit);
            using (var suspender = new ProcessSuspender(PD))
                for (var i = m_ActLevels[Unit.pAct]; i < m_ActLevels[Unit.pAct + 1]; ++i)
                    _RevealMap(i, Unit);
        }
        void _RevealMap(uint dwLevelId, UnitAny unit)
        {
            var pLevel = GetLevel(dwLevelId, unit);

            if (pLevel == 0)
                return;
            var lvl = PD.Call<Level>(gameEXE.GetLevel, MagicConvention.StdCall);
            if (lvl.dwLevelNo > 255)
                return;

            if (lvl.pRoom2First == 0)
                PD.Call(gameEXE.InitLevel, MagicConvention.StdCall, new IntPtr(pLevel));

            InitLayer(lvl.dwLevelNo);
            lvl = PD.Read<Level>(new IntPtr(pLevel));

            for (var pRoom = lvl.pRoom2First; pRoom != 0;)
            {
                var room = PD.Read<Room2>(new IntPtr(pRoom));

                var actMisc2 = PD.Read<ActMisc>(new IntPtr(lvl.pMisc));
                var roomData = false;
                if (room.pRoom1 == 0)
                {
                    roomData = true;
                    PD.Call(gameEXE.AddRoomData,
                        MagicConvention.ThisCall,
                        0, actMisc2.pAct, lvl.dwLevelNo, room.dwPosX, room.dwPosY, room.pRoom1);
                }

                room = PD.Read<Room2>(new IntPtr(pRoom));
                if (room.pRoom1 == 0)
                {
                    pRoom = room.pRoom2Next;
                    continue;
                }

                var pAutomapLayer = PD.Read<uint>(gameEXE.AutomapLayer);

                PD.Call(gameEXE.RevealMap,
                    MagicConvention.StdCall,
                    room.pRoom1,
                    1,
                    pAutomapLayer);

                if (roomData)
                    PD.Call(gameEXE.RemoveRoomData,
                        MagicConvention.StdCall,
                        actMisc2.pAct, lvl.dwLevelNo, room.dwPosX, room.dwPosY, room.pRoom1);

                pRoom = room.pRoom2Next;
            }
        }
        private void InitLayer(uint levelNo)
        {
            var pLayer = PD.Call<IntPtr>(gameEXE.GetLayer,
                MagicConvention.FastCall,
                levelNo);
            if (pLayer == IntPtr.Zero)
                return;
            var layer = PD.Read<AutomapLayer2>(pLayer);
            PD.Call(gameEXE.GetLayer,
                MagicConvention.Register,
                layer.nLayerNo);
        }
        public uint GetLevel(uint dwLevel, UnitAny unit)
        {
            if (unit.pAct == 0)
                return 0;

            var act = PD.Read<Act>(new IntPtr(unit.dwAct));
            if (act.pMisc == 0)
                return 0;
            var actMisc = PD.Read<ActMisc>(new IntPtr(act.pMisc));

            var lvl = PD.Read<Level>(new IntPtr(actMisc.pLevelFirst));
            uint pLevel = 0;
            for (pLevel = actMisc.pLevelFirst; pLevel != 0; pLevel = lvl.pNextLevel)
            {
                if (pLevel != actMisc.pLevelFirst)
                    lvl = PD.Read<Level>(new IntPtr(pLevel));

                if (lvl.dwLevelNo == (uint)dwLevel && lvl.dwPosX > 0)
                    return pLevel;
            }

            pLevel = PD.Call<uint>(gameEXE.GetLevel,
                MagicConvention.FastCall,
                act.pMisc, dwLevel);

            return pLevel;
        }
        #endregion

        #region Interface
        public int pX { get { return PD.Read<int>(gameEXE.MouseX); } }
        public int pY { get { return PD.Read<int>(gameEXE.MouseY); } }
        public void InvokeKeyPressAsString(string Input)
        {
            Input = Input.ToUpper();
            foreach (var str in Input.ToCharArray())
            {
                System.Windows.Forms.Keys key;
                Enum.TryParse(str.ToString(), out key);
                switch (str)
                {
                    case '-':
                        key = System.Windows.Forms.Keys.Subtract;
                        break;
                    case ' ':
                        key = System.Windows.Forms.Keys.Space;
                        break;
                    case '0':
                        key = System.Windows.Forms.Keys.NumPad0;
                        break;
                    case '1':
                        key = System.Windows.Forms.Keys.NumPad1;
                        break;
                    case '2':
                        key = System.Windows.Forms.Keys.NumPad2;
                        break;
                    case '3':
                        key = System.Windows.Forms.Keys.NumPad3;
                        break;
                    case '4':
                        key = System.Windows.Forms.Keys.NumPad4;
                        break;
                    case '5':
                        key = System.Windows.Forms.Keys.NumPad5;
                        break;
                    case '6':
                        key = System.Windows.Forms.Keys.NumPad6;
                        break;
                    case '7':
                        key = System.Windows.Forms.Keys.NumPad7;
                        break;
                    case '8':
                        key = System.Windows.Forms.Keys.NumPad8;
                        break;
                    case '9':
                        key = System.Windows.Forms.Keys.NumPad9;
                        break;
                }
                WinAPI.PostMessage(Game.MainWindowHandle, 0x100, (IntPtr)(key), IntPtr.Zero);
            }
        }

        internal void ConvertToExpansion()
            { new Thread(() =>
            {
                InvokeMouseClick(350, 500);
                Thread.Sleep(500);
                InvokeMouseClick(455, 330);
            }).Start(); }

        public void InvokeKeyPress(System.Windows.Forms.Keys Input)
            { WinAPI.PostMessage(Game.MainWindowHandle, 0x100, (IntPtr)(Input), IntPtr.Zero);}
        public void InvokeMouseClick(int x, int y)
        {
            WinAPI.PostMessage(Game.MainWindowHandle, 0x0201, IntPtr.Zero, new IntPtr(x + (y << 16)));
            WinAPI.PostMessage(Game.MainWindowHandle, 0x0202, IntPtr.Zero, new IntPtr(x + (y << 16)));
        }
        #endregion

        void AddInjections()
        {
            PD.ClearUsedBreakpointSlots();
            if (PD.Breakpoints.Count >= Kernel32.MaxHardwareBreakpoints)
                return;
            PD.AddBreakPoint(new MPCReceive(MainHandler));
            PD.AddBreakPoint(new GamePacketReceive(MainHandler));
        }
        public void HandleMessage(IntPtr hWnd, string message)
        {
            message = message.Replace("\"", "");
            string[] data = message.Split(';');
            if (data[0] == "0")
            {
                switch (data[1].Substring(0, 2))
                {
                    case "19":
                        Callback(1, new CharacterList(StringToByteArray(data[1])));
                        break;
                    case "05":
                        Callback(4, new GameList(StringToByteArray(data[1])));
                        break;
                    case "12":
                        InvokeMouseClick(705, 460);
                        Callback(3, null);
                        break;
                    case "04":
                        Callback(11, StringToByteArray(data[1])[15]);
                        break;
                    case "11":
                        Callback(12, StringToByteArray(data[1])[7]);
                        break;
                    case "02":
                        Callback(14, StringToByteArray(data[1])[1]);
                        switch(StringToByteArray(data[1])[1])
                        {
                            case (byte)RealmCharacterActionResult.CharacterOverlap:
                            case (byte)RealmCharacterActionResult.InvalidCharacterName:
                                InvokeMouseClick(400, 330);
                                break;
                        }
                        break;
                    case "01":
                        Callback(15, StringToByteArray(data[1])[1]);
                        break;
                }
            }
            if(data[0] == "1")
            {
                switch(data[1].Substring(0, 2).ToLower())
                {
                    case "9c":
                    case "9d":
                        var item = Items.Parser.Parse(StringToByteArray(data[1]).ToList());
                        Console.WriteLine(data[1]);
                    break;
                }
                
            }
        }

        public void RefreshBnetGames()
        {
            new Thread(() => {
                InvokeMouseClick(600, 460);
                Thread.Sleep(100);
                InvokeMouseClick(705, 460);
                Thread.Sleep(500);
            }).Start();
        }
        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
        public void LoginBnet(string Account, string Password, Region Realm)
        {
            SelectRealm(Realm);
            new Thread(() => {
                while (!ConnectedToBnet) { }
                InvokeMouseClick(400, 330);
                InvokeMouseClick(400, 330);
                Thread.Sleep(500);
                InvokeKeyPressAsString(Account);
                Thread.Sleep(100);
                InvokeMouseClick(400, 390);
                InvokeMouseClick(400, 390);
                Thread.Sleep(500);
                InvokeKeyPressAsString(Password);
                Thread.Sleep(100);
                InvokeMouseClick(400, 450);
            }).Start();
        }
        bool CreateJoin = false;
        public void JoinGame(string GameName, string Password)
        {
            CreateJoin = false;
            new Thread(() =>
            {
                InvokeMouseClick(750,460);
                Thread.Sleep(100);
                InvokeMouseClick(500, 140);
                InvokeMouseClick(500, 140);
                Thread.Sleep(100);
                InvokeKeyPressAsString(GameName);
                Thread.Sleep(200);
                InvokeMouseClick(660, 140);
                InvokeMouseClick(660, 140);
                Thread.Sleep(200);
                if (Password == "")
                    InvokeKeyPress(System.Windows.Forms.Keys.Back);
                else
                    InvokeKeyPressAsString(Password);
                Thread.Sleep(100);
                InvokeMouseClick(660, 420);
            }).Start();
        }
        public void CreateGame(string GameName, string Password, bool IsListed, string Descripton, string MaxPlayers, Difficulty Diff)
        {
            CreateJoin = true;
            Difficulty = Diff;
            if (UI.D2NGMainWindow.ISD2NG)
            { 
                gamename = GameName;
                password = Password;
                descripton = Descripton;
                this.MaxPlayers = MaxPlayers;
                this.IsListed = IsListed;
            }
            new Thread(() =>
            {
                InvokeMouseClick(600, 460);
                Thread.Sleep(100);
                InvokeMouseClick(450, 150);
                InvokeMouseClick(450, 150);
                Thread.Sleep(100);
                InvokeKeyPressAsString(GameName);
                InvokeMouseClick(450, 210);
                InvokeMouseClick(450, 210);
                Thread.Sleep(100);
                InvokeKeyPressAsString(Password);
                Thread.Sleep(100);
                InvokeMouseClick(450, 265);
                InvokeMouseClick(450, 265);
                Thread.Sleep(100);
                InvokeKeyPressAsString(Descripton);
                Thread.Sleep(100);
                InvokeMouseClick(660, 300);
                InvokeMouseClick(660, 300);
                Thread.Sleep(100);
                InvokeKeyPressAsString(MaxPlayers);
                Thread.Sleep(100);
                switch(Diff)
                {
                    case Difficulty.Normal:
                        InvokeMouseClick(440, 375);
                        break;
                    case Difficulty.Nightmare:
                        InvokeMouseClick(565, 375);
                        break;
                    case Difficulty.Hell:
                        InvokeMouseClick(705, 375);
                        break;
                }
                Thread.Sleep(1000);
                InvokeMouseClick(700, 420);
            }).Start();
            

        }
        public void SelectRealm(Region Realm)
        {
            InvokeMouseClick(300, 380);
            switch(Realm)
            {
                case Region.Asia:
                    InvokeMouseClick(380, 400);
                    InvokeMouseClick(380, 400);
                    break;
                case Region.Europe:
                    InvokeMouseClick(380, 420);
                    InvokeMouseClick(380, 420);
                    break;
                case Region.USE:
                    InvokeMouseClick(380, 380);
                    InvokeMouseClick(380, 380);
                    break;
                case Region.USW:
                    InvokeMouseClick(380, 350);
                    InvokeMouseClick(380, 350);
                    break;
            }
            InvokeMouseClick(380, 350);
        }
        public void CreateCharacter(int x, int y, string Name, bool Ladder, bool HardCore, bool Expansion)
        {
            new Thread(() => {
                InvokeMouseClick(x, y);
                InvokeMouseClick(x, y);
                Thread.Sleep(500);
                InvokeMouseClick(400, 510);
                InvokeMouseClick(400, 510);
                Thread.Sleep(500);
                InvokeKeyPressAsString(Name);
                Thread.Sleep(500);
                if(!Expansion)
                    InvokeMouseClick(330, 533);
                if (!Ladder)
                    InvokeMouseClick(330, 575);
                if (HardCore)
                    InvokeMouseClick(330, 555);
                InvokeMouseClick(680, 555);
            }).Start();
        }
        public void Dispose()
        {
            if(PD != null) PD.Dispose();
            looper.Stop();
            Game.CloseMainWindow();
        }

    }
}
