using Dapper;
using HomeHub.Models;
using Npgsql;

namespace HomeHub.Repositories
{
    public class PostgresUserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public PostgresUserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void InitializeDatabase()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            string sql = @"CREATE TABLE IF NOT EXISTS users (
                        id SERIAL PRIMARY KEY,
                        email TEXT NOT NULL UNIQUE,
                        password_hash TEXT NOT NULL,
                        created_at TIMESTAMP NOT NULL DEFAULT now()
                        );";

            connection.Execute(sql);
        }

        public int Save(User user)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            string sql = @"INSERT INTO users (email, password_hash) VALUES (@Email, @PasswordHash) RETURNING id;";
            return connection.ExecuteScalar<int>(sql, user);
        }

        public User? GetByEmail(string email)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            string sql = @"SELECT id AS Id, email AS Email, password_hash AS PasswordHash, created_at AS CreatedAt FROM users WHERE email = @Email;";
            return connection.QueryFirstOrDefault<User>(sql, new { Email = email });
        }
    }
}
