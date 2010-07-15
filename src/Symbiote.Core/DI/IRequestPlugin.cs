using System;

namespace Symbiote.Core.DI
{
    public interface IRequestPlugin
    {
        ISupplyPlugin<object> For(Type pluginType);
        ISupplyPlugin<TPlugin> For<TPlugin>();
        ISupplyPlugin<object> For(string name, Type pluginType);
        ISupplyPlugin<TPlugin> For<TPlugin>(string name);
    }
}