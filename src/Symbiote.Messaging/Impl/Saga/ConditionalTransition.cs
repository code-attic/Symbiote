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

namespace Symbiote.Messaging.Impl.Saga
{
    public class ConditionalTransition<TActor, TMessage> : 
        IConditionalTransition<TActor>
    {
        public Predicate<TActor> Guard { get; set; }
        public Action<TActor> Transition { get; set; }
        public Action<TActor, IEnvelope<TMessage>> Process { get; set; }

        public bool Execute(TActor instance, IEnvelope message)
        {
            var passed = Guard(instance);
            if (passed)
            {
                Process( instance, (IEnvelope<TMessage>) message );
                Transition(instance);
            }
            return passed;
        }

        public ConditionalTransition()
        {
            Guard = x => true;
            Process = ( x, y ) => { };
            Transition = x => { };
        }
    }
}