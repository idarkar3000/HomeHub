using HomeHub.Models;
using HomeHub.Models.Dtos;
using HomeHub.Repositories;

namespace HomeHub.Endpoints
{
    public static class TaskEndpoints
    {
        public static void MapTaskEndpoints(this WebApplication app)
        {
            app.MapGet("/api/tasks", (ITaskRepository repository) =>
            {
                var tasks = repository.GetAll();
                return Results.Ok(tasks);
            });

            app.MapGet("/api/tasks/calendar", (ITaskRepository repository) =>
            {
                var tasks = repository.GetCalendarTask();
                return Results.Ok(tasks);
            });

            app.MapGet("/api/tasks/{id}", (int id, ITaskRepository repository) =>
            {
                var task = repository.GetById(id);
                return task is null ? Results.NotFound() : Results.Ok(task);
            });

            app.MapGet("/api/rooms/{roomId}/tasks", (int roomId, ITaskRepository repository, IRoomRepository roomRepository) =>
            {
                var room = roomRepository.GetById(roomId);
                if (room is null)
                    return Results.NotFound();

                var tasks = repository.GetByRoomId(roomId);
                return Results.Ok(tasks);
            });

            app.MapPost("/api/rooms/{roomId}/tasks", (int roomId, TaskRequest request, ITaskRepository repository, IRoomRepository roomRepository) =>
            {
                if (string.IsNullOrWhiteSpace(request.Title))
                    return Results.BadRequest();

                var room = roomRepository.GetById(roomId);
                if (room is null)
                    return Results.NotFound();

                var task = new TaskItem
                {
                    RoomId = roomId,
                    Title = request.Title,
                    IsCompleted = request.IsCompleted,
                    DueDate = request.DueDate,
                    EndDate = request.EndDate,
                    Recurrence = request.Recurrence
                };

                task.Id = repository.Save(task);
                return Results.Created($"/api/tasks/{task.Id}", task);
            });

            app.MapPut("/api/tasks/{id}", (int id, TaskRequest request, ITaskRepository repository, IRoomRepository roomRepository) =>
            {
                if (string.IsNullOrWhiteSpace(request.Title))
                    return Results.BadRequest();

                var task = repository.GetById(id);
                if (task is null)
                    return Results.NotFound();

                var room = roomRepository.GetById(request.RoomId);
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
            });

            app.MapPatch("/api/tasks/{id}/toggle", (int id, ITaskRepository repository) =>
            {
                var task = repository.GetById(id);
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
            });

            app.MapDelete("/api/tasks/{id}", (int id, ITaskRepository repository) =>
            {
                var task = repository.GetById(id);
                if (task is null)
                    return Results.NotFound();

                repository.Delete(id);
                return Results.NoContent();
            });
        }
    }
}
