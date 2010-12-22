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
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Text.RegularExpressions;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Symbiote.Core.Extensions
{
    public static class JsonExtensions
    {
        private static IJsonSerializerFactory SerializerFactory { get; set; }
        public static IJsonSerializerFactory JsonSerializerFactory
        {
            get
            {
                SerializerFactory = 
                    SerializerFactory ?? 
                    Assimilate.GetInstanceOf<IJsonSerializerFactory>() ?? 
                    new JsonSerializerFactory();
                return SerializerFactory;
            }
        }
        
        public static string ToJson<T>(this T model)
        {
            return ToJson(model, true);
        }

        public static string ToJson<T>(this T model, bool includeTypeData)
        {
            var serializer = JsonSerializerFactory.GetSerializerFor(typeof(T), includeTypeData, SerializerAction.Serializing);
            var textWriter = new StringWriter();
            var jsonWriter = new JsonTextWriter(textWriter);

            serializer.Converters.Add(new IsoDateTimeConverter());
            serializer.Serialize(jsonWriter, model);
            return textWriter.ToString();
        }

        public static T FromJson<T>(this string json)
        {
            return (T) FromJson(json, typeof (T));
        }

        public static object FromJson(this string json)
        {
            var type = json.GetSerializedTypeFromJson();
            return FromJson(json, type);
        }

        public static object FromJson(this string json, Type type)
        {
            var serializer = JsonSerializerFactory.GetSerializerFor(type, true, SerializerAction.Deserializing);

            try
            {
                var textReader = new StringReader(json);
                var jsonReader = new JsonTextReader(textReader);
                
                if (type == null)
                    return serializer.Deserialize(jsonReader);
                else
                    return serializer.Deserialize(jsonReader, type);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static void RefreshFromJson<T>(this string json, T target)
        {
            var type = typeof (T);
            var serializer = JsonSerializerFactory.GetSerializerFor(type, true, SerializerAction.Deserializing);
            JsonConvert.PopulateObject(json, target, 
                new JsonSerializerSettings()
                    {
                        Binder = serializer.Binder,
                        ConstructorHandling = serializer.ConstructorHandling,
                        Context = serializer.Context,
                        ContractResolver = serializer.ContractResolver,
                        Converters = serializer.Converters,
                        DefaultValueHandling = serializer.DefaultValueHandling,
                        MissingMemberHandling = serializer.MissingMemberHandling,
                        ObjectCreationHandling = serializer.ObjectCreationHandling,
                        NullValueHandling = serializer.NullValueHandling
                    });
        }

        internal static Type GetSerializedTypeFromJson(this string json)
        {
            var typeToken = JObject.Parse(json).Value<string>("$type");
            if (string.IsNullOrEmpty(typeToken))
                return null;
            else
                return Type.GetType(typeToken);
        }

#if !SILVERLIGHT        
        public static XmlNode JsonToXml(this string json)
        {
            var scrubbedJson = Regex.Replace(json, @"[""][$]type[""][:][""][^""]+[""][,]", "");
            var rootNode = JsonConvert.DeserializeXmlNode(scrubbedJson);
            return rootNode;
        }

        public static XmlNode JsonToXml(this string json, string rootNodeName)
        {
            var scrubbedJson = Regex.Replace(json, @"[""][$]type[""][:][""][^""]+[""][,]", "");
            var rootNode = JsonConvert.DeserializeXmlNode(scrubbedJson, rootNodeName);
            return rootNode;
        }

        public static string XmlToJson(this XmlNode node)
        {
            return JsonConvert.SerializeXmlNode(node);
        }
#endif
    }
}
