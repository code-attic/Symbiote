using System;
using System.Reflection;

namespace Symbiote.Core.DI
{
    public interface IScanInstruction
    {
        void Assembly(Assembly assembly);
        void Assembly(string assemblyName);
        void TheCallingAssembly();
        void AssemblyContainingType<T>();
        void AssemblyContainingType(Type type);
        void AssembliesFromPath(string path);
        void AssembliesFromPath(string path, Predicate<Assembly> assemblyFilter);
        void AssembliesFromApplicationBaseDirectory();
        void AssembliesFromApplicationBaseDirectory(Predicate<Assembly> assemblyFilter);
        void AddAllTypesOf<TPlugin>();
        void AddAllTypesOf(Type pluginType);
        void ConnectImplementationsToTypesClosing(Type openGenericType);
        void Exclude(Predicate<Type> exclude);
        void ExcludeNamespace(string nameSpace);
        void ExcludeNamespaceContainingType<T>();
        void ExcludeType<T>();
        void Execute(IDependencyRegistry registry);
    }
}