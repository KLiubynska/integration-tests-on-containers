using System.Net;
using System.Text.Json;
using IntegrationTestsOnContainers.Web.Queries;

namespace IntegrationTestsOnContainers.TestContainers.Tests.UsingContainers;

public sealed class MuseumsApiOnContainerTests : IClassFixture<ContainersFixture>, IDisposable
{ 
    private readonly HttpClient _httpClient;

    public MuseumsApiOnContainerTests(ContainersFixture fixture)
    {
        _httpClient = new HttpClient();
        var requestUri = new UriBuilder(
            Uri.UriSchemeHttp,
            fixture.ApiContainer.Hostname,
            fixture.ApiContainer.GetMappedPublicPort(80),
            "/").Uri;

        _httpClient.BaseAddress = requestUri;
    }

    [Fact]
    public async Task Get_OpenMuseums_ReturnsCorrectResult()
    {
        // Arrange
        var museumsResponse = await _httpClient.GetAsync("/v1/museums/open");

        // Assert
        Assert.Equal(HttpStatusCode.OK, museumsResponse.StatusCode);

        var responseData = await museumsResponse.Content.ReadAsStringAsync();
        var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        MuseumReadModel[]? museums = JsonSerializer.Deserialize<IEnumerable<MuseumReadModel>>(responseData, jsonSerializerOptions)?.ToArray();

        Assert.NotNull(museums);
        Assert.Single(museums);
        Assert.Equal("History Museum", museums!.First()!.Name);
    }

    [Fact]
    public async Task Get_ClosedMuseums_ReturnsCorrectResult()
    {
        // Arrange
        var museumsResponse = await _httpClient.GetAsync("/v1/museums/closed");

        // Assert
        Assert.Equal(HttpStatusCode.OK, museumsResponse.StatusCode);

        var responseData = await museumsResponse.Content.ReadAsStringAsync();
        var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        MuseumReadModel[]? museums = JsonSerializer.Deserialize<IEnumerable<MuseumReadModel>>(responseData, jsonSerializerOptions)?.ToArray();

        Assert.NotNull(museums);
        Assert.Equal(4, museums.Length);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}