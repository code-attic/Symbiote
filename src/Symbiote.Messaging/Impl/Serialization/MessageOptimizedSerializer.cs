/* 
Copyright 2008-2010 Alex Robson

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Collections.Concurrent;

namespace Symbiote.Messaging.Impl.Serialization
{
    public class MessageOptimizedSerializer
        : IMessageSerializer
    {
        protected ConcurrentDictionary<Type, IMessageSerializer> SerializationProviders { get; set; }

        public IMessageSerializer GetSerialzier(Type messageType)
        {
            IMessageSerializer serializer = null;
            if (!SerializationProviders.TryGetValue(messageType, out serializer))
            {
                var serializerType = MessageSerializer.GetBestMessageSerializer(messageType);
                serializer = Activator.CreateInstance(serializerType) as IMessageSerializer;
                SerializationProviders.TryAdd(messageType, serializer);
            }
            return serializer;
        }

        public T Deserialize<T>(byte[] message)
        {
            return GetSerialzier(typeof(T)).Deserialize<T>(message);
        }

        public object Deserialize(byte[] message)
        {
            return null;
        }

        public object Deserialize(Type messageType, byte[] message)
        {
            return GetSerialzier(messageType).Deserialize(messageType, message);
        }

        public byte[] Serialize<T>(T body)
        {
            return GetSerialzier(typeof(T)).Serialize(body);
        }
    }
}
