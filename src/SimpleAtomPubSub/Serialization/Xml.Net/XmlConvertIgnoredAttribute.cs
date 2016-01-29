using System;

namespace SimpleAtomPubSub.Serialization.Xml.Net
{
    /// <summary>
    /// The attribute that specifies that the property it is applied to should not be serialized into XML.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class XmlConvertIgnoredAttribute : Attribute
    {
    }
}
