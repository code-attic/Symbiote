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

namespace Symbiote.Messaging.Impl.Monitor
{
    public interface IAgentMonitor
    {
        void ActorLoadedFromMemory<TActor>( string id );
        void ActorLoadedFromCache<TActor>( string id );
        void ActorLoadedFromStore<TActor>( string id );
        void ActorMemoized<TActor>( string id );
        void ActorReceivedMessage<TActor, TMessage>( string id, IEnvelope<TMessage> envelope );
        void ActorUnloaded<TActor>( string id );
    }
}