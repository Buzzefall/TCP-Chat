using DataPackaging;
using DataPackaging.Interfaces;

namespace ChatServer
{
    internal interface IUserSession
    {
        void Start();
        void Dispose();

        void SendMessage(IBinarySerializable message);
        object ReceiveMessage();
        
        string PrintInfo();
    }

}
