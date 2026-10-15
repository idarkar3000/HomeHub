using System.Net;
using System.Net.Http.Json;
using HomeHub.Models;

namespace HomeHub.Tests;

public class RoomEndpointsTests
{
    private readonly HttpClient _client;

    public RoomEndpointsTests()
    {
        var factory = new FakeApiFactory();
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetRooms_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/rooms");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateRoom_ReturnsCreated()
    {
        var response = await _client.PostAsJsonAsync("/api/rooms", new { name = "Cocina", icon = "kitchen" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var room = await response.Content.ReadFromJsonAsync<Room>();
        Assert.Equal("Cocina", room!.Name);
    }

    [Fact]
    public async Task GetRoomById_ReturnsRoom()
    {
        var created = await _client.PostAsJsonAsync("/api/rooms", new { name = "Salón", icon = "weekend" });
        var room = await created.Content.ReadFromJsonAsync<Room>();

        var response = await _client.GetAsync($"/api/rooms/{room!.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<Room>();
        Assert.Equal(room.Id, result!.Id);
    }

    [Fact]
    public async Task GetRoomById_NotFound()
    {
        var response = await _client.GetAsync("/api/rooms/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateRoom_ReturnsUpdatedRoom()
    {
        var created = await _client.PostAsJsonAsync("/api/rooms", new { name = "Cocina", icon = "kitchen" });
        var room = await created.Content.ReadFromJsonAsync<Room>();

        var response = await _client.PutAsJsonAsync($"/api/rooms/{room!.Id}", new { name = "Cocina nueva", icon = "restaurant" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<Room>();
        Assert.Equal("Cocina nueva", result!.Name);
    }

    [Fact]
    public async Task DeleteRoom_ReturnsNoContent()
    {
        var created = await _client.PostAsJsonAsync("/api/rooms", new { name = "Baño", icon = "bathtub" });
        var room = await created.Content.ReadFromJsonAsync<Room>();

        var response = await _client.DeleteAsync($"/api/rooms/{room!.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}
