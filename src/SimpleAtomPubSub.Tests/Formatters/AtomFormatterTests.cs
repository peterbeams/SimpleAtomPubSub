using System;
using System.Linq;
using NUnit.Framework;
using SimpleAtomPubSub.Formatters;
using SimpleAtomPubSub.Persistance;

namespace SimpleAtomPubSub.Tests.Formatters
{
    public class AtomFormatterTests
    {
        protected AtomFormatter target;

        [SetUp]
        public void SetUp()
        {
            target = new AtomFormatter();
        }

        private const string SampleFeed = @"<?xml version=""1.0"" encoding=""utf-16""?><feed xmlns=""http://www.w3.org/2005/Atom""><title type=""text""></title><id>a88e560c-a96d-4ce4-8d11-5e4112b53df8</id><updated>2016-01-26T14:08:54Z</updated><link rel=""next-archive"" type=""application/atom+xml"" title=""Next In Archive"" href=""https://feeds.sample.com/3"" /><link rel=""prev-archive"" type=""application/atom+xml"" title=""Previous In Archive"" href=""https://feeds.sample.com/1"" /><entry><id>10000000-0000-0000-0000-000000000000</id><title type=""text""></title><updated>2016-01-26T13:45:10Z</updated><content type=""RawSyndicationContent""><message></message></content></entry><entry><id>20000000-0000-0000-0000-000000000000</id><title type=""text""></title><updated>2016-01-26T13:45:33Z</updated><content type=""RawSyndicationContent""><message2></message2></content></entry></feed>";

        [Test]
        public void BuildFromFeedDto()
        {
            var result = target.Build(new FeedData()
            {
                Id = new Guid("a88e560c-a96d-4ce4-8d11-5e4112b53df8"),
                DateCreated = new DateTime(2016, 01, 26, 14, 8, 54),
                PreviousUri = "https://feeds.sample.com/1",
                NextUri = "https://feeds.sample.com/3",
                Messages = new []
                {
                    new Message()
                    {
                        Body = "<message></message>",
                        Id = new Guid("10000000-0000-0000-0000-000000000000"),
                        CreatedAt = new DateTime(2016, 01, 26, 13, 45, 10)
                    },
                    new Message()
                    {
                        Body = "<message2></message2>",
                        Id = new Guid("20000000-0000-0000-0000-000000000000"),
                        CreatedAt = new DateTime(2016, 01, 26, 13, 45, 33)
                    }
                }
            }, new Uri("https://feeds.sample.com"));

            Assert.AreEqual(SampleFeed, result);
        }

        [Test]
        public void BuildFromFeedXml()
        {
            var result = target.Build(SampleFeed);

            Assert.IsNotNull(result);
            Assert.AreEqual(new Guid("a88e560c-a96d-4ce4-8d11-5e4112b53df8"), result.Id);
            Assert.AreEqual(new DateTime(2016, 01, 26, 14, 8, 54), result.DateCreated);
            Assert.AreEqual("https://feeds.sample.com/1", result.PreviousUri);
            Assert.AreEqual("https://feeds.sample.com/3", result.NextUri);
            Assert.IsNotNull(result.Messages);
            Assert.AreEqual(2, result.Messages.Count());
            Assert.AreEqual(@"<message></message>", result.Messages.ToArray()[0].Body);
            Assert.AreEqual(new Guid("10000000-0000-0000-0000-000000000000"), result.Messages.ToArray()[0].Id);
            Assert.AreEqual(new DateTime(2016, 01, 26, 13, 45, 10), result.Messages.ToArray()[0].CreatedAt);
        }
    }
}
