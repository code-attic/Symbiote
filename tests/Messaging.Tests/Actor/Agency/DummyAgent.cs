using Symbiote.Messaging.Impl.Actors;

namespace Messaging.Tests.Actor.Agency
{
    public class DummyAgent : IAgent<DummyActor>
    {
        public static int Instantiated { get; set; }
        public int InstanceId { get; set; }

        public DummyActor GetActor<TKey>( TKey key )
        {
            return new DummyActor();
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