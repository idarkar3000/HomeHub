using System.Net;
using System.Net.Http.Json;
using HomeHub.Models;

namespace HomeHub.Tests;

public class ShoppingCategoryEndpointsTests
{
    private readonly HttpClient _client;

    public ShoppingCategoryEndpointsTests()
    {
        var factory = new FakeApiFactory();
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetCategories_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/shopping-categories");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateCategory_ReturnsCreatedCategory()
    {
        var response = await _client.PostAsJsonAsync("/api/shopping-categories", new { name = "Mercadona" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var category = await response.Content.ReadFromJsonAsync<ShoppingCategory>();
        Assert.Equal("Mercadona", category!.Name);
    }

    [Fact]
    public async Task CreateDuplicateCategory_ReturnsConflict()
    {
        await _client.PostAsJsonAsync("/api/shopping-categories", new { name = "Día" });

        var response = await _client.PostAsJsonAsync("/api/shopping-categories", new { name = "día" });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task CreateEmptyCategory_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/shopping-categories", new { name = "   " });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteCategory_ReturnsNoContent()
    {
        var created = await _client.PostAsJsonAsync("/api/shopping-categories", new { name = "Mercadona" });
        var category = await created.Content.ReadFromJsonAsync<ShoppingCategory>();

        var response = await _client.DeleteAsync($"/api/shopping-categories/{category!.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task CreateNoteWithCategory_ReturnsCategoryId()
    {
        var created = await _client.PostAsJsonAsync("/api/shopping-categories", new { name = "Mercadona" });
        var category = await created.Content.ReadFromJsonAsync<ShoppingCategory>();

        var response = await _client.PostAsJsonAsync(
            "/api/notes",
            new { content = "Yogures", kind = "shopping", categoryId = category!.Id });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var note = await response.Content.ReadFromJsonAsync<Note>();
        Assert.Equal(category.Id, note!.CategoryId);
    }

    [Fact]
    public async Task CreateNoteWithUnknownCategory_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/notes",
            new { content = "Yogures", kind = "shopping", categoryId = 999 });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
