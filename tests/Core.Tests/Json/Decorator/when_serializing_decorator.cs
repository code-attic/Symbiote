using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.Expando;
using System.Text;
using Symbiote.Core;

namespace Core.Tests.Json.Decorator
{
    public class Decorator<T>
    {
        public string Revision { get; set; }
        public T Base { get; set; }
    }

    public interface IHaveKey
    {
        string Id { get; set; }
    }

    public class Underlying
        : IHaveKey
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class HaveIdKeyAccessor
        : IKeyAccessor<IHaveKey>
    {
        public string GetId( IHaveKey actor )
        {
            return actor.Id;
        }

        public void SetId<TKey>( IHaveKey actor, TKey key )
        {
            actor.Id = key.ToString();
        }
    }

    class when_serializing_decorator
    {
    }
}
