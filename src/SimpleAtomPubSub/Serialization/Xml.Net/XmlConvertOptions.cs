using System;

namespace SimpleAtomPubSub.Serialization.Xml.Net
{
    /// <summary>
    ///     Indicates how the objects are formatted, serialized or deserialized.
    /// </summary>
    [Flags]
    public enum XmlConvertOptions
    {
        /// <summary>
        ///     The default option.
        /// </summary>
        None = 0,

        /// <summary>
        ///     Exclude the type of the property from serialized XML.
        /// </summary>
        ExcludeTypes = 1 << 0
    }
}