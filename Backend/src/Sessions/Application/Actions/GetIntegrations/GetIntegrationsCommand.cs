using OpenDDD.Application;

namespace Sessions.Application.Actions.GetIntegrations
{
    public class GetIntegrationsCommand : ICommand
    {
        public Guid BankId { get; set; }
        
        public GetIntegrationsCommand() { }

        public GetIntegrationsCommand(Guid bankId)
        {
            BankId = bankId;
        }
    }
}
