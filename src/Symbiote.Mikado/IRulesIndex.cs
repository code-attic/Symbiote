using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Symbiote.Mikado
{
    public interface IRulesIndex
    {
        ConcurrentDictionary<Type, List<IRule>> Rules { get; set; }
    }
}
