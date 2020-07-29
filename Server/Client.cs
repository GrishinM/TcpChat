using System;
using System.Net.Sockets;
using System.Text.Json;
using ChatLibrary;
using ChatLibrary.Requests;

namespace Server
{
    public class Client
    {
        public ClientInfo ClientInfo { get; set; }

        private readonly TcpClient client;
        private readonly NetworkStream stream;
        private readonly Server server;

        public Client(TcpClient tcpClient, Server server)
        {
            client = tcpClient;
            stream = client.GetStream();
            this.server = server;
        }

        public void MainProcess()
        {
            try
            {
                while (true)
                {
                    try
                    {
                        var request = JsonSerializer.Deserialize<Request>(GetRequest());
                        var data = request.Data;
                        switch (request.Type)
                        {
                            case RequestType.Authorization:
                                var authorization = JsonSerializer.Deserialize<Authorization>(data);
                                server.Authorize(authorization, this);
                                break;
                            case RequestType.Registration:
                                var registration = JsonSerializer.Deserialize<Registration>(data);
                                server.Register(registration, this);
                                break;
                            default:
                                server.BroadcastMessage(data, ClientInfo);
                                break;
                        }
                    }
                    catch
                    {
                        // SendMessage($"{userName} покинул чат");
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                server.RemoveConnection(ClientInfo.Login);
                Close();
            }
        }

        private string GetRequest()
        {
            return DataHelper.GetData(stream);
        }

        public void SendResponse(string response)
        {
            DataHelper.SendData(stream, response);
        }

        protected internal void Close()
        {
            stream?.Close();
            client?.Close();
        }
    }
}