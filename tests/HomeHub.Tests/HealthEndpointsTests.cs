using System.Net;
using System.Net.Http.Json;

namespace HomeHub.Tests;

public class HealthEndpointsTests
{
    [Fact]
    public async Task Root_ReturnsHealthyAndSecurityHeaders()
    {
        var client = new FakeApiFactory().CreateClient();

        var response = await client.GetAsync("/");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("nosniff", response.Headers.GetValues("X-Content-Type-Options").Single());
        Assert.Equal("DENY", response.Headers.GetValues("X-Frame-Options").Single());

        var body = await response.Content.ReadFromJsonAsync<HealthResponse>();
        Assert.Equal("healthy", body!.Status);
        Assert.Equal("HomeHub", body.App);
    }

    private sealed record HealthResponse(string Status, string App);
}
