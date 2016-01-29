using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;

namespace SimpleAtomPubSub.Serialization.Xml.Net.Serializers
{
    internal static class ListSerializer
    {
        /// <summary>
        ///     Serializes a list (e.g. List<T>, Array etc.) into a XElement using options.
        /// </summary>
        /// <param name="value">The list to serialize.</param>
        /// <param name="name">The name of the list to serialize.</param>
        /// <param name="elementNames">The custom name of collection elements.</param>
        /// <param name="options">Indicates how the output is formatted or serialized.</param>
        /// <returns>The XElement representation of the list.</returns>
        public static XElement Serialize(object value, string name, string elementNames, XmlConvertOptions options)
        {
            var parentElement = new XElement(name);

            var list = (ICollection) value;
            foreach (var childValue in list)
            {
                var childElement = ObjectSerializer.Serialize(childValue, elementNames, null, null, null, options);
                Utilities.AddChildElement(childElement, parentElement);
            }

            return parentElement;
        }

        /// <summary>
        ///     Deserializes the XElement to the list (e.g. List<T>, Array of a specified type using options.
        /// </summary>
        /// <param name="type">The type of the list to deserialize.</param>
        /// <param name="parentElement">The parent XElement used to deserialize the list.</param>
        /// <param name="options">Indicates how the output is deserialized.</param>
        /// <returns>The deserialized list from the XElement.</returns>
        public static object Deserialize(Type type, XElement parent, XmlConvertOptions options)
        {
            IList list;
            if (type.GetTypeInfo().IsInterface)
            {
                list = new List<object>();
            }
            else
            {
                list = (IList) Activator.CreateInstance(type);
            }

            var elements = parent.Elements();

            foreach (var element in elements)
            {
                var elementType = Utilities.GetElementType(element, type, 0);

                if (elementType != null)
                {
                    var obj = ObjectSerializer.Deserialize(elementType, element, options);
                    list.Add(obj);
                }
                else
                {
                    throw new InvalidOperationException(
                        "Could not deserialize this non generic dictionary without more type information.");
                }
            }

            return list;
        }
    }
}