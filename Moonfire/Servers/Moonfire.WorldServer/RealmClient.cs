using Moonfire.Core.Networking.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moonfire.Core.Cryptography;
using Moonfire.Core.Networking;
using System.Net;
using System.Net.Sockets;

namespace Moonfire.WorldServer
{
    public class RealmClient : ClientBase
    {
        public RealmClient(IServer server)
            : base(server)
        {

        }

        public override bool OnReceive(byte[] buffer)
        {
            Console.WriteLine("Received something");
            //var packet = new IncomingAuthPacket(buffer, buffer.Length);
            //Console.WriteLine("Received: {0}", packet.PacketId.ToString());
            
            return false;
        }

        public override void Send(IOutgoingPacket packet)
        {
            throw new NotImplementedException();
        }
    }
}
