using Moonfire.Core.Networking.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Moonfire.WorldServer
{
    public class WorldSession
    {
        public WorldSession(Socket clientSocket, IServer server)
        {
            this.TcpSocket = clientSocket;
            this.Server = server;
        }

        public Socket TcpSocket { get; set; }

        public IServer Server { get; set; }
    }
}
