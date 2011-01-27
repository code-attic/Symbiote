using System;
using System.Net;
using Symbiote.Core.Impl.Futures;
using Symbiote.Messaging;
using Symbiote.Messaging.Impl.Channels;

namespace Symbiote.Http.Impl.Adapter.Channel
{
    public class ChannelDefinition
        : BaseChannelDefinition
    {
        public UriBuilder UriBuilder { get; set; }
        public Uri RequestUri { get { return UriBuilder.Uri; } }
        public string BaseUri { get; set; }

        public ChannelDefinition()
        {
            UriBuilder = new UriBuilder();
        }
    }

    public interface IChannelAdapter
        : IChannel
    {
        
    }

    public class HttpEnvelope
    {

        public Type MessageType { get; set; }
    }

    public class HttpEnvelope<TMessage> :
        HttpEnvelope,
        IEnvelope<TMessage>
    {
        public TMessage Message
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
        public void Reply<TResponse>( TResponse response )
        {
            throw new NotImplementedException();
        }

        public string CorrelationId
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
        public string RoutingKey
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
        public long Sequence
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
        public long Position
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
        public bool SequenceEnd
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
        public Guid MessageId
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
    }

    public class WebClientChannelAdapter
        : IChannelAdapter
    {
        public WebRequest Client { get; set; }

        public string Name { get; set; }

        public Future<TReply> ExpectReply<TReply, TMessage>( TMessage message )
        {
            return ExpectReply<TReply, TMessage>( message, x => { } );
        }

        public Future<TReply> ExpectReply<TReply, TMessage>( TMessage message, Action<IEnvelope> modifyEnvelope )
        {
            throw new NotImplementedException();
        }

        public void Send<TMessage>( TMessage message )
        {
            throw new NotImplementedException();
        }

        public void Send<TMessage>( TMessage message, Action<IEnvelope> modifyEnvelope )
        {
            throw new NotImplementedException();
        }

        public WebClientChannelAdapter(ChannelDefinition definition)
        {
//            Client = WebRequest.Create( definition.BaseUrl );
        }
    }

    public class HttpChannel :
        IChannel
    {
        public string Name { get; set; }
        public IChannelAdapter Adapter { get; set; }

        public Future<TReply> ExpectReply<TReply, TMessage>(TMessage message)
        {
            throw new NotImplementedException();
        }

        public Future<TReply> ExpectReply<TReply, TMessage>(TMessage message, Action<IEnvelope> modifyEnvelope)
        {
            throw new NotImplementedException();
        }

        public void Send<TMessage>(TMessage message)
        {
            throw new NotImplementedException();
        }

        public void Send<TMessage>(TMessage message, Action<IEnvelope> modifyEnvelope)
        {
            throw new NotImplementedException();
        }
    }

    //public class ChannelFactory
    //    : IChannelFactory
    //{
    //    public IChannel CreateChannel( IChannelDefinition definition )
    //    {
            
    //    }
    //}
}
