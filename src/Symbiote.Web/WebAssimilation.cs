using System;
using System.Collections.Generic;
using System.Text;
using Symbiote.Core;

namespace Symbiote.Web
{
    public static class WebAssimilation
    {
        public static IAssimilate Web(this IAssimilate assimilate, Action<SymbioteApplicationConfigurator> config)
        {
            var configuration = new SymbioteApplicationConfigurator(assimilate);
            config(configuration);
            return assimilate;
        }
    }
}
