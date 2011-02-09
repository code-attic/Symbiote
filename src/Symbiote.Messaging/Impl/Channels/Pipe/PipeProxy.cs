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
using Symbiote.Core.Futures;
using Symbiote.Core.Serialization;
using Symbiote.Core.Utility;
using Symbiote.Messaging.Impl.Dispatch;
using Symbiote.Messaging.Impl.Envelope;
using Symbiote.Messaging.Impl.Serialization;

namespace Symbiote.Messaging.Impl.Channels.Pipe
{
    public interface IPipeEndpoint
        : IDisposable
    {
        PipeStream Stream { get; }
        bool Connected { get; set; }
        void Connect( Action onConnect, Action onFailure );
        void Close();
    }

    public class ServerPipeEndpoint
        : IPipeEndpoint
    {
        public NamedPipeServerStream Server { get; set; }
        public bool Running { get; set; }

        public PipeStream Stream { get { return Server; } }

        public bool Connected { get; set; }

        public void Connect( Action onConnect, Action onFailure )
        {
            Future.Of(
                x =>
                    Server.BeginWaitForConnection(x, null),
                x =>
                {
                    Server.EndWaitForConnection(x);
                    return true;
                })
                .OnValue(x =>
                {
                    if (x)
                    {
                        Connected = true;
                        Running = true;
                        onConnect();
                    }
                    else
                        Connect( onConnect, onFailure );
                })
                .OnFailure(() =>
                {
                    onFailure();
                    return true;
                })
                .Start()
                .LoopWhile( () => Running );
        }

        public void Close()
        {
            Connected = false;
            Running = false;
            Server.Close();
        }

        public ServerPipeEndpoint( NamedPipeServerStream serverStream )
        {
            Server = serverStream;
        }

        public void Dispose()
        {
            if(Connected)
                Close();
        }
    }

    public class ClientPipeEndpoint
        : IPipeEndpoint
    {
        public NamedPipeChannelDefinition Definition { get; set; }
        public NamedPipeClientStream Client { get; set; }
        public bool Running { get; set; }

        public PipeStream Stream { get { return Client; } }

        public bool Connected { get; set; }

        public void Connect( Action onConnect, Action onFailure )
        {
            Client = new NamedPipeClientStream(
                    Definition.Machine,
                    Definition.Name,
                    Definition.Direction,
                    Definition.Options,
                    Definition.Impersonation);

            try
            {
                Client.Connect(Definition.ConnectionTimeout);
                Client.ReadMode = Definition.Mode;
                Connected = true;
                onConnect();
            }
            catch ( TimeoutException timeoutException )
            {
                onFailure();
            }
        }

        public void Close()
        {
            Connected = false;
            Client.Close();
        }

        public ClientPipeEndpoint( NamedPipeChannelDefinition definition )
        {
            Definition = definition;
        }

        public void Dispose()
        {
            if( Connected )
                Close();
        }
    }

    public class PipeEndpointFactory
    {
        public IPipeEndpoint CreateEndpointForChannel(NamedPipeChannelDefinition definition)
        {
            if ( definition.IsServer )
            {
                var stream = new NamedPipeServerStream(
                definition.Name, 
                PipeDirection.InOut,
                1024, // this is arbitrary, apparently 0 doesn't = infinte 
                PipeTransmissionMode.Message, 
                PipeOptions.Asynchronous,
                definition.BufferSize,
                definition.BufferSize);

                return new ServerPipeEndpoint( stream );
            }
            else
            {
                return new ClientPipeEndpoint( definition );
            }
        }
    }

    public class PipeProxy
    {
        public NamedPipeChannelDefinition Definition { get; set; }
        public IDispatcher Dispatch { get; set; }
        public IPipeEndpoint Pipe { get; set; }
        public PipeEndpointFactory PipeFactory { get; set; }
        public RingBuffer RingBuffer { get; set; }
        public bool Running { get; set; }
        public PipeStream Stream { get { return Pipe.Stream; } }
        public IMessageSerializer Serializer { get; set; }

        public object DispatchResult(object translatedEnvelope)
        {
            var envelope = translatedEnvelope as IEnvelope;
            if (envelope != null)
            {
                Dispatch.Send(envelope);
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

        public void HandlePoisonMessage(object message)
        {
            var envelope = message as NamedPipeTransportEnvelope;
            if (envelope != null)
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

        public void Connect()
        {
            Pipe.Connect( Listen, () => { Running = false; } );
        }

        public NamedPipeEnvelope GetEnvelope(NamedPipeTransportEnvelope transportEnvelope)
        {
            var messageTypeName = transportEnvelope.MessageType;
            var messageType = Type.GetType(messageTypeName);
            var envelopeType = typeof(NamedPipeEnvelope<>).MakeGenericType(messageType);
            var envelope = Activator.CreateInstance(envelopeType) as NamedPipeEnvelope;
            envelope.ReplyStream = Stream;
            envelope.MessageId = transportEnvelope.MessageId;
            envelope.CorrelationId = transportEnvelope.CorrelationId;
            envelope.Position = transportEnvelope.Position;
            envelope.RoutingKey = transportEnvelope.RoutingKey;
            envelope.Sequence = transportEnvelope.Sequence;
            envelope.SequenceEnd = transportEnvelope.SequenceEnd;
            return envelope;
        }

        public PipeProxy( NamedPipeChannelDefinition definition, IDispatcher dispatch, IMessageSerializer serializer )
        {
            Definition = definition;
            Dispatch = dispatch;
            Serializer = serializer;
            PipeFactory = new PipeEndpointFactory();
            Pipe = PipeFactory.CreateEndpointForChannel( definition );
            
            RingBuffer = new RingBuffer( 1000 );
            RingBuffer.AddTransform( DeserializeMessage );
            RingBuffer.AddTransform( DispatchResult );
            RingBuffer.Start();
        }
    }
}
