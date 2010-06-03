using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using StructureMap;

namespace Symbiote.Core.Extensions
{
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

        public JsonSerializer GetSerializerFor(string json, bool includeTypeSpec)
        {
            var type = json.GetSerializedTypeFromJson();
            return GetSerializerFor(type, includeTypeSpec);
        }

        public JsonSerializer GetSerializerFor<T>(bool includeTypeSpec)
        {
            var type = typeof(T);
            return GetSerializerFor(type, includeTypeSpec);
        }

        public JsonSerializer GetSerializerFor(Type type, bool includeTypeSpec)
        {
            JsonSerializer serializer = null;
            if (includeTypeSpec)
                serializer = JsonSerializer.Create(GetDefaultSettings());
            else
                serializer = JsonSerializer.Create(GetSettingsWithoutTypeHandling());

            SetContractResolver(serializer, type);
            return serializer;
        }

        protected void SetContractResolver(JsonSerializer serializer, Type type)
        {
            if (type == null)
                return;
            serializer.ContractResolver = 
                GetTypeSpecificContractResolver(type) ?? 
                GetResolverByStrategy(type) ?? 
                serializer.ContractResolver;
        }

        private IContractResolver GetResolverByStrategy(Type type)
        {
            return ContractResolverStrategies
                .Where(x => x.ResolverApplies(type))
                .Select(x => x.Resolver)
                .FirstOrDefault();
        }

        protected IContractResolver GetTypeSpecificContractResolver(Type type)
        {
            return ObjectFactory.TryGetInstance<IContractResolver>(_resolverFormat.AsFormat(type.FullName));
        }

        public JsonSerializerFactory()
        {
            ContractResolverStrategies = ObjectFactory.GetAllInstances<IContractResolverStrategy>().ToList();
        }
    }
}
