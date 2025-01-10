namespace Sessions.Domain.Model.StateMachine.Factory
{
    public interface IStateMachineFactory
    {
        StateMachineBase CreateForSession(Session session);
    }
}
