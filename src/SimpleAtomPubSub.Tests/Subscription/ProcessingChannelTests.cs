using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SimpleAtomPubSub.Publisher.Persistance;
using SimpleAtomPubSub.Serialization;
using SimpleAtomPubSub.Subscriber.DeadLetter;
using SimpleAtomPubSub.Subscriber.Handlers;
using SimpleAtomPubSub.Subscriber.Subscription;

namespace SimpleAtomPubSub.Tests.Subscription
{
    [TestFixture]
    public class ProcessingChannelTests
    {
        private ProcessingChannel target;
        private Mock<IMessageDeserializer> _deserializer;
        private Mock<IHandler<object>> _handlers;
        private Mock<EventHandler<MessageFailedEventArgs>> _messageFailedHandler;

        [SetUp]
        public void SetUp()
        {
            _deserializer = new Mock<IMessageDeserializer>();
            _handlers = new Mock<IHandler<object>>();
            _messageFailedHandler = new Mock<EventHandler<MessageFailedEventArgs>>(MockBehavior.Strict);
            target = new ProcessingChannel(_deserializer.Object, _handlers.Object);

            target.MessageFailed += _messageFailedHandler.Object;
        }

        [Test]
        public void MessageWorks()
        {
            var deserializedObject = new object();
            _deserializer.Setup(m => m.Deserialize("<body/>")).Returns(deserializedObject);
            var message = new Message() { Body = "<body/>"};
            target.ProcessEvent(this, message);
            _handlers.Verify(m => m.Handle(deserializedObject));
        }

        [Test]
        public void MessageFails()
        {
            var deserializedObject = new object();
            _deserializer.Setup(m => m.Deserialize("<body/>")).Returns(deserializedObject);

            var exception = new Exception();
            _handlers.Setup(m => m.Handle(deserializedObject)).Throws(exception);
            
            var message = new Message() { Body = "<body/>" };
            _messageFailedHandler.Setup(m => m(target, It.Is<MessageFailedEventArgs>(x => x.Exception == exception && x.Message == message)));

            target.ProcessEvent(this, message);

            _messageFailedHandler.Verify(m => m(target, It.Is<MessageFailedEventArgs>(x => x.Exception == exception && x.Message == message)));
        }
    }
}
