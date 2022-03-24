using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Linq;
using DataPackaging;
//using DataStorage;

namespace ChatServer
{
    internal class Server : IServer
    {
        //protected internal DataBase LocalStorage = new DataBase();
        
        //private const int queueLimit = 100;
        public bool IsRunning { get; private set; }

        protected internal const int MainPort = 33777;

        protected internal TcpListener Listener { get; set; }
        
        protected internal readonly IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Loopback, MainPort);

        protected internal readonly UserSession systemSession;
        protected internal readonly List<UserSession> usersToAuthorize = new List<UserSession>();
        protected internal readonly List<UserSession> usersOnline = new List<UserSession>();

        protected Thread serverThread;

        public Server()
        {
            IsRunning = false;
            systemSession = new UserSession(new TcpClient()) { Name = "System", UserId = "0" };
        }

        public void Start()
        {
            serverThread = new Thread(Run);
            serverThread.Start();
        }

        public void Shutdown()
        {
            Console.WriteLine("\n[System] Server is shutting down ...");

            serverThread.Abort();

            Listener?.Stop();

            foreach (var user in usersOnline)
            {
                user?.Stream?.Close();
                user?.Client?.Close();
            }

        }

        protected void Run()
        {
            try
            {
                Listener = new TcpListener(localEndPoint);
                Listener.Start();
            }

            catch (SocketException e)
            {
                if (e.Message.Contains("Only one usage of each socket address (protocol/network address/port) is normally permitted"))
                {
                    Console.WriteLine("ERROR: Socket is in use. Possibly, another server instance is already running. Aborting...");
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey();
                    Shutdown();
                    return;
                }
            }

            Console.WriteLine("[System] Server is running now...");

            IsRunning = true;

            while (IsRunning)
            {
                CheckConnections(DisconnectUser);

                foreach (var user in usersOnline)
                {
                    try
                    {
                        if (user.Stream.DataAvailable)
                        {
                            Process(user);
                        }
                    }

                    catch (SocketException sockex)
                    {
                        if (sockex.SocketErrorCode == SocketError.NotConnected)
                            Console.WriteLine($"{user.Name} has been disconnected. [Exception: {sockex.Message}]");
                    }

                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        Console.WriteLine(e.StackTrace);
                    }
                }

                if (Listener.Pending())
                {
                    var client = Listener.AcceptTcpClient();

                    var clientIp = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                    var clientPort = ((IPEndPoint)client.Client.RemoteEndPoint).Port.ToString();
                    Console.WriteLine("\n[System] Incoming connection request from {0}:{1}...", clientIp, clientPort);

                    var session = new UserSession(client);
                    usersToAuthorize.Add(session);
                }

                foreach (var user in usersToAuthorize)
                {
                    if (user.Client.Connected)
                    {
                        if (Authorize(user))
                            break;
                    }
                    else
                    {
                        usersToAuthorize.Remove(user);
                        break;
                    }
                }
            }
            Shutdown();
        }

        public bool Authorize(UserSession user)
        {
            if (user.Stream.DataAvailable)
            {
                var message = user.ReceiveMessage();

                if (message.GetType() == typeof(UserData))
                {
                    // Вставить код для авторизации
                    user.UserId = (usersOnline.Count + 1).ToString();
                    user.Name = (message as UserData)?.Name;
                    user.Login = (message as UserData)?.Login;
                    user.Password = (message as UserData)?.Password;

                    if (user.Name == "\0" || string.IsNullOrWhiteSpace(user.Name))
                        user.Name = $"username{usersOnline.Count + 1}";

                    user.IsAuthorized = true;

                    var response = new TextMessage()
                    {
                        From = "[Server]",
                        Text = "Access granted!"
                    };

                    user.SendMessage(response);

                    usersToAuthorize.Remove(user);
                    ConnectUser(user);
                }

                else
                    //user.SendMessage("Wrong password or username.");
                    user.SendMessage(new TextMessage
                    {
                        From = "[Server]",
                        Text = "Wrong password or username."
                    });
            }
            return user.IsAuthorized;
        }

        protected void Process(UserSession user)
        {
            var message = user.ReceiveMessage();

            if (message?.GetType() == typeof(TextMessage))
            {
                Broadcast((message as TextMessage).Text, user);
            }
        }

        protected void BroadcastInfo()
        {
            var list = new List<string>();
            foreach (var onlineUser in usersOnline)
            {
                list.Add(onlineUser.Name);
            }

            //Оборачиваем список перед сериализацией
            var message = new OnlineList(list);
            foreach (var user in usersOnline)
            {
                user.SendMessage(message);
            }
        }

        public void Broadcast(string text, UserSession sender = null)
        {
            Console.WriteLine(sender == null ? $"\n[Chat] System: {text}" : $"\n[Chat] {sender.Name}: {text}");

            var message = new TextMessage()
            {
                From = sender == null ? "System" : $"{sender.Name}",
                Text = text               
            };

            foreach (var user in usersOnline)
            {
                //if (user != sender)
                //{
                //    user.SendMessage(message);
                //}
                user.SendMessage(message);
            }
        }

        public void ConnectUser(UserSession user)
        {
            usersOnline.Add(user);
            BroadcastInfo();
            Broadcast($"{user.Name} connected to the chat.", systemSession);
            Console.WriteLine($"\n[System] {user.Name} connected to the chat.");
            UsersOnline();

        }

        public void DisconnectUser(UserSession user)
        {
            usersOnline.Remove(user);
            user.Stream.Close();
            user.Client.Close();
            BroadcastInfo();
            Broadcast($"{user.Name} left the chat. Farewell, then!", systemSession);
            Console.WriteLine($"\n[System] {user.Name} left the chat. Farewell, then!");
        }


        protected delegate void DisconnectAction(UserSession user);

        protected void CheckConnections(DisconnectAction action)
        {
            var activeConnections = 
                IPGlobalProperties.
                GetIPGlobalProperties().
                GetActiveTcpConnections().
                Where(x => x.LocalEndPoint.Equals(localEndPoint));

            //CONTINUE:
            try
            {
                for (var index = 0; index < usersOnline.Count; index++)
                {
                    var user = usersOnline[index];
                    var tcpConnections = (activeConnections as TcpConnectionInformation[]) ?? activeConnections.ToArray();
                    var item = tcpConnections.SingleOrDefault(x =>
                        x.LocalEndPoint.Equals(user.Client.Client.LocalEndPoint) &&
                        x.RemoteEndPoint.Equals(user.Client.Client.RemoteEndPoint));
                    //Строго !(item.State == TcpState.Established), TcpState.Closed работает совершенно иначе (как???)
                    if (item == null || item.State != TcpState.Established)
                    {
                        action(user);
                    }
                }
            }

            catch (Exception e)
            {
                if (e.Message == "Collection was modified; enumeration operation may not execute.")
                {
                    //goto CONTINUE;
                }
            }
        }

        public void UsersOnline()
        {
            if (!IsRunning) return;
            
            Console.WriteLine($"\n[System] {usersOnline.Count} users are online.\n");
            
            for (int i = 0; i < usersOnline.Count; i++)
            {
                Console.WriteLine($"{i+1}. {usersOnline[i].Name}");
            }
        }

        protected void LogMessage(string message)
        {
            var time = DateTime.Now.ToShortTimeString();

            //log somehow
            var log = $"[{time}] " + message;
            //...
        }
    }
}
