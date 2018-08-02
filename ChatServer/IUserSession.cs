using DataPackaging;

namespace ChatServer
{
    internal interface IUserSession
    {
        void Start();
        void End();

        void SendMessage(IBinarySerializable message);
        object ReceiveMessage();
        
        string PrintInfo();
    }

}
