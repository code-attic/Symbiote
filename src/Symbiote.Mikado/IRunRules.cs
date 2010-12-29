using System;

namespace Symbiote.Mikado
{
    public interface IRunRules : IObservable<IBrokenRuleNotification>
    {
        IRulesIndex RuleIndex { get; set; }
        void ApplyRules(object target);
    }
}
