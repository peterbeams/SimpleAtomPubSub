using System;

namespace SimpleAtomPubSub.Persistance
{
    public interface IEventPersistance
    {
        void AddToWorkingFeed(Message e);
        FeedData GetMessages(string feedUri);
        void MoveToNewFeed(string sourceFeedUri, string archiveFeedUri, DateTime createdAt);
    }
}
