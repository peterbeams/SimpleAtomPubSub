using Moq;
using NUnit.Framework;
using SimpleAtomPubSub.Environment;
using SimpleAtomPubSub.Handler;
using SimpleAtomPubSub.Subscription;

namespace SimpleAtomPubSub.Tests.Subscription
{
    [TestFixture]
    public class MultipleHandlerCollectionTests
    {
        protected HandlerCollection target;
        protected Mock<IEnvironment> environment;

        #region "message types"
        public class MessageA { }
        public class MessageB { }
        public class MessageC { }
        #endregion

        #region "handler types"
        public class FirstHandler : IHandler<MessageA>
        {
            public virtual void Handle(MessageA message)
            {
                throw new System.NotImplementedException();
            }
        }
        public class SecondHandler : IHandler<MessageA>, IHandler<MessageB>
        {
            public virtual void Handle(MessageA message)
            {
                throw new System.NotImplementedException();
            }

            public virtual void Handle(MessageB message)
            {
                throw new System.NotImplementedException();
            }
        }
        public class ThirdHandler : IHandler<MessageB>
        {
            public virtual void Handle(MessageB message)
            {
                throw new System.NotImplementedException();
            }
        }
        #endregion

        [SetUp]
        public void SetUp()
        {
            environment = new Mock<IEnvironment>(MockBehavior.Strict);
            Environment.Environment.Current = environment.Object;

            target = new HandlerCollection { typeof(FirstHandler), typeof(SecondHandler), typeof(ThirdHandler) };
        }

        [Test]
        public void HandleMessageA()
        {
            var firstHandler = new Mock<FirstHandler>();
            var secondHandler = new Mock<SecondHandler>();

            environment.Setup(m => m.CreateInstance(typeof(FirstHandler))).Returns(firstHandler.Object);
            environment.Setup(m => m.CreateInstance(typeof(SecondHandler))).Returns(secondHandler.Object);

            var message = new MessageA();
            target.Handle(message);

            firstHandler.Verify(m => m.Handle(message));
            secondHandler.Verify(m => m.Handle(message));
        }

        [Test]
        public void HandleMessageB()
        {
            var secondHandler = new Mock<SecondHandler>();
            var thirdHandler = new Mock<ThirdHandler>();

            environment.Setup(m => m.CreateInstance(typeof(SecondHandler))).Returns(secondHandler.Object);
            environment.Setup(m => m.CreateInstance(typeof(ThirdHandler))).Returns(thirdHandler.Object);

            var message = new MessageB();
            target.Handle(message);

            secondHandler.Verify(m => m.Handle(message));
            thirdHandler.Verify(m => m.Handle(message));
        }

        [Test]
        public void HandleMessageC()
        {
            var message = new MessageC();
            target.Handle(message);
        }
    }
}