namespace Moonfire.Core.Networking
{
    using Moonfire.Core.Constants;

    public class IncomingRealmPacket : IncomingPacket
    {
        public IncomingRealmPacket(byte[] packet, int length) 
            : base(packet, 0, length)
        {
            this.Length = base.ReadUInt16();
            this.PacketId = (WorldOpCode)base.ReadUInt16();
        }

        public WorldOpCode PacketId { get; set; }

        public uint Length { get; set; }
    }
}
