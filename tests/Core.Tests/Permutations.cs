using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Framework;
using Machine.Specifications;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using Symbiote.Core.Extensions;

namespace Core.Tests
{

    public abstract class with_exception
    {
        protected static Exception _resultingException;
    }

    public abstract class with_permutations : with_exception
    {
        protected static IEnumerable<IEnumerable<string>> permutations;
    }

    public abstract class with_null_enumerable : with_permutations
    {
        protected static List<string> enumerable;
        private Establish context = () => { enumerable = null; };
    }

    public abstract class with_empty_enumerable : with_permutations
    {
        protected static List<string> enumerable;
        protected Establish context = () => { enumerable = new List<string>(); };
    }

    public abstract class with_one_element : with_permutations
    {
        protected static List<string> enumerable;
        private Establish context = () => { enumerable = new List<string> {"A"}; };
    }

    public abstract class with_two_elements : with_permutations
    {
        protected static List<string> enumerable;
        private Establish context = () => { enumerable = new List<string> { "A", "B" }; };
    }

    public abstract class with_three_elements : with_permutations
    {
        protected static List<string> enumerable;
        private Establish context = () => { enumerable = new List<string> { "A", "B", "C" }; };
    }

    public abstract class with_four_elements : with_permutations
    {
        protected static List<string> enumerable;
        private Establish context = () => { enumerable = new List<string> {"A", "B", "C", "D"}; };
    }

    [Subject("enumerable")]
    public class when_enumerable_is_null : with_null_enumerable
    {
        private Because of = () =>
                                 {
                                     _resultingException = Catch.Exception(() => permutations = enumerable.UniquePermutations());
                                 };

        private It should_throw_argument_null_exception = () => _resultingException.ShouldBeOfType
                                                                    <ArgumentNullException>();
    }

    [Subject("enumerable")]
    public class when_enumerable_is_empty : with_empty_enumerable
    {
        private Because of = () => { permutations = enumerable.UniquePermutations(); };

        private It should_return_an_empty_enumeration = () => permutations.ShouldBeEmpty();
    }

    [Subject("enumerable")]
    public class when_enumerable_has_one_element : with_one_element
    {
        private Because of = () => { permutations = enumerable.UniquePermutations(); };

        private It should_have_one_permutation = () => permutations.Count().ShouldEqual(1);
        private It should_have_correct_permutations = () => permutations.ShouldContain(new List<string>{"A"});
    }

    [Subject("enumerable")]
    public class when_enumerable_has_two_elements : with_two_elements
    {
        private Because of = () => { permutations = enumerable.UniquePermutations(); };

        private It should_have_three_permutations = () => permutations.Count().ShouldEqual(3);
        private It should_have_correct_permutations = () => permutations.ShouldContain(
            new [] {
                new [] { "A" },
                new [] { "A", "B"},
                new [] { "B"},
            });
    }

    [Subject("enumerable")]
    public class when_enumerable_has_three_elements : with_three_elements
    {
        private Because of = () => { permutations = enumerable.UniquePermutations(); };

        private It should_have_seven_permutations = () => permutations.Count().ShouldEqual(7);
        private It should_have_correct_permutations = () => permutations.ShouldContain(
            new[] {
                new [] { "A" },
                new [] { "B" },
                new [] { "C" },
                new [] { "A", "B"},
                new [] { "A", "C"},
                new [] { "B", "C"},
                new [] { "A", "B", "C"},
            });
    }

    [Subject("enumerable")]
    public class when_enumerable_has_four_elements : with_four_elements
    {
        private Because of = () => { permutations = enumerable.UniquePermutations(); };

        private It should_have_fifteen_permutations = () => permutations.Count().ShouldEqual(15);
        private It should_have_correct_permutations = () => permutations.ShouldContain(
            new[] {
                new [] { "A" },
                new [] { "B" },
                new [] { "C" },
                new [] { "D" },
                new [] { "A", "B"},
                new [] { "A", "C"},
                new [] { "A", "D"},
                new [] { "B", "C"},
                new [] { "B", "D"},
                new [] { "C", "D"},
                new [] { "A", "B", "C"},
                new [] { "A", "B", "D"},
                new [] { "B", "C", "D"},
                new [] { "A", "B", "C", "D"},
            });
    }


}
