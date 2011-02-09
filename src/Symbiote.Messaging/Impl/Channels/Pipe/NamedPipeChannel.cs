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
using Symbiote.Core.Futures;
using Symbiote.Messaging.Impl.Dispatch;
using Symbiote.Messaging.Impl.Envelope;
using Symbiote.Messaging.Impl.Serialization;

namespace Symbiote.Messaging.Impl.Channels.Pipe
{
    public class NamedPipeChannel
        : IChannel
    {
        public NamedPipeChannelDefinition Definition { get; set; }
        public IDispatcher Dispatcher { get; set; }
        public PipeProxy Pipe { get; set; }

        public string Name { get; set; }

        public Future<TReply> ExpectReply<TReply, TMessage>( TMessage message )
        {
            return ExpectReply<TReply, TMessage>( message, x => { } );
        }

        public Future<TReply> ExpectReply<TReply, TMessage>( TMessage message, Action<IEnvelope> modifyEnvelope )
        {
            var envelope = new NamedPipeEnvelope( )
                               {
                                   CorrelationId = Definition.GetCorrelationId( message ),
                                   RoutingKey = Definition.GetRoutingKey( message ),
                                   Message = message,
                                   MessageType = typeof( TMessage )
                               };

            modifyEnvelope( envelope );
            var future = Future.Of<TReply>( () => Pipe.Send( envelope ) );
            Dispatcher.ExpectResponse<TReply>( envelope.MessageId.ToString(), future );
            return future;
        }

        public void Send<TMessage>( TMessage message )
        {
            Send( message, x => { } );
        }

        public void Send<TMessage>( TMessage message, Action<IEnvelope> modifyEnvelope )
        {
            var envelope = new NamedPipeEnvelope( )
                               {
                                   CorrelationId = Definition.GetCorrelationId( message ),
                                   RoutingKey = Definition.GetRoutingKey( message ),
                                   Message =  message,
                                   MessageType = typeof( TMessage )
                               };
            modifyEnvelope( envelope );
            Pipe.Send( envelope );
        }

        public NamedPipeChannel( NamedPipeChannelDefinition definition, PipeProxy pipe, IDispatcher dispatcher )
        {
            Definition = definition;
            Name = definition.Name;
            Dispatcher = dispatcher;
            Pipe = pipe;
            Pipe.Connect();
        }
    }
}