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
        
        public FeedModule()
        {
            feed = SimpleAtomPubSub.Configure.AsADirectFeedReader("EventStore");

            Get["/"] = _ => feed.GetWorkingFeed(new Uri(Context.Request.Url.SiteBase));
            Get["/{name}"] = parameters => feed.GetArchiveFeed(string.Concat("/", parameters.name), new Uri(Context.Request.Url.SiteBase));
        }
    }
}
