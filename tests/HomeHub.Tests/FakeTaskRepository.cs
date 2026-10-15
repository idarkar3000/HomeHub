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

    public List<TaskItem> GetAll()
    {
        return _tasks;
    }

    public TaskItem? GetById(int id)
    {
        return _tasks.FirstOrDefault(task => task.Id == id);
    }

    public List<TaskItem> GetByRoomId(int roomid)
    {
        return _tasks.Where(task => task.RoomId == roomid).ToList();
    }

    public List<TaskItem> GetCalendarTask()
    {
        return _tasks.Where(task => task.DueDate != null).OrderBy(task => task.DueDate).ToList();
    }

    public void Update(TaskItem task)
    {
        var index = _tasks.FindIndex(t => t.Id == task.Id);
        if (index >= 0)
        {
            _tasks[index] = task;
        }
    }

    public void Delete(int id)
    {
        _tasks.RemoveAll(task => task.Id == id);
    }
}
