using D2NG.Pointers;
using System;
using System.Text;
using System.Threading;
using WhiteMagic;
using WhiteMagic.WinAPI;
using WhiteMagic.WinAPI.Structures;
using static D2NG.MessageHelper;
using WhiteMagic.Pointers;

namespace D2NG.Breakpoints
{
    public class BreakPointBase : HardwareBreakPoint
    {
        internal byte[] Packet;
        internal IntPtr ProcessID;
        internal StringBuilder hex;
        public BreakPointBase(ModulePointer Pointer, BreakpointCondition Condition, int Length, int ID, IntPtr ProcessID) : base(Pointer, Condition, Length) {
            this.ProcessID = ProcessID;
            this.ID = ID;
        }
        public override bool HandleException(ContextWrapper Wrapper)
        {
            foreach (byte b in Packet)
                hex.AppendFormat("{0:x2}", b);
            SendStringMessageToHandle(ProcessID, 1, ID + ";" + hex.ToString());
            Wrapper.Debugger.RemoveBreakPoint(ID);
            new Thread(() => { Wrapper.Debugger.AddBreakPoint(this); }).Start();
            return true;
        }
    }
    public class GamePacketReceive : BreakPointBase
    {
        public GamePacketReceive(IntPtr HostID) : base(gameEXE.ReceivePacket_I, BreakpointCondition.Code, 8, 1, HostID) { }
        public override bool HandleException(ContextWrapper Wrapper)
        {
            uint pPacket = Wrapper.Context.Ecx;
            uint len = Wrapper.Context.Edx;
            Packet = Wrapper.Debugger.ReadBytes(new IntPtr(pPacket), (int)len);
            hex = new StringBuilder(Packet.Length * 2);
            return base.HandleException(Wrapper);
        }
    }
    public class MPCReceive : BreakPointBase
    {
        public MPCReceive(IntPtr HostID) : base(gameEXE.MPCRecievePtr, BreakpointCondition.Code, 1, 0, HostID) { ProcessID = HostID; }
        public override bool HandleException(ContextWrapper Wrapper)
        {
            uint pPacket = Wrapper.Context.Ecx;
            uint len = Wrapper.Context.Edx;
            Packet = Wrapper.Debugger.ReadBytes(new IntPtr(pPacket), (int)len);
            hex = new StringBuilder(Packet.Length * 2);
            foreach (byte b in Packet)
                hex.AppendFormat("{0:x2}", b);
            SendStringMessageToHandle(ProcessID, 1, ID + ";" + hex.ToString());
            return base.HandleException(Wrapper);
        }
    }
    public class RealmReceive : BreakPointBase
    {
        public RealmReceive(IntPtr HostID) : base(gameEXE.RealmReceivePtr, BreakpointCondition.Code, 1, 2, HostID) { ProcessID = HostID; }
        public override bool HandleException(ContextWrapper Wrapper)
        {
            uint pPacket = Wrapper.Context.Esi;
            uint len = Wrapper.Context.Edi;
            Packet = Wrapper.Debugger.ReadBytes(new IntPtr(pPacket), (int)len);
            hex = new StringBuilder(Packet.Length * 2);
            foreach (byte b in Packet)
                hex.AppendFormat("{0:x2}", b);
            SendStringMessageToHandle(ProcessID, 1, ID + ";" + hex.ToString());
            return base.HandleException(Wrapper);
        }
    }
}
