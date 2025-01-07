using DDD.Application;

namespace Demo.Application.Actions.StartSession
{
    public class StartSessionCommand : ICommand
    {
        public Guid SessionId { get; set; }
        
        public StartSessionCommand() { }

        public StartSessionCommand(Guid sessionId)
        {
            SessionId = sessionId;
        }
    }
}
