namespace Moonfire.Core.Networking
{
    using System.IO;

    using Moonfire.Core.Constants;
    using Moonfire.Core.Constants.Auth;
    using Moonfire.Core.Networking.Interfaces;

    public class OutgoingPacket : PacketWriter, IOutgoingPacket
    {
        public OutgoingPacket(AuthenticationCmd packetId)
            : base(new MemoryStream())
        {
            this.PacketId = packetId;
        }

        public OutgoingPacket(WorldOpCode packetId)
            : base(new MemoryStream())
        {
            this.PacketId = packetId;
        }

        public object PacketId { get; set; }

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
