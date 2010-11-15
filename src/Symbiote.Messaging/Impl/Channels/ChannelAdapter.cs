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
using Symbiote.Messaging.Impl.Dispatch;

namespace Symbiote.Messaging.Impl.Channels
{
    public class ChannelAdapter
        : IChannelAdapter
    {
        protected IOpenChannel Channel { get; set; }

        public IChannel UnderlyingChannel { get { return Channel; } }

        public void ExpectReply<TReply>( object message, Action<IEnvelope> modifyEnvelope, IDispatcher dispatcher, Action<TReply> onReply )
        {
            Channel.ExpectReply( message, modifyEnvelope, dispatcher, onReply );
        }

        public void Send<TMessage>(object message)
        {
            Channel.Send( (TMessage) message);
        }

        public void Send<TMessage>(object message, Action<IEnvelope> modifyEnvelope)
        {
            Channel.Send((TMessage)message, modifyEnvelope);
        }

        public ChannelAdapter( IOpenChannel channel)
        {
            Channel = channel;
        }
    }

    public class ChannelAdapter<TMessage>
        : IChannelAdapter
    {
        protected IChannel<TMessage> Channel { get; set; }

        public IChannel UnderlyingChannel { get { return Channel; } }

        public void ExpectReply<TReply>(object message, Action<IEnvelope> modifyEnvelope, IDispatcher dispatcher, Action<TReply> onReply)
        {
            Channel.ExpectReply((TMessage)message, modifyEnvelope, dispatcher, onReply);
        }

        public void Send<T>(object message)
        {
            Channel.Send((TMessage)message);
        }

        public void Send<T>(object message, Action<IEnvelope> modifyEnvelope)
        {
            Channel.Send((TMessage)message, modifyEnvelope);
        }

        public ChannelAdapter(IChannel<TMessage> channel)
        {
            Channel = channel;
        }
    }
}
