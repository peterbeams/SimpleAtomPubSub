using System;

namespace SimpleAtomPubSub.Publisher.Persistance
{
    public interface IEventPersistance
    {
        void AddToWorkingFeed(Message e);
        FeedData GetMessages(string feedUri);
        void MoveToNewFeed(string sourceFeedUri, string archiveFeedUri, DateTime createdAt);
        void CreateFeedIfNotExists(string feedUri);
    }
}