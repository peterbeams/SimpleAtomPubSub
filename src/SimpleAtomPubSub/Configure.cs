using System;
using System.Threading;
using SimpleAtomPubSub.Feed;
using SimpleAtomPubSub.Formatters;
using SimpleAtomPubSub.Persistance;
using SimpleAtomPubSub.Serialization;
using SimpleAtomPubSub.Subscription;

namespace SimpleAtomPubSub
{
    public class Configure
    {
        public static IEventFeed AsAPublisher(string connectionStringName)
        {
            var feed = new EventFeed
            {
                EventPeristance = new SqlPersistance(connectionStringName),
                Serializer = new SimpleXmlMessageSerializaion(),
                Syndication = new AtomFormatter()
            };

            new Thread(() =>
            {
                do
                {
                    Thread.Sleep(new TimeSpan(0, 5, 0));
                    feed.ArhiveWorkingFeed();
                } while (true);
            }).Start();

            return feed;
        }

        private static void ArchiveFeedEvery5Minutes()
        {
            
        }

        public static IEventFeedSubscription AsASubscriber(string endpoint, Type[] eventTypes, Type[] handlerTypes)
        {
            var handlerCollection = new HandlerCollection();
            handlerCollection.AddRange(handlerTypes);

            return new EventFeedSubscription
            {
                PollingInterval = new TimeSpan(0, 1, 0),
                Url = new Uri(endpoint),
                Syndication = new AtomFormatter(),
                Deserializer = new SimpleXmlMessageSerializaion {MessageTypes = eventTypes},
                Handlers = handlerCollection,
                FeedChainFactory = new FeedChainFactory()
            };
        }
    }
}