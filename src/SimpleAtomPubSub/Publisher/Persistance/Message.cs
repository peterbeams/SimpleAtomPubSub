﻿using System;

namespace SimpleAtomPubSub.Publisher.Persistance
{
    public class Message
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Body { get; set; }
        public string FeedUri { get; set; }
    }
}