using System.Security.Claims;
using HomeHub.Models;
using HomeHub.Models.Dtos;
using HomeHub.Repositories;
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

            app.MapPost("/api/shopping-categories", (ShoppingCategoryRequest request, IShoppingCategoryRepository repository, ClaimsPrincipal user) =>
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

                return Results.Created($"/api/shopping-categories/{category.Id}", category);
            }).RequireAuthorization();

            app.MapDelete("/api/shopping-categories/{id}", (int id, IShoppingCategoryRepository repository, ClaimsPrincipal user) =>
            {
                if (repository.GetById(id, user.GetUserId()) is null)
                    return Results.NotFound();

                repository.Delete(id, user.GetUserId());
                return Results.NoContent();
            }).RequireAuthorization();
        }
    }
}
