using Dapper;
using HomeHub.Models;
using Npgsql;

namespace HomeHub.Repositories
{
    public class PostgresRoomRepository : IRoomRepository
    {
        private readonly string _connectionString;
        public PostgresRoomRepository (string connectionString)
        {
            _connectionString = connectionString;
        }

        public void InitializeDatabase()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            string sql = @"CREATE TABLE IF NOT EXISTS rooms (" +
                "id SERIAL PRIMARY KEY," +
                "user_id INT NOT NULL REFERENCES users(id) ON DELETE CASCADE," +
                "name TEXT NOT NULL," +
                "icon TEXT NOT NULL)";

            connection.Execute(sql);
            connection.Execute("ALTER TABLE rooms ADD COLUMN IF NOT EXISTS user_id INT;");
        }

        public int Save(Room room)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            string sql = @"INSERT INTO rooms (user_id, name, icon) VALUES (@UserId, @Name, @Icon) RETURNING id;";
            return connection.ExecuteScalar<int>(sql, room);
        }

        public List<Room> GetAll(int userId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            string sql = @"SELECT id AS Id, user_id AS UserId, name AS Name, icon AS Icon FROM rooms WHERE user_id = @UserId ORDER BY id ASC;";
            return connection.Query<Room>(sql, new { UserId = userId }).ToList();
        }

        public Room? GetById(int id, int userId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            string sql = @"SELECT id AS Id, user_id AS UserId, name AS Name, icon AS Icon FROM rooms WHERE id = @Id AND user_id = @UserId;";
            return connection.QueryFirstOrDefault<Room>(sql, new { Id = id, UserId = userId });
        }

        public void Update(Room room)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            string sql = @"UPDATE rooms SET name = @Name, icon = @Icon WHERE id = @Id;";
            connection.Execute(sql, room);
        }

        public void Delete(int id, int userId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            string sql = @"DELETE FROM rooms WHERE id = @Id AND user_id = @UserId;";

            connection.Execute(sql, new { Id = id, UserId = userId });
        }
    }
}
