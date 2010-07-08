using System;

namespace Symbiote.Core.DI
{
    public interface ISupplyPlugin
    {
        IPluginConfiguration Add(Type concreteType);
        IPluginConfiguration Add<TConcrete>();
        IPluginConfiguration Add<TConcrete>(TConcrete instance);
        IPluginConfiguration Use(Type concreteType);
        IPluginConfiguration Use<TConcrete>();
        IPluginConfiguration Use<TConcrete>(TConcrete instance);
        IPluginConfiguration UseFactory<TFactory>();
    }
}