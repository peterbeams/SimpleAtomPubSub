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

        [Test]
        public void SerializeMessage()
        {
            var target = new SimpleXmlMessageSerializaion();
            var xml = target.Serialize(new MessageA { Name = "ABC", Duration = 1 });
            Assert.AreEqual(@"<MessageA>
  <Name>ABC</Name>
  <Duration>1</Duration>
</MessageA>", xml);
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
