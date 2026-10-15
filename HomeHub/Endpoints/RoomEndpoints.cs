using HomeHub.Models;
using HomeHub.Models.Dtos;
using HomeHub.Repositories;

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

            app.MapGet("/api/rooms/{id}", (int id, IRoomRepository repository) =>
            {
                var room = repository.GetById(id);
                return room is null ? Results.NotFound() : Results.Ok(room);
            });

            app.MapPost("/api/rooms", (RoomRequest request, IRoomRepository repository) =>
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                    return Results.BadRequest();

                var room = new Room
                {
                    Name = request.Name,
                    Icon = request.Icon
                };

                room.Id = repository.Save(room);
                return Results.Created($"/api/rooms/{room.Id}", room);
            });

            app.MapPut("/api/rooms/{id}", (int id, RoomRequest request, IRoomRepository repository) =>
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                    return Results.BadRequest();

                var room = repository.GetById(id);
                if (room is null)
                    return Results.NotFound();

                room.Name = request.Name;
                room.Icon = request.Icon;

                repository.Update(room);
                return Results.Ok(room);
            });

            app.MapDelete("/api/rooms/{id}", (int id, IRoomRepository repository) =>
            {
                var room = repository.GetById(id);
                if (room is null)
                    return Results.NotFound();

                repository.Delete(id);
                return Results.NoContent();
            });
        }
    }
}
