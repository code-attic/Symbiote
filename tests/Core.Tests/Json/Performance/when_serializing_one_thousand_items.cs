using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Core.Extensions;
using Symbiote.Core.Serialization;
using Symbiote.StructureMapAdapter;

namespace Core.Tests.Json.Performance
{
    public class when_serializing_one_thousand_items_with_jsondotnet
    {
        protected static Stopwatch watch { get; set; }
        protected static List<SerializationTarget> deserialized { get; set; }
        private Because of = () =>
                                 {
                                     Assimilate.Initialize();
                                     var list =
                                         Enumerable.Range(0, 1000).Select(x =>
                                             new SerializationTarget() {Id = x, Message = "This is a message! YAR!"});

                                     
                                     var jsonBlobies = list.Select(x => x.ToJson());

                                     watch = Stopwatch.StartNew();
                                     deserialized = jsonBlobies.Select(x => x.FromJson<SerializationTarget>()).ToList();
                                     watch.Stop();
                                 };

        private It should_not_take_for_freaking_ever = () => watch.ElapsedMilliseconds.ShouldBeLessThan(100);

    }

    public class when_serializing_one_thousand_items_with_binary
    {
        protected static Stopwatch watch { get; set; }
        protected static List<SerializationTarget> deserialized { get; set; }
        private Because of = () =>
        {
            Assimilate.Initialize();
            var list =
                Enumerable.Range(0, 1000).Select(x =>
                    new SerializationTarget() { Id = x, Message = "This is a message! YAR!" });


            BinaryFormatter binary = new BinaryFormatter();
            var streams = list.Select(x =>
                                          {
                                              var memoryStream = new MemoryStream(128);
                                              binary.Serialize(memoryStream, x);
                                              return memoryStream;
                                          });

            watch = Stopwatch.StartNew();
            deserialized = streams.Select(x =>
                                              {
                                                  x.Position = 0;
                                                  return binary.Deserialize(x) as SerializationTarget;
                                              }).ToList();
            watch.Stop();
        };

        private It should_not_take_for_freaking_ever = () => watch.ElapsedMilliseconds.ShouldBeLessThan(100);
        private It should_return_1000_results = () => deserialized.Count.ShouldEqual(1000);

    }


    public interface IGotMessageForYou
    {
        string Message { get; set; }
    }

    [Serializable]
    public class SerializationTarget : IGotMessageForYou
        
    {
        public int Id { get; set; }
        public string Message { get; set; }
    }
}
