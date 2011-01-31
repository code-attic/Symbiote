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
using System.Runtime.Serialization;
using Symbiote.Core;

namespace Symbiote.Messaging.Impl.Envelope
{
    [Serializable]
    [DataContract]
    public class Envelope<TMessage>
        : IEnvelope<TMessage>
    {
        [DataMember( Order = 1001, IsRequired = false )]
        public Guid MessageId { get; set; }

        [DataMember( Order = 1002, IsRequired = false )]
        public Type MessageType { get; private set; }

        [DataMember( Order = 1003, IsRequired = false )]
        public TMessage Message { get; set; }

        [DataMember( Order = 1004, IsRequired = false )]
        public string CorrelationId { get; set; }

        [DataMember( Order = 1005, IsRequired = false )]
        public string RoutingKey { get; set; }

        [DataMember( Order = 1006, IsRequired = false )]
        public long Sequence { get; set; }

        [DataMember( Order = 1007, IsRequired = false )]
        public long Position { get; set; }

        [DataMember( Order = 1008, IsRequired = false )]
        public bool SequenceEnd { get; set; }

        public void Reply<TResponse>( TResponse response )
        {
            var bus = Assimilate.GetInstanceOf<IBus>();
            bus.Publish(
                "local",
                response,
                x => { x.CorrelationId = MessageId.ToString(); } );
        }

        public Envelope()
        {
        }

        public Envelope( TMessage message )
        {
            Message = message;
            MessageType = typeof( TMessage );
            MessageId = Guid.NewGuid();
        }
    }
}