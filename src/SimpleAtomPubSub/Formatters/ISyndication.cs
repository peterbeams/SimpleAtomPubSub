using System;
using SimpleAtomPubSub.Persistance;

namespace SimpleAtomPubSub.Formatters
{
    public interface ISyndication
    {
        string Build(FeedData message, Uri baseUri);
        FeedData Build(string data);
    }
}
