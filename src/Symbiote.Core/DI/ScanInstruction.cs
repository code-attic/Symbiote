using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Symbiote.Core.Extensions;
using Symbiote.Core.DI;

namespace Symbiote.Core.DI
{
    public class ScanInstruction :
        IScanInstruction
    {
        protected TypeScanner scanner { get; set; }

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

        public void Execute(IDependencyRegistry registry)
        {
            var filteredTypes = scanner.GetMatchingTypes();

            AutoWireupTypesOf
                .ForEach(x => RegisterAllTypesOf(x, filteredTypes, registry));

            AutoWireupClosersOf
                .Where(x => x.IsOpenGeneric())
                .ForEach(x => RegisterClosingTypes(x, filteredTypes, registry));
        }

        protected void RegisterClosingTypes(Type type, IEnumerable<Type> filteredTypes, IDependencyRegistry registry)
        {
            var matches =
                filteredTypes.Where(t => t.Closes(type));

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
                                 var dependencyExpression = DependencyExpression.For(m.Name, type);
                                 dependencyExpression.Add(m);
                                 registry.Register(dependencyExpression);
                             });
        }

        protected void RegisterClosingType(Type type, Type match, IDependencyRegistry registry)
        {
            Type pluginType = null;
            Type baseType = type.BaseType;
            while(pluginType == null)
            {
                if(baseType.IsGenericType)
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
            var dependencyExpression = DependencyExpression.For(pluginType);
            dependencyExpression.Use(match);
            registry.Register(dependencyExpression);
        }

        protected void RegisterTypeClosingInterface(Type type, Type match, IDependencyRegistry registry)
        {
            var pluginType = match
                .GetInterfaces()
                .Where(y => y.IsGenericType)
                .First(y => y.GetGenericTypeDefinition().Equals(type));

            var dependencyExpression = DependencyExpression.For(pluginType);
            dependencyExpression.Use(match);
            registry.Register(dependencyExpression);
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