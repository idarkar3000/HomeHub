using HomeHub.Models;

namespace HomeHub.Repositories
{
    public interface ITaskRepository
    {
        void InitializeDatabase();
        void Save(TaskItem task);
        List<TaskItem> GetAll();
        List<TaskItem> GetByRoomId(int roomid);
        List<TaskItem> GetCalendarTask();
        void Update(TaskItem task);
        void Delete(int id);
    }
}
