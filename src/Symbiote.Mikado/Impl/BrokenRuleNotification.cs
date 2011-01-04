using System;

namespace Symbiote.Mikado.Impl
{
    public class BrokenRuleNotification : IBrokenRuleNotification
    {
        public string Description { get; set; }
        public object RuleBreaker { get; set; }
        public Type BrokenRuleType { get; set; }
    }
}
