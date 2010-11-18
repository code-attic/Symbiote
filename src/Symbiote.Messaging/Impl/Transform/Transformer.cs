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
using Symbiote.Core;
using Symbiote.Core.Extensions;

namespace Symbiote.Messaging.Impl.Transform
{
    public class Transformer
    {
        public List<Type> Phases { get; set; }

        public Transformer Then<TPhase>()
            where TPhase : ITransform
        {
            return Then( typeof(TPhase) );
        }

        public Transformer Then(Type transformer)
        {
            if (Validate(transformer))
                Phases.Add(transformer);
            return this;
        }

        protected bool Validate(Type newPhase)
        {
            return Phases.Count == 0 || GetInputType( newPhase ).IsAssignableFrom( GetOutputType( Phases.Last() ) );
        }

        protected Type GetInputType(Type transform)
        {
            return transform.BaseType.GetGenericArguments()[0];
        }

        protected Type GetOutputType(Type transform)
        {
            return transform.BaseType.GetGenericArguments()[1];
        }

        public TTo Transform<TFrom, TTo>(TFrom startWith)
        {
            object state = startWith;
            Phases
                .Select(x => Assimilate.GetInstanceOf(x) as ITransform)
                .ForEach(x => state = x.To(state));
            return (TTo)state;
        }

        public TFrom Reverse<TTo, TFrom>(TTo endsWith)
        {
            object state = endsWith;
            Phases
                .Select(x => Assimilate.GetInstanceOf(x) as ITransform)
                .Reverse()
                .ForEach(x => state = x.From(state));
            return (TFrom)state;
        }

        public Transformer()
        {
            Phases = new List<Type>();
        }
    }
}
