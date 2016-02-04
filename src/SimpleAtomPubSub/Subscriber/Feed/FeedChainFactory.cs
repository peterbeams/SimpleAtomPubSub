using System.Collections.Generic;
using SimpleAtomPubSub.Formatters;
using SimpleAtomPubSub.Publisher.Persistance;

namespace SimpleAtomPubSub.Subscriber.Feed
{
    public class FeedChainFactory : IFeedChainFactory
    {
        public IEnumerable<FeedData> Get(string startUrl, ISyndicationFormatter syndicationFormatter)
        {
            return new FeedChain(startUrl, syndicationFormatter);
        }
    }
}