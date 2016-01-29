using System;
using System.Collections;
using System.Collections.Generic;
using SimpleAtomPubSub.Formatters;
using SimpleAtomPubSub.Persistance;

namespace SimpleAtomPubSub.Subscription
{
    public interface IFeedChainFactory
    {
        IEnumerable<FeedData> Get(string startUrl, ISyndication syndication);
    }

    public class FeedChainFactory : IFeedChainFactory
    {
        public IEnumerable<FeedData> Get(string startUrl, ISyndication syndication)
        {
            return new FeedChain(startUrl, syndication);
        }
    }

    public class FeedChain : IEnumerable<FeedData>
    {
        private readonly string _startUrl;
        private readonly ISyndication _syndication;

        public FeedChain(string startUrl, ISyndication syndication)
        {
            _startUrl = startUrl;
            _syndication = syndication;
        }

        public IEnumerator<FeedData> GetEnumerator()
        {
            return new FeedEnumerator(_startUrl, _syndication);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new FeedEnumerator(_startUrl, _syndication);
        }

        public class FeedEnumerator : IEnumerator<FeedData>
        {
            private string _nextUrlToRead;
            private readonly ISyndication _syndication;

            public FeedEnumerator(string startUrl, ISyndication syndication)
            {
                _nextUrlToRead = startUrl;
                _syndication = syndication;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (string.IsNullOrEmpty(_nextUrlToRead))
                    return false;

                var feedData = Environment.Environment.Current.DownloadString(_nextUrlToRead);
                Current = _syndication.Build(feedData);
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