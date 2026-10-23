using HomeHub.Models;
using HomeHub.Repositories;

namespace HomeHub.Tests;

public class FakeShoppingCategoryRepository : IShoppingCategoryRepository
{
    private readonly List<ShoppingCategory> _categories = new();
    private int _lastId;

    public void InitializeDatabase()
    {
    }

    public int Save(ShoppingCategory category)
    {
        category.Id = ++_lastId;
        category.CreatedAt = DateTime.Now;
        _categories.Add(category);
        return category.Id;
    }

    public List<ShoppingCategory> GetAll(int userId)
    {
        return _categories
            .Where(category => category.UserId == userId)
            .OrderBy(category => category.Name)
            .ToList();
    }

    public ShoppingCategory? GetById(int id, int userId)
    {
        return _categories.FirstOrDefault(category => category.Id == id && category.UserId == userId);
    }

    public void Delete(int id, int userId)
    {
        _categories.RemoveAll(category => category.Id == id && category.UserId == userId);
    }
}
