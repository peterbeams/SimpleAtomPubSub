namespace SimpleAtomPubSub.Serialization.Xml.Net
{
    /// <summary>
    /// The interface that specifies a custom XML identifier for serialized elements - this overrides any attributes on properties of the class.
    /// </summary>
    public interface IXmlConvertible
    {
        string XmlIdentifier { get; }
    }
}
