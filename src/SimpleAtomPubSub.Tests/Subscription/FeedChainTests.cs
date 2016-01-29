using System.Linq;
using Moq;
using NUnit.Framework;
using SimpleAtomPubSub.Environment;
using SimpleAtomPubSub.Formatters;
using SimpleAtomPubSub.Persistance;
using SimpleAtomPubSub.Subscription;

namespace SimpleAtomPubSub.Tests.Subscription
{
    [TestFixture]
    public class FeedChainTests
    {
        protected Mock<ISyndication> syndication;
        protected Mock<IEnvironment> environment;

        [SetUp]
        public void SetUp()
        {
            syndication = new Mock<ISyndication>();
            environment = new Mock<IEnvironment>();
            SimpleAtomPubSub.Environment.Environment.Current = environment.Object;
        }

        [Test]
        public void SingleFeedChain()
        {
            var feed1 = new FeedData() { };
            environment.Setup(m => m.DownloadString("https://feeds.sample.com/")).Returns("<feed>1</feed>");
            syndication.Setup(m => m.Build("<feed>1</feed>")).Returns(feed1);

            var target = new FeedChain("https://feeds.sample.com/", syndication.Object);
            var results = target.ToArray();

            Assert.AreEqual(1, results.Length);
            Assert.AreSame(feed1, results[0]);
        }

        [Test]
        public void ThreeFeedsInChain()
        {
            var feed1 = new FeedData() { PreviousUri = "https://feeds.sample.com/2" };
            environment.Setup(m => m.DownloadString("https://feeds.sample.com/")).Returns("<feed>1</feed>");
            syndication.Setup(m => m.Build("<feed>1</feed>")).Returns(feed1);

            var feed2 = new FeedData() { PreviousUri = "https://feeds.sample.com/3" };
            environment.Setup(m => m.DownloadString("https://feeds.sample.com/2")).Returns("<feed>2</feed>");
            syndication.Setup(m => m.Build("<feed>2</feed>")).Returns(feed2);

            var feed3 = new FeedData() { };
            environment.Setup(m => m.DownloadString("https://feeds.sample.com/3")).Returns("<feed>3</feed>");
            syndication.Setup(m => m.Build("<feed>3</feed>")).Returns(feed3);

            var target = new FeedChain("https://feeds.sample.com/", syndication.Object);
            var results = target.ToArray();

            Assert.AreEqual(3, results.Length);
            Assert.AreSame(feed1, results[0]);
            Assert.AreSame(feed2, results[1]);
            Assert.AreSame(feed3, results[2]);
        }
    }
}
