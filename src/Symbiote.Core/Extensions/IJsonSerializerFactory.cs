using System;
using Newtonsoft.Json;

namespace Symbiote.Core.Extensions
{
    public interface IJsonSerializerFactory
    {
        JsonSerializer GetSerializerFor(string json, bool includeTypeSpec);
        JsonSerializer GetSerializerFor<T>(bool includeTypeSpec);
        JsonSerializer GetSerializerFor(Type type, bool includeTypeSpec);
    }
}