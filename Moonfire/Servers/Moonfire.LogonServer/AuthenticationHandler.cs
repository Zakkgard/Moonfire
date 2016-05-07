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
            var packet2 = new OutgoingAuthPacket(AuthenticationCmd.CMD_REALM_LIST);
            packet2.Position += 2;

            packet2.Write(0); // unknown
            packet2.Write((byte)1); // num_realms
            packet2.Write(0); // type
            packet2.Write((byte)0); // flags
            packet2.WriteCString("Auuuuuub"); // realm name, null terminated
            packet2.WriteCString("127.0.0.1:8085"); // address:port, null terminated
            packet2.Write(0f); // population
            packet2.Write((byte)0); // num_chars
            packet2.Write((byte)0); // time_zone
            packet2.Write((byte)0); // unknown


            packet2.Write((short)0x0200);

            packet2.Position = 1;
            packet2.Write((short)packet2.TotalLength - 3);

            client.Send(packet2);
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
            packet.WriteInt(0);
            packet.WriteShort(0);

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
