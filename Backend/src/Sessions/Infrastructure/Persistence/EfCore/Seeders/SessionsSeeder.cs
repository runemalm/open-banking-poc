using Microsoft.EntityFrameworkCore;
using OpenDDD.Infrastructure.Persistence.EfCore.Base;
using OpenDDD.Infrastructure.Persistence.EfCore.Seeders;
using Sessions.Domain.Model;
using Sessions.Domain.Model.Bank;
using Sessions.Domain.Model.Integration;

namespace Sessions.Infrastructure.Persistence.EfCore.Seeders
{
    public class SessionsSeeder : IEfCoreSeeder
    {
        public async Task ExecuteAsync(OpenDddDbContextBase dbContext, CancellationToken ct)
        {
            // Seed banks and integrations
            if (!await dbContext.Set<Bank>().AnyAsync() && !await dbContext.Set<Integration>().AnyAsync())
            {
                // Create banks
                var swedbank = Bank.Create("Swedbank");
                var seb = Bank.Create("SEB");
                var klarna = Bank.Create("Klarna");

                // Explicitly set the IDs for seeding
                typeof(Bank).GetProperty("Id")!.SetValue(swedbank, IdConstants.SeSwedbank);
                typeof(Bank).GetProperty("Id")!.SetValue(seb, IdConstants.SeSeb);
                typeof(Bank).GetProperty("Id")!.SetValue(klarna, IdConstants.SeKlarna);

                // Create integrations linked to their respective banks
                var seSwedbank01 = Integration.Create("SeSwedbank01", "BankID i mobilen", swedbank.Id);
                var seSeb01 = Integration.Create("SeSeb01", "BankID i mobilen", seb.Id);
                var seKlarna01 = Integration.Create("SeKlarna01", "BankID i mobilen", klarna.Id);
                var seKlarna02 = Integration.Create("SeKlarna02", "BankID i mobilen (2)", klarna.Id);

                typeof(Integration).GetProperty("Id")!.SetValue(seSwedbank01, IdConstants.SeSwedbank01);
                typeof(Integration).GetProperty("Id")!.SetValue(seSeb01, IdConstants.SeSeb01);
                typeof(Integration).GetProperty("Id")!.SetValue(seKlarna01, IdConstants.SeKlarna01);
                typeof(Integration).GetProperty("Id")!.SetValue(seKlarna02, IdConstants.SeKlarna02);

                // Add the integrations to their respective banks
                swedbank.AddIntegration(seSwedbank01);
                seb.AddIntegration(seSeb01);
                klarna.AddIntegration(seKlarna01);
                klarna.AddIntegration(seKlarna02);

                // Add banks (and their integrations through EF Core cascading)
                dbContext.Set<Bank>().AddRange(swedbank, seb, klarna);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
