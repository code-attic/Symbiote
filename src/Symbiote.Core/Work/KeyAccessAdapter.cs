using System;
using Symbiote.Core.Extensions;

namespace Symbiote.Core.Work
{
    public class KeyAccessAdapter<T>
        : IKeyAccessor
        where T : class
    {
        public IKeyAccessor<T> Accessor { get; set; }

        public string GetId<TActor>(TActor actor) where TActor : class
        {
            if (typeof (T).IsAssignableFrom(typeof (TActor)))
            {
                return Accessor.GetId(actor as T);
            }
            throw new InvalidCastException("Key accessor cannot access an actor of {0} as type {1}".AsFormat(typeof(TActor), typeof(T)));
        }

        public void SetId<TActor, TKey>(TActor actor, TKey id) where TActor : class
        {
            if (typeof(T).IsAssignableFrom(typeof(TActor)))
            {
                Accessor.SetId(actor as T, id);
            }
            else
            {
                throw new InvalidCastException("Key accessor cannot access an actor of {0} as type {1}".AsFormat(typeof(TActor), typeof(T)));
            }
        }

        public KeyAccessAdapter(IKeyAccessor<T> accessor)
        {
            Accessor = accessor;
        }
    }
}