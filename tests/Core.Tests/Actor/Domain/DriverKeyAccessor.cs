using Core.Tests.Actor.Domain.Model;
using Symbiote.Core;

namespace Core.Tests.Actor.Domain
{
    public class DriverKeyAccessor : IKeyAccessor<Driver>
    {
        public string GetId(Driver actor)
        {
            return actor.SSN;
        }

        public void SetId<TKey>(Driver actor, TKey key)
        {
            // this isn't necessary with a custom factory
        }
    }
}