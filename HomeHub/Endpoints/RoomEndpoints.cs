using HomeHub.Models;
using HomeHub.Repositories;
using System.Runtime.CompilerServices;

namespace HomeHub.Endpoints
{
    public static class RoomEndpoints
    {
        public static void MapRoomEndpoints(this WebApplication app)
        {
            app.MapGet("/api/rooms", (IRoomRepository repository) =>
                {
                    var rooms = repository.GetAll();
                    return Results.Ok(rooms);
                });

            app.MapPost("/api/rooms", (Room room, IRoomRepository repository) =>
            {
                repository.Save(room);
                return Results.Created($"/api/rooms/{room.Id}", room);
            });

            app.MapDelete("/api/rooms/{id}", (int id, IRoomRepository repository) =>
            {
                repository.Delete(id);
                return Results.NoContent();
            });
        }
    }
}
