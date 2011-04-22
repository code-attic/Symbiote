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
                                            Person.Address.Address = "";
                                            Person.Address.City = "";
                                            Person.Address.State = "";
                                            Person.Address.ZipCode = "";
                                            RulesRunner = new DefaultRulesRunner(new DefaultRulesIndex());
                                            using (var subscriber = RulesRunner.Subscribe(Subscriber))
                                            {
                                                RulesRunner.ApplyRules( Person );
                                            }
                                        };

        private It should_have_seven_broken_rules_in_the_Subscriber = () => Subscriber.BrokenRules.Count.ShouldEqual( 7 );
        private It should_break_the_AgeMustBePositiveInteger_rule = () => Subscriber.BrokenRules.Count(s => s.BrokenRuleType == typeof(AgeMustBePositiveInteger)).ShouldEqual( 1 );
        private It should_break_the_FirstNameCannotExceedLengthLimit_rule = () => Subscriber.BrokenRules.Count(s => s.BrokenRuleType == typeof(FirstNameCannotExceedLengthLimit)).ShouldEqual( 1 );
        private It should_break_the_LastNameCannotExceedLengthLimit_rule = () => Subscriber.BrokenRules.Count(s => s.BrokenRuleType == typeof(LastNameCannotExceedLengthLimit)).ShouldEqual(1);
        private It should_break_the_AddressIsRequired_rule = () => Subscriber.BrokenRules.Count(s => s.BrokenRuleType == typeof(AddressIsRequired)).ShouldEqual(1);
        private It should_break_the_CityIsRequired_rule = () => Subscriber.BrokenRules.Count(s => s.BrokenRuleType == typeof(CityIsRequired)).ShouldEqual(1);
        private It should_break_the_StateIsRequired_rule = () => Subscriber.BrokenRules.Count(s => s.BrokenRuleType == typeof(StateIsRequired)).ShouldEqual(1);
        private It should_break_the_ZipCodeIsRequired_rule = () => Subscriber.BrokenRules.Count(s => s.BrokenRuleType == typeof(ZipCodeIsRequired)).ShouldEqual(1);
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
                                            Manager.Address.Address = "";
                                            Manager.Address.City = "";
                                            Manager.Address.State = "";
                                            Manager.Address.ZipCode = "";
                                            RulesRunner = new DefaultRulesRunner( new DefaultRulesIndex() );
                                            using( var subscriber = RulesRunner.Subscribe( Subscriber ) )
                                            {
                                                RulesRunner.ApplyRules( Manager );
                                            }
                                        };

        private It should_have_eight_broken_rules_in_the_Subscriber = () => Subscriber.BrokenRules.Count.ShouldEqual(8);
        private It should_break_the_AgeMustBePositiveInteger_rule = () => Subscriber.BrokenRules.Count(s => s.BrokenRuleType == typeof(AgeMustBePositiveInteger)).ShouldEqual( 1 );
        private It should_break_the_FirstNameCannotExceedLengthLimit_rule = () => Subscriber.BrokenRules.Count(s => s.BrokenRuleType == typeof(FirstNameCannotExceedLengthLimit)).ShouldEqual( 1 );
        private It should_break_the_LastNameCannotExceedLengthLimit_rule = () => Subscriber.BrokenRules.Count(s => s.BrokenRuleType == typeof(LastNameCannotExceedLengthLimit)).ShouldEqual( 1 );
        private It should_break_the_DepartmentNameIsRequired_rule = () => Subscriber.BrokenRules.Count(s => s.BrokenRuleType == typeof(DepartmentNameIsRequired)).ShouldEqual(1);
        private It should_break_the_AddressIsRequired_rule = () => Subscriber.BrokenRules.Count(s => s.BrokenRuleType == typeof(AddressIsRequired)).ShouldEqual(1);
        private It should_break_the_CityIsRequired_rule = () => Subscriber.BrokenRules.Count(s => s.BrokenRuleType == typeof(CityIsRequired)).ShouldEqual(1);
        private It should_break_the_StateIsRequired_rule = () => Subscriber.BrokenRules.Count(s => s.BrokenRuleType == typeof(StateIsRequired)).ShouldEqual(1);
        private It should_break_the_ZipCodeIsRequired_rule = () => Subscriber.BrokenRules.Count(s => s.BrokenRuleType == typeof(ZipCodeIsRequired)).ShouldEqual(1);
    }
}