using System;

namespace Symbiote.Mikado
{
    public interface IBrokenRuleNotification
    {
        string Description { get; set; }
        object RuleBreaker { get; set; }
        Type BrokenRuleType { get; set; }
    }
}
