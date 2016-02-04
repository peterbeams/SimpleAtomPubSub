using System;
using System.Collections.Generic;
using SimpleAtomPubSub.Publisher.Persistance;

namespace SimpleAtomPubSub.Subscriber.Persistance
{
    public interface IDeadLetterPersistance
    {
        void Deadletter(Message message, Exception ex);
        IEnumerable<Message> PullRetries();
    }
}