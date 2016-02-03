using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        public void Subscribe()
        {
            Task.Factory.StartNew(Poll);
        }

        internal async void Poll()
        {
            do
            {
                var x = await HandleLatestEventsAsync();
                Thread.Sleep(PollingInterval);
            } while (true);
        }

        private Task<bool> HandleLatestEventsAsync()
        {
            return Task.Factory.StartNew(HandleLatestEvents);
        }

        internal bool HandleLatestEvents()
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

            return newEvents.Any();
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