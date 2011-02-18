using System;
using Symbiote.Core;

namespace Core.Tests.Domain.Model
{
    public class DriverKeyAccessor : IKeyAccessor<IHaveTestKey>
    {
        public string GetId( IHaveTestKey actor )
        {
            return actor.Id.ToString();
        }

        public void SetId<TKey>( IHaveTestKey actor, TKey key )
        {
            actor.Id = Guid.Parse( key.ToString() );
        }
    }
}