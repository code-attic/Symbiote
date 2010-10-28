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
    public interface IChannel
    {
        string Name { get; set; }
    }

    public interface IChannel<TMessage>
        : IChannel
    {
        Func<TMessage, string> RoutingMethod { get; set; }
        Func<TMessage, string> CorrelationMethod { get; set; }
        void ExpectReply<TReply>( TMessage message, Action<IEnvelope<TMessage>> modifyEnvelope, IDispatcher dispatcher, Action<TReply> onReply );
        IEnvelope<TMessage> Send(TMessage message);
        IEnvelope<TMessage> Send(TMessage message, Action<IEnvelope<TMessage>> modifyEnvelope);
    }
}