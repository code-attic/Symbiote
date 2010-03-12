using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Symbiote.Jackalope.Impl;
using Symbiote.Core.Extensions;

namespace Jackalope.Tests
{
    public class Test1
    {
        public string Value1 { get; set; }
        public int Value2 { get; set; }
    }

    public class Test2
    {
        public int Value1 { get; set; }
        public string Value2 { get; set; }
    }

    [TestFixture]
    public class WhenDeserializing
    {
        [Test]
        public void ShouldDeserializeTheAppropriateType()
        {
            var original = new Test1() {Value1 = "test1", Value2 = 1};
            var json = original.ToJson();
            object newMessage = json.FromJson();
            Assert.AreEqual(newMessage.GetType(), typeof(Test1));
        }

        [Test]
        public void ShouldTransformToXmlWithTypeInformation()
        {
            var message = new TestMessage() { Id = 1, MessageBody = "This is a test message. Hooray!", Sent = DateTime.Now };
            var json = message.ToJson();
            var xml = json.JsonToXml("TestMessage");
            Assert.IsNotNull(xml);
        }

        [Test]
        public void ShouldDeserializeWithoutTypeData()
        {
            var message = new TestMessage()
                              {Id = 1, MessageBody = "This is a test message. Hooray!", Sent = DateTime.Now};

            var json = message.ToJson();

            Assert.IsFalse(string.IsNullOrEmpty(json));

            var xml = json.JsonToXml("TestMessage");
            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xml.OuterXml));
            var xmlSerializer = new XmlSerializer(typeof (TestMessage));
            var fromXml = xmlSerializer.Deserialize(stream);

            Assert.IsNotNull(fromXml);
        }
    }

    public class TestMessage
    {
        public virtual int Id { get; set; }
        public virtual string MessageBody { get; set; }
        public virtual DateTime Sent { get; set; }
    }
}
