using System;
using System.Xml.Linq;
using SimpleAtomPubSub.Serialization.Xml.Net.Serializers;

namespace SimpleAtomPubSub.Serialization.Xml.Net
{
    /// <summary>
    /// The class that serializes and deserializes .NET objects.
    /// </summary>
    public static class XmlConvert
    {
        /// <summary>
        /// Provides the default options for formatting, serializing and deserializing objects.
        /// </summary>
        private const XmlConvertOptions DefaultConvertOptions = XmlConvertOptions.None;
        /// <summary>
        /// Serializes the specified object to a XML string. 
        /// Note: properties without both a getter AND setter  will be ignored. Add a private setter if you wish to include the property in the XML output.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <returns>The XML string representation of the object.</returns>
        public static string SerializeObject(object value)
        {
            return SerializeObject(value, DefaultConvertOptions);
        }

        /// <summary>
        /// Serializes the specified object to a XML string using options.
        /// Note: properties without both a getter AND setter  will be ignored. Add a private setter if you wish to include the property in the XML output.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="options">Indicates how the output is formatted or serialized.</param>
        /// <returns>The XML string representation of the object.</returns>
        public static string SerializeObject(object value, XmlConvertOptions options)
        {
            return SerializeXElement(value, options).ToString();
        }

        /// <summary>
        /// Serializes the specified object to a XElement.
        /// Note: properties without both a getter AND setter  will be ignored. Add a private setter if you wish to include the property in the XML output.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <returns>The XElement representation of the object.</returns>
        public static XElement SerializeXElement(object value)
        {
            return SerializeXElement(value, DefaultConvertOptions);
        }

        /// <summary>
        /// Serializes the specified object to a XElement using options.
        /// Note: properties without both a getter AND setter  will be ignored. Add a private setter if you wish to include the property in the XML output.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="options">Indicates how the output is formatted or serialized.</param>
        /// <returns>The XElement representation of the object.</returns>
        public static XElement SerializeXElement(object value, XmlConvertOptions options)
        {
            if (value == null) { throw new ArgumentNullException(nameof(value)); }

            var identifier = Utilities.GetIdentifier(value);
            return ObjectSerializer.Serialize(value, identifier, options);
        }

        /// <summary>
        /// Deserializes the XML string to the specified .NET type.
        /// </summary>
        /// <typeparam name="T">The type of the deserialized .NET object.</typeparam>
        /// <param name="xml">The XML string to deserialize.</param>
        /// <returns>The deserialized object from the XML string.</returns>
        public static T DeserializeObject<T>(string xml) where T : new()
        {
            return DeserializeObject<T>(xml, DefaultConvertOptions);
        }

        /// <summary>
        /// Deserializes the XML string to the specified .NET type using options.
        /// </summary>
        /// <typeparam name="T">The type of the deserialized .NET object.</typeparam>
        /// <param name="xml">The XML string to deserialize.</param>
        /// <param name="options">Indicates how the output is deserialized.</param>
        /// <returns>The deserialized object from the XML string.</returns>
        public static T DeserializeObject<T>(string xml, XmlConvertOptions options) where T : new()
        {
            return (T)DeserializeObject(typeof(T), xml, options);
        }

        /// <summary>
        /// Deserializes the XML string to the specified .NET type.
        /// </summary>
        /// <param name="type">The type of the deserialized .NET object.</param>
        /// <param name="xml">The XML string to deserialize.</param>
        /// <returns>The deserialized object from the XML string.</returns>
        public static object DeserializeObject(Type type, string xml)
        {
            return DeserializeObject(type, xml, DefaultConvertOptions);
        }

        /// <summary>
        /// Deserializes the XML string to the specified .NET type using options.
        /// </summary>
        /// <param name="type">The type of the deserialized .NET object.</param>
        /// <param name="xml">The XML string to deserialize.</param>
        /// <param name="options">Indicates how the output is deserialized.</param>
        /// <returns>The deserialized object from the XML string.</returns>
        public static object DeserializeObject(Type type, string xml, XmlConvertOptions options)
        {
            if (xml == null) { throw new ArgumentNullException(nameof(xml)); }

            return DeserializeXElement(type, XElement.Parse(xml), options);
        }

        /// <summary>
        /// Deserializes the XElement to the specified .NET type.
        /// </summary>
        /// <typeparam name="T">The type of the deserialized .NET object.</typeparam>
        /// <param name="element">The XElement to deserialize.</param>
        /// <returns>The deserialized object from the XElement.</returns>
        public static T DeserializeXElement<T>(XElement element) where T : new()
        {
            return DeserializeXElement<T>(element, DefaultConvertOptions);
        }

        /// <summary>
        /// Deserializes the XElement to the specified .NET type using options.
        /// </summary>
        /// <typeparam name="T">The type of the deserialized .NET object.</typeparam>
        /// <param name="element">The XElement to deserialize.</param>
        /// <param name="options">Indicates how the output is deserialized.</param>
        /// <returns>The deserialized object from the XElement.</returns>
        public static T DeserializeXElement<T>(XElement element, XmlConvertOptions options)
        {
            return (T)DeserializeXElement(typeof(T), element, options);
        }

        /// <summary>
        /// Deserializes the XElement to the specified .NET type.
        /// </summary>
        /// <param name="type">The type of the deserialized .NET object.</param>
        /// <param name="element">The XElement to deserialize.</param>
        /// <returns>The deserialized object from the XElement.</returns>
        public static object DeserializeXElement(Type type, XElement element)
        {
            return DeserializeXElement(type, element, DefaultConvertOptions);
        }

        /// <summary>
        /// Deserializes the XElement to the specified .NET type using options.
        /// </summary>
        /// <param name="type">The type of the deserialized .NET object.</param>
        /// <param name="element">The XElement to deserialize.</param>
        /// <param name="options">Indicates how the output is deserialized.</param>
        /// <returns>The deserialized object from the XElement.</returns>
        public static object DeserializeXElement(Type type, XElement element, XmlConvertOptions options)
        {
            if (type == null) { throw new ArgumentNullException(nameof(type)); }
            if (element == null) { throw new ArgumentNullException(nameof(element)); }

            return ObjectSerializer.DeserializeObject(type, element, options);
        }
    }
}
