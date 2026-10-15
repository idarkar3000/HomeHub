using HomeHub.Models;
using HomeHub.Repositories;

namespace HomeHub.Endpoints
{
    public static class TaskEndpoints
    {
        public static void MapTaskEndpoints(this WebApplication app)
        {
            app.MapGet("api/task", (ITaskRepository repository) =>
            {
                var tasks = repository.GetAll();
                return Results.Ok(tasks);
            });

            app.MapGet("api/task/{id}", (int id, ITaskRepository repository) =>
            {
                var tasks = repository.GetByRoomId(id);
                return Results.Ok(tasks);
            });

            app.MapGet("api/calendar-task", (ITaskRepository repository) =>
            {
                var tasks = repository.GetCalendarTask();
                return Results.Ok(tasks);
            });

            app.MapPost("api/task", (TaskItem task, ITaskRepository repository) =>
            {
                repository.Save(task);
                return Results.Created($"/api/task/{task.Id}", task);
            });

            app.MapPut("api/task/update", (TaskItem item, ITaskRepository repository) =>
            {
                repository.Update(item);
                return Results.Ok(item);
            });

            app.MapDelete("api/task/delete", (int id, ITaskRepository repository) =>
            {
                repository.Delete(id);
                return Results.NoContent();
            });
        }
    }
}
