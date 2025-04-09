using IntegrationTestsOnContainers.Web.Database;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTestsOnContainers.Web;

public static class AppExtensions
{
    public static void InitializeDb(this IHost app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.Migrate(); 
            DataSeeder.SeedData(dbContext);
        }
    }
}
