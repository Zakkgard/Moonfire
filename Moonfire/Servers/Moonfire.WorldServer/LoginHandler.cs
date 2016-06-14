namespace Moonfire.WorldServer
{
    using System;
    using System.Collections.Generic;

    using Moonfire.Core.Constants;
    using Moonfire.Core.Networking;
    using Moonfire.Core.Networking.Interfaces;
    
    public static class LoginHandler
    {
        private static Dictionary<WorldOpCode, Action<IClient, IncomingRealmPacket>> actions = new Dictionary<WorldOpCode, Action<IClient, IncomingRealmPacket>>
        {
            { WorldOpCode.CMSG_AUTH_SESSION, HandleAuthSession },
        };

        public static void ProcessPacket(IClient client, IncomingRealmPacket packet)
        {
            var packetId = packet.PacketId;
            Action<IClient, IncomingRealmPacket> methodHandler;
            actions.TryGetValue(packetId, out methodHandler);

            if (methodHandler != null)
            {
                methodHandler.Invoke(client, packet);
            }
            else
            {
                Console.WriteLine("Unknown packet :(");
            }
        }

        public static void SendAuthChallenge(IClient client)
        {
            var packet = new OutgoingRealmPacket(WorldOpCode.SMSG_AUTH_CHALLENGE);
            //packet.WriteInt(0xdeadbabe);
            packet.Write((client.Server as WorldServer).Seed[0]);
            packet.Write((client.Server as WorldServer).Seed[1]);
            packet.Write((client.Server as WorldServer).Seed[2]);
            packet.Write((client.Server as WorldServer).Seed[3]);
            //packet.WriteBigInt(SecureRemotePassword.RandomNumber(16));
            //packet.WriteBigInt(SecureRemotePassword.RandomNumber(16));
            packet.Position = 0;
            packet.WriteUShortBE((ushort)(packet.TotalLength - 2));

            //var packet = new byte[] { 0, 38, 236, 1, 82, 98, 99, 225, 253, 126, 188, 160, 125, 75, 171, 52, 206, 10, 219, 112, 74, 185, 181, 176, 149, 85, 0, 177, 231, 143, 213, 82, 66, 50, 50, 63, 117, 237, 248, 211 };

            client.Send(packet);
        }

        public static void HandleAuthSession(IClient client, IncomingRealmPacket packet)
        {
            packet.ReadUInt16();
            uint version = packet.ReadUInt32();
            uint sessionId = packet.ReadUInt32();
            string account = packet.ReadCString();
            uint seed = packet.ReadUInt32();

            SendAuthSessionOkay(client);
        }

        public static void SendAuthSessionOkay(IClient client)
        {
            var packet = new OutgoingRealmPacket(WorldOpCode.SMSG_AUTH_RESPONSE);
            packet.WriteByte((byte)LoginResponse.AUTH_OK);
            packet.WriteUInt(0);
            packet.WriteByte(0x02);
            packet.WriteUInt(0);

            client.Send(packet);
        }
    }
}
