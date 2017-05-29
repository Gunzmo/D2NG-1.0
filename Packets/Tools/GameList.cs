using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2NG.Tools
{
    public class GameList
    {
        public readonly ushort RequestID;
        public readonly byte PlayerCount;
        public readonly ushort Flags;
        public readonly string Name;
        public readonly string Description = null;
        public readonly uint Unknown1;
        public readonly byte Unknown2; // always 4, except for last (empty) packet telling us listing is done, where it's 0 ?
        public readonly byte Unknown3; // always 0 ? seems to just tell string is comming....
        public GameList(byte[] data)
        {
            this.RequestID = BitConverter.ToUInt16(data, 1);
            this.Unknown1 = BitConverter.ToUInt32(data, 3);
            this.PlayerCount = data[7];
            this.Unknown2 = data[8];
            this.Flags = BitConverter.ToUInt16(data, 9);
            this.Unknown3 = data[11];
            this.Name = ByteConverter.GetNullString(data, 12);
            if (data.Length > 14 + this.Name.Length)
                this.Description = ByteConverter.GetNullString(data, 13 + this.Name.Length);
        }
    }
}
