using System.Net;
using System.Net.Http.Json;
using HomeHub.Models;

namespace HomeHub.Tests;

public class NoteEndpointsTests
{
    private readonly HttpClient _client;

    public NoteEndpointsTests()
    {
        var factory = new FakeApiFactory();
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetNotes_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/notes");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateNote_ReturnsCreatedNote()
    {
        var response = await _client.PostAsJsonAsync("/api/notes", new { content = "Comprar leche", kind = "shopping" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var note = await response.Content.ReadFromJsonAsync<Note>();
        Assert.Equal("Comprar leche", note!.Content);
        Assert.NotEqual(default, note.CreatedAt);
    }

    [Fact]
    public async Task GetNotesByKind_ReturnsOnlyThatKind()
    {
        await _client.PostAsJsonAsync("/api/notes", new { content = "Una idea", kind = "note" });
        await _client.PostAsJsonAsync("/api/notes", new { content = "Comprar pan", kind = "shopping" });

        var response = await _client.GetAsync("/api/notes?kind=shopping");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var notes = await response.Content.ReadFromJsonAsync<List<Note>>();
        Assert.Single(notes!);
        Assert.Equal("shopping", notes![0].Kind);
    }

    [Fact]
    public async Task ToggleNote_MarksAsDone()
    {
        var created = await _client.PostAsJsonAsync("/api/notes", new { content = "Comprar leche", kind = "shopping" });
        var note = await created.Content.ReadFromJsonAsync<Note>();

        var response = await _client.PatchAsync($"/api/notes/{note!.Id}/toggle", new StringContent(""));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<Note>();
        Assert.True(result!.IsDone);
    }

    [Fact]
    public async Task DeleteNote_ReturnsNoContent()
    {
        var created = await _client.PostAsJsonAsync("/api/notes", new { content = "Comprar leche", kind = "shopping" });
        var note = await created.Content.ReadFromJsonAsync<Note>();

        var response = await _client.DeleteAsync($"/api/notes/{note!.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}
