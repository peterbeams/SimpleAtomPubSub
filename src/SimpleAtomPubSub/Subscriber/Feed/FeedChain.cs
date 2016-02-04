using System;
using System.Collections;
using System.Collections.Generic;
using SimpleAtomPubSub.Formatters;
using SimpleAtomPubSub.Publisher.Persistance;

namespace SimpleAtomPubSub.Subscriber.Feed
{
    public class FeedChain : IEnumerable<FeedData>
    {
        private readonly string _startUrl;
        private readonly ISyndicationFormatter _syndicationFormatter;

        public FeedChain(string startUrl, ISyndicationFormatter syndicationFormatter)
        {
            _startUrl = startUrl;
            _syndicationFormatter = syndicationFormatter;
        }

        public IEnumerator<FeedData> GetEnumerator()
        {
            return new FeedEnumerator(_startUrl, _syndicationFormatter);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new FeedEnumerator(_startUrl, _syndicationFormatter);
        }

        public class FeedEnumerator : IEnumerator<FeedData>
        {
            private readonly ISyndicationFormatter _syndicationFormatter;
            private string _nextUrlToRead;

            public FeedEnumerator(string startUrl, ISyndicationFormatter syndicationFormatter)
            {
                _nextUrlToRead = startUrl;
                _syndicationFormatter = syndicationFormatter;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (string.IsNullOrEmpty(_nextUrlToRead))
                    return false;

                var feedData = Environment.Environment.Current.DownloadString(_nextUrlToRead);
                Current = _syndicationFormatter.Build(feedData, _nextUrlToRead);
                _nextUrlToRead = Current.PreviousUri;

                return true;
            }

            public void Reset()
            {
                throw new NotSupportedException();
            }

            public FeedData Current { get; private set; }

            object IEnumerator.Current => Current;
        }
    }
}