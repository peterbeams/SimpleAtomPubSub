using System;
using System.Linq;
using System.Xml.Linq;
using SimpleAtomPubSub.Serialization.Xml.Net;

namespace SimpleAtomPubSub.Serialization
{
    public class SimpleXmlMessageSerializaion : IMessageDeserializer, IMessageSerializer
    {
        public Type[] MessageTypes { get; set; }

        public object Deserialize(string body)
        {
            var doc = XDocument.Parse(body);
            var typeName = doc.Root.Name.LocalName;
            var type = MessageTypes.SingleOrDefault(x => x.Name.Equals(typeName));

            if (type == null)
                return new MissingMessage();

            return Xml.Net.XmlConvert.DeserializeObject(type, body);
        }

        public string Serialize(object body)
        {
            return Xml.Net.XmlConvert.SerializeObject(body, XmlConvertOptions.ExcludeTypes);
        }
    }
}
