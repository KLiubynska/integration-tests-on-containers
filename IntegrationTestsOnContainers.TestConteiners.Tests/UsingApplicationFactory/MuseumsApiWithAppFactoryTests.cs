﻿using System.Net;
using System.Text.Json;
using IntegrationTestsOnContainers.Web.Queries;
using Microsoft.AspNetCore.Mvc.Testing;
using Program = IntegrationTestsOnContainers.Web.Program;

namespace IntegrationTestsOnContainers.TestContainers.Tests.UsingApplicationFactory;

public sealed class MuseumsApiWithAppFactoryTests : IClassFixture<DbContainerFixture>, IDisposable
{
    private readonly WebApplicationFactory<Program> _webApplicationFactory;

    private readonly HttpClient _httpClient;

    public MuseumsApiWithAppFactoryTests(DbContainerFixture fixture)
    {
        var clientOptions = new WebApplicationFactoryClientOptions();

        _webApplicationFactory = new WebApplicationFactory(fixture);
        _httpClient = _webApplicationFactory.CreateClient(clientOptions);
    }

    [Fact]
    public async Task Get_OpenMuseums_ReturnsOkResult()
    {
        // Act
        var museumsResponse = await _httpClient.GetAsync("/v1/museums/open");

        // Assert
        Assert.Equal(HttpStatusCode.OK, museumsResponse.StatusCode);
    }

    [Fact]
    public async Task Get_OpenMuseums_ReturnsCorrectResult()
    {
        // Act
        var museumsResponse = await _httpClient.GetAsync("/v1/museums/open");
        var responseData = await museumsResponse.Content.ReadAsStringAsync();
        var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        MuseumReadModel[]? museums = JsonSerializer.Deserialize<IEnumerable<MuseumReadModel>>(responseData, jsonSerializerOptions)?.ToArray();

        // Assert
        Assert.Equal("History Museum", museums?.FirstOrDefault()?.Name);
    }

    [Fact]
    public async Task Get_ClosedMuseums_ReturnsOkResult()
    {
        // Act
        var museumsResponse = await _httpClient.GetAsync("/v1/museums/closed");

        // Assert
        Assert.Equal(HttpStatusCode.OK, museumsResponse.StatusCode);
    }

    [Fact]
    public async Task Get_ClosedMuseums_ReturnsCorrectResult()
    {
        // Act
        var museumsResponse = await _httpClient.GetAsync("/v1/museums/closed");
        var responseData = await museumsResponse.Content.ReadAsStringAsync();
        var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        MuseumReadModel[]? museums = JsonSerializer.Deserialize<IEnumerable<MuseumReadModel>>(responseData, jsonSerializerOptions)?.ToArray();

        // Assert
        Assert.Equal(4, museums?.Length);
    }

    public void Dispose()
    {
        _webApplicationFactory.Dispose();
    }
}