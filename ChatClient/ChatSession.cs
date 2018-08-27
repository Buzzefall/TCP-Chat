using System;
using System.Drawing;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using DataPackaging;

namespace ChatClient
{
    public class ChatSession
    {
        //Ссылки на формы для взаимодействия с GUI

        protected internal ChatClientForm loginForm;
        protected internal ChatSessionForm sessionForm;

        delegate void FormNewChatSession();
        delegate ChatSessionForm FormSwitchToChat();
        delegate void FormUpdateChat(TextMessage message);
        delegate void FormUpdateOnlineList(OnlineList list);
        delegate void FormUpdateView(string info, Color color);

        
        private FormNewChatSession NewChatSession;
        private FormSwitchToChat SwitchToChat;
        private FormUpdateChat UpdateChat;
        private FormUpdateOnlineList UpdateOnlineList;
        private FormUpdateView UpdateView;
        
        protected internal string Login { get; private set; }
        protected internal string Password { get; private set; }
        protected internal string Name { get; private set; }
        protected internal int UserID { get; private set; }
        
        protected internal IPEndPoint RemoteEndPoint { get; private set; }
        protected internal IPEndPoint LocalEndPoint { get; private set; }

        protected internal TcpClient Client { get; private set; }

        protected internal NetworkStream Stream { get; private set; }


        protected internal bool IsRunning { get; private set; }

        private Thread ListenThread;

        ~ChatSession()
        {
            // 
            End();
        }

        public ChatSession(ChatClientForm loginform)
        {

            loginForm = loginform;
            RemoteEndPoint = new IPEndPoint(IPAddress.Loopback, 33777);
            LocalEndPoint = new IPEndPoint(IPAddress.Loopback, 0);

            UpdateView = loginForm.UpdateView;
            NewChatSession = loginForm.NewChatSession;
            SwitchToChat = loginForm.SwitchToChat;
            
            IsRunning = true;
            var connectThread = new Thread(Start);
            connectThread.Start();


        }

        public void End()
        {
            //Флаг прекращения работы, который воспримет ListenThread
            IsRunning = false;
            //
            ListenThread?.Abort();
            Stream?.Close();
            Client?.Close();
        }

        public void Start()
        {
            
            
            var task = loginForm.BeginInvoke(UpdateView, "Attempting to connect...", Color.Blue);
            //var result = loginForm.EndInvoke(task);

            // Пытаемся соединиться
            do
            {
                if (!IsRunning) return;

                try
                {
                    Client = new TcpClient(RemoteEndPoint.Address.ToString(), RemoteEndPoint.Port);

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Thread.Sleep(50);
                }

            } while (Client == null);

            if (!IsRunning) return;

            Stream = Client.GetStream();

            loginForm.BeginInvoke(UpdateView, "Connection has been established.", Color.ForestGreen);
        }

        //Вход и автоматическая регистрация в случае нового юзера
        public void SignIn(string name, string login, string password)
        {
            if (Client == null) return;

            if (!IsConnected())
            {
                End();
                CrashAndReport(new Exception("Connection is lost. Attempting to reconnect..."));
                loginForm.BeginInvoke(NewChatSession);
                return;
            }

            var authorizationData = new UserData()
            {
                Name = name,
                Login = login,
                Password = password,
            };

            SendMessage(authorizationData);

            //or Environment.TickCount?
            var counter = 0;
            while (!Stream.DataAvailable && counter < 250)
            {
                counter++;
                Thread.Sleep(20);
            }

            if (Stream.DataAvailable)
            {
                var message = ReceiveMessage();
                
                if (message?.GetType() == typeof(TextMessage) && (message as TextMessage).Text == "Access granted!")
                {
                    loginForm.BeginInvoke(UpdateView, "Access granted!", Color.Blue);
                    Thread.Sleep(350);
                    
                    var form = loginForm.Invoke(SwitchToChat);
                    sessionForm = (ChatSessionForm) form;

                    UpdateChat = sessionForm.UpdateChat;
                    UpdateOnlineList = sessionForm.UpdateOnlineList;

                    ListenThread = new Thread(Listen);
                    ListenThread.Start();
                }

                else
                {
                    loginForm.BeginInvoke(UpdateView, "Wrong password or username", Color.Red);
                }
            }

            if (counter >= 250)
            {
                loginForm.BeginInvoke(UpdateView, "Message receive problem.", Color.Red);
            }
        }

        //После авторизации здесь слушаем сервер и обрабатываем сообщения
        private void Listen()
        {
            if (Client == null || Stream == null || !IsConnected())
            {
                CrashAndReport(new Exception("There is no active connection!"));
                return;
            }

            while (IsRunning)
            {
                try
                {
                    if (!Stream.DataAvailable) continue;
                    //??? or
                    //if (!Stream?.DataAvailable == null) continue;

                    var message = ReceiveMessage();

                    if (message?.GetType() == typeof(TextMessage))
                    {
                        sessionForm.BeginInvoke(UpdateChat, message as TextMessage);
                    }

                    if (message?.GetType() == typeof(OnlineList))
                    {
                        sessionForm.BeginInvoke(UpdateOnlineList, message as OnlineList);
                    }
                }

                catch (ObjectDisposedException e)
                {
                    if (IsRunning)
                    {
                        CrashAndReport(e);
                    }
                    return;
                }
                
            }

        }

        public void SendMessage(IBinarySerializable message)
        {
            try
            {
                if (Client == null || !Client.Connected || Stream == null)
                {
                    CrashAndReport(new Exception("There is no active connection!"));
                    return;
                }
                //Запаковываем сообщение и отсылаем
                message.SerializeBytes(Stream);
            }

            catch (Exception e)
            {
                CrashAndReport(e);
            }
        }

        public object ReceiveMessage()
        {
            try
            {
                if (Stream.DataAvailable)
                {
                    return new BinaryFormatter().Deserialize(Stream);
                }
                
            }

            catch (Exception e)
            {
                CrashAndReport(e);
            }

            return null;
        }

        private bool IsConnected()
        {
            var activeConnections = 
                IPGlobalProperties.
                GetIPGlobalProperties().
                GetActiveTcpConnections().
                Where(x => x.LocalEndPoint.Equals(Client.Client.LocalEndPoint));

            var currentConnection = activeConnections.SingleOrDefault(x =>  x.LocalEndPoint.Equals(Client.Client.LocalEndPoint) &&
                                                                            x.RemoteEndPoint.Equals(Client.Client.RemoteEndPoint));

            //Строго !(item.State == TcpState.Established), TcpState.Closed работает совершенно иначе (как???)
            return currentConnection != null && currentConnection.State == TcpState.Established;
        }

        private void CrashAndReport(Exception exception)
        {
            if (loginForm.Visible)
            {
                loginForm.BeginInvoke(UpdateView, exception.Message, Color.Red);
            }
            else
                sessionForm?.BeginInvoke(UpdateChat,
                    new TextMessage
                    {
                        From = "System",
                        Text = exception.Message,
                        TimeStamp = DateTime.Now.ToShortTimeString()
                    }
                );

            Thread.Sleep(3500);
            sessionForm?.Close();
        }
    }
}
