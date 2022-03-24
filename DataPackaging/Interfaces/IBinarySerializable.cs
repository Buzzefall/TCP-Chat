using System.IO;

namespace DataPackaging.Interfaces
{
    public interface IBinarySerializable
    {
        void SerializeBytes(Stream stream);
    }
}
