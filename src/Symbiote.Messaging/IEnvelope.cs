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

namespace Symbiote.Messaging
{
    public interface IEnvelope
    {
        string CorrelationId { get; set; }

        long Sequence { get; set; }
        long Position { get; set; }
        bool SequenceEnd { get; set; }

        Guid MessageId { get; set; }
        Type MessageType { get; }
    }

    public interface IEnvelope<TMessage>
        : IEnvelope
        where TMessage : class
    {
        TMessage Message { get; set; }
    }

    public abstract class BaseEnvelope<TMessage>
        : IEnvelope<TMessage>
        where TMessage : class
    {
        public string CorrelationId { get; set; }
        public long Sequence { get; set; }
        public long Position { get; set; }
        public bool SequenceEnd { get; set; }
        public Guid MessageId { get; set; }
        public Type MessageType { get; protected set; }
        public TMessage Message { get; set; }
    }

    public class LocalEnvelope<TMessage>
        : BaseEnvelope<TMessage>
        where TMessage : class
    {
        public string ReturnTo { get; set; }

        public LocalEnvelope(Guid id, string correlationId, string returnTo, TMessage message)
        {
            MessageId = id;
            CorrelationId = correlationId;
            ReturnTo = returnTo;
            Message = message;
        }
    }
}