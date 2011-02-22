// /* 
// Copyright 2008-2011 Alex Robson
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// */
using System;
using System.Diagnostics;
using System.Linq.Expressions;
using Symbiote.Core.Extensions;

namespace Symbiote.Messaging.Impl.Saga
{
    [DebuggerDisplay("Transition with guard: {GuardExpression}")]
    public class ConditionalTransition<TActor, TMessage> :
        IConditionalTransition<TActor>
    {
        public Expression<Predicate<TActor>> GuardExpression { get; set; }
        public Predicate<TActor> Guard { get; set; }
        public Func<TActor, Action<IEnvelope>> Transition { get; set; }
        public Func<TActor, TMessage, Action<IEnvelope>> Process { get; set; }

        public Action<IEnvelope> Execute( TActor instance, object message )
        {
            Action<IEnvelope> response = x => { };

            var passed = Guard( instance );
            if ( passed )
            {
                var processResponse = Process( instance, (TMessage) message );
                var transitionResponse = Transition( instance );
                response = x =>
                    {
                        processResponse( x );
                        transitionResponse( x );
                    };
            }

            return response;
        }

        public ConditionalTransition()
        {
            Guard = x => true;
            GuardExpression = x => true;
            Process = (x, y) => { return e => { }; };
            Transition = x => { return e => { }; };
        }
    }
}