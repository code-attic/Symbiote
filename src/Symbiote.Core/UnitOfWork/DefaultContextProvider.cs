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
using Symbiote.Core.Memento;

namespace Symbiote.Core.UnitOfWork
{
    /// <summary>
    /// 
    /// </summary>
    public class DefaultContextProvider : IContextProvider
    {
        /// <summary>
        /// 
        /// </summary>
        public IEventConfiguration Configuration { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public IMemoizer Memoizer { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public IKeyAccessor KeyAccessor { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public IEventPublisher Publisher { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TActor"></typeparam>
        /// <param name="actor"></param>
        /// <returns></returns>
        public IContext GetContext<TActor>( TActor actor )
            where TActor : class
        {
            return GetContext( actor, null, null, null );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TActor"></typeparam>
        /// <param name="actor"></param>
        /// <param name="listeners"></param>
        /// <returns></returns>
        public IContext GetContext<TActor>( TActor actor, IEnumerable<IObserver<IEvent>> listeners )
            where TActor : class
        {
            return GetContext( actor, listeners, null, null );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TActor"></typeparam>
        /// <param name="actor"></param>
        /// <param name="successAction"></param>
        /// <param name="failureAction"></param>
        /// <returns></returns>
        public IContext GetContext<TActor>( TActor actor, Action<TActor> successAction,
                                            Action<TActor, Exception> failureAction ) where TActor : class
        {
            return GetContext( actor, null, successAction, failureAction );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TActor"></typeparam>
        /// <param name="actor"></param>
        /// <param name="listeners"></param>
        /// <param name="successAction"></param>
        /// <param name="failureAction"></param>
        /// <returns></returns>
        public IContext GetContext<TActor>( TActor actor, IEnumerable<IObserver<IEvent>> listeners,
                                            Action<TActor> successAction, Action<TActor, Exception> failureAction )
            where TActor : class
        {
            return GetContext( actor, listeners, successAction, failureAction, null );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TActor"></typeparam>
        /// <param name="actor"></param>
        /// <param name="listeners"></param>
        /// <param name="successAction"></param>
        /// <param name="failureAction"></param>
        /// <param name="contextAction"></param>
        /// <returns></returns>
        public IContext GetContext<TActor>(TActor actor, IEnumerable<IObserver<IEvent>> listeners,
                                            Action<TActor> successAction, Action<TActor, Exception> failureAction,
                                            Action<TActor> contextAction)
            where TActor : class
        {
            var originalState = Memoizer.GetMemento(actor);
            if (Configuration.Replay)
            {
                return new ReplayContext<TActor>(actor, originalState);
            }
            var keyAccessor = Assimilate.GetInstanceOf<IKeyAccessor<TActor>>();
            return new DefaultContext<TActor>(actor, originalState, keyAccessor, Publisher, listeners, successAction,
                                               failureAction, contextAction);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="memoizer"></param>
        /// <param name="publisher"></param>
        public DefaultContextProvider( IEventConfiguration configuration, IMemoizer memoizer, IEventPublisher publisher )
        {
            Configuration = configuration;
            Memoizer = memoizer;
            Publisher = publisher;
        }
    }
}