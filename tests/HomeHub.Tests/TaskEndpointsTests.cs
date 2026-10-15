using System.Net;
using System.Net.Http.Json;
using HomeHub.Models;

namespace HomeHub.Tests;

public class TaskEndpointsTests
{
    private readonly HttpClient _client;

    public TaskEndpointsTests()
    {
        var factory = new FakeApiFactory();
        _client = factory.CreateClient();
    }

    private async Task<int> CreateRoom()
    {
        var response = await _client.PostAsJsonAsync("/api/rooms", new { name = "Cocina", icon = "kitchen" });
        var room = await response.Content.ReadFromJsonAsync<Room>();
        return room!.Id;
    }

    [Fact]
    public async Task GetRoomTasks_ReturnsRoomTasks()
    {
        var roomId = await CreateRoom();
        await _client.PostAsJsonAsync($"/api/rooms/{roomId}/tasks", new { title = "Barrer", isCompleted = false, recurrence = "none" });

        var response = await _client.GetAsync($"/api/rooms/{roomId}/tasks");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var tasks = await response.Content.ReadFromJsonAsync<List<TaskItem>>();
        Assert.Single(tasks!);
    }

    [Fact]
    public async Task CreateTask_ReturnsCreatedTask()
    {
        var roomId = await CreateRoom();

        var response = await _client.PostAsJsonAsync($"/api/rooms/{roomId}/tasks", new
        {
            title = "Fregar",
            isCompleted = false,
            dueDate = new DateTime(2026, 10, 20, 15, 0, 0),
            endDate = new DateTime(2026, 10, 20, 16, 0, 0),
            recurrence = "none",
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var task = await response.Content.ReadFromJsonAsync<TaskItem>();
        Assert.Equal("Fregar", task!.Title);
        Assert.Equal(new DateTime(2026, 10, 20, 16, 0, 0), task.EndDate);
    }

    [Fact]
    public async Task ToggleTask_MarksAsCompleted()
    {
        var roomId = await CreateRoom();
        var created = await _client.PostAsJsonAsync($"/api/rooms/{roomId}/tasks", new { title = "Barrer", isCompleted = false, recurrence = "none" });
        var task = await created.Content.ReadFromJsonAsync<TaskItem>();

        var response = await _client.PatchAsync($"/api/tasks/{task!.Id}/toggle", new StringContent(""));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<TaskItem>();
        Assert.True(result!.IsCompleted);
    }

    [Fact]
    public async Task ToggleRecurringTask_AdvancesDueDate()
    {
        var roomId = await CreateRoom();
        var due = new DateTime(2026, 10, 20, 0, 0, 0);
        var created = await _client.PostAsJsonAsync($"/api/rooms/{roomId}/tasks", new { title = "Regar", isCompleted = false, dueDate = due, endDate = (DateTime?)null, recurrence = "weekly" });
        var task = await created.Content.ReadFromJsonAsync<TaskItem>();

        var response = await _client.PatchAsync($"/api/tasks/{task!.Id}/toggle", new StringContent(""));

        var result = await response.Content.ReadFromJsonAsync<TaskItem>();
        Assert.Equal(due.AddDays(7), result!.DueDate);
    }

    [Fact]
    public async Task GetCalendar_ReturnsTasksWithDate()
    {
        var roomId = await CreateRoom();
        await _client.PostAsJsonAsync($"/api/rooms/{roomId}/tasks", new { title = "Sin fecha", isCompleted = false, recurrence = "none" });
        await _client.PostAsJsonAsync($"/api/rooms/{roomId}/tasks", new { title = "Con fecha", isCompleted = false, dueDate = new DateTime(2026, 10, 21, 0, 0, 0), recurrence = "none" });

        var response = await _client.GetAsync("/api/tasks/calendar");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var tasks = await response.Content.ReadFromJsonAsync<List<TaskItem>>();
        Assert.Single(tasks!);
        Assert.Equal("Con fecha", tasks![0].Title);
    }

    [Fact]
    public async Task DeleteTask_ReturnsNoContent()
    {
        var roomId = await CreateRoom();
        var created = await _client.PostAsJsonAsync($"/api/rooms/{roomId}/tasks", new { title = "Barrer", isCompleted = false, recurrence = "none" });
        var task = await created.Content.ReadFromJsonAsync<TaskItem>();

        var response = await _client.DeleteAsync($"/api/tasks/{task!.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}
