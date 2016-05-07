using Moonfire.LogonServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Moonfire.WorldServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new WorldServer();
            server.EndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8085);
            server.Start();

            while (server.IsRunning)
            {
                Console.ReadLine();
            }
        }
    }
}
