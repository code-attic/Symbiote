using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Symbiote.Core.Extensions
{
    public static class JsonExtensions
    {
        public static string ToJson<T>(this T model)
        {
            var settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                TypeNameHandling = TypeNameHandling.All
            };
            var serializer = JsonSerializer.Create(settings);
            var textWriter = new StringWriter();
            var jsonWriter = new JsonTextWriter(textWriter);
            
            serializer.Serialize(jsonWriter, model);
            return textWriter.ToString();
        }

        public static string ToJson<T>(this T model, bool includeTypeData)
        {
            var settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,
            };
            if(includeTypeData)
                settings.TypeNameHandling = TypeNameHandling.All;

            var serializer = JsonSerializer.Create(settings);
            var textWriter = new StringWriter();
            var jsonWriter = new JsonTextWriter(textWriter);

            serializer.Serialize(jsonWriter, model);
            return textWriter.ToString();
        }

        public static T FromJson<T>(this string json)
        {
            var settings = new JsonSerializerSettings()
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                PreserveReferencesHandling = PreserveReferencesHandling.All
            };
            var serializer = JsonSerializer.Create(settings);
            try
            {
                var textReader = new StringReader(json);
                var jsonReader = new JsonTextReader(textReader);
                return (T)serializer.Deserialize(jsonReader, typeof(T));
            }
            catch (Exception e)
            {
                return default(T);
            }
        }

        public static object FromJson(this string json)
        {
            var settings = new JsonSerializerSettings()
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                PreserveReferencesHandling = PreserveReferencesHandling.All
            };
            var serializer = JsonSerializer.Create(settings);
            StringReader textReader = null;
            try
            {
                textReader = new StringReader(json);
                var typeName = JObject.Parse(json).Value<string>("$type");
                if(string.IsNullOrEmpty(typeName))
                {
                    var jsonReader = new JsonTextReader(textReader);
                    return serializer.Deserialize(jsonReader);    
                }
                else
                {
                    var type = Type.GetType(typeName);
                    if (type == null)
                        type = Type.GetType(typeName.Split(',')[0].Split('.')[1]);
                    var jsonReader = new JsonTextReader(textReader);
                    return serializer.Deserialize(jsonReader, type);    
                }
                
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static object FromJson(this string json, Type type)
        {
            var settings = new JsonSerializerSettings()
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                PreserveReferencesHandling = PreserveReferencesHandling.All
            };
            var serializer = JsonSerializer.Create(settings);
            try
            {
                var textReader = new StringReader(json);
                var jsonReader = new JsonTextReader(textReader);
                return serializer.Deserialize(jsonReader, type);
            }
            catch (Exception e)
            {
                return null;
            }
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
