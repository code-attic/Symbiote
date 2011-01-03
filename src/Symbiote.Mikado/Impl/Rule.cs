using System;
using System.Collections.Generic;

namespace Symbiote.Mikado.Impl
{
    public abstract class Rule<T> : IRule<T> where T : class
    {
        public virtual Func<T, bool> RuleDelegate { get; private set; }
        public Func<T, string> DescriptionDelegate { get; set; }

        protected Rule(Func<T, bool> ruleDelegate, string description)
            : this(ruleDelegate, x => description)
        {

        }

        protected Rule(Func<T, bool> ruleDelegate, Func<T, string> descriptionDelegate)
        {
            RuleDelegate = ruleDelegate;
            DescriptionDelegate = descriptionDelegate;
        }
        
        public virtual void ApplyRuleTo( T target, Action<IBrokenRuleNotification> callback )
        {
            if(!RuleDelegate(target))
            {
                callback( new BrokenRuleNotification()
                              {
                                  Description = DescriptionDelegate( target ),
                                  RuleBreaker = target
                              } );
            }
        }

        public virtual void ApplyRule( object target, Action<IBrokenRuleNotification> callback )
        {
            ApplyRuleTo(target as T, callback);
        }
    }
}
