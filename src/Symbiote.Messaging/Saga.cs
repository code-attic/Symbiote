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
using System.Collections.Generic;
using System.Linq;
using Symbiote.Messaging.Impl.Saga;

namespace Symbiote.Messaging
{
    public abstract class Saga<TActor>
        : ISaga<TActor>
        where TActor : class
    {
        public StateMachine<TActor> StateMachine { get; set; }

        public Type ActorType
        {
            get { return typeof( TActor ); }
        }

        public List<Type> Handles
        {
            get { return StateMachine.Handles; }
        }

        public abstract Action<StateMachine<TActor>> Setup();

        public Action<IEnvelope> Process<TMessage>( TActor actor, TMessage message )
        {
            Action<IEnvelope> reject = e => e.Reject( "No conditions were met for the current actor's state" );

            var transitions = StateMachine
                .ConditionalTransitions
                .Where( x => x.Handles.Contains( typeof( TMessage ) ) )
                .Where( x => StateMachine.MutuallyExclusive ? x.IsValid( actor ) : true )
                .Select( x => x.Transitions[typeof( TMessage )] )
                .ToList();

            Action<IEnvelope> reply = null;
            if ( StateMachine.MutuallyExclusive )
            {
                var transition = transitions.FirstOrDefault();
                if( transition != null )
                    reply = transition.Execute( actor, message );
            }
            else
            {
                var replies = transitions.Select(x => x.Execute(actor, message)).ToList();
                reply = replies.FirstOrDefault() ?? reject;
            }

            return reply;
        }

        protected Saga( StateMachine<TActor> stateMachine )
        {
            StateMachine = stateMachine;
            Setup()( stateMachine );
        }
    }
}