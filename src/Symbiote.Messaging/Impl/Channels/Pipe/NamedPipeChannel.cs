// /* 
// Copyright 2008-2011 Alex Robson
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// */
using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using Symbiote.Core.Futures;
using Symbiote.Messaging.Impl.Envelope;
using Symbiote.Messaging.Impl.Serialization;
using Symbiote.Core.Serialization;

namespace Symbiote.Messaging.Impl.Channels.Pipe
{
    public class NamedPipeChannel
        : IChannel
    {
        public NamedPipeChannelDefinition Definition { get; set; }
        public PipeStream Pipe { get; set; }
        public MessageOptimizedSerializer SerializationProvider { get; set; }

        public string Name { get; set; }

        public Future<TReply> ExpectReply<TReply, TMessage>( TMessage message )
        {
            return ExpectReply<TReply, TMessage>( message, x => { } );
        }

        public Future<TReply> ExpectReply<TReply, TMessage>( TMessage message, Action<IEnvelope> modifyEnvelope )
        {
            var envelope = new NamedPipeTransportEnvelope( )
                               {
                                   CorrelationId = Definition.GetCorrelationId( message ),
                                   RoutingKey = Definition.GetRoutingKey( message ),
                                   Message = SerializationProvider.Serialize( message ),
                                   MessageType = typeof(TMessage).AssemblyQualifiedName
                               };

            //modifyEnvelope( envelope );
            var envelopeBuffer = envelope.ToProtocolBuffer();
            Pipe.Write( envelopeBuffer, 0, envelopeBuffer.Length );
            Pipe.Flush();
            Pipe.WaitForPipeDrain();
            return Future.Of( GetReply<TReply> );
        }

        public void Send<TMessage>( TMessage message )
        {
            Send( message, x => { } );
        }

        public void Send<TMessage>( TMessage message, Action<IEnvelope> modifyEnvelope )
        {
            var envelope = new NamedPipeTransportEnvelope( )
                               {
                                   CorrelationId = Definition.GetCorrelationId( message ),
                                   RoutingKey = Definition.GetRoutingKey( message ),
                                   Message = SerializationProvider.Serialize(message),
                                   MessageType = typeof(TMessage).AssemblyQualifiedName
                               };
            //modifyEnvelope( envelope );
            var envelopeBuffer = envelope.ToProtocolBuffer();
            Pipe.Write( envelopeBuffer, 0, envelopeBuffer.Length );
            Pipe.Flush();
        }

        public IEnvelope GetEnvelope(NamedPipeTransportEnvelope transportEnvelope)
        {
            var messageTypeName = transportEnvelope.MessageType;
            var messageType = Type.GetType(messageTypeName);
            var envelopeType = typeof(NamedPipeEnvelope<>).MakeGenericType(messageType);
            return Activator.CreateInstance(envelopeType) as IEnvelope;
        }

        public TReply GetReply<TReply>()
        {
            byte[] buffer = ReadResponse( Pipe, 1000 );

            try
            {
                var transportEnvelope = buffer.FromProtocolBuffer<NamedPipeTransportEnvelope>();
                var pipeEnvelope = GetEnvelope(transportEnvelope);
                return (TReply) SerializationProvider.Deserialize(pipeEnvelope.MessageType, transportEnvelope.Message);
            }
            catch (Exception e)
            {
                
            }
            return default(TReply);
        }

        public byte[] ReadResponse(PipeStream stream, int timeout)
        {
            var buffer = new byte[8 * 1024];
            var read = 0;
            if ( stream.CanTimeout )
                stream.ReadTimeout = timeout;
            
            using(var memoryStream = new MemoryStream())
            {
                do
                {
                    read = stream.Read( buffer, 0, 0 );
                    if ( read > 0 )
                        memoryStream.Write( buffer, 0, buffer.Length );
                } while ( !stream.IsMessageComplete || memoryStream.Length == 0 );
                return memoryStream.ToArray();
            }
        }

        public void WaitForMessage(NamedPipeClientStream stream)
        {
            while (stream.IsMessageComplete)
            {
                Thread.Sleep(5);
            }
        }

        public void ConfigurePipe()
        {
            Pipe = new NamedPipeClientStream(
                Definition.Machine,
                Definition.Name,
                Definition.Direction,
                Definition.Options,
                Definition.Impersonation);
            Pipe.Connect( Definition.ConnectionTimeout );
            Pipe.ReadMode = Definition.Mode;
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