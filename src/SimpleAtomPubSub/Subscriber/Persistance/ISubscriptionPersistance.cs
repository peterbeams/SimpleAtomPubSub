using System;

namespace SimpleAtomPubSub.Subscriber.Persistance
{
    public interface ISubscriptionPersistance
    {
        int Register(string url);
        void SetLastObservedEvent(int id, Guid eventId);
        Guid? GetLastObservedEventId(int id);
    }
}