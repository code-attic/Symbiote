using System;
using Newtonsoft.Json.Serialization;

namespace Symbiote.Core.Extensions
{
    public interface IContractResolverStrategy
    {
        bool ResolverAppliesForSerialization(Type type);
        bool ResolverAppliesForDeserialization(Type type);
        IContractResolver Resolver { get; }
    }
}