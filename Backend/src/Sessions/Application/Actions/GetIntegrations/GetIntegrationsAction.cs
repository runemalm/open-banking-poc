using OpenDDD.Application;
using Sessions.Domain.Model.Integration;

namespace Sessions.Application.Actions.GetIntegrations
{
    public class GetIntegrationsAction : IAction<GetIntegrationsCommand, List<Integration>>
    {
        private readonly IIntegrationRepository _integrationRepository;

        public GetIntegrationsAction(IIntegrationRepository integrationRepository)
        {
            _integrationRepository = integrationRepository;
        }

        public async Task<List<Integration>> ExecuteAsync(GetIntegrationsCommand command, CancellationToken ct)
        {
            var integrations = await _integrationRepository.FindWithAsync(
                integration => integration.BankId == command.BankId,
            ct);

            return integrations.ToList();
        }
    }
}
