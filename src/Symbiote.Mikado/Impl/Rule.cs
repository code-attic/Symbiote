/* 
Copyright 2008-2010 Jim Cowart

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;

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
                                  RuleBreaker = target,
                                  BrokenRuleType = GetType()
                              } );
            }
        }

        public virtual void ApplyRule( object target, Action<IBrokenRuleNotification> callback )
        {
            ApplyRuleTo(target as T, callback);
        }
    }
}
