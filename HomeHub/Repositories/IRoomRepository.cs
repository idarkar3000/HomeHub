using HomeHub.Models;

namespace HomeHub.Repositories
{
    public interface IRoomRepository
    {
        void InitializeDatabase();
        int Save(Room room);
        List<Room> GetAll();
        Room? GetById(int id);
        void Update(Room room);
        void Delete(int id);
    }
}
