using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTestsOnContainers.TestContainers.Tests.UsingAspire;

public sealed class AspireFixture : IAsyncLifetime
{
    public DistributedApplication Application { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        IDistributedApplicationTestingBuilder appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.IntegrationTestsOnContainers_AppHost>();

        // Register a default set of resilience policies using Polly under the hood, with sensible defaults
        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });

        Application = await appHost.BuildAsync();

        // Service that enables the publishing and subscribing to changes in the state of resources within a distributed application
        var resourceNotificationService = Application.Services.GetRequiredService<ResourceNotificationService>();

        await resourceNotificationService.WaitForResourceAsync("sql", KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(60));

        await resourceNotificationService.WaitForResourceAsync("integrationtestsoncontainers-web", KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(60));

        await Application.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await Application.DisposeAsync();
    }
}