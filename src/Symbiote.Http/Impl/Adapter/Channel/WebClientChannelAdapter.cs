using System;
using System.Net;
using System.Net.Security;
using System.Text;
using Symbiote.Core.Extensions;
using Symbiote.Core.Impl.Futures;
using Symbiote.Core.Impl.Serialization;
using Symbiote.Messaging;
using JsonExtensions = Symbiote.Core.Impl.Serialization.JsonExtensions;

namespace Symbiote.Http.Impl.Adapter.Channel
{
    public class WebClientChannelAdapter
        : IChannelAdapter
    {
        public WebRequest Client { get; set; }
        public ChannelDefinition Definition { get; set; }

        public string Name { get; set; }

        public Future<TReply> ExpectReply<TReply, TMessage>( TMessage message )
        {
            return ExpectReply<TReply, TMessage>( message, x => { } );
        }

        public Future<TReply> ExpectReply<TReply, TMessage>( TMessage message, Action<IEnvelope> modifyEnvelope )
        {
            var envelope = new HttpEnvelope<TMessage>( message )
            {
                CorrelationId = Definition.GetCorrelationId( message ),
                RoutingKey = Definition.GetRoutingKey( message ),
            };

            modifyEnvelope( envelope );
            return Future.Of<TReply>( x => SendEnvelope( envelope, x ) );
        }

        public void Send<TMessage>( TMessage message )
        {
            Send( message, x => { } );
        }

        public void Send<TMessage>( TMessage message, Action<IEnvelope> modifyEnvelope )
        {
            var envelope = new HttpEnvelope<TMessage>(message)
            {
                CorrelationId = Definition.GetCorrelationId(message),
                RoutingKey = Definition.GetRoutingKey(message),
            };
            SendEnvelope<TMessage, object>( envelope, null );
        }

        /// <summary>
        /// TODO: Implement this with asynchronous methods instead of blocking calls
        /// </summary>
        public void SendEnvelope<TMessage, TReply>(HttpEnvelope<TMessage> envelope, Action<TReply> callback)
        {
            string requestUriString = Definition.GetUriForEnvelope( envelope );
            Client = WebRequest.Create( requestUriString );
            Client.Method = Definition.Verb;
            Client.Credentials = Definition.Credentials;
            Client.AuthenticationLevel = AuthenticationLevel.None;
            
            Client.Headers.Add( "Content-Encoding", Definition.ContentEncoding );
            Client.ContentType = typeof(TMessage).AssemblyQualifiedName;

            var requestStream = Client.GetRequestStream();
            byte[] buffer = Definition.Serializer(envelope);
            requestStream.Write( buffer, 0, buffer.Length );
            requestStream.Close();

            var response = Client.GetResponse();
            var contentType = response.Headers["Content-Type"];
            var encoding = response.Headers["Content-Encoding"];
            var type = Type.GetType(contentType);
            object result;
            byte[] readBuffer;
            using( var stream = response.GetResponseStream() )
            {
                readBuffer = stream.ReadToEnd( 1000 );
            }

            switch (encoding.Trim())
            {
                case "application/json":
                    result = JsonExtensions.FromJson( Encoding.UTF8.GetString(readBuffer), type);
                    break;
                case "application/protocol-buffer":
                    result = readBuffer.FromProtocolBuffer(type);
                    break;
                default:
                    result = null;
                    break;
            }
            if (callback != null)
                callback( (TReply) result );
        }

        public WebClientChannelAdapter(ChannelDefinition definition)
        {
            Definition = definition;
        }
    }
}