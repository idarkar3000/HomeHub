using HomeHub.Models;

namespace HomeHub.Repositories
{
    public interface ITaskRepository
    {
        void InitializeDatabase();
        int Save(TaskItem task);
        List<TaskItem> GetAll();
        TaskItem? GetById(int id);
        List<TaskItem> GetByRoomId(int roomid);
        List<TaskItem> GetCalendarTask();
        void Update(TaskItem task);
        void Delete(int id);
    }
}
