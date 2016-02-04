using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SimpleAtomPubSub.Formatters;
using SimpleAtomPubSub.Publisher.Persistance;
using SimpleAtomPubSub.Subscriber.Feed;
using SimpleAtomPubSub.Subscriber.Persistance;

namespace SimpleAtomPubSub.Subscriber.Subscription
{
    public class EventFeedObserver : IEventFeedSubscription
    {
        private readonly Uri _uri;
        private readonly ISyndicationFormatter _atomFormatter;
        private readonly IFeedChainFactory _feedChainFactory;
        private readonly ISubscriptionPersistance _persistance;
        private readonly int _subscriptionId;

        public event EventHandler<Message> EventReceived;

        public TimeSpan PollingInterval { get; set; }
        public Guid? LastObservedEventId { get; set; }

        public EventFeedObserver(Uri uri, ISyndicationFormatter atomFormatter, IFeedChainFactory feedChainFactory, ISubscriptionPersistance persistance)
        {
            _uri = uri;
            _atomFormatter = atomFormatter;
            _feedChainFactory = feedChainFactory;
            _persistance = persistance;

            _subscriptionId = _persistance.Register(uri.ToString());
            LastObservedEventId = _persistance.GetLastObservedEventId(_subscriptionId);
        }

        public void StartWatching()
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
            return Task.Factory.StartNew(ReceiveLatestEvents);
        }

        internal bool ReceiveLatestEvents()
        {
            var feeds = GetFeedsUptoLastSeenMessage(_uri.ToString());
            var messages = feeds.SelectMany(x => x.Messages);

            var newEvents = messages
                .OrderByDescending(x => x.CreatedAt)
                .TakeWhile(x => x.Id != LastObservedEventId)
                .Reverse();

            foreach (var m in newEvents)
            {
                OnEventReceived(m);
                LastObservedEventId = m.Id;
                _persistance.SetLastObservedEvent(_subscriptionId, m.Id);
            }

            return newEvents.Any();
        }

        private IEnumerable<FeedData> GetFeedsUptoLastSeenMessage(string uri)
        {
            var feedChain = _feedChainFactory.Get(uri, _atomFormatter);
            foreach (var feed in feedChain)
            {
                yield return feed;
                if (feed.Messages.Any(x => x.Id == LastObservedEventId))
                    yield break;
            }
        }

        protected virtual void OnEventReceived(Message e)
        {
            EventReceived?.Invoke(this, e);
        }
    }
}