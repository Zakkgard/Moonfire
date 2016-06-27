namespace Moonfire.WorldServer
{
    using System;
    using System.Net;

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
