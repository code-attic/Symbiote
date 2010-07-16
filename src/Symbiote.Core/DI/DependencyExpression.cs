using System;
using Microsoft.Practices.ServiceLocation;

namespace Symbiote.Core.DI
{
    public class DependencyExpression
    {
        public static DependencyExpression<object> For(Type pluginType)
        {
            return new DependencyExpression<object>(pluginType);
        }

        public static DependencyExpression<TPlugin> For<TPlugin>()
        {
            return new DependencyExpression<TPlugin>();
        }

        public static DependencyExpression<object> For(string name, Type pluginType)
        {
            return new DependencyExpression<object>(pluginType, name);
        }

        public static DependencyExpression<TPlugin> For<TPlugin>(string name)
        {
            return new DependencyExpression<TPlugin>(name);
        }
    }

    public class DependencyExpression<TPlugin> : 
        IDependencyDefinition,
        ISupplyPlugin<TPlugin>,
        IPluginConfiguration
    {
        private object _concreteInstance { get; set; }
        public object ConcreteInstance { get; set; }
        public Type ConcreteType { get; set; }
        public Type FactoryType { get; set; }
        public bool HasFactory { get; set; }
        public bool IsAdd { get; set; }
        public bool IsNamed { get; set; }
        public bool IsSingleton { get; set; }
        public bool HasSingleton { get; set; }
        public string PluginName { get; set; }
        public Type PluginType { get; set; }

        public virtual IPluginConfiguration Add(Type concreteType)
        {
            ConcreteType = concreteType;
            IsAdd = true;
            return this;
        }

        public virtual IPluginConfiguration Add<TConcrete>()
            where TConcrete : TPlugin
        {
            ConcreteType = typeof(TConcrete);
            IsAdd = true;
            return this;
        }

        public IPluginConfiguration Add<TConcrete>(TConcrete instance)
            where TConcrete : TPlugin
        {
            ConcreteInstance = instance;
            IsAdd = true;
            return this;
        }

        public virtual IPluginConfiguration Use(Type concreteType)
        {
            ConcreteType = concreteType;
            return this;
        }

        public virtual IPluginConfiguration Use<TConcrete>()
            where TConcrete : TPlugin
        {
            ConcreteType = typeof (TConcrete);
            return this;
        }

        public virtual IPluginConfiguration Use<TConcrete>(TConcrete instance)
            where TConcrete : TPlugin
        {
            ConcreteInstance = instance;
            HasSingleton = true;
            IsSingleton = true;
            return this;
        }
        
        public virtual IPluginConfiguration UseFactory<TFactory>()
        {
            HasFactory = true;
            FactoryType = typeof (TFactory);
            return this;
        }

        public virtual IPluginConfiguration AsSingleton()
        {
            IsSingleton = true;
            return this;
        }

        public DependencyExpression(Type pluginType)
        {
            PluginType = pluginType;
        }

        public DependencyExpression(Type pluginType, string pluginName)
        {
            PluginType = pluginType;
            PluginName = pluginName;
            IsNamed = true;
        }

        public DependencyExpression()
        {
            PluginType = typeof(TPlugin);
        }

        public DependencyExpression(string pluginName)
        {
            PluginType = typeof(TPlugin);
            PluginName = pluginName;
            IsNamed = true;
        }
    }
}