using System;
using SimpleAtomPubSub.Publisher.Persistance;

namespace SimpleAtomPubSub.Formatters
{
    public interface ISyndicationFormatter
    {
        string Build(FeedData message, Uri baseUri);
        FeedData Build(string data);
    }
}