using System;
using System.Collections.Generic;

namespace Symbiote.Core.DI
{
    public interface IDependencyRegistry
    {
        IEnumerable<Type> RegisteredPluginTypes { get; }
        Type GetDefaultTypeFor<T>();
        IEnumerable<Type> GetTypesRegisteredFor<T>();
        IEnumerable<Type> GetTypesRegisteredFor(Type type);
        bool HasPluginFor<T>();
        void Register(IDependencyDefinition dependency);
        void Scan(IScanInstruction scanInstruction);
    }
}