using System;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Configuration;
using RabbitMqPerformanceTest.MessageBus;
using RabbitMqPerformanceTest.MessageBus.Messages;

namespace RabbitMqPerformanceTest.Subscriber
{
    class Program
    {
        static string? _inputQueueName;
        static IMessageBus? _messageBus;

        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);

            IConfigurationRoot configuration = builder
                .AddEnvironmentVariables()
                .Build();

            int instance = int.Parse(configuration["Instance"]);
            _inputQueueName = $"Subscriber{instance}";
            Console.WriteLine($"Subscriber {instance} started");

            Console.WriteLine("Waiting for RabbitMQ to start");
            Thread.Sleep(20000);

            string rabbitMqConnectionString = configuration.GetConnectionString("RabbitMq");
            Console.WriteLine($"Connecting to RabbitMQ at {rabbitMqConnectionString}");

            _messageBus = new RabbitMqClient.RabbitMqClient();

            _messageBus.MessageReceived += OnMessageReceived;

            _messageBus.Connect(rabbitMqConnectionString, _inputQueueName);

            switch (instance)
            {
                case 1: _messageBus.Subscribe<RequestMessage1>(); break;
                case 2: _messageBus.Subscribe<RequestMessage2>(); break;
                case 3: _messageBus.Subscribe<RequestMessage3>(); break;
                case 4: _messageBus.Subscribe<RequestMessage4>(); break;
                case 5: _messageBus.Subscribe<RequestMessage5>(); break;
                case 6: _messageBus.Subscribe<RequestMessage6>(); break;
                case 7: _messageBus.Subscribe<RequestMessage7>(); break;
                case 8: _messageBus.Subscribe<RequestMessage8>(); break;
                case 9: _messageBus.Subscribe<RequestMessage9>(); break;
                case 10: _messageBus.Subscribe<RequestMessage10>(); break;
            }

            _messageBus.Start();

            Console.WriteLine("Connected to RabbitMQ");

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

            Console.WriteLine("Test completed");
        }

        private static void OnMessageReceived(object? sender, MessageEventsArgs e)
        {
            DateTime receiveTime = DateTime.Now;
            MessageBase? responseMessage;
            string? messageText;

            switch (e.MessageType)
            {
                case "RabbitMqPerformanceTest.MessageBus.Messages.RequestMessage1":
                    {
                        RequestMessage1? message = e.Message as RequestMessage1;
                        if (message == null)
                        {
                            Console.WriteLine($"Type cast failed for type {e.MessageType}");
                            return;
                        }
                        responseMessage = new ResponseMessage1(message.RequestId, message.SendTime, receiveTime, message.Message + " reply");
                        messageText = message.Message;
                    }
                    break;
                case "RabbitMqPerformanceTest.MessageBus.Messages.RequestMessage2":
                    {
                        RequestMessage2? message = e.Message as RequestMessage2;
                        if (message == null)
                        {
                            Console.WriteLine($"Type cast failed for type {e.MessageType}");
                            return;
                        }
                        responseMessage = new ResponseMessage2(message.RequestId, message.SendTime, receiveTime, message.Message + " reply");
                        messageText = message.Message;
                    }
                    break;
                case "RabbitMqPerformanceTest.MessageBus.Messages.RequestMessage3":
                    {
                        RequestMessage3? message = e.Message as RequestMessage3;
                        if (message == null)
                        {
                            Console.WriteLine($"Type cast failed for type {e.MessageType}");
                            return;
                        }
                        responseMessage = new ResponseMessage3(message.RequestId, message.SendTime, receiveTime, message.Message + " reply");
                        messageText = message.Message;
                    }
                    break;
                case "RabbitMqPerformanceTest.MessageBus.Messages.RequestMessage4":
                    {
                        RequestMessage4? message = e.Message as RequestMessage4;
                        if (message == null)
                        {
                            Console.WriteLine($"Type cast failed for type {e.MessageType}");
                            return;
                        }
                        responseMessage = new ResponseMessage4(message.RequestId, message.SendTime, receiveTime, message.Message + " reply");
                        messageText = message.Message;
                    }
                    break;
                case "RabbitMqPerformanceTest.MessageBus.Messages.RequestMessage5":
                    {
                        RequestMessage5? message = e.Message as RequestMessage5;
                        if (message == null)
                        {
                            Console.WriteLine($"Type cast failed for type {e.MessageType}");
                            return;
                        }
                        responseMessage = new ResponseMessage5(message.RequestId, message.SendTime, receiveTime, message.Message + " reply");
                        messageText = message.Message;
                    }
                    break;
                case "RabbitMqPerformanceTest.MessageBus.Messages.RequestMessage6":
                    {
                        RequestMessage6? message = e.Message as RequestMessage6;
                        if (message == null)
                        {
                            Console.WriteLine($"Type cast failed for type {e.MessageType}");
                            return;
                        }
                        responseMessage = new ResponseMessage6(message.RequestId, message.SendTime, receiveTime, message.Message + " reply");
                        messageText = message.Message;
                    }
                    break;
                case "RabbitMqPerformanceTest.MessageBus.Messages.RequestMessage7":
                    {
                        RequestMessage7? message = e.Message as RequestMessage7;
                        if (message == null)
                        {
                            Console.WriteLine($"Type cast failed for type {e.MessageType}");
                            return;
                        }
                        responseMessage = new ResponseMessage7(message.RequestId, message.SendTime, receiveTime, message.Message + " reply");
                        messageText = message.Message;
                    }
                    break;
                case "RabbitMqPerformanceTest.MessageBus.Messages.RequestMessage8":
                    {
                        RequestMessage8? message = e.Message as RequestMessage8;
                        if (message == null)
                        {
                            Console.WriteLine($"Type cast failed for type {e.MessageType}");
                            return;
                        }
                        responseMessage = new ResponseMessage8(message.RequestId, message.SendTime, receiveTime, message.Message + " reply");
                        messageText = message.Message;
                    }
                    break;
                case "RabbitMqPerformanceTest.MessageBus.Messages.RequestMessage9":
                    {
                        RequestMessage9? message = e.Message as RequestMessage9;
                        if (message == null)
                        {
                            Console.WriteLine($"Type cast failed for type {e.MessageType}");
                            return;
                        }
                        responseMessage = new ResponseMessage9(message.RequestId, message.SendTime, receiveTime, message.Message + " reply");
                        messageText = message.Message;
                    }
                    break;
                case "RabbitMqPerformanceTest.MessageBus.Messages.RequestMessage10":
                    {
                        RequestMessage10? message = e.Message as RequestMessage10;
                        if (message == null)
                        {
                            Console.WriteLine($"Type cast failed for type {e.MessageType}");
                            return;
                        }
                        responseMessage = new ResponseMessage10(message.RequestId, message.SendTime, receiveTime, message.Message + " reply");
                        messageText = message.Message;
                    }
                    break;

                default:
                    Console.WriteLine($"ERROR: Unknown message type received: '{e.MessageType}'");
                    return;
            }

            Console.WriteLine($"RequestMessage received. Message: {messageText}");

            if (responseMessage != null)
            {
                //_messageBus?.PublishMessage(responseMessage);
                _messageBus?.Reply(responseMessage, e.ReplyQueue);
            }
            else
                Console.WriteLine("responseMessage is null");
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