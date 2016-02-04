using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SimpleAtomPubSub.Publisher.Persistance;
using SimpleAtomPubSub.Subscriber.DeadLetter;
using SimpleAtomPubSub.Subscriber.Persistance;

namespace SimpleAtomPubSub.Tests.Subscription
{
    [TestFixture]
    public class FailureChannelTests
    {
        private Mock<IDeadLetterPersistance> _persistance;
        private Mock<EventHandler<Message>> _messageReadyHandler;
        private FailureChannel target;

        [SetUp]
        public void SetUp()
        {
            _persistance = new Mock<IDeadLetterPersistance>();
            _messageReadyHandler = new Mock<EventHandler<Message>>();

            target = new FailureChannel(_persistance.Object);
            target.MessageReadyForRetry += _messageReadyHandler.Object;
        }

        [Test]
        public void DeadLetteringStoresTheMessage()
        {
            var msg = new Message();
            var ex = new Exception();
            target.DeadLetter(msg, ex);

            _persistance.Verify(m => m.Deadletter(msg, ex));
        }

        [Test]
        public void PickUpRetries()
        {
            var messages = new List<Message>
            {
                new Message(),
                new Message()
            };
            _persistance.Setup(m => m.PullRetries()).Returns(messages);

            var task = target.HandleFailuresReadyToReProcessAsync();
            task.Wait();

            _messageReadyHandler.Verify(m => m(target, messages[0]));
            _messageReadyHandler.Verify(m => m(target, messages[1]));


        }
    }
}
