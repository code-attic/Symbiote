using System;
using System.Linq;
using Machine.Specifications;
using Symbiote.Core.Hashing;

namespace Core.Tests.Distributor.Basic
{
    public class when_testing_distribution_under_load
        : with_simple_distributor_of_lists
    {
        protected static double standardDeviation { get; set; }
        protected static double variance { get; set; }
        protected static double percentDeviation { get; set; }
        protected static IHashingProvider HashProvider { get; set; }

        private Because of = () =>
                                 {
                                     HashProvider = new MD5HashProvider();
                                     var rnd = new Random();
                                     for (int i = 0; i < 10000; i++)
                                     {
                                         var bytes = Guid.NewGuid().ToByteArray();
                                         var newInt = BitConverter.ToInt64(bytes, 0) + BitConverter.ToInt64(bytes, 8);
                                         Write(newInt);
                                     }
                                     averageFetchTime = totalFetchTime/totalFetched;

                                     var average = (double) lists.Sum(x => x.Count) / lists.Count;
                                     variance = lists.Sum(x => Math.Pow((x.Count - average),2)) / (double)lists.Count;
                                     standardDeviation = Math.Sqrt(variance);
                                     percentDeviation = standardDeviation/average;
                                 };
        
        private It percent_deviation_should_be_less_than_5_percent = () => 
            percentDeviation.ShouldBeLessThan(.050);

        private It should_not_take_more_than_200_ms_to_build_tree = () =>
                                                                      treeWatch.ElapsedMilliseconds.ShouldBeLessThan(
                                                                          200);
        
        private It average_fetch_should_be_under_1_ms = () => averageFetchTime.ShouldBeLessThan(1);

    }
}
