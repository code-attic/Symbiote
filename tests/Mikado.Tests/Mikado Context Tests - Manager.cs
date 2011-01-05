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
using Symbiote.Core.Memento;
using Symbiote.Core.Work;
using Symbiote.Mikado;
using Symbiote.Mikado.Impl;

namespace Mikado.Tests
{
    public class when_testing_passing_rules_on_a_Manager_in_a_Mikado_Context : with_Manager
    {
        public static TestSubscriber Subscriber = new TestSubscriber();

        private Because of = () =>
                                 {
                                     var runner = Assimilate.GetInstanceOf<IRunRules>();
                                     var provider = Assimilate.GetInstanceOf<IContextProvider>();
                                     using (var subscription = runner.Subscribe( Subscriber ))
                                     using (var context = provider.GetContext( Manager ))
                                     {
                                         Manager.Age = 24;
                                         Manager.FirstName = "Bugs";
                                         Manager.LastName = "Bunny";
                                         Manager.Department = "AppDev";
                                     }
                                 };

        private It should_have_zero_broken_rules = () => Subscriber.BrokenRules.Count.ShouldEqual( 0 );
        private It should_have_set_the_Manager_FirstName_to_Bugs = () => Manager.FirstName.ShouldEqual( "Bugs" );
        private It should_have_set_the_Manager_LastName_to_Bugs = () => Manager.LastName.ShouldEqual( "Bunny" );
        private It should_have_set_the_Manager_Age_to_24 = () => Manager.Age.ShouldEqual( 24 );

    }

    public class when_testing_all_broken_rules_on_a_Manager_in_a_Mikado_Context : with_Manager
    {
        public static TestSubscriber Subscriber = new TestSubscriber();

        private Because of = () =>
        {
            var runner = Assimilate.GetInstanceOf<IRunRules>();
            var provider = Assimilate.GetInstanceOf<IContextProvider>();
            using (var subscription = runner.Subscribe(Subscriber))
            using (var context = provider.GetContext(Manager))
            {
                Manager.Age = -24;
                Manager.FirstName = "ThisValueIsWayTooLongForTheRuleToAllowItToPass";
                Manager.LastName = "ThisValueIsWayTooLongForTheRuleToAllowItToPass";
                Manager.Department = "";
            }
        };

        private It should_have_four_broken_rules = () => Subscriber.BrokenRules.Count.ShouldEqual(4);
        private It should_have_reverted_the_Manager_FirstName_to_Jim = () => Manager.FirstName.ShouldEqual("Jim");
        private It should_have_reverted_the_Manager_LastName_to_Cowart = () => Manager.LastName.ShouldEqual("Cowart");
        private It should_have_reverted_the_Manager_Age_to_37 = () => Manager.Age.ShouldEqual(37);
        private It should_have_reverted_the_Department_to_Development = () => Manager.Department.ShouldEqual("Development");
        private It should_break_the_AgeMustBePositiveInteger_rule = () => Subscriber.BrokenRules.Count(s => s.BrokenRuleType == typeof(AgeMustBePositiveInteger)).ShouldEqual(1);
        private It should_break_the_FirstNameCannotExceedLengthLimit_rule = () => Subscriber.BrokenRules.Count(s => s.BrokenRuleType == typeof(FirstNameCannotExceedLengthLimit)).ShouldEqual(1);
        private It should_break_the_LastNameCannotExceedLengthLimit_rule = () => Subscriber.BrokenRules.Count(s => s.BrokenRuleType == typeof(LastNameCannotExceedLengthLimit)).ShouldEqual(1);
        private It should_break_the_DepartmentNameIsRequired_rule = () => Subscriber.BrokenRules.Count(s => s.BrokenRuleType == typeof(DepartmentNameIsRequired)).ShouldEqual(1);
    }

    public class when_testing_broken_FirstName_rule_on_a_Manager_in_a_Mikado_Context : with_Manager
    {
        public static TestSubscriber Subscriber = new TestSubscriber();

        private Because of = () =>
        {
            var runner = Assimilate.GetInstanceOf<IRunRules>();
            var provider = Assimilate.GetInstanceOf<IContextProvider>();
            using (var subscription = runner.Subscribe(Subscriber))
            using (var context = provider.GetContext(Manager))
            {
                Manager.FirstName = "ThisValueIsWayTooLongForTheRuleToAllowItToPass";
            }
        };

        private It should_have_one_broken_rule = () => Subscriber.BrokenRules.Count.ShouldEqual(1);
        private It should_have_reverted_the_Manager_FirstName_to_Jim = () => Manager.FirstName.ShouldEqual("Jim");
        private It should_have_reverted_the_Manager_LastName_to_Cowart = () => Manager.LastName.ShouldEqual("Cowart");
        private It should_have_reverted_the_Manager_Age_to_37 = () => Manager.Age.ShouldEqual(37);
        private It should_have_reverted_the_Department_to_Development = () => Manager.Department.ShouldEqual("Development");
        private It should_break_the_FirstNameCannotExceedLengthLimit_rule = () => Subscriber.BrokenRules.Count(s => s.BrokenRuleType == typeof(FirstNameCannotExceedLengthLimit)).ShouldEqual(1);
    }

    public class when_testing_LastName_broken_rule_on_a_Manager_in_a_Mikado_Context : with_Manager
    {
        public static TestSubscriber Subscriber = new TestSubscriber();

        private Because of = () =>
        {
            var runner = Assimilate.GetInstanceOf<IRunRules>();
            var provider = Assimilate.GetInstanceOf<IContextProvider>();
            using (var subscription = runner.Subscribe(Subscriber))
            using (var context = provider.GetContext(Manager))
            {
                Manager.LastName = "ThisValueIsWayTooLongForTheRuleToAllowItToPass";
            }
        };

        private It should_have_one_broken_rule = () => Subscriber.BrokenRules.Count.ShouldEqual(1);
        private It should_have_reverted_the_Manager_FirstName_to_Jim = () => Manager.FirstName.ShouldEqual("Jim");
        private It should_have_reverted_the_Manager_LastName_to_Cowart = () => Manager.LastName.ShouldEqual("Cowart");
        private It should_have_reverted_the_Manager_Age_to_37 = () => Manager.Age.ShouldEqual(37);
        private It should_have_reverted_the_Department_to_Development = () => Manager.Department.ShouldEqual("Development");
        private It should_break_the_LastNameCannotExceedLengthLimit_rule = () => Subscriber.BrokenRules.Count(s => s.BrokenRuleType == typeof(LastNameCannotExceedLengthLimit)).ShouldEqual(1);
    }

    public class when_testing_Age_broken_rule_on_a_Manager_in_a_Mikado_Context : with_Manager
    {
        public static TestSubscriber Subscriber = new TestSubscriber();

        private Because of = () =>
        {
            var runner = Assimilate.GetInstanceOf<IRunRules>();
            var provider = Assimilate.GetInstanceOf<IContextProvider>();
            using (var subscription = runner.Subscribe(Subscriber))
            using (var context = provider.GetContext(Manager))
            {
                Manager.Age = -24;
            }
        };

        private It should_have_one_broken_rule = () => Subscriber.BrokenRules.Count.ShouldEqual(1);
        private It should_have_reverted_the_Manager_FirstName_to_Jim = () => Manager.FirstName.ShouldEqual("Jim");
        private It should_have_reverted_the_Manager_LastName_to_Cowart = () => Manager.LastName.ShouldEqual("Cowart");
        private It should_have_reverted_the_Manager_Age_to_37 = () => Manager.Age.ShouldEqual(37);
        private It should_have_reverted_the_Department_to_Development = () => Manager.Department.ShouldEqual("Development");
        private It should_break_the_AgeMustBePositiveInteger_rule = () => Subscriber.BrokenRules.Count(s => s.BrokenRuleType == typeof(AgeMustBePositiveInteger)).ShouldEqual(1);
    }

    public class when_testing_Department_broken_rule_on_a_Manager_in_a_Mikado_Context : with_Manager
    {
        public static TestSubscriber Subscriber = new TestSubscriber();

        private Because of = () =>
        {
            var runner = Assimilate.GetInstanceOf<IRunRules>();
            var provider = Assimilate.GetInstanceOf<IContextProvider>();
            using (var subscription = runner.Subscribe(Subscriber))
            using (var context = provider.GetContext(Manager))
            {
                Manager.Department = "";
            }
        };

        private It should_have_one_broken_rule = () => Subscriber.BrokenRules.Count.ShouldEqual(1);
        private It should_have_reverted_the_Manager_FirstName_to_Jim = () => Manager.FirstName.ShouldEqual("Jim");
        private It should_have_reverted_the_Manager_LastName_to_Cowart = () => Manager.LastName.ShouldEqual("Cowart");
        private It should_have_reverted_the_Manager_Age_to_37 = () => Manager.Age.ShouldEqual(37);
        private It should_have_reverted_the_Department_to_Development = () => Manager.Department.ShouldEqual("Development");
        private It should_break_the_DepartmentNameIsRequired_rule = () => Subscriber.BrokenRules.Count(s => s.BrokenRuleType == typeof(DepartmentNameIsRequired)).ShouldEqual(1);
    }
}
