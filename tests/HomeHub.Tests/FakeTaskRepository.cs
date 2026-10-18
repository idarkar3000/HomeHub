using HomeHub.Models;
using HomeHub.Repositories;

namespace HomeHub.Tests;

public class FakeTaskRepository : ITaskRepository
{
    private readonly List<TaskItem> _tasks = new();
    private int _lastId;

    public void InitializeDatabase()
    {
    }

    public int Save(TaskItem task)
    {
        task.Id = ++_lastId;
        _tasks.Add(task);
        return task.Id;
    }

    public List<TaskItem> GetAll(int userId)
    {
        return _tasks.Where(task => task.UserId == userId).ToList();
    }

    public TaskItem? GetById(int id, int userId)
    {
        return _tasks.FirstOrDefault(task => task.Id == id && task.UserId == userId);
    }

    public List<TaskItem> GetByRoomId(int roomid, int userId)
    {
        return _tasks.Where(task => task.RoomId == roomid && task.UserId == userId).ToList();
    }

    public List<TaskItem> GetCalendarTask(int userId)
    {
        return _tasks
            .Where(task => task.UserId == userId && task.DueDate != null)
            .OrderBy(task => task.DueDate)
            .ToList();
    }

    public void Update(TaskItem task)
    {
        var index = _tasks.FindIndex(t => t.Id == task.Id);
        if (index >= 0)
        {
            _tasks[index] = task;
        }
    }

    public void Delete(int id, int userId)
    {
        _tasks.RemoveAll(task => task.Id == id && task.UserId == userId);
    }
}
