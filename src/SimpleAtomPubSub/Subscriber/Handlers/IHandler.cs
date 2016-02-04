namespace SimpleAtomPubSub.Subscriber.Handlers
{
    public interface IHandler<TEvent>
    {
        void Handle(TEvent message);
    }
}