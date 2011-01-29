using System;

namespace Symbiote.Core.Utility
{
    public static class Pipeline
    {
        public static Func<TStart, TNext> Then<TStart, TResult, TNext>(this Func<TStart, TResult> start, Func<TResult, TNext> then)
        {
            return x => then(start(x));
        }

        public static Func<TStart, TNext> Then<TStart, TResult, TService, TNext>(this Func<TStart, TResult> start, Func<TService, TResult, TNext> then)
        {
            var service = Assimilate.GetInstanceOf<TService>();
            return x => then( service, start( x ) );
        }

        public static Func<Ti, To> Start<Ti, To>(Func<Ti, To> funcy)
        {
            return funcy;
        }

        
    }
}
