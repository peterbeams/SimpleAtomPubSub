using System;
using Moq;
using NUnit.Framework;
using SimpleAtomPubSub.Environment;
using SimpleAtomPubSub.Feed;
using SimpleAtomPubSub.Formatters;
using SimpleAtomPubSub.Persistance;
using SimpleAtomPubSub.Serialization;

namespace SimpleAtomPubSub.Tests.Feed
{
    [TestFixture]
    public class EventFeedTests
    {
        protected Mock<IMessageSerializer> serializer;
        protected Mock<IEventPersistance> persistance;
        protected Mock<ISyndication> syndication;
        protected Mock<IEnvironment> environment;
        protected EventFeed target;

        [SetUp]
        public void SetUp()
        {
            serializer = new Mock<IMessageSerializer>();
            persistance = new Mock<IEventPersistance>();
            syndication = new Mock<ISyndication>();
            environment = new Mock<IEnvironment>();

            SimpleAtomPubSub.Environment.Environment.Current = environment.Object;

            target = new EventFeed()
            {
                EventPeristance = persistance.Object,
                Serializer = serializer.Object,
                Syndication = syndication.Object
            };
        }

        [Test]
        public void EnsureWorkingFeedCreatedTest()
        {
            target.EnsureWorkingFeedExists();
            persistance.Verify(m => m.CreateFeedIfNotExists("/"));
        }

        [Test]
        public void PublishingAddsMessageToWorkingFeed()
        {
            var message = new object();
            target.Publish(message);
            persistance.Verify(x => x.AddToWorkingFeed(It.IsAny<Message>()));;
        }

        [Test]
        public void PublishingAddsMessageWithSerializedBody()
        {
            var message = new object();
            serializer.Setup(x => x.Serialize(message)).Returns("<xml />");
            target.Publish(message);
            persistance.Verify(x => x.AddToWorkingFeed(It.Is<Message>(m => m.Body.Equals("<xml />"))));
        }

        [Test]
        public void PublishingAddsMessageWithTimeStamp()
        {
            environment.Setup(m => m.UtcNow).Returns(new DateTime(2016, 01, 25, 13, 44, 2));
            var message = new object();
            target.Publish(message);
            persistance.Verify(x => x.AddToWorkingFeed(It.Is<Message>(m => m.CreatedAt == new DateTime(2016, 01, 25, 13, 44, 2))));
        }

        [Test]
        public void PublishingAddsMessageWithId()
        {
            var message = new object();
            target.Publish(message);
            persistance.Verify(x => x.AddToWorkingFeed(It.Is<Message>(m => m.Id != Guid.Empty)));
        }

        [Test]
        public void PublishingAddsMessageIntoWorkingFeed()
        {
            var message = new object();
            target.Publish(message);
            persistance.Verify(x => x.AddToWorkingFeed(It.Is<Message>(m => m.FeedUri == "/")));
        }

        [Test]
        public void GettingArchiveFeed()
        {
            var feedData = new FeedData();
            persistance.Setup(x => x.GetMessages("/2016/01/26")).Returns(feedData);
            syndication.Setup(x => x.Build(feedData, new Uri("https://feed.sample.com/"))).Returns("<feed />");

            var result = target.GetArchiveFeed("/2016/01/26", new Uri("https://feed.sample.com/"));

            Assert.AreEqual(result, "<feed />");
        }

        [Test]
        public void GettingWorkingFeed()
        {
            var feedData = new FeedData();
            persistance.Setup(x => x.GetMessages("/")).Returns(feedData);
            syndication.Setup(x => x.Build(feedData, new Uri("https://feed.sample.com/"))).Returns("<feed />");

            var result = target.GetWorkingFeed(new Uri("https://feed.sample.com/"));

            Assert.AreEqual(result, "<feed />");
        }

        [Test]
        public void ArchivingFeed()
        {
            environment.Setup(m => m.UtcNow).Returns(new DateTime(2016, 01, 25, 13, 44, 2));
            target.ArhiveWorkingFeed();
            persistance.Verify(x => x.MoveToNewFeed("/", "/2016-01-25-13-44", new DateTime(2016, 01, 25, 13, 44, 2)));
        }
    }
}
