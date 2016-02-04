using System;
using System.Collections.Generic;

namespace SimpleAtomPubSub.Publisher.Persistance
{
    public class FeedData
    {
        public Guid Id { get; set; }
        public DateTime DateCreated { get; set; }
        public string PreviousUri { get; set; }
        public string NextUri { get; set; }
        public IList<Message> Messages { get; set; }
    }
}