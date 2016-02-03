using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleAtomPubSub.Feed;

namespace SimpleAtomPubSub.FeedHost
{
    public class FeedModule : Nancy.NancyModule
    {
        internal static IEventFeed feed;
        internal static Uri feedUrl = new Uri("http://localhost:12345");

        public FeedModule()
        {
            feed = SimpleAtomPubSub.Configure.AsAPublisher("EventStore");

            Get["/"] = _ => feed.GetWorkingFeed(feedUrl);
            Get["/{name}"] = parameters => feed.GetArchiveFeed(string.Concat("/", parameters.name), feedUrl);
        }
    }
}
