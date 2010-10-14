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
using System.Collections.Concurrent;
using Microsoft.Practices.ServiceLocation;

namespace Symbiote.Messaging.Impl.Actors
{
    public class Agency : IAgency
    {
        protected ConcurrentDictionary<Type, IAgent> Agents { get; set; }

        public IAgent<TActor> GetAgentFor<TActor>() where TActor : class
        {
            IAgent agent = null;
            var actorType = typeof (TActor);
            if(!Agents.TryGetValue(actorType, out agent))
            {
                agent = ServiceLocator.Current.GetInstance<IAgent<TActor>>();
                Agents.TryAdd(actorType, agent);
            }
            return agent as IAgent<TActor>;
        }

        public Agency()
        {
            Agents = new ConcurrentDictionary<Type,IAgent>();
        }
    }
}