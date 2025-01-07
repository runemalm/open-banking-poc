using DDD.Application;

namespace Demo.Application.Actions.ProvideInput
{
    public class ProvideInputCommand : ICommand
    {
        public Guid SessionId { get; set; }
        public string Value { get; set; }
        
        public ProvideInputCommand() { }
        
        public ProvideInputCommand(Guid sessionId, string value)
        {
            SessionId = sessionId;
            Value = value;
        }
    }
}
