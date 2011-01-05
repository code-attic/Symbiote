using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Core;
using Symbiote.Core.Extensions;
using Symbiote.Core.Work;
using Symbiote.Mikado.Impl;

namespace Symbiote.Mikado
{
    public static class MikadoAssimilation
    {
        private static bool MikadoInitialized { get; set; }

        public static IAssimilate Mikado(this IAssimilate assimilate)
        {
            if (MikadoInitialized)
                return assimilate;

            WireUpRulesToContainer();
            assimilate.Dependencies( x =>
                                         {
                                             x.For<IContextProvider>().Use<MikadoContextProvider>();
                                             x.For<IBrokenRuleNotification>().Use<BrokenRuleNotification>();
                                             x.For<IRulesIndex>().Use<DefaultRulesIndex>().AsSingleton();
                                             x.For<IRunRules>().Use<DefaultRulesRunner>().AsSingleton();
                                         } );
            MikadoInitialized = true;
            return assimilate;
        }

        private static void WireUpRulesToContainer()
        {
            var assemblies = AppDomain
                                .CurrentDomain
                                .GetAssemblies()
                                .Where(a => a.GetReferencedAssemblies().Any(r => r.FullName.Contains("Mikado")) && !a.FullName.Contains("DynamicProxyGenAssembly2"))
                                .ToList();

            Assimilate.Dependencies(x => x.Scan(s =>
            {
                assemblies.ForEach(s.Assembly);
                s.ConnectImplementationsToTypesClosing(typeof(IRule<>));
            }));

            var rules =
                Assimilate
                    .Assimilation
                    .DependencyAdapter
                    .RegisteredPluginTypes
                    .Where(x => typeof(IRule).IsAssignableFrom(x) || x.IsAssignableFrom(typeof(IRule)))
                    .Distinct();

            var simpleInterface = typeof(IRule);
            Assimilate.Dependencies(x => rules.ForEach(p => x.For(simpleInterface).Add(p)));
        }
    }
}
