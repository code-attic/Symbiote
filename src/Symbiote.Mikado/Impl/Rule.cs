using System;

namespace Symbiote.Mikado.Impl
{
    public abstract class Rule<T> : IRule<T> where T : class
    {
        public virtual Func<T, bool> RuleDelegate { get; private set; }
        public virtual bool IsBroken { get; private set; }
        public virtual string Description { get; private set; }

        protected Rule(Func<T, bool> ruleDelegate, string description)
        {
            RuleDelegate = ruleDelegate;
            Description = description;
        }

        public virtual void ApplyRuleTo(T target)
        {
            IsBroken = !RuleDelegate(target);
        }

        public virtual void ApplyRule(object target)
        {
            ApplyRuleTo(target as T);
        }
    }

}
