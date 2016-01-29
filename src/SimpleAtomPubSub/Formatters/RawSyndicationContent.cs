using System;
using System.ServiceModel.Syndication;
using System.Xml;

namespace SimpleAtomPubSub.Formatters
{
    public class RawSyndicationContent : SyndicationContent
    {
        private readonly string _xml;

        public RawSyndicationContent(string xml)
        {
            _xml = xml;
        }

        public override string Type
        {
            get { return "RawSyndicationContent"; }
        }

        protected override void WriteContentsTo(XmlWriter writer)
        {
            writer.WriteRaw(_xml);
        }

        public override SyndicationContent Clone()
        {
            throw new NotSupportedException();
        }
    }
}