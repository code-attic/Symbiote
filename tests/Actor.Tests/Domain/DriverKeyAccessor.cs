using Actor.Tests.Domain.Model;
using Symbiote.Actor;
using Symbiote.Core.Impl.UnitOfWork;

namespace Actor.Tests.Domain
{
    public class DriverKeyAccessor : IKeyAccessor<Driver>
    {
        public string GetId( Driver actor )
        {
            return actor.SSN;
        }

        public void SetId<TKey>( Driver actor, TKey id )
        {
            // this isn't necessary with a customer factory
        }
    }
}