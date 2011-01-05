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
using Symbiote.Core.Impl.Futures;

namespace Symbiote.Messaging
{
    public interface IBus
    {
        /// <summary>
        /// Checks to see if any typed channels have been defined
        /// for message type T with the specified name.
        /// </summary>
        bool HasChannelFor(string channelName);

        /// <summary>
        /// Publish a message to the named channel.
        /// </summary>
        void Publish<TMessage>(string channelName, TMessage message);
        
        /// <summary>
        /// Publish a message to the named channel defined for
        /// the message type after modifying the envelope according
        /// to the provided delegate.
        /// </summary>
        void Publish<TMessage>(string channelName, TMessage message, Action<IEnvelope> modifyEnvelope);

        /// <summary>
        /// Publish a message to the named channel with an expected response.
        /// </summary>
        Future<TResponse> Request<TMessage, TResponse>(string channelName, TMessage message);

        /// <summary>
        /// Publish a message to the named channel defined for
        /// the message type after modifying the envelope according
        /// to the provided delegate.
        /// </summary>
        Future<TResponse> Request<TMessage, TResponse>(string channelName, TMessage message, Action<IEnvelope> modifyEnvelope);

        /// <summary>
        /// Starts listening for messages associated with the named subscription.
        /// </summary>
        void StartSubscription(string subscription);

        /// <summary>
        /// Stops listening for messages associated with the named subscription.
        /// </summary>
        void StopSubscription(string subscription);
    }
}