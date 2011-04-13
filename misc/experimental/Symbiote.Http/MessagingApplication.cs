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
using Symbiote.Http.NetAdapter.Channel;
using Symbiote.Http.Owin;
using Symbiote.Messaging;
using Symbiote.Messaging.Impl.Dispatch;
using Symbiote.Messaging.Impl.Subscriptions;

namespace Symbiote.Http
{
    public class MessagingApplication
        : Application
    {
        public IDispatcher Dispatcher { get; set; }
		public List<ArraySegment<byte>> Buffer { get; set; }
		
		public override bool HandleRequestSegment( ArraySegment<byte> data, Action continuation )
		{
			Buffer.Add( data );
			return false;
		}
		
		public void Dispatch()
		{
			var contentType = Request.Headers["Content-Type"] ?? "";
    		var encoding = Request.Headers["Content-Encoding"] ?? "";
        	var type = Type.GetType( contentType );
        	var envelopeType = typeof( HttpEnvelope<> ).MakeGenericType( type );
        	HttpEnvelope result;
			var buffer = new byte[ Buffer.Sum( x => x.Count ) ];
			var index = 0;
			Buffer.ForEach( x => 
			            {
			               	System.Buffer.BlockCopy( x.Array, x.Offset, buffer, index, x.Count );
							index += x.Count;
						} );
        	switch( encoding.Trim() )
        	{
        		case "application/json":
        			result = Encoding.UTF8.GetString( buffer ).FromJson( envelopeType ) as HttpEnvelope;
                        break;
                case "application/protocol-buffer":
                    result = buffer.FromProtocolBuffer( envelopeType ) as HttpEnvelope;
                    break;
                default:
                    result = null;
                    break;
            }

            result.Callback = Response;
            Dispatcher.Send( result as IEnvelope );	
		}
		
		public override void CompleteResponse ()
		{
			Dispatch();
		}

        public override void OnError( Exception exception )
        {

        }

        public MessagingApplication( IDispatcher dispatcher )
        {
            Dispatcher = dispatcher;
			Buffer = new List<ArraySegment<byte>>();
        }
    }
}