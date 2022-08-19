using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Configuration;
using RabbitMqPerformanceTest.MessageBus;
using RabbitMqPerformanceTest.MessageBus.Messages;

namespace RabbitMqPerformanceTest.Publisher
{
    class Program
    {
        static string? _inputQueueName;
        static IMessageBus? _messageBus;
        static Random? _randomgenerator;
        static int _instance;
        static int _longestRoundtripDelayMs;
        static int _shortestRoundtripDelayMs = int.MaxValue;
        static int _longestSendDelayMs;
        static int _shortestSendDelayMs = int.MaxValue;
        static int _longestPublishTimeMs;
        static int _totalResponsesReceived;
        static int _totalPublishTimeMs;
        static string? _longestRoundtripDelayMessage;
        static int _minPublishIntervalInMs;
        static int _maxPublishIntervalInMs;
        static Dictionary<int, int>? _requestsSent;
        static Dictionary<int, int>? _responsesReceived;

        static void Main(string[] args)
        {
            _randomgenerator = new Random();

            _requestsSent = new Dictionary<int, int>();
            _responsesReceived = new Dictionary<int, int>();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);

            IConfigurationRoot configuration = builder
                .AddEnvironmentVariables()
                .Build();

            _instance = int.Parse(configuration["Instance"]);
            int messagesToSend = int.Parse(configuration["MessagesToSend"]);

            _minPublishIntervalInMs = int.Parse(configuration["MinPublishIntervalInMs"]);
            _maxPublishIntervalInMs = int.Parse(configuration["MaxPublishIntervalInMs"]);

            _inputQueueName = $"Publisher{_instance}";
            Console.WriteLine($"Publisher {_instance} started");

            Console.WriteLine("Waiting for RabbitMQ to start");
            Thread.Sleep(20000);

            string rabbitMqConnectionString = configuration.GetConnectionString("RabbitMq");
            Console.WriteLine($"Connecting to RabbitMQ at {rabbitMqConnectionString}");

            _messageBus = new RabbitMqClient.RabbitMqClient();

            _messageBus.MessageReceived += OnMessageReceived;

            _messageBus.Connect(rabbitMqConnectionString, _inputQueueName);

            switch (_instance)
            {
                case 1: _messageBus.Subscribe<ResponseMessage1>(); break;
                case 2: _messageBus.Subscribe<ResponseMessage2>(); break;
                case 3: _messageBus.Subscribe<ResponseMessage3>(); break;
                case 4: _messageBus.Subscribe<ResponseMessage4>(); break;
                case 5: _messageBus.Subscribe<ResponseMessage5>(); break;
                case 6: _messageBus.Subscribe<ResponseMessage6>(); break;
                case 7: _messageBus.Subscribe<ResponseMessage7>(); break;
                case 8: _messageBus.Subscribe<ResponseMessage8>(); break;
                case 9: _messageBus.Subscribe<ResponseMessage9>(); break;
                case 10: _messageBus.Subscribe<ResponseMessage10>(); break;
            }

            _messageBus.Start();

            Console.WriteLine($"Sending {messagesToSend} request messages to random subscribers");

            for (int i=1; i <= messagesToSend; i++)
            {
                SendRequestMessage(i);

                if (_minPublishIntervalInMs > 0 && _maxPublishIntervalInMs > _minPublishIntervalInMs)
                {
                    // Wait a random delay before sending next message
                    Thread.Sleep(_randomgenerator.Next(_minPublishIntervalInMs, _maxPublishIntervalInMs));
                }
            }

            // Wait for all responses to arrive
            Thread.Sleep(5000);

            // Write stats
            Console.WriteLine();
            Console.WriteLine("------------------------------------------");
            Console.WriteLine($"Longest roundtrip delay: {_longestRoundtripDelayMs} ms");
            Console.WriteLine($"Shortest roundtrip delay: {_shortestRoundtripDelayMs} ms");
            Console.WriteLine($"Longest roundtrip message text: {_longestRoundtripDelayMessage}");
            Console.WriteLine($"Longest publish time: {_longestPublishTimeMs} ms");
            Console.WriteLine($"Longest send delay: {_longestSendDelayMs} ms");
            Console.WriteLine($"Shortest send delay: {_shortestSendDelayMs} ms");
            Console.WriteLine($"Total publish time: {_totalPublishTimeMs} ms");
            Console.WriteLine();

            foreach (KeyValuePair<int, int> item in _requestsSent)
            {
                int responses = 0;

                if (_responsesReceived.ContainsKey(item.Key))
                    responses = _responsesReceived[item.Key];

                Console.WriteLine($"Sent {item.Value} requests to subscriber {item.Key}. Got {responses} responses.");
            }

            Console.WriteLine($"Total responses received: {_totalResponsesReceived}");
            Console.WriteLine("------------------------------------------");

            if (IsRunningInContainer())
            {
                new ManualResetEvent(false).WaitOne();
            }
            else
            {
                Console.WriteLine("\nPress any key to exit");
                Console.ReadKey();
            }

            _messageBus.Dispose();
        }

        private static void SendRequestMessage(int messageCounter)
        {
            MessageBase requestMessage;
            Guid id = Guid.NewGuid();

            // Pick a random subscriber from 1 to 10 to send message to
            int targetSubscriber = _randomgenerator.Next(1, 11);

            string message = $"Request number {messageCounter} to subscriber {targetSubscriber} from publisher {_instance}";

            if (!_requestsSent.ContainsKey(targetSubscriber))
                _requestsSent.Add(targetSubscriber, 1);
            else
                _requestsSent[targetSubscriber]++;

            Console.WriteLine($"Publishing request to subscriber {targetSubscriber}");

            DateTime sendTime = DateTime.Now;

            switch (targetSubscriber)
            {
                case 1: requestMessage = new RequestMessage1(id, sendTime, message); break;
                case 2: requestMessage = new RequestMessage2(id, sendTime, message); break;
                case 3: requestMessage = new RequestMessage3(id, sendTime, message); break;
                case 4: requestMessage = new RequestMessage4(id, sendTime, message); break;
                case 5: requestMessage = new RequestMessage5(id, sendTime, message); break;
                case 6: requestMessage = new RequestMessage6(id, sendTime, message); break;
                case 7: requestMessage = new RequestMessage7(id, sendTime, message); break;
                case 8: requestMessage = new RequestMessage8(id, sendTime, message); break;
                case 9: requestMessage = new RequestMessage9(id, sendTime, message); break;
                case 10: requestMessage = new RequestMessage10(id, sendTime, message); break;

                default:
                    return;
            }

            var stopwatch = Stopwatch.StartNew();
            _messageBus?.PublishMessage(requestMessage);
            stopwatch.Stop();

            int publishTimeMs = (int)stopwatch.ElapsedMilliseconds;

            _totalPublishTimeMs += publishTimeMs;

            if (publishTimeMs > _longestPublishTimeMs)
                _longestPublishTimeMs = publishTimeMs;
        }

        private static void OnMessageReceived(object? sender, MessageEventsArgs e)
        {
            DateTime receiveTime = DateTime.Now;
            int delayMs;
            int sendDelayMs;
            int instance;
            string? text;

            switch (e.MessageType)
            {
                case "RabbitMqPerformanceTest.MessageBus.Messages.ResponseMessage1":
                    {
                        ResponseMessage1? message = e.Message as ResponseMessage1;
                        delayMs = (int)receiveTime.Subtract(message.RequestSendTime).TotalMilliseconds;
                        sendDelayMs = (int)message.ResponseReplyTime.Subtract(message.RequestSendTime).TotalMilliseconds;
                        text = message.Message;
                        instance = 1;
                    }
                    break;
                case "RabbitMqPerformanceTest.MessageBus.Messages.ResponseMessage2":
                    {
                        ResponseMessage2? message = e.Message as ResponseMessage2;
                        delayMs = (int)receiveTime.Subtract(message.RequestSendTime).TotalMilliseconds;
                        sendDelayMs = (int)message.ResponseReplyTime.Subtract(message.RequestSendTime).TotalMilliseconds;
                        text = message.Message;
                        instance = 2;
                    }
                    break;
                case "RabbitMqPerformanceTest.MessageBus.Messages.ResponseMessage3":
                    {
                        ResponseMessage3? message = e.Message as ResponseMessage3;
                        delayMs = (int)receiveTime.Subtract(message.RequestSendTime).TotalMilliseconds;
                        sendDelayMs = (int)message.ResponseReplyTime.Subtract(message.RequestSendTime).TotalMilliseconds;
                        text = message.Message;
                        instance = 3;
                    }
                    break;
                case "RabbitMqPerformanceTest.MessageBus.Messages.ResponseMessage4":
                    {
                        ResponseMessage4? message = e.Message as ResponseMessage4;
                        delayMs = (int)receiveTime.Subtract(message.RequestSendTime).TotalMilliseconds;
                        sendDelayMs = (int)message.ResponseReplyTime.Subtract(message.RequestSendTime).TotalMilliseconds;
                        text = message.Message;
                        instance = 4;
                    }
                    break;
                case "RabbitMqPerformanceTest.MessageBus.Messages.ResponseMessage5":
                    {
                        ResponseMessage5? message = e.Message as ResponseMessage5;
                        delayMs = (int)receiveTime.Subtract(message.RequestSendTime).TotalMilliseconds;
                        sendDelayMs = (int)message.ResponseReplyTime.Subtract(message.RequestSendTime).TotalMilliseconds;
                        text = message.Message;
                        instance = 5;
                    }
                    break;
                case "RabbitMqPerformanceTest.MessageBus.Messages.ResponseMessage6":
                    {
                        ResponseMessage6? message = e.Message as ResponseMessage6;
                        delayMs = (int)receiveTime.Subtract(message.RequestSendTime).TotalMilliseconds;
                        sendDelayMs = (int)message.ResponseReplyTime.Subtract(message.RequestSendTime).TotalMilliseconds;
                        text = message.Message;
                        instance = 6;
                    }
                    break;
                case "RabbitMqPerformanceTest.MessageBus.Messages.ResponseMessage7":
                    {
                        ResponseMessage7? message = e.Message as ResponseMessage7;
                        delayMs = (int)receiveTime.Subtract(message.RequestSendTime).TotalMilliseconds;
                        sendDelayMs = (int)message.ResponseReplyTime.Subtract(message.RequestSendTime).TotalMilliseconds;
                        text = message.Message;
                        instance = 7;
                    }
                    break;
                case "RabbitMqPerformanceTest.MessageBus.Messages.ResponseMessage8":
                    {
                        ResponseMessage8? message = e.Message as ResponseMessage8;
                        delayMs = (int)receiveTime.Subtract(message.RequestSendTime).TotalMilliseconds;
                        sendDelayMs = (int)message.ResponseReplyTime.Subtract(message.RequestSendTime).TotalMilliseconds;
                        text = message.Message;
                        instance = 8;
                    }
                    break;
                case "RabbitMqPerformanceTest.MessageBus.Messages.ResponseMessage9":
                    {
                        ResponseMessage9? message = e.Message as ResponseMessage9;
                        delayMs = (int)receiveTime.Subtract(message.RequestSendTime).TotalMilliseconds;
                        sendDelayMs = (int)message.ResponseReplyTime.Subtract(message.RequestSendTime).TotalMilliseconds;
                        text = message.Message;
                        instance = 9;
                    }
                    break;
                case "RabbitMqPerformanceTest.MessageBus.Messages.ResponseMessage10":
                    {
                        ResponseMessage10? message = e.Message as ResponseMessage10;
                        delayMs = (int)receiveTime.Subtract(message.RequestSendTime).TotalMilliseconds;
                        sendDelayMs = (int)message.ResponseReplyTime.Subtract(message.RequestSendTime).TotalMilliseconds;
                        text = message.Message;
                        instance = 10;
                    }
                    break;

                default:
                    Console.WriteLine($"ERROR: Unknown message type received: '{e.MessageType}'");
                    return;
            }

            if (delayMs > _longestRoundtripDelayMs)
            {
                _longestRoundtripDelayMs = delayMs;
                _longestRoundtripDelayMessage = text;
            }

            if (delayMs < _shortestRoundtripDelayMs)
                _shortestRoundtripDelayMs = delayMs;

            if (sendDelayMs > _longestSendDelayMs)
                _longestSendDelayMs = sendDelayMs;

            if (sendDelayMs < _shortestSendDelayMs)
                _shortestSendDelayMs = sendDelayMs;

            Console.WriteLine($"ResponseMessage received. Message: {text}");

            if (!_responsesReceived.ContainsKey(instance))
                _responsesReceived.Add(instance, 1);
            else
                _responsesReceived[instance]++;

            _totalResponsesReceived++;
        }


        public static bool IsRunningInContainer()
        {
            string? dotNetRunningInContainerEnvVariable = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER");
            if (!string.IsNullOrEmpty(dotNetRunningInContainerEnvVariable))
            {
                if (bool.TryParse(dotNetRunningInContainerEnvVariable, out bool runningInDocker))
                    return runningInDocker;
            }

            return false;
        }
    }
}