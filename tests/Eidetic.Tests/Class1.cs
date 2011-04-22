using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Eidetic;
using Symbiote.Core.Extensions;

namespace Eidetic.Tests
{
    public abstract class with_cache
    {
        private Establish context = () =>
                                        {
                                            Assimilate
                                                .Initialize()
                                                .Eidetic(x => x.AddLocalServer());
                                        };
    }

    public class when_checking_locks
        : with_cache
    {
        protected static string result { get; set; }
        protected static int index { get; set; }
        protected static Guid id = Guid.NewGuid();
        protected static Stopwatch watch { get; set; }

        private Because of = () =>
                                 {
                                     watch = Stopwatch.StartNew();
                                     Action action = Append;
                                     Parallel.Invoke(action, action, action, action);
                                     watch.Stop();
                                 };

        private It should_produce_ordered_and_complete_result = () => result.ShouldEqual("-1-2-3-4");
        private It should_take_less_than_1_second = () => watch.ElapsedMilliseconds.ShouldBeLessThan(100);
        
        public static void Append()
        {
            result = "{0}-{1}".AsFormat(result, ++index);
        }
    }
}
