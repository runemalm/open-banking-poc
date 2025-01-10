using Microsoft.EntityFrameworkCore;
using DDD.Infrastructure.Repositories.EF;
using Sessions.Domain.Model;
using Sessions.Infrastructure.Repositories.EF.Context;

namespace Sessions.Infrastructure.Repositories.EF
{
    public class EfSessionRepository : EfRepositoryBase<Session, Guid, SessionDbContext>, ISessionRepository
    {
        public EfSessionRepository(ISessionDbContext context) : base((SessionDbContext)context)
        {
            
        }
        
        // protected override IQueryable<Session> IncludeRelatedEntities(IQueryable<Session> query)
        // {
        //     return query
        //         .Include(s => s.User)
        //         .Include(s => s.BankAccounts)
        //         .Include(s => s.TransactionHistory.Transactions);
        // }
        
        protected override void AttachRelatedEntities(Session aggregateRoot)
        {
            // Attach User if not null
            if (aggregateRoot.User != null)
            {
                _context.Entry(aggregateRoot.User).State = EntityState.Modified;
            }

            // Attach BankAccounts if not null
            if (aggregateRoot.BankAccounts != null)
            {
                foreach (var account in aggregateRoot.BankAccounts.Accounts)
                {
                    _context.Entry(account).State = EntityState.Modified;
                }
            }

            // Attach TransactionHistory and Transactions if not null
            if (aggregateRoot.TransactionHistory != null)
            {
                foreach (var transaction in aggregateRoot.TransactionHistory.Transactions)
                {
                    _context.Entry(transaction).State = EntityState.Modified;
                }
            }
        }
    }
}
