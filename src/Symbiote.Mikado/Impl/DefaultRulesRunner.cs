using System;
using System.Collections.Generic;
using System.Linq;

namespace Symbiote.Mikado.Impl
{
    public class DefaultRulesRunner : IRunRules
    {
        private List<IObserver<IBrokenRuleNotification>> _observers = new List<IObserver<IBrokenRuleNotification>>();

        public IRulesIndex RuleIndex { get; set; }

        public DefaultRulesRunner(IRulesIndex rulesIndex)
        {
            RuleIndex = rulesIndex;
        }

        public virtual void ApplyRules(object target)
        {
            var rulesToRun = new List<IRule>();
            if(RuleIndex.TypeRules.ContainsKey(target.GetType()))
            {
                RuleIndex.TypeRules[target.GetType()].ToList().ForEach(rulesToRun.Add);
            }
            if (RuleIndex.InstanceRules.ContainsKey(target))
            {
                RuleIndex.InstanceRules[target.GetType()].ToList().ForEach(rulesToRun.Add);
            }

            rulesToRun.ForEach(rule =>
                                   {
                                       rule.ApplyRule(target);
                                       _observers.ForEach(a => a.OnNext(new BrokenRuleNotification() {Rule = rule}));
                                   });
        }

        public virtual IDisposable Subscribe(IObserver<IBrokenRuleNotification> observer)
        {
            _observers.Add(observer);
            return new RuleUnsubscriber(_observers, observer);
        }
    }
}
