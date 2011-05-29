using System;
using Symbiote.Core;

namespace Actor.Tests.Sagas
{
    public class PersonKeyAccessor : IKeyAccessor<Person>
    {
        public string GetId( Person actor )
        {
            return actor.Id.ToString();
        }

        public void SetId<TKey>( Person actor, TKey key )
        {
            actor.Id = Guid.Parse( key.ToString() );
        }
    }
}