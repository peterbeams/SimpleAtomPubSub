using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SimpleAtomPubSub.Formatters;
using SimpleAtomPubSub.Publisher.Persistance;
using SimpleAtomPubSub.Serialization;
using SimpleAtomPubSub.Subscriber.Feed;
using SimpleAtomPubSub.Subscriber.Handlers;
using SimpleAtomPubSub.Subscriber.Persistance;
using SimpleAtomPubSub.Subscriber.Subscription;

namespace SimpleAtomPubSub.Tests.Subscription
{
    [TestFixture]
    public class EventFeedSubscriptionTests
    {
        private EventFeedObserver target;
        private Mock<IFeedChainFactory> feedChainFactory;
        private Mock<ISyndicationFormatter> syndication;
        private Mock<EventHandler<Message>> messageReceivedHandler;
        private Mock<ISubscriptionPersistance> _persistance;

        [SetUp]
        public void SetUp()
        {
            feedChainFactory = new Mock<IFeedChainFactory>();
            syndication = new Mock<ISyndicationFormatter>();
            messageReceivedHandler = new Mock<EventHandler<Message>>(MockBehavior.Strict);
            _persistance = new Mock<ISubscriptionPersistance>();

            target = new EventFeedObserver(new Uri("https://feeds.sample.com/"), syndication.Object, feedChainFactory.Object, _persistance.Object);
            target.EventReceived += messageReceivedHandler.Object;
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
            
            feedChainFactory.Setup(m => m.Get("https://feeds.sample.com/", syndication.Object)).Returns(feeds);

            messageReceivedHandler.Setup(m => m(target, feeds[0].Messages[0]));
            messageReceivedHandler.Setup(m => m(target, feeds[0].Messages[1]));
            messageReceivedHandler.Setup(m => m(target, feeds[1].Messages[0]));
            messageReceivedHandler.Setup(m => m(target, feeds[1].Messages[1]));
            
            target.ReceiveLatestEvents();

            messageReceivedHandler.Verify(m => m(target, feeds[0].Messages[0]), Times.Once);
            messageReceivedHandler.Verify(m => m(target, feeds[0].Messages[1]), Times.Once);
            messageReceivedHandler.Verify(m => m(target, feeds[1].Messages[0]), Times.Once);
            messageReceivedHandler.Verify(m => m(target, feeds[1].Messages[1]), Times.Once);
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

            messageReceivedHandler.Setup(m => m(target, feeds[0].Messages[0]));
            messageReceivedHandler.Setup(m => m(target, feeds[0].Messages[1]));
            messageReceivedHandler.Setup(m => m(target, feeds[1].Messages[0]));

            feedChainFactory.Setup(m => m.Get("https://feeds.sample.com/", syndication.Object)).Returns(feeds);
            
            target.LastObservedEventId = new Guid("40000000-0000-0000-0000-000000000000");
            target.ReceiveLatestEvents();

            messageReceivedHandler.Verify(m => m(target, feeds[0].Messages[0]), Times.Once);
            messageReceivedHandler.Verify(m => m(target, feeds[0].Messages[1]), Times.Once);
            messageReceivedHandler.Verify(m => m(target, feeds[1].Messages[0]), Times.Once);
        }
    }
}
