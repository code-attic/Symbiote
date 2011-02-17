using System.Linq;
using Symbiote.Core;
using Symbiote.Core.Extensions;

namespace Symbiote.Mikado.Configuration
{
    public class MikadoInitialization : IInitializeSymbiote
    {
        public void Initialize()
        {
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