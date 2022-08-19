using RabbitMqPerformanceTest.MessageBus.Messages;

namespace RabbitMqPerformanceTest.MessageBus
{
    public interface IMessageBus : IDisposable
    {
        event EventHandler<MessageEventsArgs>? MessageReceived;

        void Subscribe<T>();

        void Connect(string connectionString, string queueName);

        void Start();

        void PublishMessage(MessageBase message);

        void Reply(MessageBase responseMessage, string queueName);
    }
}