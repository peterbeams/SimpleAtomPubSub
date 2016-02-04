using System;
using System.Threading;
using System.Threading.Tasks;
using SimpleAtomPubSub.Formatters;
using SimpleAtomPubSub.Publisher.Feed;
using SimpleAtomPubSub.Publisher.Persistance;
using SimpleAtomPubSub.Serialization;
using SimpleAtomPubSub.Subscriber.DeadLetter;
using SimpleAtomPubSub.Subscriber.Feed;
using SimpleAtomPubSub.Subscriber.Handlers;
using SimpleAtomPubSub.Subscriber.Subscription;

namespace SimpleAtomPubSub
{
    public class Configure
    {
        internal static IEventFeed AsADirectFeedReader(string connectionStringName)
        {
            var feed = new EventFeed
            {
                EventPeristance = new SqlPersistance(connectionStringName),
                Serializer = new SimpleXmlMessageSerializaion(),
                SyndicationFormatter = new AtomFormatter()
            };

            feed.EnsureWorkingFeedExists();

            return feed;
        }

        public static IEventFeed AsAPublisher(string connectionStringName)
        {
            var feed = AsADirectFeedReader(connectionStringName);

            Task.Factory.StartNew(() =>
            {
                do
                {
                    Thread.Sleep(new TimeSpan(0, 5, 0));
                    Task.Factory.StartNew(() => feed.ArhiveWorkingFeed()).Wait();
                } while (true);
            });
            
            return feed;
        }

        public static IEventFeedSubscription AsASubscriber(string endpoint, string connectionStringName, Type[] eventTypes, Type[] handlerTypes)
        {
            var handlerCollection = new HandlerCollection();
            handlerCollection.AddRange(handlerTypes);

            var deserializer = new SimpleXmlMessageSerializaion { MessageTypes = eventTypes };

            var deadLetterPersistance = new Subscriber.Persistance.SqlPersistance(connectionStringName);

            var failureChannel = new FailureChannel(deadLetterPersistance)
            {
                PollingInterval = new TimeSpan(0, 1, 0)
            };

            var processingChannel = new ProcessingChannel(deserializer, handlerCollection);

            var subscription = new EventFeedObserver(new Uri(endpoint), new AtomFormatter(), new FeedChainFactory())
            {
                PollingInterval = new TimeSpan(0, 1, 0)
            };

            //deadletter messages when they fail
            processingChannel.MessageFailed += (sender, args) => failureChannel.DeadLetter(args.Message, args.Exception);

            //process messages in handlers when they're picked up from the feed
            subscription.EventReceived += processingChannel.ProcessEvent;

            //process failed messages again when they're reader
            failureChannel.MessageReadyForRetry += processingChannel.ProcessEvent;

            return subscription;
        }
    }
}