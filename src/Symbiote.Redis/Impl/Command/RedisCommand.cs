using System;
using Microsoft.Practices.ServiceLocation;
using Symbiote.Redis.Impl.Connection;
using Symbiote.Redis.Impl.Serialization;

namespace Symbiote.Redis.Impl.Command
{
    public abstract class RedisCommand<TReturn>
    {
        protected ICacheSerializer Serializer { get; set; }
        public Func<IRedisConnection, TReturn> Command { get; protected set; }

        public byte[] Serialize<T>(T value)
        {
            return Serializer.Serialize(value);
        }

        public T Deserialize<T>(byte[] bytes)
        {
            return Serializer.Deserialize<T>(bytes);
        }

        public TReturn Execute()
        {
            using(var handle = ConnectionHandle.Acquire())
            {
                return Command(handle.Connection);
            }
        }

        protected RedisCommand()
        {
            Serializer = ServiceLocator.Current.GetInstance<ICacheSerializer>();
        }
    }
}