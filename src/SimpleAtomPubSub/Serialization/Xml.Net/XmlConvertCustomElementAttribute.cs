using System;

namespace SimpleAtomPubSub.Serialization.Xml.Net
{
    /// <summary>
    ///     The attribute that provides a custom name elements in a list or dictionary when serialized into XML.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct)]
    public class XmlConvertCustomElementAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="XmlConvertCustomElementAttribute" /> class with a name.
        /// </summary>
        /// <param name="name">The custom name of the object when serialized into XML.</param>
        public XmlConvertCustomElementAttribute(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("The xml object name cannot be empty", nameof(name));
            }

            Name = name;
        }

        /// <summary>
        ///     The custom name of the object when serialized into XML.
        /// </summary>
        public string Name { get; }
    }
}