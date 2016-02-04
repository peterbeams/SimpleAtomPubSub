using System.Collections.Generic;
using System.Xml.Linq;
using NUnit.Framework;
using SimpleAtomPubSub.Serialization;

namespace SimpleAtomPubSub.Tests.Serialization
{
    [TestFixture]
    public class SimpleXmlMessageSerializaionTests
    {
        public class MessageA
        {
            public string Name { get; set; }
            public int Duration { get; set; }
        }

        public class ComplicatedMessage
        {
            public Dictionary<int, string> Lookup { get; set; }
            public List<string> Things { get; set; }
        }

        [Test]
        public void ComplicatedMessageSerializationTest()
        {
            var expected = @"<ComplicatedMessage>
                  <Lookup>
                    <Element>
                      <Key>1</Key>
                      <Value>A</Value>
                    </Element>
                    <Element>
                      <Key>2</Key>
                      <Value>B</Value>
                    </Element>
                  </Lookup>
                  <Things>
                    <Element>C</Element>
                    <Element>D</Element>
                  </Things>
                </ComplicatedMessage>";

            var m = new ComplicatedMessage()
            {
                Lookup = new Dictionary<int, string> { { 1, "A" }, { 2, "B" }, },
                Things = new List<string> { "C", "D" }
            };
            var target = new SimpleXmlMessageSerializaion();
            var xml = target.Serialize(m);
            Assert.AreEqual(XElement.Parse(expected).ToString(), XElement.Parse(xml).ToString());
        }

        [Test]
        public void ComplicatedMessageDeserializationTest()
        {
            var xml = @"<ComplicatedMessage>
                  <Lookup>
                    <Element>
                      <Key>1</Key>
                      <Value>A</Value>
                    </Element>
                    <Element>
                      <Key>2</Key>
                      <Value>B</Value>
                    </Element>
                  </Lookup>
                  <Things>
                    <Element>C</Element>
                    <Element>D</Element>
                  </Things>
                </ComplicatedMessage>";

            var target = new SimpleXmlMessageSerializaion()
            {
                MessageTypes = new[] { typeof(ComplicatedMessage) }
            };
            
            var result = target.Deserialize(xml) as ComplicatedMessage;
            Assert.AreEqual("A", result.Lookup[1]);
            Assert.AreEqual("B", result.Lookup[2]);
            Assert.AreEqual("C", result.Things[0]);
            Assert.AreEqual("D", result.Things[1]);
        }

        [Test]
        public void SerializeMessage()
        {
            var target = new SimpleXmlMessageSerializaion();
            var xml = target.Serialize(new MessageA { Name = "ABC", Duration = 1 });
            Assert.AreEqual(XElement.Parse(@"<MessageA><Name>ABC</Name><Duration>1</Duration></MessageA>").ToString(), XElement.Parse(xml).ToString());
        }

        [Test]
        public void DeserializeMessage()
        {
            var xml = @"<MessageA><Name>ABC</Name><Duration>1</Duration></MessageA>";
            var target = new SimpleXmlMessageSerializaion() { MessageTypes = new[] { typeof(MessageA) } };
            var result = target.Deserialize(xml);
            Assert.IsInstanceOf<MessageA>(result);
            Assert.AreEqual("ABC", (result as MessageA).Name);
            Assert.AreEqual(1, (result as MessageA).Duration);
        }

        [Test]
        public void DeserializeMissingTypeMessage()
        {
            var xml = @"<MessageB></MessageB>";
            var target = new SimpleXmlMessageSerializaion() { MessageTypes = new[] { typeof(MessageA) } };
            var result = target.Deserialize(xml);
            Assert.IsInstanceOf<MissingMessage>(result);
        }
    }
}