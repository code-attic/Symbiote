using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Core.Extensions;
using Symbiote.Core.Serialization;

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
}
