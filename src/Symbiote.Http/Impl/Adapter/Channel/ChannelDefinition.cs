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
using System.Net;
using System.Net.Security;
using System.Text;
using Symbiote.Messaging.Impl.Channels;
using Symbiote.Core.Impl.Serialization;

namespace Symbiote.Http.Impl.Adapter.Channel
{
    public class ChannelDefinition
        : BaseChannelDefinition
    {
        public UriBuilder UriBuilder { get; set; }
        public Uri RequestUri { get { return UriBuilder.Uri; } }
        public string Verb { get; set; }
        public string BaseUri { get; set; }
        public Func<object, byte[]> Serializer { get; set; }
        public string ContentType { get; set; }
        public string ContentEncoding { get; set; }
        public NetworkCredential Credentials { get; set; }
        public override Type ChannelType { get { return typeof(HttpChannel); } }
        public override Type FactoryType { get { return typeof(HttpChannelFactory); }}

        public string GetUriForEnvelope<T>(HttpEnvelope<T> envelope)
        {
            string additionalPath = envelope.RoutingKey;
            var uri = BaseUri;
            if(!string.IsNullOrEmpty(additionalPath))
            {
                var separator = additionalPath.StartsWith( "?" )
                                    ? ""
                                    : "/";
                uri = string.Join( separator, uri, additionalPath );
            }
            return uri;
        }

        public byte[] JsonSerialize(object body)
        {
            return Encoding.UTF8.GetBytes( body.ToJson() );
        }

        public byte[] ProtoBufSerialize(object body)
        {
            return body.ToProtocolBuffer();
        }

        public ChannelDefinition()
        {
            UriBuilder = new UriBuilder();
            Serializer = JsonSerialize;
        }
    }
}
