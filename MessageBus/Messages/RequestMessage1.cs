﻿using System;

namespace RabbitMqPerformanceTest.MessageBus.Messages
{
    public class RequestMessage1 : MessageBase
    {
        public Guid RequestId { get;  }

        public DateTime SendTime { get; }

        public string Message { get; }

        public RequestMessage1(Guid requestId, DateTime sendTime, string message)
        {
            RequestId = requestId;
            SendTime = sendTime;
            Message = message;
        }
    }
}
