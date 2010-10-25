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
using Symbiote.Messaging.Impl.Dispatch;

namespace Symbiote.Messaging.Impl.Channels
{
    public class LocalChannel<TMessage>
        : IChannel<TMessage>
        where TMessage : class
    {
        protected IDispatcher messageDirector { get; set; }

        public void Send(TMessage message)
        {
            var correlate = (message as ICorrelate);
            var correlationId = correlate == null ? null : correlate.CorrelationId;
            messageDirector.Send(new SimpleEnvelope<TMessage>(
                                     Guid.NewGuid(),
                                     correlationId, 
                                     "NA", 
                                     message));
        }

        public LocalChannel(IDispatcher messageDirector)
        {
            this.messageDirector = messageDirector;
        }
    }
}