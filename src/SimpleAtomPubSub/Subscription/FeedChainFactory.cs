using System.Collections.Generic;
using SimpleAtomPubSub.Formatters;
using SimpleAtomPubSub.Persistance;

namespace SimpleAtomPubSub.Subscription
{
    public class FeedChainFactory : IFeedChainFactory
    {
        public IEnumerable<FeedData> Get(string startUrl, ISyndication syndication)
        {
            return new FeedChain(startUrl, syndication);
        }
    }
}