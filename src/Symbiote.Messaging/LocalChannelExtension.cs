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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Core;
using Symbiote.Messaging.Impl.Channels;

namespace Symbiote.Messaging
{
    public static class LocalChannelExtension
    {
        public static IBus AddLocalChannelForMessageOf<T>(this IBus bus)
        {
            IChannelManager manager = Assimilate.GetInstanceOf<IChannelManager>();
            manager.AddDefinition(new LocalChannelDefinition<T>());
            return bus;
        }

        public static IBus AddLocalChannelForMessageOf<T>(this IBus bus, Action<IConfigureChannel<T>> configure)
        {
            IChannelManager manager = Assimilate.GetInstanceOf<IChannelManager>();
            var localChannelDefinition = new LocalChannelDefinition<T>();
            configure(localChannelDefinition);
            manager.AddDefinition(localChannelDefinition);
            return bus;
        }

        public static IBus AddLocalChannelUntypedChannel(this IBus bus)
        {
            IChannelManager manager = Assimilate.GetInstanceOf<IChannelManager>();
            manager.AddDefinition(new LocalChannelDefinition());
            return bus;
        }

        public static IBus AddLocalChannelUntypedChannel(this IBus bus, Action<IConfigureChannel> configure)
        {
            IChannelManager manager = Assimilate.GetInstanceOf<IChannelManager>();
            var localChannelDefinition = new LocalChannelDefinition();
            configure(localChannelDefinition);
            manager.AddDefinition(localChannelDefinition);
            return bus;
        }
    }
}
