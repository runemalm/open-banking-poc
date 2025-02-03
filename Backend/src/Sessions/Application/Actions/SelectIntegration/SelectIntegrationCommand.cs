using OpenDDD.Application;

namespace Sessions.Application.Actions.SelectIntegration
{
    public class SelectIntegrationCommand : ICommand
    {
        public Guid SessionId { get; set; }
        public string IntegrationId { get; set; }
        
        public SelectIntegrationCommand() { }

        public SelectIntegrationCommand(Guid sessionId, string integrationId)
        {
            SessionId = sessionId;
            IntegrationId = integrationId;
        }
    }
}
