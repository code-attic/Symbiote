/* 
Copyright 2008-2010 Jim Cowart

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
