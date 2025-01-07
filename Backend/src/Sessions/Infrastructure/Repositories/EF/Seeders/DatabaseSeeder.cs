using Microsoft.EntityFrameworkCore;
using Demo.Domain.Model;
using Demo.Domain.Model.Bank;
using Demo.Domain.Model.Integration;
using Demo.Infrastructure.Repositories.EF.Context;

namespace Demo.Infrastructure.Repositories.EF.Seeders
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<SessionDbContext>();

            // Ensure database exists
            await context.Database.EnsureCreatedAsync();

            // Seed banks and integrations
            if (!await context.Banks.AnyAsync() && !await context.Integrations.AnyAsync())
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
                context.Banks.AddRange(swedbank, seb, klarna);
                await context.SaveChangesAsync();
            }
        }
    }
}
