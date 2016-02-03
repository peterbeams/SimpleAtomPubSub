using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SimpleAtomPubSub.Formatters;
using SimpleAtomPubSub.Handler;
using SimpleAtomPubSub.Persistance;
using SimpleAtomPubSub.Serialization;
using SimpleAtomPubSub.Subscription;

namespace SimpleAtomPubSub.Tests.Subscription
{
    [TestFixture]
    public class EventFeedSubscriptionTests
    {
        private EventFeedSubscription target;
        private Mock<IFeedChainFactory> feedChainFactory;
        private Mock<ISyndication> syndication;
        private Mock<IMessageDeserializer> deserializer;
        private Mock<IHandler<object>> handlers;

        [SetUp]
        public void SetUp()
        {
            feedChainFactory = new Mock<IFeedChainFactory>();
            syndication = new Mock<ISyndication>();
            deserializer = new Mock<IMessageDeserializer>();
            handlers = new Mock<IHandler<object>>(MockBehavior.Strict);

            target = new EventFeedSubscription()
            {
                Url = new Uri("https://feeds.sample.com/"),
                FeedChainFactory = feedChainFactory.Object,
                Syndication = syndication.Object,
                Deserializer = deserializer.Object,
                Handlers = handlers.Object
            };
        }

        [Test]
        public void ReadAllMessagesFromTwoFeeds()
        {
            var feeds = new[]
            {
                new FeedData {
                    Messages = new List<Message>() {
                        new Message() { Id = Guid.Parse("10000000-0000-0000-0000-000000000000"), Body = "A", CreatedAt = new DateTime(2016, 01, 26) },
                        new Message() { Id = Guid.Parse("20000000-0000-0000-0000-000000000000"), Body = "B", CreatedAt = new DateTime(2016, 01, 25) },
                    }
                },
                new FeedData {
                    Messages = new List<Message>() {
                        new Message() { Id = Guid.Parse("30000000-0000-0000-0000-000000000000"), Body = "C", CreatedAt = new DateTime(2016, 01, 24) },
                        new Message() { Id = Guid.Parse("40000000-0000-0000-0000-000000000000"), Body = "D", CreatedAt = new DateTime(2016, 01, 23) },
                    }
                }
            };

            var messageA = new object();
            deserializer.Setup(m => m.Deserialize("A")).Returns(messageA);
            var messageB = new object();
            deserializer.Setup(m => m.Deserialize("B")).Returns(messageB);
            var messageC = new object();
            deserializer.Setup(m => m.Deserialize("C")).Returns(messageC);
            var messageD = new object();
            deserializer.Setup(m => m.Deserialize("D")).Returns(messageD);

            feedChainFactory.Setup(m => m.Get("https://feeds.sample.com/", syndication.Object)).Returns(feeds);

            handlers.Setup(m => m.Handle(messageA));
            handlers.Setup(m => m.Handle(messageB));
            handlers.Setup(m => m.Handle(messageC));
            handlers.Setup(m => m.Handle(messageD));

            target.HandleLatestEvents();

            handlers.Verify(m => m.Handle(messageA), Times.Once);
            handlers.Verify(m => m.Handle(messageB), Times.Once);
            handlers.Verify(m => m.Handle(messageC), Times.Once);
            handlers.Verify(m => m.Handle(messageD), Times.Once);
        }

        [Test]
        public void ReadAllMessagesFromThreeFeedsStoppingMidFeed()
        {
            var feeds = new[]
            {
                new FeedData {
                    Messages = new List<Message>() {
                        new Message() { Id = Guid.Parse("10000000-0000-0000-0000-000000000000"), Body = "A", CreatedAt = new DateTime(2016, 01, 26) },
                        new Message() { Id = Guid.Parse("20000000-0000-0000-0000-000000000000"), Body = "B", CreatedAt = new DateTime(2016, 01, 25) },
                    }
                },
                new FeedData {
                    Messages = new List<Message>() {
                        new Message() { Id = Guid.Parse("30000000-0000-0000-0000-000000000000"), Body = "C", CreatedAt = new DateTime(2016, 01, 24) },
                        new Message() { Id = Guid.Parse("40000000-0000-0000-0000-000000000000"), Body = "D", CreatedAt = new DateTime(2016, 01, 23) },
                    }
                }
                ,
                new FeedData {
                    Messages = new List<Message>() {
                        new Message() { Id = Guid.Parse("50000000-0000-0000-0000-000000000000"), Body = "E", CreatedAt = new DateTime(2016, 01, 22) },
                        new Message() { Id = Guid.Parse("60000000-0000-0000-0000-000000000000"), Body = "F", CreatedAt = new DateTime(2016, 01, 21) },
                    }
                }
            };

            var messageA = new object();
            deserializer.Setup(m => m.Deserialize("A")).Returns(messageA);
            var messageB = new object();
            deserializer.Setup(m => m.Deserialize("B")).Returns(messageB);
            var messageC = new object();
            deserializer.Setup(m => m.Deserialize("C")).Returns(messageC);

            feedChainFactory.Setup(m => m.Get("https://feeds.sample.com/", syndication.Object)).Returns(feeds);


            handlers.Setup(m => m.Handle(messageA));
            handlers.Setup(m => m.Handle(messageB));
            handlers.Setup(m => m.Handle(messageC));
            
            target.LastObservedEventId = new Guid("40000000-0000-0000-0000-000000000000");
            target.HandleLatestEvents();

            handlers.Verify(m => m.Handle(messageA), Times.Once);
            handlers.Verify(m => m.Handle(messageB), Times.Once);
            handlers.Verify(m => m.Handle(messageC), Times.Once);
        }
    }
}
