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
using Symbiote.Core;
using Symbiote.Messaging.Impl.Dispatch;
using Symbiote.Messaging.Impl.Serialization;

namespace Symbiote.Messaging.Impl.Channels.Pipe
{
    public class NamedPipeChannelFactory
        : IChannelFactory
    {
        public IDispatcher Dispatcher { get; set; }

        public IChannel CreateChannel( IChannelDefinition definition )
        {
            var namedPipeChannelDefinition = definition as NamedPipeChannelDefinition;
            var serializer = Assimilate.GetInstanceOf( definition.SerializerType ) as IMessageSerializer;
            var proxy = new PipeProxy( namedPipeChannelDefinition, Dispatcher, serializer );
            var namedPipeChannel = new NamedPipeChannel( namedPipeChannelDefinition, proxy, Dispatcher );
            return namedPipeChannel;
        }

        public NamedPipeChannelFactory( IDispatcher dispatcher )
        {
            Dispatcher = dispatcher;
        }
    }
}