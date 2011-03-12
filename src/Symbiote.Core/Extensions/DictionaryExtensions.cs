using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Core.Extensions
{
    public static class DictionaryExtensions
    {
        public static Tuple<bool, TValue> TryGet<TKey, TValue>( this IDictionary<TKey, TValue> dictionary, TKey key )
        {
            TValue value = default( TValue );
            var exists = dictionary.TryGetValue( key, out value );
            return Tuple.Create( exists, value );
        }

        public static TValue AddOrUpdate<TKey, TValue>( this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> addValue, Func<TKey, TValue, TValue> updateValue )
        {
            TValue value = default( TValue );
            if( dictionary.TryGetValue( key, out value ) )
            {
                value = updateValue( key, value );
            }
            else
            {
				value = addValue( key );
				dictionary.Add( key, value );
            }
            return value;
        }
    }
}
