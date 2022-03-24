using System;
using System.Drawing;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;

using DataPackaging;
using DataPackaging.Interfaces;

namespace ChatClient
{
    public class ChatSession : IDisposable
    {
        //Ссылки на формы для взаимодействия с GUI

        private readonly ChatClientForm _loginForm;
        private ChatSessionForm _sessionForm;

        private delegate ChatSessionForm FormSwitchToChat();

        private delegate void FormNewChatSession();

        private delegate void FormUpdateOnlineList(OnlineList list);

        private delegate void FormUpdateChat(TextMessage message);

        private delegate void FormUpdateView(string info, Color color);


        private readonly FormUpdateView _updateView;
        private readonly FormSwitchToChat _switchToChat;
        private readonly FormNewChatSession _newChatSession;
        private FormUpdateChat _updateChat;
        private FormUpdateOnlineList _updateOnlineList;

        private string Login { get; set; }
        private string Password { get; set; }
        private string Name { get; set; }
        private int UserId { get; set; }

        private IPEndPoint RemoteEndPoint { get; set; }
        private IPEndPoint LocalEndPoint { get; set; }

        private TcpClient Client { get; set; }
        private NetworkStream Stream { get; set; }

        private bool IsRunning { get; set; }
        private Thread _listenThread;

        ~ChatSession()
        {
            // 
            Dispose();
        }

        public ChatSession(ChatClientForm loginform)
        {
            _loginForm = loginform;
            RemoteEndPoint = new IPEndPoint(IPAddress.Loopback, 33777);
            LocalEndPoint = new IPEndPoint(IPAddress.Loopback, 0);

            _updateView = _loginForm.UpdateView;
            _newChatSession = _loginForm.NewChatSession;
            _switchToChat = _loginForm.SwitchToChat;

            IsRunning = true;
            var connectThread = new Thread(Start);
            connectThread.Start();
        }

        public void Dispose()
        {
            // Flag to shutdown ListenThread()
            IsRunning = false;
            //
            //ListenThread?.Abort();
            Stream?.Close();
            Client?.Close();
        }

        public void Start()
        {
            var task = _loginForm.BeginInvoke(_updateView, "Attempting to connect...", Color.Blue);
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

            _loginForm.BeginInvoke(_updateView, "Connection has been established.", Color.ForestGreen);
        }

        //Вход и автоматическая регистрация в случае нового юзера
        public void SignIn(string name, string login, string password)
        {
            if (Client == null) return;

            if (!IsConnected())
            {
                CrashAndReport(new Exception("Connection is lost. Attempting to reconnect..."));
                _loginForm.BeginInvoke(_newChatSession);
                Dispose();
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
                    _loginForm.BeginInvoke(_updateView, "Access granted!", Color.Blue);
                    Thread.Sleep(350);

                    var form = _loginForm.Invoke(_switchToChat);
                    _sessionForm = (ChatSessionForm) form;

                    _updateChat = _sessionForm.UpdateChat;
                    _updateOnlineList = _sessionForm.UpdateOnlineList;

                    _listenThread = new Thread(Listen);
                    _listenThread.Start();
                }

                else
                {
                    _loginForm.BeginInvoke(_updateView, "Wrong password or username", Color.Red);
                }
            }

            if (counter >= 250)
            {
                _loginForm.BeginInvoke(_updateView, "Message receive problem.", Color.Red);
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
                        _sessionForm.BeginInvoke(_updateChat, message as TextMessage);
                    }

                    if (message?.GetType() == typeof(OnlineList))
                    {
                        _sessionForm.BeginInvoke(_updateOnlineList, message as OnlineList);
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
                IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpConnections()
                    .Where(x => x.LocalEndPoint.Equals(Client.Client.LocalEndPoint));

            var currentConnection = activeConnections.SingleOrDefault(x =>
                x.LocalEndPoint.Equals(Client.Client.LocalEndPoint) &&
                x.RemoteEndPoint.Equals(Client.Client.RemoteEndPoint));

            //Строго !(item.State == TcpState.Established), TcpState.Closed работает совершенно иначе (как???)
            return currentConnection != null && currentConnection.State == TcpState.Established;
        }

        private void CrashAndReport(Exception exception)
        {
            if (_loginForm.Visible)
            {
                _loginForm.BeginInvoke(_updateView, exception.Message, Color.Red);
            }
            else
                _sessionForm?.BeginInvoke(_updateChat,
                    new TextMessage
                    {
                        From = "System",
                        Text = exception.Message,
                        TimeStamp = DateTime.Now.ToShortTimeString()
                    }
                );

            Thread.Sleep(3500);
            _sessionForm?.Close();
        }
    }
}