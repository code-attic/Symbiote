using System.Diagnostics;
using System.Linq;
using Machine.Specifications;

namespace Core.Tests.Hashing
{
    public class when_hashing_md5 : with_substantial_keyset
    {
        private Because of = () => 
                                 {
                                     var watch = Stopwatch.StartNew();
                                     for ( int i = 0; i < Total; i++ )
                                     {
                                         HashSet1[i] = (ulong) Keys[i].MD5();
                                     }
                                     watch.Stop();
                                     Elasped = watch.ElapsedMilliseconds;

                                     for ( int i = 0; i < Total; i++ )
                                     {
                                         HashSet2[i] = (ulong) Keys[i].MD5();
                                     }
                                 };

        private It should_have_only_unique_HashSet1 = () => 
                                                      HashSet1.Distinct().Count().ShouldEqual( Total );

        private It should_be_quick = () => Elasped.ShouldBeLessThanOrEqualTo( 300 );

        private It should_produce_identical_sets = () =>
                                                       {
                                                           var x = 0;
                                                           HashSet1.All( h => h == HashSet2[x++] ).ShouldBeTrue();
                                                       };
    }
}