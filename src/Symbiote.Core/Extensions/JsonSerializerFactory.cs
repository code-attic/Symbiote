using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Symbiote.Core.Extensions
{
    public enum SerializerAction
    {
        Serializing,
        Deserializing
    }

    public class JsonSerializerFactory : IJsonSerializerFactory
    {
        private readonly string _resolverFormat = "CustomJsonResolver-{0}";

        protected List<IContractResolverStrategy> ContractResolverStrategies { get; set; }

        protected JsonSerializerSettings GetDefaultSettings()
        {
            var settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                TypeNameHandling = TypeNameHandling.All,
                TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple
            };
            settings.Converters.Add(new IsoDateTimeConverter());
            return settings;
        }

        protected JsonSerializerSettings GetSettingsWithoutTypeHandling()
        {
            var settings = GetDefaultSettings();
            settings.TypeNameHandling = TypeNameHandling.None;
            return settings;
        }

        public JsonSerializer GetSerializerFor(string json, bool includeTypeSpec, SerializerAction action)
        {
            var type = json.GetSerializedTypeFromJson();
            return GetSerializerFor(type, includeTypeSpec, action);
        }

        public JsonSerializer GetSerializerFor<T>(bool includeTypeSpec, SerializerAction action)
        {
            var type = typeof(T);
            return GetSerializerFor(type, includeTypeSpec, action);
        }

        public JsonSerializer GetSerializerFor(Type type, bool includeTypeSpec, SerializerAction action)
        {
            JsonSerializer serializer = null;
            if (includeTypeSpec)
                serializer = JsonSerializer.Create(GetDefaultSettings());
            else
                serializer = JsonSerializer.Create(GetSettingsWithoutTypeHandling());

            SetContractResolver(serializer, type, action);
            return serializer;
        }

        protected void SetContractResolver(JsonSerializer serializer, Type type, SerializerAction action)
        {
            if (type == null)
                return;

            Func<Type, IContractResolver> get = action == SerializerAction.Deserializing
                          ? (Func<Type, IContractResolver>) GetResolverForDeserializationByStrategy
                          : (Func<Type, IContractResolver>) GetResolverForSerializationByStrategy;

            serializer.ContractResolver = 
                GetTypeSpecificContractResolver(type) ?? 
                get(type) ?? 
                serializer.ContractResolver;
        }

        private IContractResolver GetResolverForSerializationByStrategy(Type type)
        {
            return ContractResolverStrategies
                .Where(x => x.ResolverAppliesForSerialization(type))
                .Select(x => x.Resolver)
                .FirstOrDefault();
        }

        private IContractResolver GetResolverForDeserializationByStrategy(Type type)
        {
            return ContractResolverStrategies
                .Where(x => x.ResolverAppliesForDeserialization(type))
                .Select(x => x.Resolver)
                .FirstOrDefault();
        }

        protected IContractResolver GetTypeSpecificContractResolver(Type type)
        {
            return ServiceLocator.Current.GetInstance<IContractResolver>(_resolverFormat.AsFormat(type.FullName));
        }

        public JsonSerializerFactory()
        {
            ContractResolverStrategies = ServiceLocator.Current.GetAllInstances<IContractResolverStrategy>().ToList();
        }
    }
}
