using OpenDDD.Application;
using Sessions.Domain.Model;

namespace Sessions.Application.Actions.CreateSession
{
    public class CreateSessionCommand : ICommand
    {
        public SessionType Type { get; set; }
        public string? BankId { get; set; }
        public string? IntegrationId { get; set; }
        
        public CreateSessionCommand() { }

        public CreateSessionCommand(SessionType type, string? bankId, string? integrationId)
        {
            Type = type;
            BankId = bankId;
            IntegrationId = integrationId;
        }
    }
}
