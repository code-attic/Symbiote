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


namespace Symbiote.Messaging.Impl.Monitor
{
    public class AgentMonitor
        : IAgentMonitor
    {
        public IBus Bus { get; set; }
        public MonitorConfiguration Configuration { get; set; }

        public void ActorLoadedFromMemory<TActor>( string id )
        {
            Bus.Publish( Configuration.EventChannel, new ActorEvent( id ) );
        }

        public void ActorLoadedFromCache<TActor>( string id )
        {
            Bus.Publish(Configuration.EventChannel, new ActorEvent(id));
        }

        public void ActorLoadedFromStore<TActor>( string id )
        {
            Bus.Publish(Configuration.EventChannel, new ActorEvent(id));
        }

        public void ActorMemoized<TActor>( string id )
        {
            Bus.Publish(Configuration.EventChannel, new ActorEvent(id));
        }

        public void ActorReceivedMessage<TActor, TMessage>( string id, IEnvelope<TMessage> envelope )
        {
            Bus.Publish(Configuration.EventChannel, new ActorEvent(id));
        }

        public void ActorUnloaded<TActor>( string id )
        {
            Bus.Publish(Configuration.EventChannel, new ActorEvent(id));
        }

        public AgentMonitor( IBus bus, MonitorConfiguration configuration )
        {
            Bus = bus;
            Configuration = configuration;
        }
    }
}