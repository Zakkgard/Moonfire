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
        private static Dictionary<AuthenticationCmd, Action<IAuthClient, IncomingAuthPacket>> authActions = new Dictionary<AuthenticationCmd, Action<IAuthClient, IncomingAuthPacket>>
        {
            { AuthenticationCmd.CMD_AUTH_LOGON_CHALLENGE, HandleLogonChallenge },
            { AuthenticationCmd.CMD_AUTH_LOGON_PROOF, HandleLogonProof },
            { AuthenticationCmd.CMD_REALM_LIST, HandleRealmList }
        };

        private static void HandleRealmList(IAuthClient client, IncomingAuthPacket packet)
        {
            LoadRealmList(client);
        }

        private static void LoadRealmList(IAuthClient client)
        {
            var packet = new OutgoingAuthPacket(AuthenticationCmd.CMD_REALM_LIST);
            packet.Position += 2;
            packet.Write(0); // unknown

            packet.Write((byte)1); // # realms, to be repalced with actual number

            packet.Write(0);
            packet.WriteByte(0x40);
            packet.WriteCString("Auuuuuub");
            packet.WriteCString("127.0.0.1:8085");
            packet.WriteFloat(2);
            packet.WriteByte(0x00);
            packet.WriteByte(0x01);
            packet.WriteByte(0x00);

            packet.Write((short)0x0002);

            packet.Position = 1; // set the stream offset to write packet size
            packet.Write((short)packet.TotalLength - 3); // write packet size

            client.Send(packet);
        }

        private static void HandleLogonProof(IAuthClient client, IncomingAuthPacket packet)
        {
            client.Authenticator.SRP.PublicEphemeralValueA = packet.ReadBigInteger(32);
            BigInteger proof = packet.ReadBigInteger(20);

            SendLogonProofReply(client);
        }

        private static void SendLogonProofReply(IAuthClient client)
        {
            var packet = new OutgoingAuthPacket(AuthenticationCmd.CMD_AUTH_LOGON_PROOF);
            packet.Write((byte)0x00);
            packet.WriteBigInt(client.Authenticator.SRP.ServerSessionKeyProof, 20);
            packet.WriteInt(0);

            client.Send(packet);
        }

        public static void ProcessPacket(IAuthClient client, IncomingAuthPacket packet)
        {
            var packetId = packet.PacketId;
            Action<IAuthClient, IncomingAuthPacket> methodHandler;
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

        private static void HandleLogonChallenge(IAuthClient client, IncomingAuthPacket packet)
        {
            packet.Position = 33;
            var username = packet.ReadPascalString();
            var passHash = SecureRemotePassword.GenerateCredentialsHash(username, "CHANGEME");

            client.Authenticator = new Authenticator(new SecureRemotePassword(username, passHash, true));

            SendLogonChallengeReply(client);
        }

        private static void SendLogonChallengeReply(IAuthClient client)
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
