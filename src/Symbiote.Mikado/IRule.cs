using System;

namespace Symbiote.Mikado
{
    public interface IRule
    {
        void ApplyRule(object target, Action<IBrokenRuleNotification> callback);
    }

    public interface IRule<T> : IRule where T : class
    {
        void ApplyRuleTo(T target, Action<IBrokenRuleNotification> callback);
        Func<T, string> DescriptionDelegate { get; }
    }
}
