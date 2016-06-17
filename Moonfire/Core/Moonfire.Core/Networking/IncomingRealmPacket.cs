using Moonfire.Core.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonfire.Core.Networking
{
    public class IncomingRealmPacket : IncomingPacket
    {
        public IncomingRealmPacket(byte[] packet, int length) 
            : base(packet, 0, length)
        {
            this.Lenght = base.ReadUInt16();
            this.PacketId = (WorldOpCode)base.ReadUInt16();
        }

        public WorldOpCode PacketId { get; set; }

        public uint Lenght { get; set; }
    }
}
