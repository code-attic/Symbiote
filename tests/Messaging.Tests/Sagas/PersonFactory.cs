using Symbiote.Core.Actor;

namespace Actor.Tests.Sagas
{
    public class PersonFactory : IActorFactory<Person>
    {
        public Person CreateInstance<TKey>( TKey id )
        {
            return new Person();
        }
    }
}