using System;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace Core.Tests.Utility.distributor
{
    public class when_testing_load_distribution
        : with_simple_distributor_of_lists
    {
        protected static double standardDeviation { get; set; }
        protected static double variance { get; set; }
        protected static double percentDeviation { get; set; }

        private Because of = () =>
                                 {
                                     var rnd = new Random();
                                     for (int i = 0; i < 100000; i++)
                                     {
                                         Write(i);
                                     }
                                     averageFetchTime = totalFetchTime/totalFetched;

                                     var average = (double) lists.Sum(x => x.Count) / lists.Count;
                                     variance = lists.Sum(x => Math.Pow((x.Count - average),2)) / (double)lists.Count;
                                     standardDeviation = Math.Sqrt(variance);
                                     percentDeviation = standardDeviation/average;
                                 };
        
        private It percent_deviation_should_be_less_than_10_percent = () => 
            percentDeviation.ShouldBeLessThan(.10);

        private It should_not_take_more_than_1_second_to_build_tree = () =>
                                                                      treeWatch.ElapsedMilliseconds.ShouldBeLessThan(
                                                                          1000);

        private It average_fetch_should_be_2_ms = () => averageFetchTime.ShouldBeLessThan(1);

    }
}
