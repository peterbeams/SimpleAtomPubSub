using System;
using System.Net;

namespace SimpleAtomPubSub.Environment
{
    internal class EnvironmentImpl : IEnvironment
    {
        public DateTime UtcNow => DateTime.UtcNow;

        public object CreateInstance(Type type)
        {
            return Activator.CreateInstance(type);
        }

        public string DownloadString(string url)
        {
            using (var client = new WebClient())
            {
                return client.DownloadString(url);
            }
        }
    }
}