namespace Moonfire.LogonServer
{
    using System;

    using Moonfire.Core.Networking;
    using Moonfire.Core.Networking.Interfaces;
    using Core.Cryptography;
    using Core.Constants.Auth;

    public class AuthClient : ClientBase
    {
        public AuthClient(IServer server)
            : base(server)
        {

        }

        public override Authenticator Authenticator { get; set; }
        
        public override bool OnReceive(byte[] buffer)
        {
            var packet = new IncomingAuthPacket(buffer, buffer.Length);
            Console.WriteLine("Received: {0}", packet.packetId.ToString());

            packet.Position = 33;
            var username = packet.ReadPascalString();
            var identityHash = SRP6.GenerateCredentialsHash(username, "changeme");
            this.Authenticator = new Authenticator(new SRP6(username, identityHash));

            AuthenticationHandler.SendAuthChallenge(this);

            return true;
        }

        public override void Send(OutgoingAuthPacket packet)
        {
            var bytePacket = packet.GetFinalizedPacket();
            Console.WriteLine("Client sent {0}", packet.packetId.ToString());
            this.Send(bytePacket, 0, bytePacket.Length);
        }
    }
}
