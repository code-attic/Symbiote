// /* 
// Copyright 2008-2011 Alex Robson
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// */
using System;
using System.Collections.Generic;
using Symbiote.Messaging.Extensions;

namespace Symbiote.Messaging.Impl.Dispatch
{
    public class ResponseDispatcher<TResponse>
        : IDispatchMessage<TResponse>
    {
        protected Action<TResponse> Handle { get; set; }

        #region IDispatchMessage<TResponse> Members

        public Type ActorType
        {
            get { return null; }
        }

        public bool CanHandle( object payload )
        {
            return payload.IsOfType<TResponse>();
        }

        public bool DispatchForActor
        {
            get { return false; }
        }

        public IEnumerable<Type> Handles
        {
            get { yield return typeof( TResponse ); }
        }

        public void Dispatch( IEnvelope envelope )
        {
            var message = (TResponse) envelope.Message;
            Handle( message );
        }

        #endregion

        public ResponseDispatcher( Action<TResponse> handle )
        {
            Handle = handle;
        }
    }
}