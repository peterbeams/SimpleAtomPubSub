using System;
using System.Collections.Generic;

namespace SimpleAtomPubSub.Persistance
{
    public class FeedData
    {
        public Guid Id { get; set; }
        public DateTime DateCreated { get; set; }
        public string PreviousUri { get; set; }
        public string NextUri { get; set; }
        public IEnumerable<Message> Messages { get; set; }
    }
}
