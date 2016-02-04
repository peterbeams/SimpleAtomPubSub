using System.Collections.Generic;
using SimpleAtomPubSub.Formatters;
using SimpleAtomPubSub.Publisher.Persistance;

namespace SimpleAtomPubSub.Subscriber.Feed
{
    public interface IFeedChainFactory
    {
        IEnumerable<FeedData> Get(string startUrl, ISyndicationFormatter syndicationFormatter);
    }
}