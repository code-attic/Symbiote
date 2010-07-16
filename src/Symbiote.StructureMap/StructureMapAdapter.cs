using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StructureMap;
using StructureMap.Pipeline;
using Symbiote.Core.DI;

namespace Symbiote.StructureMap
{
    public class StructureMapAdapter : 
        IDependencyAdapter
    {
        public IEnumerable<Type> RegisteredPluginTypes
        {
            get
            {
                return ObjectFactory.Container.Model.PluginTypes.Select(x => x.PluginType);
            }
        }

        public Type GetDefaultTypeFor<T>()
        {
            return ObjectFactory.Model.DefaultTypeFor<T>();
        }

        public bool HasPluginFor<T>()
        {
            return ObjectFactory.Container.Model.HasDefaultImplementationFor<T>();
        }

        public object GetInstance(Type serviceType)
        {
            return ObjectFactory.TryGetInstance(serviceType);
        }

        public object GetInstance(Type serviceType, string key)
        {
            return ObjectFactory.TryGetInstance(serviceType, key);
        }

        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return ObjectFactory.GetAllInstances(serviceType).Cast<object>();
        }

        public TService GetInstance<TService>()
        {
            return ObjectFactory.TryGetInstance<TService>();
        }

        public TService GetInstance<TService>(string key)
        {
            return ObjectFactory.TryGetInstance<TService>(key);
        }

        public IEnumerable<TService> GetAllInstances<TService>()
        {
            return ObjectFactory.GetAllInstances<TService>();
        }

        public object GetService(Type serviceType)
        {
            return GetInstance(serviceType);
        }

        public void Register(IDependencyDefinition dependency)
        {
            if (dependency.IsAdd)
            {
                HandleAdd(dependency);
            }
            else
            {
                HandleFor(dependency);
            }
            
        }

        public void Scan(IScanInstruction instruction)
        {
            instruction.Execute(this);
        }

        private void HandleAdd(IDependencyDefinition dependency)
        {
            ObjectFactory.Configure(x =>
                    {
                        if(dependency.IsSingleton)
                        {
                            var singleton = x.For(dependency.PluginType).Singleton();
                            if (dependency.HasSingleton)
                                singleton.Add(dependency.ConcreteInstance);
                            else
                                singleton.Add(dependency.ConcreteType);
                        }
                        else if(dependency.IsNamed)
                        {
                            x.For(dependency.PluginType).Add(dependency.ConcreteType).Named(dependency.PluginName);
                        }
                        else
                        {
                            x.For(dependency.PluginType).Add(dependency.ConcreteType);
                        }
                    });
        }

        private void HandleFor(IDependencyDefinition dependency)
        {
            ObjectFactory.Configure(x =>
                    {
                        var forExpression = x.For(dependency.PluginType);
                        Instance instance;
                        if (dependency.IsSingleton)
                        {
                            if (dependency.HasSingleton)
                                instance = forExpression.Singleton().Use(dependency.ConcreteInstance);
                            else
                                instance = forExpression.Singleton().Use(dependency.ConcreteType);
                        }
                        else
                        {
                            instance = forExpression.Use(dependency.ConcreteType);
                        }

                        if (dependency.IsNamed)
                            instance.Name = dependency.PluginName;
                    }
                );
        }

        public StructureMapAdapter()
        {
        }
    }
}
