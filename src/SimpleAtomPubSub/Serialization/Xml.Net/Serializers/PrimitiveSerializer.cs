using System;
using System.Xml.Linq;

namespace SimpleAtomPubSub.Serialization.Xml.Net.Serializers
{
    public static class PrimitiveSerializer
    {
        /// <summary>
        /// Serializes a fundamental primitive object (e.g. string, int etc.) into a XElement using options.
        /// </summary>
        /// <param name="value">The primitive to serialize.</param>
        /// <param name="name">The name of the primitive to serialize.</param>
        /// <param name="options">Indicates how the output is formatted or serialized.</param>
        /// <returns>The XElement representation of the primitive.</returns>
        public static XElement Serialize(object value, string name, XmlConvertOptions options)
        {
            var stringValue = Convert.ToString(value);

            var element = new XElement(name, stringValue);
            return element;
        }
        
        /// <summary>
        /// Deserializes the XElement to the fundamental primitive (e.g. string, int etc.) of a specified type using options.
        /// </summary>
        /// <param name="type">The type of the fundamental primitive to deserialize.</param>
        /// <param name="parentElement">The parent XElement used to deserialize the fundamental primitive.</param>
        /// <param name="options">Indicates how the output is deserialized.</param>
        /// <returns>The deserialized fundamental primitive from the XElement.</returns>
        public static object Deserialize(Type type, XElement parentElement, XmlConvertOptions options)
        {
            return Convert.ChangeType(parentElement.Value, type);
        }
    }
}
