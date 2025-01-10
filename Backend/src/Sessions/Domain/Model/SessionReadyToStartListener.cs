using MediatR;
using Sessions.Application.Actions.StartSession;

namespace Sessions.Domain.Model
{
    public class SessionReadyToStartListener : INotificationHandler<SessionReadyToStart>
    {
        private readonly StartSessionAction _startSessionAction;

        public SessionReadyToStartListener(StartSessionAction startSessionAction)
        {
            _startSessionAction = startSessionAction;
        }

        public async Task Handle(SessionReadyToStart notification, CancellationToken cancellationToken)
        {
            await _startSessionAction.ExecuteAsync(new StartSessionCommand(notification.SessionId), cancellationToken);
        }
    }
}
