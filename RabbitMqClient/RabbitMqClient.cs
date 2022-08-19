using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMqPerformanceTest.MessageBus;
using RabbitMqPerformanceTest.MessageBus.Messages;

namespace RabbitMqPerformanceTest.RabbitMqClient
{
    /// <summary>
    /// A RabbitMQ client that can be used both for publishing and receiving messages
    /// represented as C# classes.
    /// </summary>
    public class RabbitMqClient : IMessageBus
    {
        private const string TopicsExchangeName = "RabbitTopics";
        private const string DirectExchangeName = "RabbitDirect";
        private IConnection? _sendConnection;
        private IConnection? _receiveConnection;
        private IModel? _receiveChannel;
        private IModel? _sendChannel;
        private EventingBasicConsumer? _consumer;
        private string? _queueName;
        private string? _replyQueueName;

        public event EventHandler<MessageEventsArgs>? MessageReceived;

        public void Connect(string connectionString, string queueName)
        {
            var factory = new ConnectionFactory()
            {
                Uri = new Uri(connectionString)
            };

            _sendConnection = factory.CreateConnection();
            _receiveConnection = factory.CreateConnection();

            _sendChannel = _sendConnection.CreateModel();
            _receiveChannel = _receiveConnection.CreateModel();

            _queueName = queueName;
            _replyQueueName = _receiveChannel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null).QueueName;

            // The tpics exchange is used for sending C# message classes
            _receiveChannel.ExchangeDeclare(exchange: TopicsExchangeName, type: ExchangeType.Topic);

            // The direct exchange is used for replying directly back to sender.
            _receiveChannel.ExchangeDeclare(exchange: DirectExchangeName, type: ExchangeType.Direct);

            // Always bind the input queue to the direct exchange
            _receiveChannel?.QueueBind(queue: _queueName, exchange: DirectExchangeName, routingKey: _queueName);

            _consumer = new EventingBasicConsumer(_receiveChannel);
            _consumer.Received += OnMessageReceived;
        }

        public void Subscribe<T>()
        {
            string routingKey = typeof(T).ToString();

            Console.WriteLine($"Binding queue '{_queueName}' to topics exchange '{TopicsExchangeName}' with routing key '{routingKey}'");

            _receiveChannel?.QueueBind(queue: _queueName, exchange: TopicsExchangeName, routingKey: routingKey);
        }

        public void Start()
        {
            // Start consumer
            _receiveChannel?.BasicConsume(queue: _queueName, autoAck: true, consumer: _consumer);
        }

        public void PublishMessage(MessageBase message)
        {
            if (_sendChannel is null)
                return;

            var json = JsonSerializer.Serialize(message, message.GetType());
            var body = Encoding.UTF8.GetBytes(json);
            string routingKey = message.GetType().ToString();

            IBasicProperties props = _sendChannel.CreateBasicProperties();
            props.CorrelationId = Guid.NewGuid().ToString();
            props.ReplyTo = _replyQueueName;
            props.Type = routingKey;

            _sendChannel.BasicPublish(exchange: TopicsExchangeName, routingKey: routingKey, basicProperties: props, body: body);
        }

        public void Reply(MessageBase responseMessage, string queueName)
        {
            // Send replies via the direct exchange
            if (_sendChannel is null)
                return;

            var json = JsonSerializer.Serialize(responseMessage, responseMessage.GetType());
            var body = Encoding.UTF8.GetBytes(json);
            IBasicProperties props = _sendChannel.CreateBasicProperties();
            props.Type = responseMessage.GetType().ToString();

            _sendChannel.BasicPublish(exchange: DirectExchangeName, routingKey: queueName, basicProperties: props, body: body);
        }

        public void Dispose()
        {
            _sendConnection?.Dispose();
            _receiveConnection?.Dispose();
        }


        private void OnMessageReceived(object? sender, BasicDeliverEventArgs e)
        {
            byte[] body = e.Body.ToArray();
            string json = Encoding.UTF8.GetString(body);
            MessageBase? message = null;

            Console.WriteLine($"Received message. Exchange: '{e.Exchange}', RoutingKey: '{e.RoutingKey}', Type: '{e.BasicProperties.Type}', ReplyTo: '{e.BasicProperties.ReplyTo}'");

            switch (e.BasicProperties.Type)
            {
                case "RabbitMqPerformanceTest.MessageBus.Messages.RequestMessage1": 
                    message = JsonSerializer.Deserialize<RequestMessage1>(json); 
                    break;
                case "RabbitMqPerformanceTest.MessageBus.Messages.RequestMessage2":
                    message = JsonSerializer.Deserialize<RequestMessage2>(json);
                    break;
                case "RabbitMqPerformanceTest.MessageBus.Messages.RequestMessage3":
                    message = JsonSerializer.Deserialize<RequestMessage3>(json);
                    break;
                case "RabbitMqPerformanceTest.MessageBus.Messages.RequestMessage4":
                    message = JsonSerializer.Deserialize<RequestMessage4>(json);
                    break;
                case "RabbitMqPerformanceTest.MessageBus.Messages.RequestMessage5":
                    message = JsonSerializer.Deserialize<RequestMessage5>(json);
                    break;
                case "RabbitMqPerformanceTest.MessageBus.Messages.RequestMessage6":
                    message = JsonSerializer.Deserialize<RequestMessage6>(json);
                    break;
                case "RabbitMqPerformanceTest.MessageBus.Messages.RequestMessage7":
                    message = JsonSerializer.Deserialize<RequestMessage7>(json);
                    break;
                case "RabbitMqPerformanceTest.MessageBus.Messages.RequestMessage8":
                    message = JsonSerializer.Deserialize<RequestMessage8>(json);
                    break;
                case "RabbitMqPerformanceTest.MessageBus.Messages.RequestMessage9":
                    message = JsonSerializer.Deserialize<RequestMessage9>(json);
                    break;
                case "RabbitMqPerformanceTest.MessageBus.Messages.RequestMessage10":
                    message = JsonSerializer.Deserialize<RequestMessage10>(json);
                    break;

                case "RabbitMqPerformanceTest.MessageBus.Messages.ResponseMessage1":
                    message = JsonSerializer.Deserialize<ResponseMessage1>(json); 
                    break;
                case "RabbitMqPerformanceTest.MessageBus.Messages.ResponseMessage2":
                    message = JsonSerializer.Deserialize<ResponseMessage2>(json);
                    break;
                case "RabbitMqPerformanceTest.MessageBus.Messages.ResponseMessage3":
                    message = JsonSerializer.Deserialize<ResponseMessage3>(json);
                    break;
                case "RabbitMqPerformanceTest.MessageBus.Messages.ResponseMessage4":
                    message = JsonSerializer.Deserialize<ResponseMessage4>(json);
                    break;
                case "RabbitMqPerformanceTest.MessageBus.Messages.ResponseMessage5":
                    message = JsonSerializer.Deserialize<ResponseMessage5>(json);
                    break;
                case "RabbitMqPerformanceTest.MessageBus.Messages.ResponseMessage6":
                    message = JsonSerializer.Deserialize<ResponseMessage6>(json);
                    break;
                case "RabbitMqPerformanceTest.MessageBus.Messages.ResponseMessage7":
                    message = JsonSerializer.Deserialize<ResponseMessage7>(json);
                    break;
                case "RabbitMqPerformanceTest.MessageBus.Messages.ResponseMessage8":
                    message = JsonSerializer.Deserialize<ResponseMessage8>(json);
                    break;
                case "RabbitMqPerformanceTest.MessageBus.Messages.ResponseMessage9":
                    message = JsonSerializer.Deserialize<ResponseMessage9>(json);
                    break;
                case "RabbitMqPerformanceTest.MessageBus.Messages.ResponseMessage10":
                    message = JsonSerializer.Deserialize<ResponseMessage10>(json);
                    break;

                default:
                    Console.WriteLine($"Unknown message type: {e.RoutingKey}");
                    break;
            }

            if (message != null)
            {
                MessageReceived?.Invoke(this, new MessageEventsArgs(e.BasicProperties.Type, e.BasicProperties.ReplyTo, message));
            }
        }
    }
}
