using HomeHub.Models;

namespace HomeHub.Repositories
{
    public interface INoteRepository
    {
        void InitializeDatabase();
        int Save(Note note);
        List<Note> GetAll(string? kind, int userId);
        Note? GetById(int id, int userId);
        void Update(Note note);
        void Delete(int id, int userId);
    }
}
