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
using System.Collections.Generic;
using System.Runtime.Serialization;
using Symbiote.Messaging;
using ProtoBuf;

namespace Symbiote.Http.Impl.Adapter.Channel
{
    [ProtoInclude( 1, typeof( HttpEnvelope<> ) )]
    [DataContract]
    public class HttpEnvelope
    {
        [IgnoreDataMember] protected Type _messageType;

        [DataMember( Order = 20 )]
        public Type MessageType
        {
            get
            {
                _messageType = _messageType ?? Message.GetType();
                return _messageType;
            }
            protected set { _messageType = value; }
        }

        [IgnoreDataMember]
        public virtual object Message { get; set; }

        [DataMember( Order = 30 )]
        public string CorrelationId { get; set; }

        [DataMember( Order = 40 )]
        public Guid MessageId { get; set; }

        [DataMember( Order = 50 )]
        public string RoutingKey { get; set; }

        [DataMember( Order = 60 )]
        public DateTime TimeStamp { get; set; }

        [DataMember( Order = 70 )]
        public long Sequence { get; set; }

        [DataMember( Order = 80 )]
        public long Position { get; set; }

        [DataMember( Order = 90 )]
        public bool SequenceEnd { get; set; }

        [IgnoreDataMember]
        public OwinResponse Callback { get; set; }
    }

    public class HttpEnvelope<TMessage> :
        HttpEnvelope,
        IEnvelope<TMessage>
    {
        [DataMember( Order = 10 )]
        public new TMessage Message
        {
            get { return (TMessage) base.Message; }
            set { base.Message = value; }
        }

        /// <summary>
        /// --!ATTENTION!--
        /// This method ONLY allows you to respond with:
        /// 
        /// 200 OK
        /// Content-Type: {fully qualified type name of TResponse}
        /// Content-Encoding: application/json
        /// {the json encoded messages}
        /// 
        /// This is the correct method. For more control over the
        /// response, please use the Callback property's Build()
        /// extension method to get access to an OWIN compliant
        /// response mechanism.
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="response"></param>
        public void Reply<TResponse>( TResponse response )
        {
            Callback
                .Build()
                .AppendJson( response )
                .DefineHeaders(
                    x =>
                    x.ContentType( typeof( TResponse ).AssemblyQualifiedName ).ContentEncoding( "application/json" ) )
                .Submit( HttpStatus.Ok );
        }

        public void Acknowledge()
        {
            Callback
                .Build()
                .Submit(HttpStatus.Ok);
        }

        public void Reject( string reason )
        {
            Callback
                .Build()
                .Submit(HttpStatus.InternalServerError);
        }

        public HttpEnvelope()
        {
        }

        public HttpEnvelope( TMessage message )
        {
            Message = message;
            MessageType = typeof( TMessage );
        }
    }
}