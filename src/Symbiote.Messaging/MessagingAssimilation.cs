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
using Symbiote.Core;
using Symbiote.Core.UnitOfWork;
using Symbiote.Messaging.Config;
using Symbiote.Messaging.Impl.Dispatch;
using Symbiote.Messaging.Impl.Eventing;

namespace Symbiote.Messaging
{
    public static class MessagingAssimilation
    {
        private static IDisposable EventSubscription { get; set; }

        public static IAssimilate Messaging( this IAssimilate assimilate, Action<EventChannelConfigurator> eventChannels )
        {
            var publisher = Assimilate.GetInstanceOf<IEventPublisher>() as IObservable<IEvent>;
            if ( publisher == null )
            {
                throw new AssimilationException(
                    "You must call the Actor assimilation extension method before setting up event channels in Symbiote.Messaging." );
            }

            var configurator = new EventChannelConfigurator();
            eventChannels( configurator );
            Assimilate.Dependencies( x => x.For<IEventChannelConfiguration>().Use( configurator.Configuration ) );

            EventSubscription = publisher.Subscribe( Assimilate.GetInstanceOf<EventChannel>() );

            Preload();

            return assimilate;
        }

        private static void Preload()
        {
            Assimilate.GetInstanceOf<IDispatcher>();
        }
        
    }
}