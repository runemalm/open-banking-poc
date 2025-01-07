using DDD.Application;

namespace Demo.Application.Actions.GetSession
{
    public class GetSessionCommand : ICommand
    {
        public Guid SessionId { get; }
        
        public GetSessionCommand() { }

        public GetSessionCommand(Guid sessionId)
        {
            SessionId = sessionId;
        }
    }
}
