using HomeHub.Models;

namespace HomeHub.Repositories
{
    public interface IRoomRepository
    {
        void InitializeDatabase();
        int Save(Room room);
        List<Room> GetAll(int userId);
        Room? GetById(int id, int userId);
        void Update(Room room);
        void Delete(int id, int userId);
    }
}
