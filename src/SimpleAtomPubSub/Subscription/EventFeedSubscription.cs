using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SimpleAtomPubSub.Formatters;
using SimpleAtomPubSub.Handler;
using SimpleAtomPubSub.Persistance;
using SimpleAtomPubSub.Serialization;

namespace SimpleAtomPubSub.Subscription
{
    public class EventFeedSubscription : IEventFeedSubscription
    {
        public Uri Url { get; set; }
        public IFeedChainFactory FeedChainFactory { get; set; }
        public ISyndication Syndication { get; set; }
        public TimeSpan PollingInterval { get; set; }
        public IMessageDeserializer Deserializer { get; set; }
        public Guid? LastObservedEventId { get; set; }
        internal IHandler<object> Handlers { get; set; }

        public void Poll()
        {
            do
            {
                Run();
                Thread.Sleep(PollingInterval);
            } while (true);
        }

        public void Run()
        {
            var feeds = GetFeedsUptoLastSeenMessage(Url.ToString());
            var messages = feeds.SelectMany(x => x.Messages);

            var newEvents = messages
                .OrderByDescending(x => x.CreatedAt)
                .TakeWhile(x => x.Id != LastObservedEventId)
                .Reverse();

            foreach (var m in newEvents)
            {
                var mo = Deserializer.Deserialize(m.Body);
                Handlers.Handle(mo);

                LastObservedEventId = m.Id;
            }
        }

        private IEnumerable<FeedData> GetFeedsUptoLastSeenMessage(string uri)
        {
            var feedChain = FeedChainFactory.Get(uri, Syndication);
            foreach (var feed in feedChain)
            {
                yield return feed;
                if (feed.Messages.Any(x => x.Id == LastObservedEventId))
                    yield break;
            }
        }
    }
}