using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using Symbiote.Core;
using Symbiote.Core.Extensions;

namespace Demo
{
    class Symbiote_Core_1
    {
        static void Main(string[] args)
        {
            // Note that dependency takes the same action as StructureMap's
            // ObjectFactory.Configure
            Assimilate
                .Core()
                .Dependencies(x => x.For<IDependency>().Use<Concrete>());
        }

        public static void JSONDemo()
        {
            var instance = new JsonTest() {Number = 10, Text = "text"};
            var json1 = instance.ToJson(); // includes type tokens in the json
            var json2 = instance.ToJson(false); // exclude type tokens in json

            var result1 = json1.FromJson(); // result1 is an object but of type JsonTest
            var result2 = json2.FromJson(); // w/o type token in json2, results in JObject
            var result3 = json2.FromJson<JsonTest>(); // result3 is JsonTest b/c of the generic parameter
        }

    }

    public class JsonTest
    {
        public int Number { get; set; }
        public string Text { get; set; }
    }

    internal class Concrete : IDependency
    {
    }

    internal interface IDependency
    {
    }
}
