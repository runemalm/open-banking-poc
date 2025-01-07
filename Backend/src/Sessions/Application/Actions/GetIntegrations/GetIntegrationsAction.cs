using DDD.Application;
using Demo.Domain.Model.Integration;

namespace Demo.Application.Actions.GetIntegrations
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
                integration => integration.BankId == command.BankId
            );

            return integrations.ToList();
        }
    }
}
