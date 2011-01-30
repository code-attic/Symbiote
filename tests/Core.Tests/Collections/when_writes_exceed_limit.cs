using System.Linq;
using Machine.Specifications;
using IEnumerableExtenders = Symbiote.Core.Extensions.IEnumerableExtenders;

namespace Core.Tests.Collections
{
    public class when_writes_exceed_limit
        : with_mrudictionary
    {
        public static int[] Values { get; set; }
        private Because of = () =>
                                 {
                                     IEnumerableExtenders.ForEach<int>( Enumerable
                                                            .Range( 1, 20 ), x => nrulookup.Add( x, x ) );
                                     var values = nrulookup.Values;
                                     Values = values.ToArray();
                                 };
        
        private It should_only_have_item_limit = () => nrulookup.Count.ShouldEqual( 10 );
        private It should_have_last_10_items = () =>
                                                   {
                                                       Values.ShouldEqual( new[] {11, 12, 13, 14, 15, 16, 17, 18, 19, 20} );
                                                   };
    }
}