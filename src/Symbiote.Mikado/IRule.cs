namespace Symbiote.Mikado
{
    public interface IRule
    {
        void ApplyRule(object target);
        string Description { get; }
        bool IsBroken { get; }
    }

    public interface IRule<T> : IRule where T : class
    {
        void ApplyRuleTo(T target);
    }
}
