namespace Moonfire.LogonServer
{
    using System;
    using System.Net;

    class Program
    {
        static void Main(string[] args)
        {
            var server = new LogonServer();
            server.EndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3724);
            server.Start();

            while (server.IsRunning)
            {
                Console.ReadLine();
            }
        }
    }
}
