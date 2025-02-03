using OpenDDD.Domain.Model;

namespace Sessions.Domain.Model
{
    public class SessionReadyToStart : IDomainEvent
    {
        public Guid SessionId { get; }

        public SessionReadyToStart(Guid sessionId)
        {
            SessionId = sessionId;
        }
    }
}
