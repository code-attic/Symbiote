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
using System.IO;
using System.IO.Pipes;
using System.Text;
using Symbiote.Core.Futures;
using Symbiote.Core.Serialization;
using Symbiote.Core.Utility;
using Symbiote.Messaging.Impl.Dispatch;
using Symbiote.Messaging.Impl.Envelope;
using Symbiote.Messaging.Impl.Serialization;
using System.Linq;

namespace Symbiote.Messaging.Impl.Channels.Pipe
{
    public class PipeProxy
    {
        public NamedPipeChannelDefinition Definition { get; set; }
        public IDispatcher Dispatcher { get; set; }
        public IPipeEndpoint Pipe { get; set; }
        public PipeEndpointFactory PipeFactory { get; set; }
        public RingBuffer RingBuffer { get; set; }
        public bool Running { get; set; }
        public PipeStream Stream { get { return Pipe.Stream; } }
        public IMessageSerializer Serializer { get; set; }

        public void Connect()
        {
            Pipe.Connect(Listen, () => { Running = false; });
        }

        public object DispatchResult(object translatedEnvelope)
        {
            var envelope = translatedEnvelope as IEnvelope;
            if (envelope != null)
            {
                Dispatcher.Send(envelope);
            }
            else
            {
                HandlePoisonMessage(translatedEnvelope);
            }
            return envelope;
        }

        public object DeserializeMessage(object value)
        {
            var buffer = (byte[])value;
            var typeHeaderLength = BitConverter.ToInt32( buffer.Take( 4 ).ToArray(), 0 );
            var typeHeader = Encoding.UTF8.GetString( buffer, 4, typeHeaderLength );
            var messageType = Type.GetType( typeHeader );
            var envelopeType = typeof( NamedPipeEnvelope<> ).MakeGenericType( messageType );

            NamedPipeEnvelope pipeEnvelope = null;
            try
            {
                pipeEnvelope = Serializer.Deserialize( envelopeType, buffer.Skip(4 + typeHeaderLength).ToArray() ) as NamedPipeEnvelope;
                pipeEnvelope.ReplyStream = this;
                if ( pipeEnvelope.Message == null )
                    HandlePoisonMessage( value );
            }
            catch ( Exception e )
            {
                HandlePoisonMessage( value );
            }
            return pipeEnvelope;
        }

        public void HandlePoisonMessage(object message)
        {
            if (message != null)
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
            ReadAsync(Stream, 0, x =>
            {
                RingBuffer.Write(x);
            });
        }

        public void ReadAsync(PipeStream stream, int timeout, Action<byte[]> onComplete)
        {
            var memoryStream = new MemoryStream();
            var buffer = new byte[8 * 1024];
            var read = 0;
            if (stream.CanTimeout)
                stream.ReadTimeout = timeout;

            Future.Of(
                x => stream.BeginRead(buffer, 0, buffer.Length, x, null),
                x =>
                {
                    read = stream.EndRead(x);
                    if (read > 0)
                    {
                        memoryStream.Write(buffer, 0, read);
                    }

                    if (stream.IsMessageComplete && memoryStream.Length > 0)
                    {
                        onComplete(memoryStream.ToArray());
                        memoryStream.Close();
                        memoryStream.Dispose();
                        memoryStream = new MemoryStream();
                    }
                    return read;
                })
                .LoopWhile(() => Running)
                .Start();
        }

        public void Send(NamedPipeEnvelope envelope)
        {
            var envelopeBuffer = Serializer.Serialize( envelope );
            var header = Encoding.UTF8.GetBytes( envelope.MessageType.AssemblyQualifiedName );
            var buffer = BitConverter
                .GetBytes( header.Length )
                .Concat( header )
                .Concat( envelopeBuffer )
                .ToArray();

            Future.Of(
                x => Pipe.Stream.BeginWrite( buffer, 0, buffer.Length, x, null ),
                x =>
                    {
                        Pipe.Stream.EndWrite( x );
                        Pipe.Stream.Flush();
                        Pipe.Stream.WaitForPipeDrain();
                        return true;
                    }
                ).Start();
        }

        public PipeProxy( NamedPipeChannelDefinition definition, IDispatcher dispatch, IMessageSerializer messageSerializer )
        {
            Definition = definition;
            Dispatcher = dispatch;
            Serializer = messageSerializer;
            PipeFactory = new PipeEndpointFactory();
            Pipe = PipeFactory.CreateEndpointForChannel( definition );
            
            RingBuffer = new RingBuffer( 1000 );
            RingBuffer.AddTransform( DeserializeMessage );
            RingBuffer.AddTransform( DispatchResult );
            RingBuffer.Start();
        }
    }
}
