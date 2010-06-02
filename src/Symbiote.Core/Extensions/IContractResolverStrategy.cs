using System;
using Newtonsoft.Json.Serialization;

namespace Symbiote.Core.Extensions
{
    public interface IContractResolverStrategy
    {
        bool ResolverApplies(Type type);
        IContractResolver Resolver { get; }
    }
}