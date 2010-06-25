using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Symbiote.Jackalope.Impl.Serialization
{
    public class NetBinarySerializer : IMessageSerializer
    {
        public T Deserialize<T>(byte[] message)
            where T : class
        {
            var formatter = new BinaryFormatter();
            var stream = new MemoryStream(message);
            var body = formatter.Deserialize(stream) as T;
            return body;
        }

        public object Deserialize(byte[] message)
        {
            var formatter = new BinaryFormatter();
            var stream = new MemoryStream(message);
            var body = formatter.Deserialize(stream);
            return body;
        }

        public byte[] Serialize<T>(T body)
            where T : class
        {
            var formatter = new BinaryFormatter();
            var stream = new MemoryStream();
            formatter.Serialize(stream, body);
            return stream.ToArray();
        }
    }
}