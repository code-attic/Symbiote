using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using ProtoBuf;

namespace Symbiote.Redis.Impl.Serialization
{
    public class ConditionalCacheSerializer: IConditionalCacheSerializer
    {
        protected ICacheSerializer CacheSerializer { get; set; }
        protected Dictionary<Type, Func<object, byte[]>> SerializeDict { get; set; }
        protected Dictionary<Type, Func< byte[], object>> DeserializeDict { get; set; }

        /// <summary>
        /// Initializes a new instance of the ConditionalProtobufCacheSerializer class.
        /// </summary>
        public ConditionalCacheSerializer(ICacheSerializer cacheSerializer)
        {
            CacheSerializer = cacheSerializer;
            SerializeDict = new Dictionary<Type, Func<object, byte[]>>();
            DeserializeDict = new Dictionary<Type, Func<byte[], object>>();
            ConfigureDicts(SerializeDict, DeserializeDict);
        }

        private void ConfigureDicts(Dictionary<Type, Func<object, byte[]>> pDict, Dictionary<Type, Func<byte[], object>> pDeDict)
        {
            //AddDefaultCondition(typeof(int));
            //AddDefaultCondition(typeof(string));

        }

        protected static  byte[] SerializeAsString(object value)
        {
            return Encoding.UTF8.GetBytes(value.ToString());
        }

        protected static object DeserializeAsString(byte[] value)
        {
            return Encoding.UTF8.GetString(value);
            
        }

        public byte[] Serialize<T>(T value)
        {
            //Func<object, byte[]> serializeit;
            //return (SerializeDict.TryGetValue( typeof( T ), out serializeit )? serializeit( value ): CacheSerializer.Serialize(value));

            return CacheSerializer.Serialize(value);
        }

        public T Deserialize<T>(byte[] bytes)
        {
            //Func<byte[], object> deserializeit;
            //return (DeserializeDict.TryGetValue(typeof(T), out deserializeit) ? (T)Convert.ChangeType(deserializeit(bytes), typeof(T)) : CacheSerializer.Deserialize<T>(bytes));

            return CacheSerializer.Deserialize<T>(bytes);
        }


        public void AddDefaultCondition(Type conditionalType)
        {
            AddSerializeCondition(conditionalType, SerializeAsString);
            AddDeserializeCondition(conditionalType, DeserializeAsString);
        }

        public void AddSerializeCondition( Type conditionalType, Func<object, byte[]> serialize )
        {
            SerializeDict.Add(conditionalType, serialize);
        }

        public void AddDeserializeCondition( Type conditionalType, Func<byte[], object> deserialize )
        {
            DeserializeDict.Add(conditionalType, deserialize);
        }

    }
}
