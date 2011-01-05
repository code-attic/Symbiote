using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Mikado.Tests.Domain;
using Mikado.Tests.Domain.Rules;
using Mikado.Tests.TestSetup;
using Symbiote.Mikado.Impl;
using System.Linq;

namespace Mikado.Tests
{
    public class when_testing_passing_rules_on_a_Person : with_Person
    {
        public static DefaultRulesRunner RulesRunner;
        public static TestSubscriber Subscriber = new TestSubscriber();

        private Establish context = () =>
        {
            RulesRunner = new DefaultRulesRunner(new DefaultRulesIndex());
            using (var subscriber = RulesRunner.Subscribe(Subscriber))
            {
                RulesRunner.ApplyRules(Person);
            }
        };

        private It should_have_zero_broken_rules_in_the_Subscriber = () => Subscriber.BrokenRules.Count.ShouldEqual(0);
    }

    public class when_testing_passing_rules_on_a_Manager : with_Manager
    {
        public static DefaultRulesRunner RulesRunner;
        public static TestSubscriber Subscriber = new TestSubscriber();

        private Establish context = () =>
        {
            RulesRunner = new DefaultRulesRunner(new DefaultRulesIndex());
            using (var subscriber = RulesRunner.Subscribe(Subscriber))
            {
                RulesRunner.ApplyRules(Manager);
            }
        };

        private It should_have_zero_broken_rules_in_the_Subscriber = () => Subscriber.BrokenRules.Count.ShouldEqual(0);
    }
}
