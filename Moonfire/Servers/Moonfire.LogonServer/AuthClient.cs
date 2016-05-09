namespace Moonfire.LogonServer
{
    using System;

    using Moonfire.Core.Networking;
    using Moonfire.Core.Networking.Interfaces;
    using Core.Cryptography;
    using Core.Constants.Auth;
    using System.Threading.Tasks;
    public class AuthClient : ClientBase, IAuthClient
    {
        public AuthClient(IServer server)
            : base(server)
        {

        }

        public Authenticator Authenticator { get; set; }
        
        public override async Task OnReceive(byte[] buffer)
        {
            var packet = new IncomingAuthPacket(buffer, buffer.Length);
            Console.WriteLine("Received: {0}", packet.PacketId.ToString());

            AuthenticationHandler.ProcessPacket(this, packet);
        }

        public override void Send(IOutgoingPacket packet)
        {
            var bytePacket = packet.GetFinalizedPacket();
            Console.WriteLine("Client sent {0}", packet.PacketId.ToString());
            this.Send(bytePacket, 0, bytePacket.Length);
        }
    }
}
