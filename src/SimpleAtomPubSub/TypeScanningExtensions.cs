using System;
using System.Linq;
using System.Reflection;
using SimpleAtomPubSub.Handler;

namespace SimpleAtomPubSub
{
    public static class TypeScanningExtensions
    {
        public static Type[] ScanForHandlers(this Assembly assembly)
        {
            return assembly.GetTypes()
                    .Where(t => t.GetInterfaces().Any( x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IHandler<>) ))
                    .ToArray();
        }
    }
}