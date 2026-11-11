using System.Net;
using System.Net.Http.Json;
using HomeHub.Models;

namespace HomeHub.Tests;

public class OwnershipTests
{
    private readonly HttpClient _client;

    public OwnershipTests()
    {
        var factory = new FakeApiFactory();
        _client = factory.CreateClient();
    }

    private static HttpRequestMessage Request(HttpMethod method, string url, int userId, object? body = null)
    {
        var request = new HttpRequestMessage(method, url);
        request.Headers.Add("X-Test-User", userId.ToString());

        if (body is not null)
        {
            request.Content = JsonContent.Create(body);
        }

        return request;
    }

    [Fact]
    public async Task UserCannotReadUpdateOrDeleteAnotherUsersRoom()
    {
        var created = await _client.SendAsync(
            Request(HttpMethod.Post, "/api/rooms", 1, new { name = "Cocina", icon = "kitchen" }));
        var room = await created.Content.ReadFromJsonAsync<Room>();

        var get = await _client.SendAsync(Request(HttpMethod.Get, $"/api/rooms/{room!.Id}", 2));
        var update = await _client.SendAsync(
            Request(HttpMethod.Put, $"/api/rooms/{room.Id}", 2, new { name = "Hack", icon = "x" }));
        var delete = await _client.SendAsync(Request(HttpMethod.Delete, $"/api/rooms/{room.Id}", 2));

        Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, update.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, delete.StatusCode);
    }

    [Fact]
    public async Task UserCannotReadUpdateOrDeleteAnotherUsersTask()
    {
        var roomResponse = await _client.SendAsync(
            Request(HttpMethod.Post, "/api/rooms", 1, new { name = "Cocina", icon = "kitchen" }));
        var room = await roomResponse.Content.ReadFromJsonAsync<Room>();

        var taskResponse = await _client.SendAsync(
            Request(HttpMethod.Post, $"/api/rooms/{room!.Id}/tasks", 1, new { title = "Barrer", isCompleted = false, recurrence = "none" }));
        var task = await taskResponse.Content.ReadFromJsonAsync<TaskItem>();

        var get = await _client.SendAsync(Request(HttpMethod.Get, $"/api/tasks/{task!.Id}", 2));
        var update = await _client.SendAsync(
            Request(HttpMethod.Put, $"/api/tasks/{task.Id}", 2, new { roomId = room.Id, title = "Hack", isCompleted = false, recurrence = "none" }));
        var toggle = await _client.SendAsync(Request(HttpMethod.Patch, $"/api/tasks/{task.Id}/toggle", 2));
        var delete = await _client.SendAsync(Request(HttpMethod.Delete, $"/api/tasks/{task.Id}", 2));
        var roomTasks = await _client.SendAsync(Request(HttpMethod.Get, $"/api/rooms/{room.Id}/tasks", 2));

        Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, update.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, toggle.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, delete.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, roomTasks.StatusCode);
    }

    [Fact]
    public async Task UserCannotReadUpdateOrDeleteAnotherUsersNote()
    {
        var created = await _client.SendAsync(
            Request(HttpMethod.Post, "/api/notes", 1, new { content = "Comprar leche", kind = "shopping" }));
        var note = await created.Content.ReadFromJsonAsync<Note>();

        var get = await _client.SendAsync(Request(HttpMethod.Get, $"/api/notes/{note!.Id}", 2));
        var update = await _client.SendAsync(
            Request(HttpMethod.Put, $"/api/notes/{note.Id}", 2, new { content = "Hack", kind = "shopping" }));
        var toggle = await _client.SendAsync(Request(HttpMethod.Patch, $"/api/notes/{note.Id}/toggle", 2));
        var delete = await _client.SendAsync(Request(HttpMethod.Delete, $"/api/notes/{note.Id}", 2));
        var list = await _client.SendAsync(Request(HttpMethod.Get, "/api/notes", 2));

        Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, update.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, toggle.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, delete.StatusCode);

        var notes = await list.Content.ReadFromJsonAsync<List<Note>>();
        Assert.Empty(notes!);
    }

    [Fact]
    public async Task UserCannotUseAnotherUsersShoppingCategory()
    {
        var created = await _client.SendAsync(
            Request(HttpMethod.Post, "/api/shopping-categories", 1, new { name = "Mercadona" }));
        var category = await created.Content.ReadFromJsonAsync<ShoppingCategory>();

        var noteResponse = await _client.SendAsync(
            Request(HttpMethod.Post, "/api/notes", 2, new { content = "Leche", kind = "shopping", categoryId = category!.Id }));
        var delete = await _client.SendAsync(
            Request(HttpMethod.Delete, $"/api/shopping-categories/{category.Id}", 2));

        Assert.Equal(HttpStatusCode.BadRequest, noteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, delete.StatusCode);
    }
}
