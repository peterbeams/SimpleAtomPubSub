using System;
using System.Threading;
using System.Threading.Tasks;
using SimpleAtomPubSub.Publisher.Persistance;
using SimpleAtomPubSub.Subscriber.Persistance;

namespace SimpleAtomPubSub.Subscriber.DeadLetter
{
    public class FailureChannel
    {
        private readonly IDeadLetterPersistance _persistance;
        public event EventHandler<Message> MessageReadyForRetry;

        public TimeSpan PollingInterval { get; set; } = new TimeSpan(0, 1, 0);

        public FailureChannel(IDeadLetterPersistance persistance)
        {
            _persistance = persistance;
        }

        public void DeadLetter(Message message, Exception exception)
        {
            _persistance.Deadletter(message, exception);
        }

        internal async void Poll()
        {
            do
            {
                var x = await HandleFailuresReadyToReProcessAsync();
                Thread.Sleep(PollingInterval);
            } while (true);
        }

        internal Task<bool> HandleFailuresReadyToReProcessAsync()
        {
            return Task.Factory.StartNew(PickupRetries);
        }

        private bool PickupRetries()
        {
            var retries = _persistance.PullRetries();

            foreach (var m in retries)
            {
                OnMessageReadyForRetry(m);
            }

            return true;
        }
        
        protected virtual void OnMessageReadyForRetry(Message e)
        {
            MessageReadyForRetry?.Invoke(this, e);
        }
    }
}