using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Core.Extensions;

namespace Symbiote.Jackalope.Impl
{
    public class JsonMessageSerializer : IMessageSerializer
    {
        private NetBinarySerializer _netBinarySerializer;

        public T Deserialize<T>(byte[] message)
            where T : class
        {
            var json = _netBinarySerializer.Deserialize<string>(message);
            return json.FromJson<T>();
        }

        public object Deserialize(byte[] message)
        {
            var json = _netBinarySerializer.Deserialize<string>(message);
            return json.FromJson();
        }

        public byte[] Serialize<T>(T body)
            where T : class
        {
            var json = body.ToJson();
            return _netBinarySerializer.Serialize(json);
        }

        public JsonMessageSerializer()
        {
            _netBinarySerializer = new NetBinarySerializer();
        }
    }

}
