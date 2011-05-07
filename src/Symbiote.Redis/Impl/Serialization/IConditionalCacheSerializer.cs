using System;

namespace Symbiote.Redis.Impl.Serialization
{
    public interface IConditionalCacheSerializer : ICacheSerializer
    {
        void AddDefaultCondition(Type conditionalType);
        void AddSerializeCondition(Type conditionalType, Func<object, byte[]> serialize);
        void AddDeserializeCondition(Type conditionalType, Func<byte[], object> deserialize);
    }
}
