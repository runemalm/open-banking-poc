using OpenDDD.Domain.Model;

namespace Sessions.Domain.Model.Input
{
    public class InputError : IValueObject
    {
        public InputErrorType Type { get; private set; }
        public string? Message { get; private set; }
        
        public InputError() { }
        
        public InputError(InputErrorType type, string message)
        {
            Type = type;
            Message = message;
        }
    }
}
