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
using Symbiote.Actor.Impl;
using Symbiote.Actor.Impl.Saga;
using Symbiote.Core.Impl.Reflection;
using Symbiote.Messaging.Extensions;

namespace Symbiote.Messaging.Impl.Dispatch
{
    public class SagaMessageDispatcher<TSaga, TActor, TMessage>
        : IDispatchToSaga<TSaga, TActor, TMessage>
        where TSaga : class
        where TActor : class
    {
        protected IEnumerable<Type> HandlesMessagesOf { get; set; }
        protected IAgency Agency { get; set; }
        protected IAgent<TActor> Agent { get; set; }
        protected ISaga<TActor> Saga { get; set; }

        public Type ActorType
        {
            get { return typeof(TActor); }
        }

        public bool CanHandle(object payload)
        {
            return payload.IsOfType<TMessage>();
        }

        public bool DispatchForActor
        {
            get { return true; }
        }

        public IEnumerable<Type> Handles
        {
            get
            {
                HandlesMessagesOf = HandlesMessagesOf ?? GetMessageChain().ToList();
                return HandlesMessagesOf;
            }
        }

        private IEnumerable<Type> GetMessageChain()
        {
            yield return typeof(TMessage);
            var chain = Reflector.GetInheritanceChain(typeof(TMessage));
            if (chain != null)
            {
                foreach (var type in chain)
                {
                    yield return type;
                }
            }
            yield break;
        }

        public void Dispatch(IEnvelope envelope)
        {
            var typedEnvelope = envelope as IEnvelope<TMessage>;
            var actor = Agent.GetActor(envelope.CorrelationId);
            Saga.Process( actor, typedEnvelope );
            Agent.Memoize(actor);
        }

        public SagaMessageDispatcher(IAgency agency, ISaga<TActor> saga)
        {
            Agency = agency;
            Agent = Agency.GetAgentFor<TActor>();
            Saga = saga;
        }
    }
}