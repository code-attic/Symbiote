using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
