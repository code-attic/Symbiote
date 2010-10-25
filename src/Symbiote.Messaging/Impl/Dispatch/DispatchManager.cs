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
using Symbiote.Core;
using Symbiote.Core.Extensions;

namespace Symbiote.Messaging.Impl.Dispatch
{
    public class DispatchManager
        : IDispatcher
    {
        public ConcurrentDictionary<Type, IDispatchMessage> Dispatchers { get; set; }
        public Director<IEnvelope> Fibers { get; set; }
        public int Count { get; set; }

        public void Send<TMessage>(IEnvelope<TMessage> envelope)
             where TMessage : class
        {
            Count++;
            Fibers.SendTo(
                string.IsNullOrEmpty(
                    envelope.CorrelationId) 
                    ? envelope.MessageId.ToString() 
                    : envelope.CorrelationId,
                envelope);
        }

        public void Send(IEnvelope envelope)
        {
            Count++;
            Fibers.SendTo(
                string.IsNullOrEmpty(
                    envelope.CorrelationId)
                    ? envelope.MessageId.ToString()
                    : envelope.CorrelationId,
                envelope);
        }

        public void SendToHandler(string id, IEnvelope envelope)
        {
            IDispatchMessage dispatcher = null;
            if (Dispatchers.TryGetValue(envelope.MessageType, out dispatcher))
            {
                dispatcher.Dispatch(envelope);
            }
        }

        public void WireupDispatchers()
        {
            if (Dispatchers.Count == 0)
            {
                var dispatchers = Assimilate.GetAllInstancesOf<IDispatchMessage>();
                dispatchers
                    .ForEach(x => x.Handles.ForEach(y => Dispatchers.AddOrUpdate((Type) y, (IDispatchMessage) x, (t, m) => x)));
            }
        }

        public DispatchManager()
        {
            Dispatchers = new ConcurrentDictionary<Type, IDispatchMessage>();
            WireupDispatchers();
            Fibers = new Director<IEnvelope>(SendToHandler);
        }
    }
}