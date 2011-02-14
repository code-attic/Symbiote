using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Machine.Specifications;
using Symbiote.Core.Collections;

namespace Core.Tests.Collections 
{
    public class when_attempting_writes_in_parallel_to_exclusive_dictionary 
    {
        static int value;
        private Because of = () => 
        {
            var accessCount = 0;
            var dictionary = new ExclusiveConcurrentDictionary<string, int>();


            Enumerable
                .Range( 1, 4 )
                .Select( x => new ManualResetEvent( false ) )
                .ToList()
                .AsParallel()
                .ForAll( x =>
                             {
                                 dictionary.ReadOrWrite( "key", () => ++accessCount );
                             } );
            value = dictionary[ "key" ];
        };
        
        private It should_equal_one = () => value.ShouldEqual( 1 );
    }
}
