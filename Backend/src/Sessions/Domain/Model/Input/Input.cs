using DDD.Domain.Model;

namespace Demo.Domain.Model.Input
{
    public class Input : AggregateRootBase<Guid>
    {
        public InputStatus Status { get; private set; }
        public DateTime? RequestedAt { get; private set; }
        public InputRequestType? RequestType { get; private set; }
        public Dictionary<string, string?>? RequestParams { get; private set; }

        public int? Attempt { get; private set; }
        public DateTime? ProvidedAt { get; private set; }
        public string? Value { get; private set; }
        public InputError? Error { get; private set; }
        public string? ErrorMessage { get; private set; }

        private Input(Guid id) : base(id)
        {
            
        }

        public static Input CreateForSession(Guid sessionId)
        {
            var inputId = sessionId;

            var input = new Input(inputId)
            {
                Status = InputStatus.NotRequested
            };
            return input;
        }

        public void Request(InputRequestType requestType, Dictionary<string, string?>? requestParams = null)
        {
            Status = InputStatus.Requested;
            RequestedAt = DateTime.UtcNow;
            RequestType = requestType;
            RequestParams = requestParams;

            ProvidedAt = null;
            Value = null;
            Error = null;
            ErrorMessage = null;
            Attempt = null;
        }

        public void SetRequestParam(string name, string value)
        {
            if (Status != InputStatus.Requested)
                throw new InvalidOperationException("Input has not been requested.");

            RequestParams?.TryAdd(name, value);
        }

        public void Provide(string value)
        {
            if (Status == InputStatus.Provided)
                throw new InvalidOperationException("Input has already been provided.");

            Status = InputStatus.Provided;
            ProvidedAt = DateTime.UtcNow;
            Value = value;
            Error = null;
            ErrorMessage = null;
            Attempt++;
        }
        
        public void MarkAsError(InputError error, string? errorMessage = null)
        {
            if (Status != InputStatus.Provided)
                throw new InvalidOperationException("Cannot mark input as error if it has not been provided.");

            Status = InputStatus.Error;
            Error = error;
            ErrorMessage = errorMessage;
        }
    }
}
