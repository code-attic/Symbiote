using System.Collections.Generic;
using Machine.Specifications;

namespace Lucene.Tests
{
    public abstract class with_index_feeder
    {
        protected static Dictionary<string, string> dictionary;
        protected static IndexFeeder feeder;

        private Establish context = () =>
                                        {
                                            feeder = new IndexFeeder();
                                            dictionary = new Dictionary<string, string>()
                                                             {
                                                                 {"FirstName", "Alex"},
                                                                 {"LastName", "Robson"},
                                                                 {"Age", "31"},
                                                             };
                                        };
    }
}