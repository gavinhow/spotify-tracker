using System;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Gavinhow.SpotifyStatistics.Database;
using Gavinhow.SpotifyStatistics.Database.Entity;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Gavinhow.SpotifyStatistics.Web.Tests.Controllers;

public class UnitTest1 : IClassFixture<CustomWebApplicationFactory<Startup>>
{
    
    private readonly CustomWebApplicationFactory<Startup> _factory;

    public UnitTest1(CustomWebApplicationFactory<Startup> factory)
    {
        _factory = factory;
    }
    
    [Fact]
    public async Task TestDisplayName()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Test");
        // Act
        var response = await client.GetAsync($"/User/DisplayName/test_user");

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        string returnedDisplayName = await response.Content.ReadAsStringAsync();
        Assert.Equal("Gavin How", returnedDisplayName); 
    }
    
    [Fact]
    public async Task TestFriends()
    {
        // Arrange
        var client = _factory.CreateClient();
        var db = _factory.Services.GetRequiredService<SpotifyStatisticsContext>();
        client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Test");
        // Act
        var response = await client.GetAsync($"/User/Friends");

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        string[] returnedDisplayName = await response.Content.ReadFromJsonAsync<string[]>() ?? throw new InvalidOperationException();
        Assert.Empty(returnedDisplayName); 
    }
}