using System;

namespace RabbitMqPerformanceTest.MessageBus.Messages
{
    public class ResponseMessage3 : MessageBase
    {
        public Guid RequestId { get; }

        public DateTime RequestSendTime { get; }

        public DateTime ResponseReplyTime { get; }

        public string Message { get; }

        public ResponseMessage3(Guid requestId, DateTime requestSendTime, DateTime responseReplyTime, string message)
        {
            RequestId = requestId;
            RequestSendTime = requestSendTime;
            ResponseReplyTime = responseReplyTime;
            Message = message;
        }
    }
}
