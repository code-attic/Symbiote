using System.Diagnostics;
using System.Linq;
using Machine.Specifications;

namespace Core.Tests.Hashing
{
    public class when_hashing_64 : with_substantial_keyset
    {
        private Because of = () => 
                                 {
                                     var watch = Stopwatch.StartNew();
                                     for ( int i = 0; i < Total; i++ )
                                     {
                                         HashSet1[i] = Keys[i].Murmur64();
                                     }
                                     watch.Stop();
                                     Elasped = watch.ElapsedMilliseconds;

                                     for ( int i = 0; i < Total; i++ )
                                     {
                                         HashSet2[i] = Keys[i].Murmur64();
                                     }
                                 };

        private It should_have_only_unique_HashSet1 = () => 
                                                      HashSet1.Distinct().Count().ShouldEqual( Total );

        private It should_be_quick = () => Elasped.ShouldBeLessThanOrEqualTo( 60 );

        private It should_produce_identical_sets = () =>
                                                       {
                                                           var x = 0;
                                                           HashSet1.All( h => h == HashSet2[x++] ).ShouldBeTrue();
                                                       };
    }
}