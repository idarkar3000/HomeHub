using Dapper;
using HomeHub.Models;
using Npgsql;

namespace HomeHub.Repositories
{
    public class PostgresShoppingCategoryRepository : IShoppingCategoryRepository
    {
        private readonly string _connectionString;

        public PostgresShoppingCategoryRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void InitializeDatabase()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            string sql = @"CREATE TABLE IF NOT EXISTS shopping_categories (
                        id SERIAL PRIMARY KEY,
                        user_id INT NOT NULL REFERENCES users(id) ON DELETE CASCADE,
                        name TEXT NOT NULL,
                        created_at TIMESTAMP NOT NULL DEFAULT now(),
                        UNIQUE (user_id, name)
                        );";

            connection.Execute(sql);
        }

        public int Save(ShoppingCategory category)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            string sql = @"INSERT INTO shopping_categories (user_id, name) VALUES (@UserId, @Name) RETURNING id;";
            return connection.ExecuteScalar<int>(sql, category);
        }

        public List<ShoppingCategory> GetAll(int userId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            string sql = @"SELECT id AS Id, user_id AS UserId, name AS Name, created_at AS CreatedAt
                   FROM shopping_categories
                   WHERE user_id = @UserId
                   ORDER BY name ASC;";

            return connection.Query<ShoppingCategory>(sql, new { UserId = userId }).ToList();
        }

        public ShoppingCategory? GetById(int id, int userId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            string sql = @"SELECT id AS Id, user_id AS UserId, name AS Name, created_at AS CreatedAt
                   FROM shopping_categories WHERE id = @Id AND user_id = @UserId;";
            return connection.QueryFirstOrDefault<ShoppingCategory>(sql, new { Id = id, UserId = userId });
        }

        public void Delete(int id, int userId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            connection.Execute("DELETE FROM shopping_categories WHERE id = @Id AND user_id = @UserId;", new { Id = id, UserId = userId });
        }
    }
}
