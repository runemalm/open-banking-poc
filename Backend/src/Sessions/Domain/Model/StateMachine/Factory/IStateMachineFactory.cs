namespace Demo.Domain.Model.StateMachine.Factory
{
    public interface IStateMachineFactory
    {
        StateMachineBase CreateForSession(Session session);
    }
}
