using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WhiteMagic;
using WhiteMagic.Modules;
using WhiteMagic.WinAPI;
using D2NG_1._0;
using D2NG;
using WhiteMagic.Pointers;

namespace D2NG.Pointers
{
    class gameEXE : ModulePointer
    {

        public gameEXE(int offset) : base("Game.exe", offset) {}
        public static int Location { private set; get; }

        public static readonly int BNETSend = 0x0051C5C0 -0x400000;
        public static readonly int BNETRecieve = 0x00521B00 - 0x400000;
        public static readonly int BNETRecieveListner = 0x0051C7AF - 0x400000;
        public static readonly int MPCSend = 0x006BC8A0 - 0x400000;
        public static readonly int MPCRecieve = 0x0044B190 - 0x400000;
        public static readonly int MPCGlobalSocket = 0x00798FB4 - 0x400000;
        public static readonly int InGame = 0x3A78F0;
        public static readonly int GameLocation = 0x00779944 - 0x400000;
        public static readonly int RealmReceive = 0x00524930 - 0x400000;
        public static readonly int pAutomapLayer = 0x3A5164;
        public static readonly int ReadExpFlag = 0x3A04F4;
        public static readonly int ReadDifficulty = 0x3A060C;

        public static gameEXE BNETSendPtr = new gameEXE(BNETSend);
        public static gameEXE BNETRecievePtr = new gameEXE(BNETRecieveListner);
        public static gameEXE RealmReceivePtr = new gameEXE(RealmReceive);
        public static gameEXE MPCSendPtr = new gameEXE(MPCSend);
        public static gameEXE MPCRecievePtr = new gameEXE(MPCRecieve);
        public static gameEXE MPCGlobalSocketPtr = new gameEXE(MPCGlobalSocket);
        public static gameEXE MouseX = new gameEXE(0x3D55A8);
        public static gameEXE MouseY = new gameEXE(0x03D55A4);
        public static gameEXE _InGame = new gameEXE(0x0750974);
        public static gameEXE InGamePTR = new gameEXE(InGame);
        public static gameEXE CreateGameCharacterDiffrenceLevel = new gameEXE(0x30EBD0);
        public static gameEXE MouseVisible = new gameEXE(0x32DC3C);

        public static gameEXE SendPacket = new gameEXE(0x12AE50); //Updated 1.14d //0052AE50-BASE
        public static gameEXE ReceivePacket = new gameEXE(0x12AEB0); //Updated 1.14d //0052AEB0-BASE
        public static gameEXE ReceivePacket_I = new gameEXE(0x12B920); //Updated 1.14d //0052AEB0-BASE

        public static gameEXE GameLocationPtr = new gameEXE(GameLocation);
        public static gameEXE Mouse = new gameEXE(new IntPtr(0x0461861).ToInt32() + 53);


        /*<Tye77920>*/

        #region MapHack
        //0x591A0
        public static gameEXE RemoveRoomData = new gameEXE(0x21A0C0);//FUNCPTR(D2COMMON, RemoveRoomData, void __stdcall, (Act * ptAct, int LevelId, int Xpos, int Ypos, Room1 * pRoom), 0x21A0C0) //Updated 1.14d //0061A0C0-BASE
        public static gameEXE GetObjectTxt = new gameEXE(0x240E90); // FUNCPTR(D2COMMON, GetObjectTxt, ObjectTxt * __stdcall, (DWORD objno), 0x240E90) // Updated 1.14d //00640E90-BASE
        public static gameEXE GetTownLevel = new gameEXE(0x21AB70); // FUNCPTR(D2COMMON, GetTownLevel, int __stdcall, (int dwActNo), 0x21AB70) //Updated 1.14d //0061AB70-BASE
        public static gameEXE GetActNoByLevelNo = new gameEXE(0x2427F0); //FUNCPTR(D2COMMON, GetActNoByLevelNo, int __stdcall, (int dwLevelNo), 0x2427F0) //Updated 1.14d //006427F0-BASE
        public static gameEXE GetLevelNoByRoom = new gameEXE(0x21A1B0); //FUNCPTR(D2COMMON, GetLevelNoByRoom, int __stdcall, (Room1* ptRoom), 0x21A1B0) //Updated 1.14d //0061A1B0-BASE
        public static gameEXE InitLevel = new gameEXE(0x2424A0); //FUNCPTR(D2COMMON, InitLevel, void __stdcall, (Level *pLevel), 0x2424A0)//Updated 1.14d //006424A0-BASE //006424A3
        public static gameEXE GetLayer = new gameEXE(0x21E470); // FUNCPTR(D2COMMON, GetLayer, AutomapLayer2* __fastcall, (DWORD dwLevelNo), 0x21E470) //Updated 1.14d //0061E470-BASE
        public static gameEXE AutomapLayer = new gameEXE(pAutomapLayer); // FUNCPTR(InitAutomapLayer_I, AutomapLayer*, __fastcall, (uint32_t layerNo), 0x58D40) // 1.14d
        public static gameEXE RevealMap = new gameEXE(0x58F40); // FUNCPTR(D2CLIENT, RevealAutomapRoom, void __stdcall, (Room1 *pRoom1, DWORD dwClipFlag, AutomapLayer *aLayer), 0x58F40) //Updated 1.14d //00458F40-BASE
        public static gameEXE PlayerUnit = new gameEXE(0x63DD0); // VARPTR(PlayerUnit, UnitAny*, 0x3A6A70) // 1.14d
        public static gameEXE GetRoomFromUnit = new gameEXE(0x220BB0); //(D2COMMON, GetRoomFromUnit, Room1* __stdcall, (UnitAny * ptUnit), 0x220BB0)
        public static IntPtr GetDifficulty = new IntPtr(0x4DCD0); // (GetDifficulty, uint8_t, __stdcall, (void), 0x04DCD0)
        public static IntPtr GetExpCharFlag = new IntPtr(0x4DCD0); //(GetExpCharFlag, uint32_t, __stdcall, (void), 0x4DCC0)
        public static gameEXE GetUnitFromId_I = new gameEXE(0x63940); //ASMPTR(GetUnitFromId_I, 0x63940) 
        public static gameEXE InitAutomapLayer_I = new gameEXE(0x58D40); //FUNCPTR(D2CLIENT, InitAutomapLayer_I, AutomapLayer* __fastcall, (DWORD nLayerNo), 0x58D40)//Updated 1.14d //00458D40-BASE
        public static gameEXE GetLevel = new gameEXE(0x242AE0); // FUNCPTR(D2COMMON, GetLevel, Level * __fastcall, (ActMisc *pMisc, DWORD dwLevelNo), 0x242AE0) //Updated 1.14d //00642AE0-BASE
        public static gameEXE AddRoomData = new gameEXE(0x21A070); //FUNCPTR(D2COMMON, AddRoomData, void __stdcall, (Act * ptAct, int LevelId, int Xpos, int Ypos, Room1 * pRoom), 0x21A070)
        public static gameEXE LoadAct = new gameEXE(0x2194A0); //FUNCPTR(LoadAct, Act*, __stdcall, (uint32_t actNumber, uint32_t mapId, uint32_t unk, uint32_t unk_2, uint32_t unk_3, uint32_t unk_4, uint32_t townLevelId, uint32_t func_1, uint32_t func_2), 0x2194A0) // 1.14d
        public static gameEXE LoadAct_1 = new gameEXE(0x59150);
        public static gameEXE LoadAct_2 = new gameEXE(0x62D00);
        public static gameEXE LoadAct_3 = new gameEXE(0x2194A0);
        #endregion

        /// <summary>
        ///<casualcat> 0051C5C0 bnet send(msg id, buf, size)
        ///<casualcat> 00521B00 bnet recv(msg id, buf, size)
        ///<casualcat> 006BC8A0 mcp send(socket, buf, size) mcp socket global @ 00798FB4
        ///<casualcat> 0044B190 mcp recv(buf, size)
        ///Send Ingame 52AE50
        ///Receive Ingame 52AEB0

        private static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
    }
}
