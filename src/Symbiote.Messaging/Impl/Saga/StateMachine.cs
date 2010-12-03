/* 
Copyright 2008-2010 Alex Robson

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
using System.Collections.Generic;
using System.Linq;

namespace Symbiote.Messaging.Impl.Saga
{
    public class StateMachine<TActor>
        where TActor : class
    {
        protected List<Type> MessagesHandled { get; set; }
        public List<Condition<TActor>> ConditionalTransitions { get; set; }

        public List<Type> Handles
        {
            get
            {
                MessagesHandled = MessagesHandled ?? ConditionalTransitions.SelectMany(x => x.Handles).ToList();
                return MessagesHandled;
            }
        }

        public ICondition<TActor> Unconditionally()
        {
            var unconditionally = new Condition<TActor>();
            ConditionalTransitions.Add( unconditionally );
            return unconditionally;
        }

        public ICondition<TActor> When(Predicate<TActor> predicate)
        {
            var condition = new Condition<TActor>(predicate);
            ConditionalTransitions.Add(condition);
            return condition;
        }

        public StateMachine( )
        {
            ConditionalTransitions = new List<Condition<TActor>>();
        }
    }
}