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

namespace Symbiote.Messaging.Impl.Channels
{
    public class LocalChannelDefiniton<TMessage>
        : IChannelDefinition
    {
        public string Name { get; set; }
        public Type ChannelType { get { return typeof(LocalChannel<>).MakeGenericType(MessageType); } }
        public Type MessageType { get { return typeof (TMessage); } }
        public Type FactoryType { get { return typeof (ChannelFactory); } }

        public LocalChannelDefiniton() : this("default")
        {
            
        }

        public LocalChannelDefiniton(string name)
        {
            Name = name;
        }
    }

    public class LocalChannelDefiniton
        : IChannelDefinition
    {
        public string Name { get; set; }

        public Type ChannelType { get { return typeof(LocalChannel<>).MakeGenericType(MessageType); } }
        public Type MessageType { get; private set; }
        public Type FactoryType { get { return typeof(ChannelFactory); } }

        public LocalChannelDefiniton(Type messageType)
            : this(messageType, "default")
        {

        }

        public LocalChannelDefiniton(Type messageType, string name)
        {
            MessageType = messageType;
            Name = name;
        }
    }
}