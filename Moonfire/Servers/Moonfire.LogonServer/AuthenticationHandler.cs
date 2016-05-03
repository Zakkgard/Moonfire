namespace Moonfire.LogonServer
{
    using Moonfire.Core.Constants.Auth;
    using Moonfire.Core.Networking;
    using Moonfire.Core.Networking.Interfaces;

    public static class AuthenticationHandler
    {
        public static void SendAuthChallenge(IClient client)
        {
            var packet = new OutgoingAuthPacket(AuthenticationCmd.CMD_AUTH_LOGON_CHALLENGE);
            packet.Write((byte)0x00);
            packet.Write((byte)0x00);
            client.Authenticator.WriteServerChallenge(packet);
            
            client.Send(packet);
        }
    }
}
