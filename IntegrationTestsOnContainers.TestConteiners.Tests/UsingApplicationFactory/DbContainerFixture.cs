using System.Net.Http;
using Testcontainers.MsSql;

namespace IntegrationTestsOnContainers.TestContainers.Tests.UsingApplicationFactory;

public sealed class DbContainerFixture : IAsyncLifetime
{
    public readonly MsSqlContainer SqlContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithPassword("123Pass!")
        .WithExposedPort("1433")
        .WithEnvironment("ACCEPT_EULA", "Y")
        .WithName("Sql_IntegrationTests")
        .Build();

    public Task InitializeAsync()
    {
        return SqlContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return SqlContainer.DisposeAsync().AsTask();
    }
}