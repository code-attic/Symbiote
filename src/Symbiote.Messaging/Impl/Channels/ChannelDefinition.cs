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
    public class ChannelDefinition<TChannel, TMessage>
        : IChannelDefinition
        where TChannel : IChannel<TMessage>
        where TMessage : class
    {
        public string Name { get; set; }
        public Type ChannelType { get { return typeof(TChannel); } }
        public Type MessageType { get { return typeof (TMessage); } }
        public Type FactoryType { get { return typeof(ChannelFactory); } }

        public ChannelDefinition()
            : this("default")
        {
        }

        public ChannelDefinition(string name)
        {
            Name = name;
        }
    }



    public class ChannelDefinition
        : IChannelDefinition
    {
        public string Name { get; set; }

        public Type ChannelType { get { return BaseChannelType.MakeGenericType(MessageType); } }
        public Type MessageType { get; private set; }
        protected Type BaseChannelType { get; private set; }
        public Type FactoryType { get { return typeof(ChannelFactory); } }

        public ChannelDefinition(Type channelType, Type messageType)
            : this(channelType, messageType, "default")
        {

        }

        public ChannelDefinition(Type channelType, Type messageType, string name)
        {
            BaseChannelType = channelType;
            MessageType = messageType;
            Name = name;
        }
    }
}