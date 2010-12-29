using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Symbiote.Mikado
{
    public interface IRulesIndex
    {
        ConcurrentDictionary<Type, List<IRule>> TypeRules { get; set; }
        ConcurrentDictionary<Object, List<IRule>> InstanceRules { get; set; }
    }
}
