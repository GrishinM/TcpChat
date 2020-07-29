using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using ChatLibrary;
using ChatLibrary.Requests;
using ChatLibrary.Responses;
using Authorization = ChatLibrary.Requests.Authorization;

namespace WpfClient.Models
{
    public class Client : IClient
    {
        public List<Message> Messages { get; }

        public ClientInfo ClientInfo { get; private set; }

        private const string host = "127.0.0.1";
        private const int port = 8888;
        private readonly TcpClient client;
        private NetworkStream stream;
        private bool isConnected;

        public event Action AuthorizationSucceeded;
        public event Action<string> AuthorizationFailed;
        public event Action RegistrationSucceeded;
        public event Action<string> RegistrationFailed;
        public event Action Message;

        public Client()
        {
            client = new TcpClient();
            Messages = new List<Message>();
        }

        private void Connect()
        {
            if (isConnected)
                return;
            try
            {
                client.Connect(IPAddress.Parse(host), port);
                isConnected = true;
            }
            catch
            {
                throw new Exception("Сервер недоступен");
            }

            stream = client.GetStream();
            new Thread(MainProcess).Start();
        }

        private void MainProcess()
        {
            while (true)
            {
                try
                {
                    var response = JsonSerializer.Deserialize<Response>(GetResponse());
                    var data = response.Data;
                    switch (response.Type)
                    {
                        case ResponseType.AuthorizationResult:
                            var authorizationResult = JsonSerializer.Deserialize<AuthorizationResult>(data);
                            if (authorizationResult.Success)
                            {
                                Do(() => AuthorizationSucceeded());
                                ClientInfo = authorizationResult.ClientInfo;
                            }
                            else
                            {
                                var error = authorizationResult.Message;
                                Do(() => AuthorizationFailed(error));
                            }

                            break;
                        case ResponseType.RegistrationResult:
                            var registrationResult = JsonSerializer.Deserialize<RegistrationResult>(data);
                            if (registrationResult.Success)
                            {
                                Do(() => RegistrationSucceeded());
                            }
                            else
                            {
                                var error = registrationResult.Message;
                                Do(() => RegistrationFailed(error));
                            }

                            break;
                        default:
                            AddMessage(JsonSerializer.Deserialize<Message>(data));
                            break;
                    }
                }
                catch
                {
                    MessageBox.Show("Подключение прервано!");
                    Disconnect();
                    break;
                }
            }
        }

        public void Authorization(string login, string password)
        {
            try
            {
                Connect();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return;
            }

            var authorization = JsonSerializer.Serialize(new Authorization {Login = login, Password = password});
            var request = JsonSerializer.Serialize(new Request {Type = RequestType.Authorization, Data = authorization});
            SendRequest(request);
        }

        public void Registration(string login, string password)
        {
            try
            {
                Connect();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return;
            }

            var registration = JsonSerializer.Serialize(new Registration {Login = login, Password = password});
            var request = JsonSerializer.Serialize(new Request {Type = RequestType.Registration, Data = registration});
            SendRequest(request);
        }

        private void AddMessage(Message message)
        {
            Messages.Add(message);
            Message();
        }

        public void SendMessage(string message)
        {
            if (message == "")
                return;
            var request = JsonSerializer.Serialize(new Request {Data = message});
            SendRequest(request);
            AddMessage(new Message {Sender = ClientInfo, Text = message});
        }

        private static void Do(Action action)
        {
            Dispatcher.CurrentDispatcher.Invoke(action);
        }

        private string GetResponse()
        {
            return DataHelper.GetData(stream);
        }

        private void SendRequest(string data)
        {
            DataHelper.SendData(stream, data);
        }

        private void Disconnect()
        {
            stream?.Close();
            client?.Close();
            Environment.Exit(0);
        }
    }
}