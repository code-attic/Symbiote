using System;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Text.RegularExpressions;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using StructureMap;

namespace Symbiote.Core.Extensions
{
    public static class JsonExtensions
    {
        public static IJsonSerializerFactory JsonSerializerFactory
        {
            get
            {
                return ObjectFactory.GetInstance<IJsonSerializerFactory>();
            }
        }

        public static string ToJson<T>(this T model)
        {
            return ToJson(model, true);
        }

        public static string ToJson<T>(this T model, bool includeTypeData)
        {
            var serializer = JsonSerializerFactory.GetSerializerFor(typeof(T), includeTypeData);
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
            var serializer = JsonSerializerFactory.GetSerializerFor(type, true);

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

        internal static Type GetSerializedTypeFromJson(this string json)
        {
            var typeToken = JObject.Parse(json).Value<string>("$type");
            if (string.IsNullOrEmpty(typeToken))
                return null;
            else
                return Type.GetType(typeToken);
        }
        
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
    }
}
