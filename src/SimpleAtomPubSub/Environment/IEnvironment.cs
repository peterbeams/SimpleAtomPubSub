using System;

namespace SimpleAtomPubSub.Environment
{
    public interface IEnvironment
    {
        DateTime UtcNow { get; }
        object CreateInstance(Type type);
        string DownloadString(string url);
    }
}