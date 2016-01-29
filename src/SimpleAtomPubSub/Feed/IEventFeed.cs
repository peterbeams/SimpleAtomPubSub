using System;

namespace SimpleAtomPubSub.Feed
{
    public interface IEventFeed
    {
        void Publish<TEvent>(TEvent e);
        string GetWorkingFeed(Uri baseUri);
        string GetArchiveFeed(string uri, Uri baseUri);
        void ArhiveWorkingFeed();
    }
}
