using IntegrationTestsOnContainers.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace IntegrationTestsOnContainers.TestContainers.Tests.UsingApplicationFactory;

public sealed class WebApplicationFactory(DbContainerFixture fixture) : WebApplicationFactory<Program>
{
    private readonly string _connectionString = fixture.SqlContainer.GetConnectionString();
     
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var connectionString = _connectionString.Replace("master", "MuseumsDb");
        builder.UseSetting("ConnectionStrings:MuseumsDb", connectionString);
    }
}