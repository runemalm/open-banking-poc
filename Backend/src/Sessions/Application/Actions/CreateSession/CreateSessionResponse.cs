using Demo.Domain.Model;

namespace Demo.Application.Actions.CreateSession
{
    public class SessionResponse
    {
        public Guid SessionId { get; set; }
        public State CurrentState { get; set; }
        public Domain.Model.Input.Input? Input { get; set; } = null;

        private SessionResponse() { }

        public static SessionResponse Create(Guid sessionId, State currentState, Domain.Model.Input.Input? input)
        {
            var sessionResponse = new SessionResponse()
            {
                SessionId = sessionId,
                CurrentState = currentState,
                Input = input
            };
            return sessionResponse;
        }
    }
}
