using System.Security.Claims;
using HomeHub.Hubs;
using HomeHub.Models;
using HomeHub.Models.Dtos;
using HomeHub.Repositories;
using Microsoft.AspNetCore.SignalR;
using Npgsql;

namespace HomeHub.Endpoints
{
    public static class ShoppingCategoryEndpoints
    {
        public static void MapShoppingCategoryEndpoints(this WebApplication app)
        {
            app.MapGet("/api/shopping-categories", (IShoppingCategoryRepository repository, ClaimsPrincipal user) =>
            {
                return Results.Ok(repository.GetAll(user.GetUserId()));
            }).RequireAuthorization();

            app.MapPost("/api/shopping-categories", async (ShoppingCategoryRequest request, IShoppingCategoryRepository repository, IHubContext<NotesHub> hub, ClaimsPrincipal user) =>
            {
                var name = (request.Name ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(name))
                    return Results.BadRequest(new { error = "El nombre de la etiqueta no puede estar vacío." });

                var userId = user.GetUserId();
                if (repository.GetAll(userId).Any(category => category.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                    return Results.Conflict(new { error = "Ya tienes una etiqueta con ese nombre." });

                var category = new ShoppingCategory
                {
                    UserId = userId,
                    Name = name
                };

                try
                {
                    category.Id = repository.Save(category);
                }
                catch (PostgresException exception) when (exception.SqlState == "23505")
                {
                    return Results.Conflict(new { error = "Ya tienes una etiqueta con ese nombre." });
                }

                await hub.Clients.Group(NotesHub.GroupName(userId)).SendAsync("notesChanged");
                return Results.Created($"/api/shopping-categories/{category.Id}", category);
            }).RequireAuthorization();

            app.MapDelete("/api/shopping-categories/{id}", async (int id, IShoppingCategoryRepository repository, IHubContext<NotesHub> hub, ClaimsPrincipal user) =>
            {
                var userId = user.GetUserId();
                if (repository.GetById(id, userId) is null)
                    return Results.NotFound();

                repository.Delete(id, userId);
                await hub.Clients.Group(NotesHub.GroupName(userId)).SendAsync("notesChanged");
                return Results.NoContent();
            }).RequireAuthorization();
        }
    }
}
