using System;
using System.Collections.Generic;
using System.Linq;
using SimpleAtomPubSub.Handler;

namespace SimpleAtomPubSub.Subscription
{
    public class HandlerCollection : List<Type>, IHandler<object>
    {
        public void Handle(object message)
        {
            var handlerInterface = typeof(IHandler<>).MakeGenericType(message.GetType());

            var matchingHandlers = this.Where(
                t => t.GetInterfaces().Any(x => x == handlerInterface)
                );

            foreach (var h in matchingHandlers)
            {
                var handler = Environment.Environment.Current.CreateInstance(h);
                handlerInterface.GetMethod("Handle").Invoke(handler, new[] { message });
            }
        }
    }
}