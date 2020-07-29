using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Server;

namespace Client
{
    internal static class Program
    {
        private const string host = "127.0.0.1";
        private const int port = 8888;
        private static TcpClient client;
        private static NetworkStream stream;

        private static void Main()
        {
            Console.Write("Введите ваше имя: ");
            var userName = Console.ReadLine();
            client = new TcpClient();
            try
            {
                client.Connect(IPAddress.Parse(host), port);
                stream = client.GetStream();
                DataHelper.SendData(stream, userName);
                Console.WriteLine($"Добро пожаловать, {userName}!{Environment.NewLine}Введите сообщение: ");
                new Thread(GetMessage).Start();
                new Thread(SendMessage).Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Disconnect();
            }
        }

        private static void GetMessage()
        {
            MessageAction(() => Console.WriteLine(DataHelper.GetData(stream)));
        }

        private static void SendMessage()
        {
            MessageAction(() => DataHelper.SendData(stream, Console.ReadLine()));
        }

        private static void MessageAction(Action action)
        {
            while (true)
            {
                try
                {
                    action();
                }
                catch
                {
                    Console.WriteLine("Подключение прервано!");
                    Console.ReadLine();
                    Disconnect();
                    break;
                }
            }
        }

        private static void Disconnect()
        {
            stream?.Close();
            client?.Close();
            Environment.Exit(0);
        }
    }
}