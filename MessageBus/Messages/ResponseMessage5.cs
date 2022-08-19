using System;

namespace RabbitMqPerformanceTest.MessageBus.Messages
{
    public class ResponseMessage5 : MessageBase
    {
        public Guid RequestId { get; }

        public DateTime RequestSendTime { get; }

        public DateTime ResponseReplyTime { get; }

        public string Message { get; }

        public ResponseMessage5(Guid requestId, DateTime requestSendTime, DateTime responseReplyTime, string message)
        {
            RequestId = requestId;
            RequestSendTime = requestSendTime;
            ResponseReplyTime = responseReplyTime;
            Message = message;
        }
    }
}
