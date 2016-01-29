namespace SimpleAtomPubSub.Handler
{
    public interface IHandler<TEvent>
    {
        void Handle(TEvent message);
    }
}
