namespace Moonfire.Core.Networking
{
    using Moonfire.Core.Constants;

    public class OutgoingRealmPacket : OutgoingPacket
    {
        public OutgoingRealmPacket(WorldOpCode opcode) 
            : base(opcode)
        {
            this.Position += 2;
            this.Write((ushort)opcode);
        }
    }
}
