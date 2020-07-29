using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;
using ChatLibrary;
using ChatLibrary.Requests;
using ChatLibrary.Responses;
using Authorization = ChatLibrary.Requests.Authorization;

namespace Server
{
    public class Server
    {
        private readonly TcpListener tcpListener;
        private readonly Dictionary<string, ClientInfo> clients;
        private readonly Dictionary<string, Client> onlineClients;

        public Server(IPAddress address, int port)
        {
            tcpListener = new TcpListener(address, port);
            onlineClients = new Dictionary<string, Client>();
            clients = File.Exists("clients.json") ? JsonSerializer.Deserialize<Dictionary<string, ClientInfo>>(File.ReadAllText("clients.json")) : new Dictionary<string, ClientInfo>();
        }

        protected internal void Listen()
        {
            try
            {
                tcpListener.Start();
                Console.WriteLine("Сервер запущен");
                new Thread(() =>
                {
                    try
                    {
                        while (true)
                        {
                            switch (Console.ReadLine())
                            {
                                case "0":
                                    Disconnect();
                                    break;
                                case "1":
                                    clients.Values.ToList().ForEach(client => Console.WriteLine(client.Login));
                                    break;
                                case "2":
                                    onlineClients.Values.ToList().ForEach(client => Console.WriteLine(client.ClientInfo.Login));
                                    break;
                            }
                        }
                    }
                    catch
                    {
                        Disconnect();
                    }
                }).Start();
                while (true)
                {
                    var tcpClient = tcpListener.AcceptTcpClient();
                    new Thread(new Client(tcpClient, this).MainProcess).Start();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Disconnect();
            }
        }

        public void BroadcastMessage(string message, ClientInfo sender)
        {
            Console.WriteLine($"{sender.Name}: {message}");
            var response = JsonSerializer.Serialize(new Response {Data = JsonSerializer.Serialize(new Message {Sender = sender, Text = message})});
            onlineClients.Values.ToList().Where(client => client.ClientInfo != sender).ToList().ForEach(client => client.SendResponse(response));
        }

        private void AddConnection(Client client)
        {
            onlineClients.Add(client.ClientInfo.Login, client);
        }

        public void RemoveConnection(string login)
        {
            if (!onlineClients.ContainsKey(login))
                return;
            onlineClients.Remove(login);
            Console.WriteLine($"Пользователь {login} вышел");
        }

        public void Authorize(Authorization authorization, Client client)
        {
            string response;
            var login = authorization.Login;
            var password = authorization.Password;
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                response = JsonSerializer.Serialize(new Response
                {
                    Type = ResponseType.AuthorizationResult,
                    Data = JsonSerializer.Serialize(new AuthorizationResult {Success = false, Message = "Некорректные данные"})
                });
            }
            else
            {
                if (!clients.ContainsKey(login))
                {
                    response = JsonSerializer.Serialize(new Response
                    {
                        Type = ResponseType.AuthorizationResult,
                        Data = JsonSerializer.Serialize(new AuthorizationResult {Success = false, Message = "Пользователь с таким логином не найден"})
                    });
                }
                else
                {
                    var clientInfo = clients[login];
                    if (password.GetHashCode() != clientInfo.PasswordHash)
                    {
                        response = JsonSerializer.Serialize(new Response
                        {
                            Type = ResponseType.AuthorizationResult,
                            Data = JsonSerializer.Serialize(new AuthorizationResult {Success = false, Message = "Неверный пароль"})
                        });
                    }
                    else
                    {
                        response = JsonSerializer.Serialize(new Response
                        {
                            Type = ResponseType.AuthorizationResult,
                            Data = JsonSerializer.Serialize(new AuthorizationResult {Success = true, ClientInfo = clientInfo})
                        });
                        client.ClientInfo = clientInfo;
                        AddConnection(client);
                        Console.WriteLine($"Пользователь {login} авторизовался");
                    }
                }
            }

            client.SendResponse(response);
        }

        public void Register(Registration registration, Client client)
        {
            var login = registration.Login;
            var password = registration.Password;
            string response;
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                response = JsonSerializer.Serialize(new Response
                {
                    Type = ResponseType.RegistrationResult,
                    Data = JsonSerializer.Serialize(new RegistrationResult {Success = false, Message = "Некорректные данные"})
                });
            }
            else
            {
                if (clients.ContainsKey(login))
                {
                    response = JsonSerializer.Serialize(new Response
                    {
                        Type = ResponseType.RegistrationResult,
                        Data = JsonSerializer.Serialize(new RegistrationResult {Success = false, Message = "Пользователь с таким логином уже существует"})
                    });
                }
                else
                {
                    response = JsonSerializer.Serialize(new Response
                    {
                        Type = ResponseType.RegistrationResult,
                        Data = JsonSerializer.Serialize(new RegistrationResult {Success = true})
                    });
                    clients.Add(login, new ClientInfo {Login = login, PasswordHash = password.GetHashCode(), Name = login});
                    Console.WriteLine($"Пользователь {login} зарегистрировался");
                }
            }

            client.SendResponse(response);
        }

        protected internal void Disconnect()
        {
            onlineClients.Values.ToList().ForEach(client => client.Close());
            tcpListener.Stop();
            using (var writer = new StreamWriter("clients.json"))
            {
                writer.Write(JsonSerializer.Serialize(clients));
            }

            Environment.Exit(0);
        }
    }
}