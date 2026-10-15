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
                "name TEXT NOT NULL," +
                "icon TEXT NOT NULL)";

            connection.Execute(sql);
        }

        public int Save(Room room)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            string sql = @"INSERT INTO rooms (name, icon) VALUES (@Name, @Icon) RETURNING id;";
            return connection.ExecuteScalar<int>(sql, room);
        }

        public List<Room> GetAll()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            string sql = @"SELECT id AS Id, name AS Name, icon AS Icon FROM rooms ORDER BY id ASC;";
            return connection.Query<Room>(sql).ToList();
        }

        public Room? GetById(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            string sql = @"SELECT id AS Id, name AS Name, icon AS Icon FROM rooms WHERE id = @Id;";
            return connection.QueryFirstOrDefault<Room>(sql, new { Id = id });
        }

        public void Update(Room room)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            string sql = @"UPDATE rooms SET name = @Name, icon = @Icon WHERE id = @Id;";
            connection.Execute(sql, room);
        }

        public void Delete(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            string sql = @"DELETE FROM rooms WHERE id = @Id;";

            connection.Execute(sql, new { Id = id });
        }
    }
}
