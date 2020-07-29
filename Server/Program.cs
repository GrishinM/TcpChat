using System;
using System.Net;
using System.Threading;

namespace Server
{
    internal static class Program
    {
        private static Server server;
        private const string host = "127.0.0.1";
        private const int port = 8888;

        private static void Main()
        {
            try
            {
                server = new Server(IPAddress.Parse(host), port);
                new Thread(server.Listen).Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                server.Disconnect();
            }
        }
    }
}