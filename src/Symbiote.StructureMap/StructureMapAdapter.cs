/* 
Copyright 2008-2010 Alex Robson

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
using StructureMap;
using StructureMap.Pipeline;
using StructureMap.TypeRules;
using Symbiote.Core.Impl.DI;

namespace Symbiote.StructureMap
{
    public class StructureMapAdapter : 
        IDependencyAdapter
    {
        public IEnumerable<Type> RegisteredPluginTypes
        {
            get
            {
                return ObjectFactory
                    .Container
                    .Model
                    .PluginTypes
                    .SelectMany(x => x.Instances.Select(i => i.ConcreteType));
            }
        }

        public Type GetDefaultTypeFor<T>()
        {
            return ObjectFactory.Model.DefaultTypeFor<T>();
        }

        public IEnumerable<Type> GetTypesRegisteredFor<T>()
        {
            return GetTypesRegisteredFor(typeof(T));
        }

        public IEnumerable<Type> GetTypesRegisteredFor(Type type)
        {
            return ObjectFactory
                .Container
                .Model
                .PluginTypes
                .Where(x => x.PluginType.Equals(type))
                .SelectMany(x => x.Instances)
                .Select(x =>
                {
                    return x.ConcreteType;
                });
        }

        public bool HasPluginFor<T>()
        {
            return ObjectFactory.Container.Model.HasDefaultImplementationFor<T>();
        }

        public object GetInstance(Type serviceType)
        {
            if (ScannerExtensions.IsConcrete(serviceType))
                return ObjectFactory.GetInstance(serviceType);
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
            if (ScannerExtensions.IsConcrete(typeof(TService)))
                return ObjectFactory.GetInstance<TService>();
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
                        else if(dependency.HasDelegate)
                        {
                            instance = forExpression.Use(f => dependency.CreatorDelegate.DynamicInvoke());
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
