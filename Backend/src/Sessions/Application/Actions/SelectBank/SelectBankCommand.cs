using DDD.Application;

namespace Sessions.Application.Actions.SelectBank
{
    public class SelectBankCommand : ICommand
    {
        public Guid SessionId { get; set; }
        public string? BankId { get; set; }
        
        public SelectBankCommand() { }

        public SelectBankCommand(Guid sessionId, string? bankId)
        {
            SessionId = sessionId;
            BankId = bankId;
        }
    }
}
