using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Messaging;

namespace Messaging.Tests.Local.Performance
{
    public class when_sending_a_million_messages : with_bus
    {
        private static Stopwatch watch;
        private Because of = () =>
            {
                watch = Stopwatch.StartNew();
                Enumerable.Range( 0, 1000000 ).AsParallel().ForAll( x => bus.Publish( "local", new ThroughputMessage(), e => e.CorrelationId = ( x % 100 ).ToString() ) );
                watch.Stop();
            };

        private It should_happen_in_like_10_seconds = () => watch.ElapsedMilliseconds.ShouldBeLessThan( 1000 );
    }

    public class ThroughputMessageHandler : IHandle<ThroughputMeasuringActor, ThroughputMessage>
    {
        public Action<IEnvelope> Handle( ThroughputMeasuringActor actor, ThroughputMessage message )
        {
            actor.RecordMessage();
            return x => x.Acknowledge();
        }
    }

    public class ThroughputMessage
    {
        
    }

    public class ThroughputKeyAccessor : IKeyAccessor<ThroughputMeasuringActor>
    {
        public string GetId( ThroughputMeasuringActor actor )
        {
            return actor.Id;
        }

        public void SetId<TKey>( ThroughputMeasuringActor actor, TKey key )
        {
            actor.Id = key.ToString();
        }
    }

    public class ThroughputMeasuringActor
    {
        public string Id { get; set; }
        public static int Total { get; set; }

        public void RecordMessage()
        {
            Total ++;
        }
    }
}
