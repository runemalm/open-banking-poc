using Sessions.Domain.Model;

namespace Sessions.Application.Actions.CreateSession
{
    public class SessionResponse
    {
        public Guid SessionId { get; set; }
        public State CurrentState { get; set; }
        public object? Input { get; set; } = null;

        private SessionResponse() { }

        public static SessionResponse Create(Guid sessionId, State currentState, Domain.Model.Input.Input? input)
        {
            var sessionResponse = new SessionResponse()
            {
                SessionId = sessionId,
                CurrentState = currentState,
                Input = input == null ? null : new
                {
                    input.Status,
                    input.RequestedAt,
                    input.RequestType,
                    input.Attempt,
                    input.ProvidedAt,
                    input.Value,
                    input.Error,
                    RequestParams = input.RequestParams?.Data
                }
            };
            return sessionResponse;
        }
    }
}
