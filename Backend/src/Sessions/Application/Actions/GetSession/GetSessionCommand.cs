using OpenDDD.Application;

namespace Sessions.Application.Actions.GetSession
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
