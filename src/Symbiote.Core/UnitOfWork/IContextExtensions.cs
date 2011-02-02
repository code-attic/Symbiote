using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Core.UnitOfWork
{
    public static class ContextExtensions
    {
        /// <summary>
        /// Extends IContext to provide the ability to pass in listeners for published IEvents.
        /// </summary>
        /// <typeparam name="TActor">Must be a reference type - the Actor type on which action is being taken.</typeparam>
        /// <param name="context">The IContext instance.</param>
        /// <param name="listeners">Collection of IObserver of IEvent.</param>
        /// <returns>The IContext instance.</returns>
        public static IContext<TActor> WithEventListeners<TActor>(this IContext<TActor> context, IEnumerable<IObserver<IEvent>> listeners)
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
        public static IContext<TActor> OnException<TActor>(this IContext<TActor> context, Action<TActor, Exception> action)
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
        public static IContext<TActor> OnSuccess<TActor>(this IContext<TActor> context, Action<TActor> action)
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
        public static IContext<TActor> OnCommit<TActor>(this IContext<TActor> context, Action<TActor> action)
        {
            context.CommitAction = action;
            return context;
        }
    }
}
