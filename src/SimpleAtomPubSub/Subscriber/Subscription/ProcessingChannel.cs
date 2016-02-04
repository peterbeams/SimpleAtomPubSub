using System;
using SimpleAtomPubSub.Publisher.Persistance;
using SimpleAtomPubSub.Serialization;
using SimpleAtomPubSub.Subscriber.DeadLetter;
using SimpleAtomPubSub.Subscriber.Handlers;

namespace SimpleAtomPubSub.Subscriber.Subscription
{
    public class ProcessingChannel
    {
        public event EventHandler<MessageFailedEventArgs> MessageFailed;
        public event EventHandler<Message> MessageProcessed;
        
        private readonly IMessageDeserializer _deserializer;
        private readonly IHandler<object> _handlers;

        public ProcessingChannel(IMessageDeserializer deserializer, IHandler<object> handlers)
        {
            _deserializer = deserializer;
            _handlers = handlers;
        }
        
        public void ProcessEvent(object sender, Message message)
        {
            try
            {
                var mbody = _deserializer.Deserialize(message.Body);
                _handlers.Handle(mbody);
                MessageProcessed?.Invoke(this, message);
            }
            catch (Exception ex)
            {
                MessageFailed?.Invoke(this, new MessageFailedEventArgs { Message = message, Exception = ex });
            }
        }
    }
}