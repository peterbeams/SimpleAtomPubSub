﻿using NUnit.Framework;
using SimpleAtomPubSub.Subscriber.Handlers;

namespace SimpleAtomPubSub.Tests.Subscription
{
    [TestFixture]
    public class EmptyHandlerCollectionTests
    {
        protected HandlerCollection target;

        [SetUp]
        public void SetUp()
        {
            target = new HandlerCollection();
        }

        [Test]
        public void DispatchingToNoHandlers()
        {
            target.Handle(new object());
        }
    }
}
