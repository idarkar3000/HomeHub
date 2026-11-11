using System.Net;
using System.Text;

namespace HomeHub.Tests;

public class RealtimeTests
{
    [Fact]
    public async Task Negotiate_IsMappedAndAllowsCredentialsFromConfiguredOrigin()
    {
        var client = new FakeApiFactory().CreateClient();

        var request = new HttpRequestMessage(
            HttpMethod.Post,
            "/hubs/notes/negotiate?negotiateVersion=1");
        request.Headers.Add("Origin", "http://localhost:4200");
        request.Content = new StringContent(string.Empty, Encoding.UTF8, "text/plain");

        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(
            "http://localhost:4200",
            response.Headers.GetValues("Access-Control-Allow-Origin").Single());
        Assert.Equal(
            "true",
            response.Headers.GetValues("Access-Control-Allow-Credentials").Single());
    }
}
