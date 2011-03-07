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
using System.Linq;
using System.Text;
using Symbiote.Core.Serialization;
using Symbiote.Http.Impl.Adapter.Channel;
using Symbiote.Http.Owin;
using Symbiote.Messaging;
using Symbiote.Messaging.Impl.Dispatch;
using Symbiote.Messaging.Impl.Subscriptions;

namespace Symbiote.Http
{
    public class MessagingApplication
        : IApplication
    {
        public IDispatcher Dispatcher { get; set; }

        public void Process( IDictionary<string, object> requestItems, OwinResponse respond, Action<Exception> onException )
        {
            var request = requestItems.ExtractRequest();
            request.ReadNext( 
                x => {
                    var contentType = request.Headers["Content-Type"] ?? "";
                    var encoding = request.Headers["Content-Encoding"] ?? "";
                    var type = Type.GetType( contentType );
                    var envelopeType = typeof( HttpEnvelope<> ).MakeGenericType( type );
                    HttpEnvelope result;
                    switch( encoding.Trim() )
                    {
                        case "application/json":
                            result = Encoding.UTF8.GetString( x.Array ).FromJson( envelopeType ) as HttpEnvelope;
                            break;
                        case "application/protocol-buffer":
                            result = x.Array.FromProtocolBuffer( envelopeType ) as HttpEnvelope;
                            break;
                        default:
                            result = null;
                            break;
                    }

                    result.Callback = respond;
                    Dispatcher.Send( result as IEnvelope );
                },
                x => { }
                );
        }

        public MessagingApplication( IDispatcher dispatcher )
        {
            Dispatcher = dispatcher;
        }
    }
}