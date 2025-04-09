using IntegrationTestsOnContainers.Web.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace IntegrationTestsOnContainers.Aspire.Tests;

public class MuseumApiTests(ITestOutputHelper testOutput)
{ 
    [Fact]
    public async Task GetWebResourceRootReturnsOkStatusCode()
    {
        // Arrange
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.IntegrationTestsOnContainers_AppHost>(); 
        appHost.Services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.AddXUnit(testOutput);
            loggingBuilder.SetMinimumLevel(LogLevel.Debug);
        });

        appHost.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(appHost.Configuration.GetConnectionString("sql"));
        });
        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler(config =>
            {
                TimeSpan timeSpan = TimeSpan.FromMinutes(2);
                config.AttemptTimeout.Timeout = timeSpan;
                config.CircuitBreaker.SamplingDuration = timeSpan * 2;
                config.TotalRequestTimeout.Timeout = timeSpan * 3;
            });
        });
        // To output logs to the xUnit.net ITestOutputHelper, consider adding a package from https://www.nuget.org/packages?q=xunit+logging
       
        await using var app = await appHost.BuildAsync();
        var resourceNotificationService = app.Services.GetRequiredService<ResourceNotificationService>();
        await resourceNotificationService.WaitForResourceAsync("sql", KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(60));

        
        await app.StartAsync();
        var c = await app.GetConnectionStringAsync("sql");
        
        // Act
        var httpClient = app.CreateHttpClient("integrationtestsoncontainers-web");
        await resourceNotificationService.WaitForResourceAsync("integrationtestsoncontainers-web", KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(60));

        var response = await httpClient.GetAsync("/v1/Museums/open");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
