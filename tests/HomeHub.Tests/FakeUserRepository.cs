using HomeHub.Models;
using HomeHub.Repositories;

namespace HomeHub.Tests;

public class FakeUserRepository : IUserRepository
{
    private readonly List<User> _users = new();
    private int _lastId;

    public void InitializeDatabase()
    {
    }

    public int Save(User user)
    {
        user.Id = ++_lastId;
        _users.Add(user);
        return user.Id;
    }

    public User? GetByEmail(string email)
    {
        return _users.FirstOrDefault(user => user.Email == email);
    }
}
