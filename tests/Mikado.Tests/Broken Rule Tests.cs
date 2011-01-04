using Machine.Specifications;
using Mikado.Tests.Domain;
using Mikado.Tests.Domain.Rules;
using Mikado.Tests.TestSetup;
using Symbiote.Mikado.Impl;
using System.Linq;

namespace Mikado.Tests
{
    public class when_testing_broken_rules_on_a_Person : with_Person
    {
        public static DefaultRulesRunner RulesRunner;
        public static TestSubscriber Subscriber = new TestSubscriber();

        private Establish context = () =>
                                        {
                                            Person.Age = -3;
                                            Person.FirstName = "ThisNameIsLongerThanTwentyCharacters";
                                            Person.LastName = "ThisNameIsLongerThanTwentyCharacters";
                                            RulesRunner = new DefaultRulesRunner(new DefaultRulesIndex());
                                            using (var subscriber = RulesRunner.Subscribe(Subscriber))
                                            {
                                                RulesRunner.ApplyRules( Person );
                                            }
                                        };

        private It should_have_three_broken_rules_in_the_Subscriber = () => Subscriber.BrokenRules.Count.ShouldEqual( 3 );
        private It should_break_the_AgeMustBePositiveInteger_rule = () => Subscriber.BrokenRules.Count(s => s.BrokenRuleType == typeof(AgeMustBePositiveInteger)).ShouldEqual( 1 );
        private It should_break_the_FirstNameCannotExceedLengthLimit_rule = () => Subscriber.BrokenRules.Count(s => s.BrokenRuleType == typeof(FirstNameCannotExceedLengthLimit)).ShouldEqual( 1 );
        private It should_break_the_LastNameCannotExceedLengthLimit_rule = () => Subscriber.BrokenRules.Count(s => s.BrokenRuleType == typeof(LastNameCannotExceedLengthLimit)).ShouldEqual( 1 );
    }

    public class when_testing_broken_rules_on_a_Manager : with_Manager
    {
        public static DefaultRulesRunner RulesRunner;
        public static TestSubscriber Subscriber = new TestSubscriber();

        private Establish context = () =>
        {
            Manager.Age = -3;
            Manager.FirstName = "ThisNameIsLongerThanTwentyFiveCharacters";
            Manager.LastName = "ThisNameIsLongerThanTwentyCharacters";
            Manager.Department = "";
            RulesRunner = new DefaultRulesRunner(new DefaultRulesIndex());
            using (var subscriber = RulesRunner.Subscribe(Subscriber))
            {
                RulesRunner.ApplyRules(Manager);
            }
        };

        private It should_have_four_broken_rules_in_the_Subscriber = () => Subscriber.BrokenRules.Count.ShouldEqual(4);
        private It should_break_the_AgeMustBePositiveInteger_rule = () => Subscriber.BrokenRules.Count(s => s.BrokenRuleType == typeof(AgeMustBePositiveInteger)).ShouldEqual( 1 );
        private It should_break_the_FirstNameCannotExceedLengthLimit_rule = () => Subscriber.BrokenRules.Count(s => s.BrokenRuleType == typeof(FirstNameCannotExceedLengthLimit)).ShouldEqual( 1 );
        private It should_break_the_LastNameCannotExceedLengthLimit_rule = () => Subscriber.BrokenRules.Count(s => s.BrokenRuleType == typeof(LastNameCannotExceedLengthLimit)).ShouldEqual( 1 );
        private It should_break_the_DepartmentNameIsRequired_rule = () => Subscriber.BrokenRules.Count(s => s.BrokenRuleType == typeof(DepartmentNameIsRequired)).ShouldEqual(1);
    }
}