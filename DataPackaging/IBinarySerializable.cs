using System.IO;

namespace DataPackaging
{
    public interface IBinarySerializable
    {
        void SerializeBytes(Stream stream);
    }
}
