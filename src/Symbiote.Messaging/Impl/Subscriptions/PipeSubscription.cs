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
using Symbiote.Core.Extensions;
using Symbiote.Messaging.Impl.Channels.Pipe;
using Symbiote.Messaging.Impl.Dispatch;
using Symbiote.Messaging.Impl.Subscriptions;

namespace Symbiote.Messaging.Impl.Adapter
{
    public class PipeSubscription :
        ISubscription
    {
        protected NamedPipeListener Listener { get; set; }
        protected NamedPipeChannelDefinition Definition { get; set; }
        protected IDispatcher Dispatcher { get; set; }

        public string Name { get; set; }
        public bool Started { get; private set; }
        public bool Starting { get; private set; }
        public bool Stopped { get; private set; }
        public bool Stopping { get; private set; }

        public void Dispose()
        {
            Listener.Endpoint.Close();
            Listener = null;
        }

        public void Start()
        {
            Listener = new NamedPipeListener( Definition, Dispatcher );
            "Started subscription to Named Pipe '{0}'".ToInfo<IBus>( Definition.Name );
        }

        public void Stop()
        {
            Listener.Running = false;
            Listener = null;
            "Started subscription to Named Pipe '{0}'".ToInfo<IBus>( Definition.Name );
        }

        public PipeSubscription(
            IDispatcher dispatcher,
            NamedPipeChannelDefinition definition)
        {
            Dispatcher = dispatcher;
            Definition = definition;
        }
    }
}