// /* 
// Copyright 2008-2011 Jim Cowart & Alex Robson
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
using System.Linq;

namespace Symbiote.Core.UnitOfWork
{
    public static class ContextExtensions
    {
        /// <summary>
        /// Extends IContext to provide the ability to pass in listeners for published IEvents.
        /// </summary>
        /// <typeparam name="TActor">Must be a reference type - the Actor type on which action is being taken.</typeparam>
        /// <param name="context">The IContext instance.</param>
        /// <param name="listeners">Collection of IEventListener.</param>
        /// <returns>The IContext instance.</returns>
        public static IContext<TActor> WithEventListeners<TActor>(this IContext<TActor> context, IEnumerable<IEventListener> listeners) where TActor : class
        {
            listeners
                .ToList()
                .ForEach(a => context.Disposables.Add(context.Publisher.Subscribe(a)));

            return context;
        }

        /// <summary>
        /// Extends IContext to allow for an "OnException" continuation to be executed when the unit of work throws an exception.
        /// </summary>
        /// <typeparam name="TActor">Must be a reference type - the Actor type on which action is being taken.</typeparam>
        /// <param name="context">The IContext instance.</param>
        /// <param name="action">The action to take if an exception is thrown during the unit of work.</param>
        /// <returns>The IContext instance.</returns>
        public static IContext<TActor> OnException<TActor>(this IContext<TActor> context, Action<TActor, Exception> action) where TActor : class
        {
            context.ExceptionAction = action;
            return context;
        }

        /// <summary>
        /// Extends IContext to allow for an "OnSuccess" continuation to be executed when the unit of work succeeds.
        /// </summary>
        /// <typeparam name="TActor">Must be a reference type - the Actor type on which action is being taken.</typeparam>
        /// <param name="context">The IContext instance.</param>
        /// <param name="action">The action to take once the work succeeds.</param>
        /// <returns>The IContext instance.</returns>
        public static IContext<TActor> OnSuccess<TActor>(this IContext<TActor> context, Action<TActor> action) where TActor : class
        {
            context.SuccessAction = action;
            return context;
        }

        /// <summary>
        /// Extends IContext to allow for an Action taking the TActor instance to be passed and executed as part of the unit of work.
        /// </summary>
        /// <typeparam name="TActor">Must be a reference type - the Actor type on which action is being taken.</typeparam>
        /// <param name="context">The IContext instance.</param>
        /// <param name="action">The action to take as part of the unit of work (called when Commit() is invoked).</param>
        /// <returns>The IContext instance.</returns>
        public static IContext<TActor> OnCommit<TActor>(this IContext<TActor> context, Action<TActor> action) where TActor : class
        {
            context.CommitAction = action;
            return context;
        }
    }
}
