using System;
using Symbiote.Core;

namespace Mikado.Tests.Domain.Model
{
    public class TestKeyAccessor : IKeyAccessor<ITestKey>
    {
        public string GetId( ITestKey actor )
        {
            return actor.Id.ToString();
        }

        public void SetId<TKey>( ITestKey actor, TKey key )
        {
            actor.Id = Guid.Parse(key.ToString());
        }
    }
}