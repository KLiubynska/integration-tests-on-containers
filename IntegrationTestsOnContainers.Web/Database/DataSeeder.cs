using IntegrationTestsOnContainers.Domain;

namespace IntegrationTestsOnContainers.Web.Database;

public abstract class DataSeeder
{
    public static void SeedData(ApplicationDbContext context)
    {
        if (!context.Museums.Any())
        {
            context.Museums.AddRange(
                new Museum("History Museum", true),
                new Museum("Art Museum", false),
                new Museum("Modern Art Museum", false),
                new Museum("Mathematics Museum", false),
                new Museum("Museum Of Photography", false)
                );
            context.SaveChanges();
        }
    }
}