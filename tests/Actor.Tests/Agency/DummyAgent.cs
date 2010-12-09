using System;
using Symbiote.Actor;
using Symbiote.Actor.Impl;

namespace Actor.Tests.Agency
{
    public class DummyAgent : IAgent<DummyActor>
    {
        public static int Instantiated { get; set; }
        public int InstanceId { get; set; }

        public DummyActor GetActor<TKey>( TKey key )
        {
            return new DummyActor();
        }

        public void RegisterActor<TKey>( TKey key, DummyActor actor )
        {
            
        }

        public void Memoize( DummyActor actor )
        {
            
        }

        public DummyAgent()
        {
            InstanceId = Instantiated++;
        }
    }
}