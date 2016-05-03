using Moonfire.Core.Constants.Auth;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonfire.Core.Networking
{
    public class OutgoingPacket : PacketWriter
    {
        public AuthenticationCmd packetId;

        public OutgoingPacket(AuthenticationCmd packetId)
            : base(new MemoryStream())
        {
            this.packetId = packetId;
        }

        public int TotalLength
        {
            get
            {
                return (int)base.BaseStream.Length;
            }
            set
            {
                base.BaseStream.SetLength(value);
            }
        }

        public byte[] GetFinalizedPacket()
        {
            var bytes = new byte[this.TotalLength];
            base.BaseStream.Position = 0;
            base.BaseStream.Read(bytes, 0, this.TotalLength);
            return bytes;
        }
    }
}
