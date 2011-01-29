using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Core;
using Symbiote.Core.UnitOfWork;
using Symbiote.Mikado.Impl;

namespace Symbiote.Mikado.Extensions
{
    public static class IContextExtensions
    {
        public static IContext HandleBrokenRules<TActor>(this IContext context, Action<TActor, IList<IBrokenRuleNotification>> brokenRulesHandler) where TActor : class
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
