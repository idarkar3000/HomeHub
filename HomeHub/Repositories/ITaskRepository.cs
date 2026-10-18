using HomeHub.Models;

namespace HomeHub.Repositories
{
    public interface ITaskRepository
    {
        void InitializeDatabase();
        int Save(TaskItem task);
        List<TaskItem> GetAll(int userId);
        TaskItem? GetById(int id, int userId);
        List<TaskItem> GetByRoomId(int roomid, int userId);
        List<TaskItem> GetCalendarTask(int userId);
        void Update(TaskItem task);
        void Delete(int id, int userId);
    }
}
