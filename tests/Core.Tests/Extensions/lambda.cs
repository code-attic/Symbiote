using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Actor.Tests;
using Machine.Specifications;

namespace Core.Tests.Extensions
{
    public class when_parsing_a_lambda
    {
        public static string result;
        private Because of = () =>
                                 {
                                     result = Parse<DummyActor>(x => x.Id.Contains("Dude"));
                                 };

        public static string Parse<T>(Expression<Predicate<T>> predicate)
        {
            return predicate.ToString();
        }

        private It should_produce_human_readable_string = () => result.ShouldEqual( "x => x.Id.Contains(\"Dude\")" );
    }
}
