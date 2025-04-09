using IntegrationTestsOnContainers.Domain;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTestsOnContainers.Web.Database;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Museum> Museums { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
