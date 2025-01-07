using DDD.Domain.Model;
using MediatR;

namespace Demo.Domain.Model
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
