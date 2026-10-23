using HomeHub.Models;

namespace HomeHub.Repositories
{
    public interface IShoppingCategoryRepository
    {
        void InitializeDatabase();
        int Save(ShoppingCategory category);
        List<ShoppingCategory> GetAll(int userId);
        ShoppingCategory? GetById(int id, int userId);
        void Delete(int id, int userId);
    }
}
