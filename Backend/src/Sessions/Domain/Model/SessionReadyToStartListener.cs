using OpenDDD.API.HostedServices;
using OpenDDD.API.Options;
using OpenDDD.Infrastructure.Events;
using OpenDDD.Infrastructure.Events.Base;
using Sessions.Application.Actions.StartSession;

namespace Sessions.Domain.Model
{
    public class SessionReadyToStartListener : EventListenerBase<SessionReadyToStart, StartSessionAction>
    {
        public SessionReadyToStartListener(
            IMessagingProvider messagingProvider,
            OpenDddOptions options,
            IServiceScopeFactory serviceScopeFactory,
            StartupHostedService startupService,
            ILogger<SessionReadyToStartListener> logger)
            : base(messagingProvider, options, serviceScopeFactory, startupService, logger) { }

        public async Task Handle(SessionReadyToStart notification, CancellationToken ct)
        {
            
        }

        public override async Task HandleAsync(SessionReadyToStart @event, StartSessionAction action, CancellationToken ct)
        {
            var command = new StartSessionCommand(@event.SessionId);
            await action.ExecuteAsync(command, ct);
        }
    }
}
