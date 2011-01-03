using Machine.Specifications;
using Mikado.Tests.Domain.Model;
using Mikado.Tests.Domain.Rules;
using Mikado.Tests.TestSetup;
using Symbiote.Mikado;
using Symbiote.Mikado.Impl;
using System.Linq;

namespace Mikado.Tests
{
    public class when_testing_rules_loaded_against_a_Person_type : with_Person
    {
        public static DefaultRulesIndex RulesIndex;

        private Establish context = () =>
                                        {
                                            RulesIndex = new DefaultRulesIndex();
                                        };

        private It should_have_three_rules_in_the_TypeRules_dictionary = () => RulesIndex.Rules[typeof(Person)].Count.ShouldEqual(3);
        private It should_have_AgeMustBePositiveInteger_rule_mapped = () => RulesIndex.Rules[typeof(Person)].Count(x => x is AgeMustBePositiveInteger).ShouldEqual(1);
        private It should_have_FirstNameCannotExceedLengthLimit_rule_mapped = () => RulesIndex.Rules[typeof(Person)].Count(x => x is FirstNameCannotExceedLengthLimitRule).ShouldEqual(1);
        private It should_have_LastNameCannotExceedLengthLimit_rule_mapped = () => RulesIndex.Rules[typeof(Person)].Count(x => x is LastNameCannotExceedLengthLimitRule).ShouldEqual(1);
    }

    public class when_testing_rules_loaded_against_a_Manager_type : with_Manager
    {
        public static DefaultRulesIndex RulesIndex;

        private Establish context = () =>
        {
            RulesIndex = new DefaultRulesIndex();
        };

        private It should_have_four_rules_in_the_TypeRules_dictionary = () => RulesIndex.Rules[typeof(Manager)].Count.ShouldEqual(4);
        private It should_have_AgeMustBePositiveInteger_rule_mapped = () => ShouldExtensionMethods.ShouldEqual(RulesIndex.Rules[typeof(Manager)].Count(x => x is AgeMustBePositiveInteger), 1);
        private It should_have_FirstNameCannotExceedLengthLimit_rule_mapped = () => ShouldExtensionMethods.ShouldEqual(RulesIndex.Rules[typeof(Manager)].Count(x => x is FirstNameCannotExceedLengthLimitRule), 1);
        private It should_have_LastNameCannotExceedLengthLimit_rule_mapped = () => ShouldExtensionMethods.ShouldEqual(RulesIndex.Rules[typeof(Manager)].Count(x => x is LastNameCannotExceedLengthLimitRule), 1);
        private It should_have_DepartmentNameIsRequired_rule_mapped = () => ShouldExtensionMethods.ShouldEqual(RulesIndex.Rules[typeof(Manager)].Count(x => x is DepartmentNameIsRequired), 1);
    }
}
