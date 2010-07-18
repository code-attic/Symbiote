using System;

namespace Symbiote.Core.DI
{
    public interface ISupplyPlugin<TPlugin>
    {
        IPluginConfiguration Add<TConcrete>()
            where TConcrete : TPlugin;
        IPluginConfiguration Add<TConcrete>(TConcrete instance)
            where TConcrete : TPlugin;
        IPluginConfiguration Add(Type concreteType);
        IPluginConfiguration Use<TConcrete>()
            where TConcrete : TPlugin;
        IPluginConfiguration Use<TConcrete>(TConcrete instance)
            where TConcrete : TPlugin;
        IPluginConfiguration Use(Type concreteType);
        IPluginConfiguration CreateWithDelegate<TConcrete>(Func<TConcrete> factory)
            where TConcrete : TPlugin;
    }
}