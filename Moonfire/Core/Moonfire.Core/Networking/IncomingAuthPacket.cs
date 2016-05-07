namespace Moonfire.Core.Networking
{
    using Moonfire.Core.Constants.Auth;

    public class IncomingAuthPacket : IncomingPacket
    {
        public AuthenticationCmd PacketId { get; set; }

        public IncomingAuthPacket(byte[] packet, int length)
            : base(packet, 0, length)
        {
            this.PacketId = (AuthenticationCmd)base.ReadByte();
        }
    }
}
