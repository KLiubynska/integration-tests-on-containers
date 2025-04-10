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

        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });

        Application = await appHost.BuildAsync();

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