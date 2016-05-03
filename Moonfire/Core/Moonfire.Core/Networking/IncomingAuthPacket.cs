namespace Moonfire.Core.Networking
{
    using Moonfire.Core.Constants.Auth;

    public class IncomingAuthPacket : IncomingPacket
    {
        public AuthenticationCmd packetId;

        public IncomingAuthPacket(byte[] packet, int length)
            : base(packet, 0, length)
        {
            this.packetId = (AuthenticationCmd)base.ReadByte();
        }
    }
}
