using System;
using SimpleAtomPubSub.Formatters;
using SimpleAtomPubSub.Persistance;
using SimpleAtomPubSub.Serialization;

namespace SimpleAtomPubSub.Feed
{
    public class EventFeed : IEventFeed
    {
        private const string WORKING_FEED_URI = "/";

        internal EventFeed()
        {
        }

        public IEventPersistance EventPeristance { get; set; }
        public IMessageSerializer Serializer { get; set; }
        public ISyndication Syndication { get; set; }

        public void Publish<TEvent>(TEvent e)
        {
            var message = new Message
            {
                Body = Serializer.Serialize(e),
                CreatedAt = Environment.Environment.Current.UtcNow,
                Id = Guid.NewGuid(),
                FeedUri = WORKING_FEED_URI
            };

            EventPeristance.AddToWorkingFeed(message);
        }

        internal void EnsureWorkingFeedExists()
        {
            EventPeristance.CreateFeedIfNotExists(WORKING_FEED_URI);
        }

        public string GetArchiveFeed(string uri, Uri baseUri)
        {
            return GetFeed(uri, baseUri);
        }

        public string GetWorkingFeed(Uri baseUri)
        {
            return GetFeed(WORKING_FEED_URI, baseUri);
        }

        public void ArhiveWorkingFeed()
        {
            var archiveFeedDate = Environment.Environment.Current.UtcNow;
            var archiveFeedUri = archiveFeedDate.ToString("/yyyy-MM-dd-HH-mm");
            EventPeristance.MoveToNewFeed(WORKING_FEED_URI, archiveFeedUri, archiveFeedDate);
        }

        private string GetFeed(string uri, Uri baseUri)
        {
            var messages = EventPeristance.GetMessages(uri);
            return Syndication.Build(messages, baseUri);
        }
    }
}