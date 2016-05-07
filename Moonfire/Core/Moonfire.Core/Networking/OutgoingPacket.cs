using Moonfire.Core.Constants.Auth;
using Moonfire.Core.Networking.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonfire.Core.Networking
{
    public class OutgoingPacket : PacketWriter, IOutgoingPacket
    {
        public OutgoingPacket(AuthenticationCmd packetId)
            : base(new MemoryStream())
        {
            this.PacketId = packetId;
        }

        public AuthenticationCmd PacketId { get; set; }

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
