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
using Symbiote.Core.Utility;

namespace Symbiote.Actor.Impl.Actor
{
    public class Agency : IAgency
    {
        public IAgentFactory AgentFactory { get; set; }
        public ExclusiveConcurrentDictionary<Type, IAgent> Agents { get; set; }

        public IAgent<TActor> GetAgentFor<TActor>() where TActor : class
        {
            var actorType = typeof (TActor);
            return Agents.ReadOrWrite(actorType, () => AgentFactory.GetAgentFor<TActor>()) as IAgent<TActor>;
        }

        public IAgent GetAgentFor( Type actorType )
        {
            return Agents.ReadOrWrite( actorType, () => AgentFactory.GetAgentFor( actorType ));
        }

        public Agency( IAgentFactory agentFactory )
        {
            AgentFactory = agentFactory;
            Agents = new ExclusiveConcurrentDictionary<Type, IAgent>();
        }
    }
}