namespace Moonfire.Core.Networking
{
    using Moonfire.Core.Constants.Auth;

    public class OutgoingAuthPacket : OutgoingPacket
    {
        public OutgoingAuthPacket(AuthenticationCmd opcode)
            : base(opcode)
        {
            base.WriteByte((byte)opcode);
        }
    }
}
