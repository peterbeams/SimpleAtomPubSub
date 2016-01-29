using System;

namespace SimpleAtomPubSub.Serialization.Xml.Net
{
    /// <summary>
    /// The attribute that provides a custom name of elements in a list or dictionary when serialized into XML.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class XmlConvertElementsNameAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlConvertElementsNameAttribute"/> class with a name.
        /// </summary>
        /// <param name="name">The custom name of the elements in a list or dictionary when serialized into XML.</param>
        public XmlConvertElementsNameAttribute(string name)
        {
            if (name == null) { throw new ArgumentNullException(nameof(name)); }
            if (string.IsNullOrWhiteSpace(name)) { throw new ArgumentException("The collection element name cannot be empty", nameof(name)); }

            Name = name;
        }

        /// <summary>
        /// The custom name of elements in a list or dictionary when serialized into XML.
        /// </summary>
        public string Name { get; }
    }
}
