using System;

namespace Symbiote.Core.DI
{
    public class DependencyExpression : 
        IDependencyDefinition,
        ISupplyPlugin,
        IRequestPlugin,
        IPluginConfiguration
    {
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
        {
            ConcreteType = typeof(TConcrete);
            IsAdd = true;
            return this;
        }

        public IPluginConfiguration Add<TConcrete>(TConcrete instance)
        {
            ConcreteInstance = instance;
            IsAdd = true;
            return this;
        }

        public virtual ISupplyPlugin For(Type pluginType)
        {
            PluginType = pluginType;
            return this;
        }

        public virtual ISupplyPlugin For<TPlugin>()
        {
            PluginType = typeof (TPlugin);
            return this;
        }

        public virtual ISupplyPlugin For(string name, Type pluginType)
        {
            PluginName = name;
            IsNamed = true;
            PluginType = pluginType;
            return this;
        }

        public virtual ISupplyPlugin For<TPlugin>(string name)
        {
            PluginName = name;
            IsNamed = true;
            PluginType = typeof(TPlugin);
            return this;
        }

        public virtual IPluginConfiguration Use(Type concreteType)
        {
            ConcreteType = concreteType;
            return this;
        }

        public virtual IPluginConfiguration Use<TConcrete>()
        {
            ConcreteType = typeof (TConcrete);
            return this;
        }

        public virtual IPluginConfiguration Use<TConcrete>(TConcrete instance)
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
    }
}