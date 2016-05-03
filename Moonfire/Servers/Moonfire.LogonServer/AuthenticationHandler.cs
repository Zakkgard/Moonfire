namespace Moonfire.LogonServer
{
    using System;
    using System.Collections.Generic;

    using Moonfire.Core.Constants.Auth;
    using Moonfire.Core.Networking;
    using Moonfire.Core.Networking.Interfaces;
    using Moonfire.Core.Cryptography;

    public static class AuthenticationHandler
    {
        private static Dictionary<AuthenticationCmd, Action<IClient, IncomingAuthPacket>> authActions = new Dictionary<AuthenticationCmd, Action<IClient, IncomingAuthPacket>>
        {
            { AuthenticationCmd.CMD_AUTH_LOGON_CHALLENGE, HandleLogonChallenge },
            { AuthenticationCmd.CMD_AUTH_LOGON_PROOF, HandleLogonProof }
        };

        private static void HandleLogonProof(IClient client, IncomingAuthPacket packet)
        {
            client.Authenticator.SRP.PublicEphemeralValueA = packet.ReadBigInteger(32);
            BigInteger proof = packet.ReadBigInteger(20);
            Console.WriteLine(client.Authenticator.SRP.IsClientProofValid(proof));

            SendLogonProofReply(client);
        }

        private static void SendLogonProofReply(IClient client)
        {
            var packet = new OutgoingAuthPacket(AuthenticationCmd.CMD_AUTH_LOGON_PROOF);
            packet.Write((byte)0x00);
            packet.WriteBigInt(client.Authenticator.SRP.ServerSessionKeyProof, 20);
            packet.WriteInt(0);
            packet.WriteInt(0);
            packet.WriteShort(0);

            client.Send(packet);
        }

        public static void ProcessPacket(IClient client, IncomingAuthPacket packet)
        {
            var packetId = packet.packetId;
            Action<IClient, IncomingAuthPacket> methodHandler;
            authActions.TryGetValue(packetId, out methodHandler);

            if (methodHandler != null)
            {
                methodHandler.Invoke(client, packet);
            }
            else
            {
                Console.WriteLine("Unknown packet :(");
            }
        }

        private static void HandleLogonChallenge(IClient client, IncomingAuthPacket packet)
        {
            packet.Position = 33;
            var username = packet.ReadPascalString();
            var passHash = SecureRemotePassword.GenerateCredentialsHash(username, "CHANGEME");

            client.Authenticator = new Authenticator(new SecureRemotePassword(username, passHash, true));

            SendLogonChallengeReply(client);
        }

        private static void SendLogonChallengeReply(IClient client)
        {
            var packet = new OutgoingAuthPacket(AuthenticationCmd.CMD_AUTH_LOGON_CHALLENGE);
            packet.Write((byte)0x00);
            packet.Write((byte)0x00);
            client.Authenticator.WriteServerChallenge(packet);
            packet.Write((byte)0x00);
            
            client.Send(packet);
        }
    }
}
