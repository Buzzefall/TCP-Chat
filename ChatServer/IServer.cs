namespace ChatServer
{
    internal interface IServer
    {
        void Start();
        void Shutdown();

        void Broadcast(string text, UserSession sender = null);

        void ConnectUser(UserSession user);
        void DisconnectUser(UserSession user);
    }
}
