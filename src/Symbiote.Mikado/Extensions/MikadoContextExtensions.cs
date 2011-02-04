using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Core;
using Symbiote.Core.UnitOfWork;
using Symbiote.Mikado.Impl;

namespace Symbiote.Mikado.Extensions
{
    public static class MikadoContextExtensions
    {
        /// <summary>
        /// Extends IContext to allow for an Action to act upon the TActor instance and the list of rules that the TActor has broken
        /// </summary>
        /// <typeparam name="TActor">Must be a reference type - the Actor type on which action is being taken.</typeparam>
        /// <param name="context">The IContext instance.</param>
        /// <param name="brokenRulesHandler">Action that takes a TActor and list of IBrokenRuleNotification objects.</param>
        /// <returns>The IContext instance.</returns>
        /// <exception cref="AssimilationException"></exception>
        public static IContext<TActor> HandleBrokenRules<TActor>(this IContext<TActor> context, Action<TActor, IList<IBrokenRuleNotification>> brokenRulesHandler) where TActor : class
        {
            var mikado = context as MikadoContext<TActor>;
            if(mikado == null)
                throw new AssimilationException( "You cannot extend a non-Mikado unit of work with the HandleBrokenRules extension method." );
            else
            {
                mikado.OnBrokenRules = brokenRulesHandler;
                return context;
            }
        }
    }
}
