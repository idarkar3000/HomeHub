using HomeHub.Models;

namespace HomeHub.Repositories
{
    public interface IRoomRepository
    {
        void InitializeDatabase();
        void Save(Room room);
        List<Room> GetAll();
        void Delete(int id);
    }
}
