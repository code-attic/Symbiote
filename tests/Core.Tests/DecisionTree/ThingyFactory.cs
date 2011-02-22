using Symbiote.Core.Actor;

namespace Core.Tests.DecisionTree
{
    public class ThingyFactory : IActorFactory<Thingy>
    {
        public Thingy CreateInstance<TKey>( TKey id )
        {
            return new Thingy( id.ToString(), true, true, true );
        }
    }
}