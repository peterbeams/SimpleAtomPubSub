using System;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Xml.Linq;
using SimpleAtomPubSub.Persistance;

namespace SimpleAtomPubSub.Formatters
{
    public class AtomFormatter : ISyndication
    {
        private const string ContentType = "application/atom+xml";
        private const string NextInArchiveRelationshipType = "next-archive";
        private const string PrevInArchiveRelationshipType = "prev-archive";

        public string Build(FeedData dataFeed, Uri baseUri)
        {
            var feed = new SyndicationFeed()
            {
                Id = dataFeed.Id.ToString(),
                LastUpdatedTime = dataFeed.DateCreated,
                Items = dataFeed.Messages.Select(x => new SyndicationItem()
                {
                    Content = new RawSyndicationContent(x.Body),
                    Id = x.Id.ToString(),
                    LastUpdatedTime = x.CreatedAt
                })
            };

            if (!string.IsNullOrEmpty(dataFeed.NextUri))
                feed.Links.Add(new SyndicationLink(new Uri(baseUri, dataFeed.NextUri), NextInArchiveRelationshipType, "Next In Archive", ContentType, 0));

            if (!string.IsNullOrEmpty(dataFeed.PreviousUri))
                feed.Links.Add(new SyndicationLink(new Uri(baseUri, dataFeed.PreviousUri), PrevInArchiveRelationshipType, "Previous In Archive", ContentType, 0));

            var formatter = new Atom10FeedFormatter(feed);

            var sw = new StringWriter();
            using (var writer = XmlWriter.Create(sw))
            {
                formatter.WriteTo(writer);
            }

            return sw.ToString();
        }

        public FeedData Build(string data)
        {
            var formatter = new Atom10FeedFormatter();
            using (var reader = new StringReader(data))
            {
                using (var xmlReader = XmlReader.Create(reader))
                {
                    formatter.ReadFrom(xmlReader);
                    var feed = formatter.Feed;
                    return new FeedData
                    {
                        Id = Guid.Parse(feed.Id),
                        DateCreated = feed.LastUpdatedTime.UtcDateTime,
                        PreviousUri = GetUriFromLink(PrevInArchiveRelationshipType, feed),
                        NextUri = GetUriFromLink(NextInArchiveRelationshipType, feed),
                        Messages = feed.Items.Select(x =>
                        {
                            return new Message
                            {
                                Body = GetXmlString(x),
                                CreatedAt = x.LastUpdatedTime.UtcDateTime,
                                Id = Guid.Parse(x.Id)
                            };
                        })
                    };
                }
            }
        }

        private static string GetXmlString(SyndicationItem x)
        {
            var element = ((XmlSyndicationContent) x.Content).ReadContent<XElement>().ToString();
            
            //strip out the annoying xml ns added by atom
            element = element.Replace(@" xmlns=""http://www.w3.org/2005/Atom""", string.Empty);

            return element;
        }

        private string GetUriFromLink(string relationshipType, SyndicationFeed feed)
        {
            var link = feed.Links.SingleOrDefault(x => x.RelationshipType.Equals(relationshipType));
            return link != null ? link.Uri.ToString() : null;
        }
    }
}
