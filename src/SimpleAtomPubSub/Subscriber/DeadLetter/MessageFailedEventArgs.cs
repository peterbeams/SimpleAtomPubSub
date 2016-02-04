using System;
using SimpleAtomPubSub.Publisher.Persistance;

namespace SimpleAtomPubSub.Subscriber.DeadLetter
{
    public class MessageFailedEventArgs : EventArgs
    {
        public Message Message { get; set; }
        public Exception Exception { get; set; }
    }
}