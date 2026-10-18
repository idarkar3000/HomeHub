using System.Net;
using System.Net.Http.Json;
using HomeHub.Models.Dtos;

namespace HomeHub.Tests;

public class AuthEndpointsTests
{
    private readonly HttpClient _client;

    public AuthEndpointsTests()
    {
        var factory = new FakeApiFactory();
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_ReturnsCreated()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/auth/register",
            new { email = "nuevo@homehub.com", password = "secreta123" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Register_ExistingEmail_ReturnsConflict()
    {
        await _client.PostAsJsonAsync(
            "/api/auth/register",
            new { email = "repetido@homehub.com", password = "secreta123" });

        var response = await _client.PostAsJsonAsync(
            "/api/auth/register",
            new { email = "repetido@homehub.com", password = "otra1234" });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Register_EmptyFields_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/auth/register",
            new { email = "", password = "" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsToken()
    {
        await _client.PostAsJsonAsync(
            "/api/auth/register",
            new { email = "login@homehub.com", password = "secreta123" });

        var response = await _client.PostAsJsonAsync(
            "/api/auth/login",
            new { email = "login@homehub.com", password = "secreta123" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.False(string.IsNullOrWhiteSpace(auth!.Token));
        Assert.Equal("login@homehub.com", auth.Email);
    }

    [Fact]
    public async Task Login_WrongPassword_ReturnsUnauthorized()
    {
        await _client.PostAsJsonAsync(
            "/api/auth/register",
            new { email = "login@homehub.com", password = "secreta123" });

        var response = await _client.PostAsJsonAsync(
            "/api/auth/login",
            new { email = "login@homehub.com", password = "incorrecta" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
