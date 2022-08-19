using RabbitMqPerformanceTest.MessageBus.Messages;

namespace RabbitMqPerformanceTest.MessageBus
{
    public class MessageEventsArgs : EventArgs
    {
        public string MessageType { get; }

        public string ReplyQueue { get; }

        public MessageBase Message { get; }

        public MessageEventsArgs(string messageType, string replyQueue, MessageBase message)
        {
            MessageType = messageType;
            ReplyQueue = replyQueue;
            Message = message;
        }
    }
}
