using System;
using System.Linq;
using Symbiote.Core;
using Symbiote.Core.Extensions;

namespace Symbiote.Mikado.Impl
{
    public static class MikadoAssimilation
    {
        public static IAssimilate Mikado(this IAssimilate assimilate)
        {
            ConfigureStandardDependencies();
            return assimilate;
        }

        private static void ConfigureStandardDependencies()
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
