using Symbiote.Core;

namespace Core.Tests.Actor.Domain.Model
{
    public class VehicleKeyAccessor : IKeyAccessor<Vehicle>
    {
        public string GetId( Vehicle actor )
        {
            return actor.VIN;
        }

        public void SetId<TKey>( Vehicle actor, TKey key )
        {
            // won't get used due to factory
        }
    }
}