using System.Collections.Generic;
using SimpleAtomPubSub.Formatters;
using SimpleAtomPubSub.Persistance;

namespace SimpleAtomPubSub.Subscription
{
    public interface IFeedChainFactory
    {
        IEnumerable<FeedData> Get(string startUrl, ISyndication syndication);
    }
}