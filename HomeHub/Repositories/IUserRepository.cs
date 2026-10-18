using HomeHub.Models;

namespace HomeHub.Repositories
{
    public interface IUserRepository
    {
        void InitializeDatabase();
        int Save(User user);
        User? GetByEmail(string email);
    }
}
