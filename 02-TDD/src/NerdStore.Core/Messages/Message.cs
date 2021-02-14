using System;

namespace NerdStore.Core.Messages
{
    public abstract class Message
    {
        public string MessageType { get; private set; }
        public Guid AggregateId { get; set; }

        public Message()
        {
            MessageType = GetType().Name;
        }
    }
}