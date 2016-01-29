namespace SimpleAtomPubSub.Serialization
{
    public interface IMessageDeserializer
    {
        object Deserialize(string body);
    }
}