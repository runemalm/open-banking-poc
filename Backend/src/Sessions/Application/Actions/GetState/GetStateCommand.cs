using DDD.Application;

namespace Demo.Application.Actions.GetState
{
    public class GetStateCommand : ICommand
    {
        public Guid SessionId { get; }
        
        public GetStateCommand() { }

        public GetStateCommand(Guid sessionId)
        {
            SessionId = sessionId;
        }
    }
}
