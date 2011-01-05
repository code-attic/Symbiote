using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Actor;
using Symbiote.Core.Impl.UnitOfWork;
using Symbiote.Messaging.Config;
using Symbiote.Core.Extensions;

namespace Symbiote.Messaging.Impl.Eventing
{
    public class EventChannel
        : IObserver<IEvent>
    {
        public IBus Bus { get; set; }
        public IEventChannelConfiguration Configuration { get; set; }

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

        public EventChannel( IBus bus, IEventChannelConfiguration configuration )
        {
            Bus = bus;
            Configuration = configuration;
        }
    }
}
