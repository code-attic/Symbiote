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
using System.Text;
using Symbiote.Core;
using Symbiote.Core.Futures;
using Symbiote.Core.Utility;
using Symbiote.Messaging.Impl.Dispatch;
using Symbiote.Messaging.Impl.Endpoint;
using Symbiote.Messaging.Impl.Envelope;
using Symbiote.Messaging.Impl.Serialization;
using Symbiote.Core.Serialization;

namespace Symbiote.Messaging.Impl.Adapter
{
    public class NamedPipeListener
    {
        public IDispatcher Dispatch { get; set; }
        public NamedPipeEndpoint Endpoint { get; set; }
        public RingBuffer RingBuffer { get; set; }
        public IMessageSerializer Serializer { get; set; }
        public bool Running { get; set; }

        public NamedPipeEnvelope GetEnvelope( NamedPipeTransportEnvelope transportEnvelope )
        {
            var messageTypeName = transportEnvelope.MessageType;
            var messageType = Type.GetType( messageTypeName );
            var envelopeType = typeof( NamedPipeEnvelope<> ).MakeGenericType( messageType );
            var envelope = Activator.CreateInstance( envelopeType ) as NamedPipeEnvelope;
            envelope.ReplyStream = Endpoint.Stream;
            envelope.MessageId = transportEnvelope.MessageId;
            envelope.CorrelationId = transportEnvelope.CorrelationId;
            envelope.Position = transportEnvelope.Position;
            envelope.RoutingKey = transportEnvelope.RoutingKey;
            envelope.Sequence = transportEnvelope.Sequence;
            envelope.SequenceEnd = transportEnvelope.SequenceEnd;
            return envelope;
        }

        public object DispatchResult( object translatedEnvelope )
        {
            var envelope = translatedEnvelope as IEnvelope;
            if ( envelope != null )
            {
                Dispatch.Send( envelope );
            }
            else
            {
                HandlePoisonMessage( translatedEnvelope );
            }
            return envelope;
        }

        public object DeserializeMessage( object value )
        {
            var buffer = (byte[]) value;
            NamedPipeEnvelope pipeEnvelope = null;
            NamedPipeTransportEnvelope transportEnvelope = null;
            try
            {
                transportEnvelope = buffer.FromProtocolBuffer<NamedPipeTransportEnvelope>();
                pipeEnvelope = GetEnvelope( transportEnvelope );
                pipeEnvelope.Message = Serializer.Deserialize( pipeEnvelope.MessageType, transportEnvelope.Message );
                if ( pipeEnvelope.Message == null )
                    HandlePoisonMessage( transportEnvelope );
            }
            catch ( Exception e )
            {
                HandlePoisonMessage( transportEnvelope );
            }
            return pipeEnvelope;
        }

        public void HandlePoisonMessage( object message )
        {
            var envelope = message as NamedPipeTransportEnvelope;
            if ( envelope != null )
            {
                //"Received bad message from \r\n\t Exchange: {0} \r\n\t Queue: {1} \r\n\t MessageId: {2} \r\n\t CorrelationId: {3} \r\n\t Type: {4}"
                //    .ToInfo<RabbitQueueListener>(
                //        envelope.Exchange,
                //        Proxy.QueueName,
                //        envelope.MessageId,
                //        envelope.CorrelationId,
                //        envelope.MessageType
                //    );
            }
            else
            {
                //"Received garbage from queue {0}. No deserialization was possible."
                //    .ToInfo<RabbitQueueListener>( Proxy.QueueName );
            }
        }

        public void Listen()
        {
            ReadAsync(Endpoint.Stream, 0, x =>
                                             {
                                                 RingBuffer.Write( x );
                                                 if(Running)
                                                     Listen();
                                             } );
        }

        public void ReadAsync(NamedPipeServerStream stream, int timeout, Action<byte[]> onComplete)
        {
            var memoryStream = new MemoryStream();
            var buffer = new byte[8 * 1024];
            var read = 0;
            if (stream.CanTimeout)
                stream.ReadTimeout = timeout;

            Future.Of(
                x => stream.BeginRead( buffer, 0, buffer.Length, x, null ),
                x =>
                {
                    read = stream.EndRead( x );
                    if ( read > 0 )
                    {
                        memoryStream.Write( buffer, 0, read );
                    }

                    if ( stream.IsMessageComplete )
                    {
                        onComplete( memoryStream.ToArray() );
                        memoryStream.Close();
                        memoryStream.Dispose();
                    }
                    return read;
                })
                .LoopWhile( () => !stream.IsMessageComplete )
                .Start();
        }

        public void WaitForConnection()
        {
            Future.Of(
                x => 
                    Endpoint.Stream.BeginWaitForConnection( x, null ),
                x =>
                    {
                        Endpoint.Stream.EndWaitForConnection( x );
                        return true;
                    } )
                    .OnValue( x =>
                                  {
                                      if(x)
                                          Listen();
                                      else
                                          WaitForConnection();
                                  } )
                    .OnFailure( () =>
                                    {
                                        WaitForConnection();
                                        return true;
                                    })
                .Start();
        }

        public NamedPipeListener( NamedPipeEndpoint endpoint, IDispatcher dispatch )
        {
            Serializer = Assimilate.GetInstanceOf(endpoint.SerializerType) as IMessageSerializer;
            RingBuffer = new RingBuffer(10000);
            RingBuffer.AddTransform(DeserializeMessage);
            RingBuffer.AddTransform(DispatchResult);
            RingBuffer.Start();
            Running = true;
            Dispatch = dispatch;
            Endpoint = endpoint;
            Endpoint.OpenStream();
            WaitForConnection();
        }
    }
}