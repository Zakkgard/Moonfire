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
            { AuthenticationCmd.CMD_AUTH_LOGON_PROOF, HandleLogonProof },
            { AuthenticationCmd.CMD_REALM_LIST, HandleRealmList }
        };

        private static void HandleRealmList(IClient client, IncomingAuthPacket packet)
        {
            LoadRealmList(client);
        }

        private static void LoadRealmList(IClient client)
        {
            var packet = new OutgoingAuthPacket(AuthenticationCmd.CMD_REALM_LIST);

            packet.Position += 2;
            packet.Write(0);
            packet.Write((byte)1);

            packet.Write(0); // server type
            packet.Write((byte)0x20); // server flags
            packet.WriteCString("AUUUB"); // server name
            packet.WriteCString("127.0.0.1:8085"); // server ip:port
            packet.WriteFloat(1.7f); // server population
            packet.Write((byte)0); // char count
            packet.Write((byte)0);
            packet.Write((byte)0);
            packet.Write(0x0002);
            
            packet.Position = 1;
            packet.Write((short)packet.TotalLength - 3);

            client.Send(packet);
        }

        private static void HandleLogonProof(IClient client, IncomingAuthPacket packet)
        {
            client.Authenticator.SRP.PublicEphemeralValueA = packet.ReadBigInteger(32);
            BigInteger proof = packet.ReadBigInteger(20);
            //Console.WriteLine(client.Authenticator.SRP.IsClientProofValid(proof));

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
