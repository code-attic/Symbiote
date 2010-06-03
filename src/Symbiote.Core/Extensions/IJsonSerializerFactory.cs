using System;
using Newtonsoft.Json;

namespace Symbiote.Core.Extensions
{
    public interface IJsonSerializerFactory
    {
        JsonSerializer GetSerializerFor(string json, bool includeTypeSpec, SerializerAction action);
        JsonSerializer GetSerializerFor<T>(bool includeTypeSpec, SerializerAction action);
        JsonSerializer GetSerializerFor(Type type, bool includeTypeSpec, SerializerAction action);
    }
}