using System.Security.Claims;
using HomeHub.Models;
using HomeHub.Models.Dtos;
using HomeHub.Repositories;

namespace HomeHub.Endpoints
{
    public static class RoomEndpoints
    {
        public static void MapRoomEndpoints(this WebApplication app)
        {
            app.MapGet("/api/rooms", (IRoomRepository repository, ClaimsPrincipal user) =>
            {
                var rooms = repository.GetAll(user.GetUserId());
                return Results.Ok(rooms);
            }).RequireAuthorization();

            app.MapGet("/api/rooms/{id}", (int id, IRoomRepository repository, ClaimsPrincipal user) =>
            {
                var room = repository.GetById(id, user.GetUserId());
                return room is null ? Results.NotFound() : Results.Ok(room);
            }).RequireAuthorization();

            app.MapPost("/api/rooms", (RoomRequest request, IRoomRepository repository, ClaimsPrincipal user) =>
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                    return Results.BadRequest();

                var room = new Room
                {
                    UserId = user.GetUserId(),
                    Name = request.Name,
                    Icon = request.Icon
                };

                room.Id = repository.Save(room);
                return Results.Created($"/api/rooms/{room.Id}", room);
            }).RequireAuthorization();

            app.MapPut("/api/rooms/{id}", (int id, RoomRequest request, IRoomRepository repository, ClaimsPrincipal user) =>
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                    return Results.BadRequest();

                var room = repository.GetById(id, user.GetUserId());
                if (room is null)
                    return Results.NotFound();

                room.Name = request.Name;
                room.Icon = request.Icon;

                repository.Update(room);
                return Results.Ok(room);
            }).RequireAuthorization();

            app.MapDelete("/api/rooms/{id}", (int id, IRoomRepository repository, ClaimsPrincipal user) =>
            {
                var room = repository.GetById(id, user.GetUserId());
                if (room is null)
                    return Results.NotFound();

                repository.Delete(id, user.GetUserId());
                return Results.NoContent();
            }).RequireAuthorization();
        }
    }
}
