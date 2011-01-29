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

using Symbiote.Messaging.Impl.Dispatch;

namespace Symbiote.Messaging.Impl.Channels
{
    public class LocalChannelFactory
        : IChannelFactory
    {
        public IDispatcher Dispatcher { get; set; }

        public IChannel CreateChannel(IChannelDefinition definition)
        {
            var channel = new LocalChannel( Dispatcher, definition as LocalChannelDefinition );
            return channel;
        }

        public LocalChannelFactory(IDispatcher dispatcher)
        {
            Dispatcher = dispatcher;
        }
    }
}
