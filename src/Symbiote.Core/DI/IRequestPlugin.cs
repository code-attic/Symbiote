using System;

namespace Symbiote.Core.DI
{
    public interface IRequestPlugin
    {
        ISupplyPlugin For(Type pluginType);
        ISupplyPlugin For<TPlugin>();
        ISupplyPlugin For(string name, Type pluginType);
        ISupplyPlugin For<TPlugin>(string name);
    }
}