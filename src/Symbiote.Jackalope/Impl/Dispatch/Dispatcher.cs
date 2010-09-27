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
using Microsoft.Practices.ServiceLocation;
using Symbiote.Core.Reflection;
using System.Linq;

namespace Symbiote.Jackalope.Impl.Dispatch
{
    public class Dispatcher<TMessage> : IDispatch<TMessage>
        where TMessage : class
    {
        protected List<Type> handlesMessagesOf { get; set; }

        public bool CanHandle(object payload)
        {
            return payload as TMessage != null;
        }

        public IEnumerable<Type> Handles
        {
            get
            {
                handlesMessagesOf = handlesMessagesOf ?? GetMessageChain().ToList();
                return handlesMessagesOf;
            }
        }

        private IEnumerable<Type> GetMessageChain()
        {
            yield return typeof (TMessage);
            var chain = Reflector.GetInheritenceChain(typeof (TMessage));
            if(chain != null)
            {
                foreach (var type in chain)
                {
                    yield return type;
                }
            }
            yield break;
        }

        public void Dispatch(Envelope envelope)
        {
            try
            {
                var handler = ServiceLocator.Current.GetInstance<IMessageHandler<TMessage>>();
                handler.Process(envelope.Message as TMessage, envelope.MessageDelivery);
            }
            catch (Exception e)
            {
                envelope.MessageDelivery.Reject();
                //throw;
            }
        }
    }
}