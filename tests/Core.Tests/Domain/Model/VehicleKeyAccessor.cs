using System;
using Symbiote.Core;

namespace Core.Tests.Domain.Model
{
    public class VehicleKeyAccessor : IKeyAccessor<Vehicle>
    {
        public string GetId( Vehicle actor )
        {
            return actor.Id.ToString();
        }

        public void SetId<TKey>( Vehicle actor, TKey key )
        {
            actor.Id = Guid.Parse( key.ToString() );
        }
    }
}