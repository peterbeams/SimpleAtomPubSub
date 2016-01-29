namespace SimpleAtomPubSub.Serialization
{
    public interface IMessageSerializer
    {
        string Serialize(object body);
    }
}