using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Security.Principal;
using System.Text;
using Symbiote.Core.Impl.Futures;
using Symbiote.Messaging.Impl.Envelope;
using Symbiote.Messaging.Impl.Serialization;
using Symbiote.Messaging.Impl.Transform;
using Symbiote.Core.Extensions;

namespace Symbiote.Messaging.Impl.Channels
{
    public class NamePipeConfigurator
    {
        public NamedPipeChannelDefinition Defintion { get; set; }

        public NamePipeConfigurator Server(string server)
        {
            Defintion.Server = server;
            return this;
        }

        public NamePipeConfigurator Direction(PipeDirection direction)
        {
            Defintion.Direction = direction;
            return this;
        }

        public NamePipeConfigurator Rights(PipeAccessRights rights)
        {
            Defintion.Rights = rights;
            return this;
        }

        public NamePipeConfigurator Mode(PipeTransmissionMode mode)
        {
            Defintion.Mode = mode;
            return this;
        }

        public NamePipeConfigurator Options(PipeOptions options)
        {
            Defintion.Options = options;
            return this;
        }

        public NamePipeConfigurator Impersonation(TokenImpersonationLevel impersonationLevel)
        {
            Defintion.Impersonation = impersonationLevel;
            return this;
        }

        public NamePipeConfigurator ConnectTimeout(int msTimeout)
        {
            Defintion.ConnectionTimeout = msTimeout;
            return this;
        }

        public NamePipeConfigurator( string channelName )
        {
            Defintion = new NamedPipeChannelDefinition( channelName );
        }
    }

    public class NamedPipeChannelDefinition : BaseChannelDefinition
    {
        public string Server { get; set; }
        public PipeDirection Direction { get; set; }
        public PipeAccessRights Rights { get; set; }
        public PipeTransmissionMode Mode { get; set; }
        public PipeOptions Options { get; set; }
        public TokenImpersonationLevel Impersonation { get; set; }
        public int ConnectionTimeout { get; set; }
        public override Type ChannelType { get { return typeof(NamedPipeChannel); } }
        public override Type FactoryType { get { return typeof(NamedPipeChannelFactory); } }

        public NamedPipeChannelDefinition(string name)
        {
            Name = name;
            Server = "Symbiote.Pipes.Host";
            Direction = PipeDirection.InOut;
            Rights = PipeAccessRights.FullControl;
            Mode = PipeTransmissionMode.Message;
            Options = PipeOptions.Asynchronous;
            Impersonation = TokenImpersonationLevel.Anonymous;
            ConnectionTimeout = 1000;
        }
    }

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

    public class NamedPipeChannelFactory
        : IChannelFactory
    {
        public IChannel CreateChannel( IChannelDefinition definition )
        {
            return new NamedPipeChannel( definition as NamedPipeChannelDefinition );
        }
    }

    
}
