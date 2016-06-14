namespace Moonfire.WorldServer
{
    using System;
    using System.Threading.Tasks;

    using Moonfire.Core.Networking;
    using Moonfire.Core.Networking.Interfaces;

    public class RealmClient : ClientBase
    {
        public RealmClient(IServer server)
            : base(server)
        {
        }

        public override async Task OnReceive(byte[] buffer)
        {
            var packet = new IncomingRealmPacket(buffer, buffer.Length);
            Console.WriteLine("Received: {0}", packet.PacketId.ToString());

            LoginHandler.ProcessPacket(this, packet);
        }

        public override void Send(IOutgoingPacket packet)
        {
            var bytePacket = packet.GetFinalizedPacket();
            Console.WriteLine("Sent {0}", packet.PacketId.ToString());
            this.Send(bytePacket, 0, bytePacket.Length);
        }
    }
}
