/* 
Copyright 2008-2010 Alex Robson

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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
        protected ConcurrentDictionary<Type, JsonSerializer> Serializers { get; set; }

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
            if(!Serializers.TryGetValue(type, out serializer))
            {
                if (includeTypeSpec)
                    serializer = JsonSerializer.Create(GetDefaultSettings());
                else
                    serializer = JsonSerializer.Create(GetSettingsWithoutTypeHandling());

                SetContractResolver(serializer, type, action);
                //Serializers.AddOrUpdate( type, serializer, ( x, y ) => serializer );
            }
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
            return Assimilate.GetInstanceOf<IContractResolver>(_resolverFormat.AsFormat(type.FullName));
        }

        public JsonSerializerFactory()
        {
            ContractResolverStrategies = Assimilate.GetAllInstancesOf<IContractResolverStrategy>().ToList();
            Serializers = new ConcurrentDictionary<Type, JsonSerializer>();
        }
    }
}
