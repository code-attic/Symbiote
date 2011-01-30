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
namespace Symbiote.Http.Impl.Adapter.Channel
{
    public class ChannelConfigurator
    {
        public ChannelDefinition ChannelDefinition { get; set; }

        public ChannelConfigurator Verb( string httpVerb )
        {
            ChannelDefinition.Verb = httpVerb;
            return this;
        }

        public ChannelConfigurator BaseUrl( string uri )
        {
            ChannelDefinition.BaseUri = uri;
            return this;
        }

        public ChannelConfigurator Name( string name )
        {
            ChannelDefinition.Name = name;
            return this;
        }

        public ChannelConfigurator UseProtocolBuffers()
        {
            ChannelDefinition.ContentEncoding = "application/protocol-buffer";
            ChannelDefinition.Serializer = ChannelDefinition.ProtoBufSerialize;
            return this;
        }

        public ChannelConfigurator()
        {
            ChannelDefinition = new ChannelDefinition();
            ChannelDefinition.BaseUri = "localhost";
            ChannelDefinition.ContentEncoding = "application/json";
            ChannelDefinition.Verb = "POST";
            ChannelDefinition.Serializer = ChannelDefinition.JsonSerialize;
        }
    }
}