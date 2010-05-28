using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using Machine.Specifications;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Core.Tests.Json.Collections
{
    public interface IGotsaNameYo
    {
        string Name { get; set; }
    }

    public class Person : IGotsaNameYo
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class Car : IGotsaNameYo
    {
        public string Name { get; set; }
        public string Make { get; set; }
        public int Year { get; set; }
    }

    public class Stuff
    {
        public IGotsaNameYo Thingy { get; set; }
        public List<IGotsaNameYo> list { get; set; }
    }

    public abstract class with_collection_of_interface_type
    {
        protected static List<IGotsaNameYo> collection;
        protected static Stuff stuff;

        private Establish context = () =>
                                        {
                                            collection = new List<IGotsaNameYo>()
                                                             {
                                                                 new Car() {Make="Chevy",Name="Equinox",Year=2007},
                                                                 new Person() {Age=30, Name="Me"}
                                                             };

                                            stuff = new Stuff()
                                                        {
                                                            list = collection,
                                                            Thingy = new Car() { Make="Honda",Name="Civic",Year=2009}
                                                        };
                                        };
    }

    public class when_serializing_interface_collection : with_collection_of_interface_type
    {
        protected static string json;
        protected static Stuff result;

        private Because of = () =>
                                 {
                                     var settings = new JsonSerializerSettings()
                                     {
                                         NullValueHandling = NullValueHandling.Ignore,
                                         MissingMemberHandling = MissingMemberHandling.Ignore,
                                         //TypeNameHandling = TypeNameHandling.All,
                                         TypeNameHandling = TypeNameHandling.Objects,
                                         TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple
                                     };

                                     var serializer = JsonSerializer.Create(settings);
                                     var textWriter = new StringWriter();
                                     var jsonWriter = new JsonTextWriter(textWriter);

                                     serializer.Converters.Add(new IsoDateTimeConverter());
                                     serializer.Serialize(jsonWriter, stuff);

                                     json = textWriter.ToString();

                                     var textReader = new StringReader(json);

                                     var typeName = JObject.Parse(json).Value<string>("$type");
                                     if(string.IsNullOrEmpty(typeName))
                                     {
                                        var jsonReader = new JsonTextReader(textReader);
                                        result = (Stuff) serializer.Deserialize(jsonReader);    
                                     }
                                     else
                                     {
                                         var type = Type.GetType(typeName);
                                         if (type == null)
                                             type = Type.GetType(typeName.Split(',')[0].Split('.')[1]);
                                         var jsonReader = new JsonTextReader(textReader);
                                         result = (Stuff) serializer.Deserialize(jsonReader, type);    
                                     }
                                 };

        private It should_not_produce_empty_json = () => 
            json.ShouldNotBeEmpty();
    }
}
