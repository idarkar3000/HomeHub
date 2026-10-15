using HomeHub.Models;

namespace HomeHub.Repositories
{
    public interface INoteRepository
    {
        void InitializeDatabase();
        int Save(Note note);
        List<Note> GetAll(string? kind);
        Note? GetById(int id);
        void Update(Note note);
        void Delete(int id);
    }
}
