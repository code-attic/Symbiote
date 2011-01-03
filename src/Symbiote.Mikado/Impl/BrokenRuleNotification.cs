namespace Symbiote.Mikado.Impl
{
    public class BrokenRuleNotification : IBrokenRuleNotification
    {
        public string Description { get; set; }
        public object RuleBreaker { get; set; }
    }
}
