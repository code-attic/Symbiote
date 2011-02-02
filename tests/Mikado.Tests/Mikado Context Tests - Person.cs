using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Mikado.Tests.Domain;
using Mikado.Tests.Domain.Model;
using Mikado.Tests.Domain.Rules;
using Mikado.Tests.TestSetup;
using Symbiote.Core;
using Symbiote.Core.UnitOfWork;
using Symbiote.Mikado;
using Symbiote.Mikado.Extensions;
using Symbiote.Mikado.Impl;

namespace Mikado.Tests
{
    public class when_testing_passing_rules_on_a_Person_in_a_Mikado_Context : with_Person
    {
        public static TestSubscriber Subscriber = new TestSubscriber();

        private Because of = () =>
                                 {
                                     var runner = Assimilate.GetInstanceOf<IRunRules>();
                                     var provider = Assimilate.GetInstanceOf<IContextProvider>();
                                     using (var subscription = runner.Subscribe( Subscriber ))
                                     using (var context = provider.GetContext( Person ))
                                     {
                                         Person.Age = 24;
                                         Person.FirstName = "Bugs";
                                         Person.LastName = "Bunny";
                                     }
                                 };

        private It should_have_zero_broken_rules = () => Subscriber.BrokenRules.Count.ShouldEqual( 0 );
        private It should_have_set_the_Person_FirstName_to_Bugs = () => Person.FirstName.ShouldEqual( "Bugs" );
        private It should_have_set_the_Person_LastName_to_Bugs = () => Person.LastName.ShouldEqual( "Bunny" );
        private It should_have_set_the_Person_Age_to_24 = () => Person.Age.ShouldEqual( 24 );

    }

    public class when_testing_all_broken_rules_on_a_Person_in_a_Mikado_Context : with_Person
    {
        public static TestSubscriber Subscriber = new TestSubscriber();

        private Because of = () =>
        {
            var runner = Assimilate.GetInstanceOf<IRunRules>();
            var provider = Assimilate.GetInstanceOf<IContextProvider>();
            using (var subscription = runner.Subscribe(Subscriber))
            using (var context = provider.GetContext(Person))
            {
                Person.Age = -24;
                Person.FirstName = "ThisValueIsWayTooLongForTheRuleToAllowItToPass";
                Person.LastName = "ThisValueIsWayTooLongForTheRuleToAllowItToPass";
            }
        };

        private It should_have_three_broken_rules = () => Subscriber.BrokenRules.Count.ShouldEqual(3);
        private It should_have_reverted_the_Person_FirstName_to_Jim = () => Person.FirstName.ShouldEqual("Jim");
        private It should_have_reverted_the_Person_LastName_to_Cowart = () => Person.LastName.ShouldEqual("Cowart");
        private It should_have_reverted_the_Person_Age_to_37 = () => Person.Age.ShouldEqual(37);
        private It should_break_the_AgeMustBePositiveInteger_rule = () => Subscriber.BrokenRules.Count(s => s.BrokenRuleType == typeof(AgeMustBePositiveInteger)).ShouldEqual(1);
        private It should_break_the_FirstNameCannotExceedLengthLimit_rule = () => Subscriber.BrokenRules.Count(s => s.BrokenRuleType == typeof(FirstNameCannotExceedLengthLimit)).ShouldEqual(1);
        private It should_break_the_LastNameCannotExceedLengthLimit_rule = () => Subscriber.BrokenRules.Count(s => s.BrokenRuleType == typeof(LastNameCannotExceedLengthLimit)).ShouldEqual(1);
    }

    public class when_testing_broken_FirstName_rule_on_a_Person_in_a_Mikado_Context : with_Person
    {
        public static TestSubscriber Subscriber = new TestSubscriber();

        private Because of = () =>
        {
            var runner = Assimilate.GetInstanceOf<IRunRules>();
            var provider = Assimilate.GetInstanceOf<IContextProvider>();
            using (var subscription = runner.Subscribe(Subscriber))
            using (var context = provider.GetContext(Person))
            {
                Person.FirstName = "ThisValueIsWayTooLongForTheRuleToAllowItToPass";
            }
        };

        private It should_have_one_broken_rule = () => Subscriber.BrokenRules.Count.ShouldEqual(1);
        private It should_have_reverted_the_Person_FirstName_to_Jim = () => Person.FirstName.ShouldEqual("Jim");
        private It should_have_reverted_the_Person_LastName_to_Cowart = () => Person.LastName.ShouldEqual("Cowart");
        private It should_have_reverted_the_Person_Age_to_37 = () => Person.Age.ShouldEqual(37);
        private It should_break_the_FirstNameCannotExceedLengthLimit_rule = () => Subscriber.BrokenRules.Count(s => s.BrokenRuleType == typeof(FirstNameCannotExceedLengthLimit)).ShouldEqual(1);
    }

    public class when_testing_LastName_broken_rule_on_a_Person_in_a_Mikado_Context : with_Person
    {
        public static TestSubscriber Subscriber = new TestSubscriber();

        private Because of = () =>
        {
            var runner = Assimilate.GetInstanceOf<IRunRules>();
            var provider = Assimilate.GetInstanceOf<IContextProvider>();
            using (var subscription = runner.Subscribe(Subscriber))
            using (var context = provider.GetContext(Person))
            {
                Person.LastName = "ThisValueIsWayTooLongForTheRuleToAllowItToPass";
            }
        };

        private It should_have_one_broken_rule = () => Subscriber.BrokenRules.Count.ShouldEqual(1);
        private It should_have_reverted_the_Person_FirstName_to_Jim = () => Person.FirstName.ShouldEqual("Jim");
        private It should_have_reverted_the_Person_LastName_to_Cowart = () => Person.LastName.ShouldEqual("Cowart");
        private It should_have_reverted_the_Person_Age_to_37 = () => Person.Age.ShouldEqual(37);
        private It should_break_the_LastNameCannotExceedLengthLimit_rule = () => Subscriber.BrokenRules.Count(s => s.BrokenRuleType == typeof(LastNameCannotExceedLengthLimit)).ShouldEqual(1);
    }

    public class when_testing_Age_broken_rule_on_a_Person_in_a_Mikado_Context : with_Person
    {
        public static TestSubscriber Subscriber = new TestSubscriber();

        private Because of = () =>
        {
            var runner = Assimilate.GetInstanceOf<IRunRules>();
            var provider = Assimilate.GetInstanceOf<IContextProvider>();
            using (var subscription = runner.Subscribe(Subscriber))
            using (var context = provider.GetContext(Person))
            {
                Person.Age = -24;
            }
        };

        private It should_have_one_broken_rule = () => Subscriber.BrokenRules.Count.ShouldEqual(1);
        private It should_have_reverted_the_Person_FirstName_to_Jim = () => Person.FirstName.ShouldEqual("Jim");
        private It should_have_reverted_the_Person_LastName_to_Cowart = () => Person.LastName.ShouldEqual("Cowart");
        private It should_have_reverted_the_Person_Age_to_37 = () => Person.Age.ShouldEqual(37);
        private It should_break_the_AgeMustBePositiveInteger_rule = () => Subscriber.BrokenRules.Count(s => s.BrokenRuleType == typeof(AgeMustBePositiveInteger)).ShouldEqual(1);
    }

    public class when_testing_success_failure_and_BrokenRule_actions_on_a_MikadoContext_with_breaking_rules : with_Person
    {
        public static TestSubscriber Subscriber = new TestSubscriber();
        public static bool SuccessActionFired;
        public static bool FailureActionFired;
        public static bool BrokenRuleAction;

        private Because of = () =>
        {
            var runner = Assimilate.GetInstanceOf<IRunRules>();
            var provider = Assimilate.GetInstanceOf<IContextProvider>();
            using (var subscription = runner.Subscribe(Subscriber))
            using (var context = provider.GetContext(Person)
                                         .OnSuccess(x => SuccessActionFired = true)
                                         .OnException((a, e) => FailureActionFired = true)
                                         .HandleBrokenRules<Person>((actor,rules) => BrokenRuleAction = true))
            {
                Person.Age = -24;
            }
        };

        private It should_not_have_fired_the_onSuccess_action = () => SuccessActionFired.ShouldBeFalse();
        private It should_have_fired_the_BrokenRuleAction_action = () => BrokenRuleAction.ShouldBeTrue();
        private It should_not_have_fired_the_onFailure_action = () => FailureActionFired.ShouldBeFalse();
    }

    public class when_testing_success_failure_and_BrokenRule_actions_on_a_MikadoContext_with_passing_rules : with_Person
    {
        public static TestSubscriber Subscriber = new TestSubscriber();
        public static bool SuccessActionFired;
        public static bool FailureActionFired;
        public static bool BrokenRuleAction;

        private Because of = () =>
        {
            var runner = Assimilate.GetInstanceOf<IRunRules>();
            var provider = Assimilate.GetInstanceOf<IContextProvider>();
            using (var subscription = runner.Subscribe(Subscriber))
            using (var context = provider.GetContext(Person)
                                         .OnSuccess(x => SuccessActionFired = true)
                                         .OnException((a, e) => FailureActionFired = true)
                                         .HandleBrokenRules<Person>((actor, rules) => BrokenRuleAction = true))
            {
                Person.Age = 35;
            }
        };

        private It should_have_fired_the_onSuccess_action = () => SuccessActionFired.ShouldBeTrue();
        private It should_not_have_fired_the_BrokenRuleAction_action = () => BrokenRuleAction.ShouldBeFalse();
        private It should_not_have_fired_the_onFailure_action = () => FailureActionFired.ShouldBeFalse();
    }

    public class when_testing_success_failure_and_BrokenRule_actions_on_a_MikadoContext_with_exception_thrown : with_Person
    {
        public static TestSubscriber Subscriber = new TestSubscriber() { ThrowException = true };
        public static bool SuccessActionFired;
        public static bool FailureActionFired;
        public static bool BrokenRuleAction;

        private Because of = () =>
        {
            var runner = Assimilate.GetInstanceOf<IRunRules>();
            var provider = Assimilate.GetInstanceOf<IContextProvider>();
            using (var subscription = runner.Subscribe(Subscriber))
            using (var context = provider.GetContext(Person)
                                         .OnSuccess(x => SuccessActionFired = true)
                                         .OnException((a, e) => FailureActionFired = true)
                                         .HandleBrokenRules<Person>((actor, rules) => BrokenRuleAction = true))
            {
                Person.Age = -24;
            }
        };

        private It should_not_have_fired_the_onSuccess_action = () => SuccessActionFired.ShouldBeFalse();
        private It should_not_have_fired_the_BrokenRuleAction_action = () => BrokenRuleAction.ShouldBeFalse();
        private It should_have_fired_the_onFailure_action = () => FailureActionFired.ShouldBeTrue();
    }
}
