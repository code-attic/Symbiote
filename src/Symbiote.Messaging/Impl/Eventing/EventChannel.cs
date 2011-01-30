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
using Symbiote.Core.Extensions;
using Symbiote.Core.UnitOfWork;
using Symbiote.Messaging.Config;

namespace Symbiote.Messaging.Impl.Eventing
{
    public class EventChannel
        : IObserver<IEvent>
    {
        public IBus Bus { get; set; }
        public IEventChannelConfiguration Configuration { get; set; }

        #region IObserver<IEvent> Members

        public void OnNext( IEvent value )
        {
            Configuration
                .PublishTo
                .ForEach( x => Bus.Publish( x, value ) );
        }

        public void OnError( Exception error )
        {
        }

        public void OnCompleted()
        {
        }

        #endregion

        public EventChannel( IBus bus, IEventChannelConfiguration configuration )
        {
            Bus = bus;
            Configuration = configuration;
        }
    }
}