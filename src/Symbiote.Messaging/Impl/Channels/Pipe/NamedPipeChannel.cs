using System;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using Symbiote.Core.Extensions;
using Symbiote.Core.Futures;
using Symbiote.Messaging.Impl.Envelope;
using Symbiote.Messaging.Impl.Serialization;

namespace Symbiote.Messaging.Impl.Channels
{
    public class NamedPipeChannel
        : IChannel
    {
        public NamedPipeChannelDefinition Definition { get; set; }
        public string Name { get; set; }
        public NamedPipeClientStream ClientStream { get; set; }
        public MessageOptimizedSerializer SerializationProvider { get; set; }

        public Future<TReply> ExpectReply<TReply, TMessage>( TMessage message )
        {
            return ExpectReply<TReply, TMessage>(message, x => { });
        }

        public Future<TReply> ExpectReply<TReply, TMessage>( TMessage message, Action<IEnvelope> modifyEnvelope )
        {
            var envelope = new Envelope<TMessage>(message)
                               {
                                   CorrelationId = Definition.GetCorrelationId(message),
                                   RoutingKey = Definition.GetRoutingKey(message)
                               };
            modifyEnvelope(envelope);
            var envelopeBuffer = SerializeEnvelope(envelope);
            ClientStream.Write(envelopeBuffer, 0, envelopeBuffer.Length);
            ClientStream.Flush();

            return Future.Of(GetReply<TReply>);
        }

        public TReply GetReply<TReply>() 
        {
            var readBuffer = ClientStream.ReadToEnd( 1000 );
            var typeHeaderLength = BitConverter.ToInt32( readBuffer, 0 );
            var typeHeader = BitConverter.ToString( readBuffer, 4, typeHeaderLength );
            var type = Type.GetType( typeHeader );
            var envelopeType = typeof(Envelope<>).MakeGenericType( type );
            var envelope = SerializationProvider.Deserialize( envelopeType, readBuffer.Skip( 4 + typeHeaderLength ).ToArray() );
            return ( envelope as IEnvelope<TReply> ).Message;
        }

        public void Send<TMessage>( TMessage message )
        {
            Send( message, x => { } );
        }

        public void Send<TMessage>( TMessage message, Action<IEnvelope> modifyEnvelope )
        {
            var envelope = new Envelope<TMessage>( message )
                               {
                                   CorrelationId = Definition.GetCorrelationId( message ),
                                   RoutingKey = Definition.GetRoutingKey( message )
                               };
            modifyEnvelope( envelope );
            var envelopeBuffer = SerializeEnvelope( envelope );
            ClientStream.Write( envelopeBuffer, 0, envelopeBuffer.Length );
            ClientStream.Flush();
        }

        public byte[] SerializeEnvelope<TMessage>(Envelope<TMessage> envelope)
        {
            var messageType = typeof(TMessage);
            var typeName = messageType.AssemblyQualifiedName;
            var typeBuffer = Encoding.UTF8.GetBytes( typeName );
            var headerLength = typeBuffer.Length;
            var messageBuffer = SerializationProvider.Serialize( envelope );
            int messageLength = messageBuffer.Length;
            using(var stream = new MemoryStream(4 + headerLength + messageLength))
            {
                stream.Write( BitConverter.GetBytes( headerLength ), 0, 4 );
                stream.Write( typeBuffer, 0, headerLength );
                stream.Write( messageBuffer, 0, messageLength );
                return stream.ToArray();
            }
        }

        public void ConfigurePipe()
        {
            ClientStream = new NamedPipeClientStream( 
                Definition.Name, 
                Definition.Server, 
                Definition.Direction, 
                Definition.Options );
            ClientStream.ReadMode = Definition.Mode;
            ClientStream.Connect( Definition.ConnectionTimeout );
        }

        public NamedPipeChannel( NamedPipeChannelDefinition definition )
        {
            Definition = definition;
            Name = definition.Name;
            ConfigurePipe();
            SerializationProvider = new MessageOptimizedSerializer();
        }
    }
}