using Sessions.Infrastructure.Integrations.Se.Klarna;
using Sessions.Infrastructure.Integrations.Se.Seb;
using Sessions.Infrastructure.Integrations.Se.Swedbank;

namespace Sessions.Domain.Model.StateMachine.Factory
{
    public class StateMachineFactory : IStateMachineFactory
    {
        private readonly Dictionary<Guid, StateMachineBase> _stateMachines;

        public StateMachineFactory(SeSeb01 seSeb01, SeSwedbank01 seSwedbank01, SeKlarna01 seKlarna01)
        {
            _stateMachines = new Dictionary<Guid, StateMachineBase>
            {
                { IdConstants.SeSeb01, seSeb01 },
                { IdConstants.SeSwedbank01, seSwedbank01 },
                { IdConstants.SeKlarna01, seKlarna01 }
            };
        }

        public StateMachineBase CreateForSession(Session session)
        {
            if (session == null)
                throw new ArgumentNullException(nameof(session), "Session cannot be null.");

            if (string.IsNullOrEmpty(session.SelectedIntegrationId))
                throw new InvalidOperationException("Session does not have a selected integration.");

            if (!Guid.TryParse(session.SelectedIntegrationId, out var selectedIntegrationGuid))
                throw new FormatException($"Invalid GUID format for SelectedIntegrationId: {session.SelectedIntegrationId}");

            if (_stateMachines.TryGetValue(selectedIntegrationGuid, out var stateMachine))
            {
                stateMachine.InitializeForSession(session);
                return stateMachine;
            }

            throw new NotSupportedException($"Integration with ID '{session.SelectedIntegrationId}' is not supported.");
        }
    }
}
