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
using System.Reflection;
using Symbiote.Core.Extensions;

namespace Symbiote.Core.Impl.DI
{
    public class ScanInstruction :
        IScanInstruction
    {
        protected TypeScanner scanner { get; set; }
        protected bool ShouldAddSingleImplementations { get; set; }
        public IList<Type> AutoWireupTypesOf { get; set; }
        public IList<Type> AutoWireupClosersOf { get; set; }

        public ScanInstruction()
        {
            scanner = new TypeScanner();
            AutoWireupTypesOf = new List<Type>();
            AutoWireupClosersOf = new List<Type>();
        }

        public void Assembly(Assembly assembly)
        {
            scanner.AddAssembly(assembly);
        }

        public void TheCallingAssembly()
        {
            scanner.AddCallingAssembly();
        }

        public void AssemblyContainingType<T>()
        {
            scanner.AddAssemblyContaining<T>();
        }

        public void AssemblyContainingType(Type type)
        {
            scanner.AddAssemblyContaining(type);
        }

#if !SILVERLIGHT
        public void AssembliesFromPath(string path)
        {
            scanner.AddAssembliesFromPath(path);
        }

        public void AssembliesFromPath(string path, Predicate<Assembly> assemblyFilter)
        {
            scanner.AddAssembliesFromPath(path);
            scanner.AssemblyFilters.Add(assemblyFilter);
        }
#endif

        public void AssembliesFromApplicationBaseDirectory()
        {
            scanner.AddAssembliesFromBaseDirectory();
        }

        public void AssembliesFromApplicationBaseDirectory(Predicate<Assembly> assemblyFilter)
        {
            scanner.AddAssembliesFromBaseDirectory();
            scanner.AssemblyFilters.Add(assemblyFilter);
        }

        public void AddAllTypesOf<TPlugin>()
        {
            AutoWireupTypesOf.Add(typeof(TPlugin));
        }

        public void AddAllTypesOf(Type pluginType)
        {
            AutoWireupTypesOf.Add(pluginType);
        }

        public void AddSingleImplementations()
        {
            ShouldAddSingleImplementations = true;
        }

        public void Execute(IDependencyRegistry registry)
        {
            var filteredTypes = scanner.GetMatchingTypes();

            AutoWireupTypesOf
                .ForEach(x => RegisterAllTypesOf(x, filteredTypes, registry));

            AutoWireupClosersOf
                .Where(x => x.IsOpenGeneric())
                .ForEach(x => RegisterClosingTypes(x, filteredTypes, registry));

            if (ShouldAddSingleImplementations)
            {
                RegisterSingleImplementations(filteredTypes, registry);
            }
        }

        protected void RegisterClosingTypes(Type type, IEnumerable<Type> filteredTypes, IDependencyRegistry registry)
        {
            var matches =
                filteredTypes.Where(t => t.Closes(type) && !t.IsAbstract && !t.IsInterface);

            foreach(var match in matches)
            {
                if(type.IsInterface)
                {
                    RegisterTypeClosingInterface(type, match, registry);
                }
                else
                {
                    RegisterClosingType(type, match, registry);
                }
            }
        }

        protected void RegisterAllTypesOf(Type type, IEnumerable<Type> filteredTypes, IDependencyRegistry registry)
        {
            filteredTypes
                .Where(t => t.IsConcreteAndAssignableTo(type))
                .ForEach(m =>
                             {
                                 var dependencyExpression = DependencyExpression.For(type);
                                 dependencyExpression.Add(m);
                                 registry.Register(dependencyExpression);
                             });
        }

        protected void RegisterClosingType(Type type, Type match, IDependencyRegistry registry)
        {
            Type pluginType = null;
            Type baseType = match.BaseType;
            while(pluginType == null && baseType != typeof(object))
            {
                if(baseType != null && baseType.IsGenericType)
                {
                    var genericTypeDefinition = baseType.GetGenericTypeDefinition();
                    if(genericTypeDefinition.Equals(type))
                    {
                        pluginType = baseType;
                    }
                    else
                    {
                        baseType = baseType.BaseType;
                    }
                }
            }
            if(pluginType != null)
            {
                var dependencyExpression = DependencyExpression.For(pluginType);
                dependencyExpression.Use(match);
                registry.Register(dependencyExpression);    
            }
            
        }

        protected void RegisterTypeClosingInterface(Type type, Type match, IDependencyRegistry registry)
        {
            var pluginTypes = match
                .GetInterfaces()
                .Where(y => y.IsGenericType && y.GetGenericTypeDefinition().Equals(type));

            foreach(var pluginType in pluginTypes)
            {
                var dependencyExpression = DependencyExpression.For(pluginType);
                dependencyExpression.Use(match);
                registry.Register(dependencyExpression);    
            }
        }

        protected void RegisterSingleImplementations(IEnumerable<Type> filteredTypes, IDependencyRegistry registry)
        {
            var interfaces = filteredTypes
                .Where(t => t.IsInterface);
            var lookups = interfaces.ToDictionary(
                x => x,
                x => filteredTypes.Where(f => f.IsConcreteAndAssignableTo(x)));
            lookups
                .Where(kp => kp.Value.Count() == 1)
                .ForEach(kp =>
                             {
                                 try
                                 {
                                     var dependencyExpression = DependencyExpression.For(kp.Key);
                                     dependencyExpression.Use(kp.Value.First());
                                     registry.Register(dependencyExpression);
                                 }
                                 catch (Exception e)
                                 {
                                     
                                 }
                             });

        }

        public void Exclude(Predicate<Type> exclude)
        {
            scanner.TypeFilters.Add(exclude);
        }

        public void ExcludeNamespace(string nameSpace)
        {
            scanner.TypeFilters.Add(x => x.Namespace.Contains(nameSpace));
        }

        public void ExcludeNamespaceContainingType<T>()
        {
            scanner.TypeFilters.Add(x => x.Namespace.Contains(x.Namespace));
        }

        public void ExcludeType<T>()
        {
            scanner.TypeFilters.Add(x => x.Equals(typeof(T)));
        }

        public void ConnectImplementationsToTypesClosing(Type openGenericType)
        {
            AutoWireupClosersOf.Add(openGenericType);
        }
    }
}