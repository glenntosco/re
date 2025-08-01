using Microsoft.EntityFrameworkCore;
using Re.Data;
public static class Seed
{
    public static async Task<IHost> SeedDataAsync(this IHost host)
    {
        using (var scope = host.Services.CreateScope())
        {
            var identityContext = scope.ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>();
            await identityContext.Database.MigrateAsync();

            if (!await identityContext.Users.AnyAsync())
            {
                await identityContext.Seed();
            }

            var realEstateContext = scope.ServiceProvider.GetRequiredService<RealEstateDBContext>();
            await realEstateContext.Database.MigrateAsync();
            if (!await realEstateContext.Properties.AnyAsync())
            {
                await realEstateContext.Seed();
            }
        }
        return host;
    }
}