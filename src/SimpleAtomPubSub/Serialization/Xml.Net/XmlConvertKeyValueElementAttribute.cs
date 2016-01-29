using System;

namespace SimpleAtomPubSub.Serialization.Xml.Net
{
    /// <summary>
    /// The attribute that provides a custom key name and value name for elements in a dictionary when serialized into XML.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class XmlConvertKeyValueElementAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlConvertKeyValueElementAttribute"/> class with a key name and a value name.
        /// </summary>
        /// <param name="keyName">The custom name of the key of a dictionary element when serialized into XML.</param>
        /// <param name="valueName">The custom name of the value of a dictionary element when serialized into XML.</param>
        public XmlConvertKeyValueElementAttribute(string keyName, string valueName)
        {
            if (keyName == null) { throw new ArgumentNullException(nameof(keyName)); }
            if (string.IsNullOrWhiteSpace(keyName)) { throw new ArgumentException("The dictionary key element name cannot be empty", nameof(keyName)); }

            if (valueName == null) { throw new ArgumentNullException(nameof(valueName)); }
            if (string.IsNullOrWhiteSpace(valueName)) { throw new ArgumentException("The dictionary value element name cannot be empty", nameof(valueName)); }

            KeyName = keyName;
            ValueName = valueName;
        }

        /// <summary>
        /// The custom name of the key of a dictionary element when serialized into XML.
        /// </summary>
        public string KeyName { get; }

        /// <summary>
        /// The custom name of the value of a dictionary element when serialized into XML.
        /// </summary>
        public string ValueName { get; }
    }
}
