using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Symbiote.Core.Extensions;

namespace Core.Tests
{
    [Subject("JSON Serialization")]
    public class when_serializing_string
    {
        protected static string test = "this is a string";
        protected static string result;

        private Because of = () => { result = test.ToJson(false); };

        private It should_not_include_braces = () => result.ShouldEqual("\"{0}\"".AsFormat(test));
    }

    public class TestClass
    {
        public object Content { get; set; }
    }

    public class when_serializing_objects
    {
        protected static TestClass test = new TestClass() { Content = "yay"};
        protected static TestClass result;

        private Because of = () =>
                                 {
                                     var json = test.ToJson();
                                     result = json.FromJson<TestClass>();
                                 };

        private It should_deserialize_correctly = () => result.Content.ShouldEqual(test.Content);
    }

    [Subject("JSON Deserialization")]
    public class when_deserializing_attachments
    {
        protected static string test = @"{""_id"":""attachment_doc"",""_attachments"":{""foo.txt"":{""content_type"":""text/plain"",""data"":""abcdef""},""bar.txt"":{""content_type"":""text/plain"",""data"":""abcdef""}}}";

        protected static TestTarget target;

        private Because of = () => { target = test.FromJson<TestTarget>(); };

        private It should_not_be_null = () =>
                                            {
                                                target.ShouldNotBeNull();
                                            };

        private It should_have_2_attachments = () => target.Attachments.Count().ShouldEqual(2);

        private It should_have_foo_as_first_attachment = () => target.Attachments.First().ShouldEqual("foo.txt");
        private It should_have_bar_as_last_attachment = () => target.Attachments.Last().ShouldEqual("bar.txt");

        private It should_serialize_correctly = () => target.ToJson(false).ShouldEqual(test);
    }

    public class when_adding_attachment_stub
    {
        protected static string test = @"{""_id"":""attachment_doc"",""_attachments"":{""foo.txt"":{""content_type"":""text/plain"",""data"":""abcdef""},""bar.txt"":{""content_type"":""text/plain"",""data"":""abcdef""}}}";

        protected static TestTarget target;

        private Because of = () =>
                                 {
                                     target = test.FromJson<TestTarget>();
                                     target.RemoveAttachment("foo.txt");
                                     target.AddAttachment("baz.txt", "text/plain", 64);
                                 };

        private It should_have_2_attachments = () => target.Attachments.Count().ShouldEqual(2);

        private It should_have_bar_as_first_attachment = () => target.Attachments.First().ShouldEqual("bar.txt");
        private It should_have_baz_as_last_attachment = () => target.Attachments.Last().ShouldEqual("baz.txt");
    }

    public class TestTarget
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("_attachments")]
        private JObject attachments { get; set; }

        [JsonIgnore]
        public IEnumerable<string> Attachments
        {
            get
            {
                return attachments.Root.Children().Select(x => (x as JProperty).Name);
            }
        }

        public void AddAttachment(string attachmentName, string contentType, long contentLength)
        {
            var attachment = new 
            {
                Stub = true,
                ContentType = contentType,
                ContentLength = contentLength
            };
            var jsonStub = new JProperty(attachmentName, JToken.FromObject(attachment));
            attachments.Add(jsonStub);
        }

        public void RemoveAttachment(string attachmentName)
        {
            attachments.Remove(attachmentName);
        }
    }
}
