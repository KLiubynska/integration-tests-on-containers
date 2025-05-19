using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Images;
using Testcontainers.MsSql;
using IContainer = DotNet.Testcontainers.Containers.IContainer;

namespace IntegrationTestsOnContainers.TestContainers.Tests.UsingContainers;

public sealed class ContainersFixture : IAsyncLifetime
{
    private static string _sqlPassword = Guid.NewGuid().ToString();
    private const int SqlPort = 1433; 
    private const int ApiPort = 11111;
    private const string SqlHost = "Sql_IntegrationTests_Host";

    private MsSqlContainer SqlContainer { get; set; }

    public IContainer ApiContainer { get; private set; }

    private readonly IFutureDockerImage _apiImage = new ImageFromDockerfileBuilder()
        .WithDockerfileDirectory(CommonDirectoryPath.GetSolutionDirectory(), string.Empty)
        .WithDockerfile("Dockerfile")
        .WithCleanUp(true)
        .Build();

    public ContainersFixture()
    {
        var integrationTestNetwork = new NetworkBuilder()
            .Build(); 

        SqlContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithNetwork(integrationTestNetwork)
            .WithNetworkAliases(SqlHost)
            .WithPassword(_sqlPassword)
            .WithExposedPort(SqlPort)
            .WithEnvironment("ACCEPT_EULA", "Y")
            .WithName("Sql_IntegrationTests")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(1433))
            .Build();

        ApiContainer = new ContainerBuilder()
            .WithImage(_apiImage)
            .WithName("MuseumApi_IntegrationTests") 
            .WithEnvironment("ASPNETCORE_HTTP_PORTS", "80")
            .WithPortBinding(ApiPort, 80)
            .WithNetwork(integrationTestNetwork)
            .WithExposedPort(ApiPort)
            .WithEnvironment(
                $"ASPNETCORE_ENVIRONMENT", "Development")
            .WithEnvironment(
                $"ConnectionStrings__MuseumsDb",
                GetConnectionString("TestMuseumDb", SqlHost, SqlPort))
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(80))
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _apiImage.CreateAsync();
        var sqlStartupTask = SqlContainer.StartAsync();
        var appStartupTask = ApiContainer.StartAsync();
        await Task.WhenAll(sqlStartupTask, appStartupTask);
    }

    public async Task DisposeAsync()
    {
        var sqlDisposeTask = SqlContainer.DisposeAsync().AsTask();
        var appDisposeTask = ApiContainer.DisposeAsync().AsTask();
        await Task.WhenAll(sqlDisposeTask, appDisposeTask);
    }

    private static string GetConnectionString(string dbName, string host, int port)
        => $"Server={host},{port};" +
           $"User ID=sa;Password={_sqlPassword};" +
           $"Database={dbName};TrustServerCertificate=True;Integrated Security=False";
}