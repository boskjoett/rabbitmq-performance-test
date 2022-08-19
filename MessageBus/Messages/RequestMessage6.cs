using System;

namespace RabbitMqPerformanceTest.MessageBus.Messages
{
    public class RequestMessage6 : MessageBase
    {
        public Guid RequestId { get;  }

        public DateTime SendTime { get; }

        public string Message { get; }

        public RequestMessage6(Guid requestId, DateTime sendTime, string message)
        {
            RequestId = requestId;
            SendTime = sendTime;
            Message = message;
        }
    }
}
