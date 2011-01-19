using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Machine.Specifications;
using Symbiote.Core.Impl.Collections;
using Symbiote.Core.Extensions;

namespace Core.Tests.Collections
{
    public class with_nrudictionary
    {
        public static MruDictionary<int, int> nrulookup;

        private Establish context = () =>
        {
            nrulookup = new MruDictionary<int, int>();
        };
    }

    public class when_nothing_is_accessed
        : with_nrudictionary
    {
        private Because of = () =>
        {
            Enumerable
                .Range( 1, 10 )
                .ForEach( x => nrulookup.Add( x, x ) );
            // let the cleansing begin... bwuhuhuhuhuhu
            Thread.Sleep( 3000 );
        };

        private It should_not_have_any_value = () => nrulookup.Count.ShouldEqual( 0 );
    }
}
