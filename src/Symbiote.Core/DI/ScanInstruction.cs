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

        public void Assembly(string assemblyName)
        {
            scanner.AddAssemblyByName(assemblyName);
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

        public void AssembliesFromPath(string path)
        {
            scanner.AddAssembliesFromPath(path);
        }

        public void AssembliesFromPath(string path, Predicate<Assembly> assemblyFilter)
        {
            scanner.AddAssembliesFromPath(path);
            scanner.AssemblyFilters.Add(assemblyFilter);
        }

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
                .ForEach(x =>
                             {
                                 var matches =
                                     filteredTypes.Where(t => t.IsConcreteAndAssignableTo(x));

                                 matches.ForEach(m =>
                                                     {
                                                         var dependencyExpression = new DependencyExpression();
                                                         dependencyExpression.For(m.Name, x).Add(m);
                                                         registry.Register(dependencyExpression);
                                                     });
                             });

            AutoWireupClosersOf
                .Where(x => x.IsOpenGeneric())
                .ForEach(x =>
                {
                    var match =
                        filteredTypes.FirstOrDefault(t => t.Closes(x));

                    if (match != null)
                    {
                        var dependencyExpression = new DependencyExpression();
                        dependencyExpression.For(x).Use(match);
                        registry.Register(dependencyExpression);
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