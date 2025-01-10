using DDD.Domain.Model;
using MediatR;

namespace Sessions.Domain.Model
{
    public class SessionReadyToStart : IDomainEvent, INotification
    {
        public Guid SessionId { get; }

        public SessionReadyToStart(Guid sessionId)
        {
            SessionId = sessionId;
        }
    }
}
