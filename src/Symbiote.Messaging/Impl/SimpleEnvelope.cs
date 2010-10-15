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

namespace Symbiote.Messaging.Impl
{
    public class SimpleEnvelope<TMessage>
        : IEnvelope<TMessage>
         where TMessage : class
    {
        public string CorrelationId { get; set; }
        public string ReturnTo { get; set; }
        public Guid MessageId { get; set; }
        public TMessage Message { get; set; }
        public Type MessageType { get { return typeof (TMessage); } }

        public SimpleEnvelope(Guid id, string correlationId, string returnTo, TMessage message)
        {
            MessageId = id;
            CorrelationId = correlationId;
            ReturnTo = returnTo;
            Message = message;
        }
    }
}