namespace Demo.Domain.Model.Input
{
    public class InputError
    {
        public InputErrorType Type { get; private set; }
        public string? Message { get; private set; }
        
        public InputError(InputErrorType type, string message)
        {
            Type = type;
            Message = message;
        }
    }
}
