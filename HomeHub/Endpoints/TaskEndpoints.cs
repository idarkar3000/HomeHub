using System.Security.Claims;
using HomeHub.Models;
using HomeHub.Models.Dtos;
using HomeHub.Repositories;

namespace HomeHub.Endpoints
{
    public static class TaskEndpoints
    {
        public static void MapTaskEndpoints(this WebApplication app)
        {
            app.MapGet("/api/tasks", (ITaskRepository repository, ClaimsPrincipal user) =>
            {
                var tasks = repository.GetAll(user.GetUserId());
                return Results.Ok(tasks);
            }).RequireAuthorization();

            app.MapGet("/api/tasks/calendar", (ITaskRepository repository, ClaimsPrincipal user) =>
            {
                var tasks = repository.GetCalendarTask(user.GetUserId());
                return Results.Ok(tasks);
            }).RequireAuthorization();

            app.MapGet("/api/tasks/{id}", (int id, ITaskRepository repository, ClaimsPrincipal user) =>
            {
                var task = repository.GetById(id, user.GetUserId());
                return task is null ? Results.NotFound() : Results.Ok(task);
            }).RequireAuthorization();

            app.MapGet("/api/rooms/{roomId}/tasks", (int roomId, ITaskRepository repository, IRoomRepository roomRepository, ClaimsPrincipal user) =>
            {
                var userId = user.GetUserId();
                var room = roomRepository.GetById(roomId, userId);
                if (room is null)
                    return Results.NotFound();

                var tasks = repository.GetByRoomId(roomId, userId);
                return Results.Ok(tasks);
            }).RequireAuthorization();

            app.MapPost("/api/rooms/{roomId}/tasks", (int roomId, TaskRequest request, ITaskRepository repository, IRoomRepository roomRepository, ClaimsPrincipal user) =>
            {
                if (string.IsNullOrWhiteSpace(request.Title))
                    return Results.BadRequest();

                var userId = user.GetUserId();
                var room = roomRepository.GetById(roomId, userId);
                if (room is null)
                    return Results.NotFound();

                var task = new TaskItem
                {
                    UserId = userId,
                    RoomId = roomId,
                    Title = request.Title,
                    IsCompleted = request.IsCompleted,
                    DueDate = request.DueDate,
                    EndDate = request.EndDate,
                    Recurrence = request.Recurrence
                };

                task.Id = repository.Save(task);
                return Results.Created($"/api/tasks/{task.Id}", task);
            }).RequireAuthorization();

            app.MapPut("/api/tasks/{id}", (int id, TaskRequest request, ITaskRepository repository, IRoomRepository roomRepository, ClaimsPrincipal user) =>
            {
                if (string.IsNullOrWhiteSpace(request.Title))
                    return Results.BadRequest();

                var userId = user.GetUserId();
                var task = repository.GetById(id, userId);
                if (task is null)
                    return Results.NotFound();

                var room = roomRepository.GetById(request.RoomId, userId);
                if (room is null)
                    return Results.NotFound();

                task.RoomId = request.RoomId;
                task.Title = request.Title;
                task.IsCompleted = request.IsCompleted;
                task.DueDate = request.DueDate;
                task.EndDate = request.EndDate;
                task.Recurrence = request.Recurrence;

                repository.Update(task);
                return Results.Ok(task);
            }).RequireAuthorization();

            app.MapPatch("/api/tasks/{id}/toggle", (int id, ITaskRepository repository, ClaimsPrincipal user) =>
            {
                var task = repository.GetById(id, user.GetUserId());
                if (task is null)
                    return Results.NotFound();

                if (!task.IsCompleted && task.DueDate is not null && task.Recurrence != "none")
                {
                    task.DueDate = Recurrence.Next(task.DueDate.Value, task.Recurrence);

                    if (task.EndDate is not null)
                        task.EndDate = Recurrence.Next(task.EndDate.Value, task.Recurrence);
                }
                else
                {
                    task.IsCompleted = !task.IsCompleted;
                }

                repository.Update(task);
                return Results.Ok(task);
            }).RequireAuthorization();

            app.MapDelete("/api/tasks/{id}", (int id, ITaskRepository repository, ClaimsPrincipal user) =>
            {
                var task = repository.GetById(id, user.GetUserId());
                if (task is null)
                    return Results.NotFound();

                repository.Delete(id, user.GetUserId());
                return Results.NoContent();
            }).RequireAuthorization();
        }
    }
}
