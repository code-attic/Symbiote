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

namespace Symbiote.Messaging.Impl.Monitor
{
    public class MonitorConfiguration
    {
        public string EventChannel { get; set; }

        public MonitorConfiguration()
        {
            EventChannel = "system.events";
        }
    }

    public class MonitorConfigurator
    {
        public MonitorConfiguration Configuration { get; set; }

        public MonitorConfigurator SendEventsTo( string channelName )
        {
            Configuration.EventChannel = channelName;
            return this;
        }

        public MonitorConfigurator()
        {
            Configuration = new MonitorConfiguration();
        }
    }

    public interface IAdapterMonitor
    {
        void MessageReceived<TMessage>(IEnvelope<TMessage> envelope);
    }

    public interface IDispatchMonitor
    {
        void MessageDispatched<TMessage>( IEnvelope<TMessage> envelope );
    }

    public interface IChannelMonitor
    {
        void MessageSent<TMessage>(IEnvelope<TMessage> envelope);
    }

    public interface IEnvelopeMonitor
    {
        void MessageAcknowledged<TMessage>( IEnvelope<TMessage> envelope );
        void MessageRejected<TMessage>( IEnvelope<TMessage> envelope );
    }

    public interface IAgentMonitor
    {
        void ActorLoadedFromMemory<TActor>( string id );
        void ActorLoadedFromCache<TActor>( string id );
        void ActorLoadedFromStore<TActor>( string id );
        void ActorMemoized<TActor>( string id );
        void ActorReceivedMessage<TActor, TMessage>( string id, IEnvelope<TMessage> envelope );
        void ActorUnloaded<TActor>( string id );
    }

    public class ActorEvent
    {
        public string ActorId { get; set; }
        public DateTime Occurred { get; set; }
        public string Machine { get; set; }
    }

    public class MessageEvent
    {
        public DateTime Occurred { get; set; }
        public string Machine { get; set; }
    }
}
