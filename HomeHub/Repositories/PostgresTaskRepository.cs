using Dapper;
using HomeHub.Models;
using Npgsql;

namespace HomeHub.Repositories
{

    public class PostgresTaskRepository : ITaskRepository
    {
        private readonly string _connectionstring;

        public PostgresTaskRepository(string connectionString)
        {
            _connectionstring = connectionString;
        }

        public void InitializeDatabase()
        {
            using var connection = new NpgsqlConnection(_connectionstring);
            connection.Open();

            string sql = @"CREATE TABLE IF NOT EXISTS tasks (
                        id SERIAL PRIMARY KEY,
                        room_id INT NOT NULL,
                        title TEXT NOT NULL,
                        is_completed BOOLEAN NOT NULL,
                        due_date TIMESTAMP,
                        FOREIGN KEY (room_id) REFERENCES rooms(id) ON DELETE CASCADE
                        );";

            connection.Execute(sql);
        }

        public void Save(TaskItem task)
        {
            using var connection = new NpgsqlConnection(_connectionstring);
            connection.Open();

            string sql = @"INSERT INTO tasks (title, is_completed, due_date, room_id) VALUES (@Title, @IsCompleted, @DueDate, @RoomId);";

            connection.Execute(sql, task);
        }

        public List<TaskItem> GetAll()
        {
            using var connection = new NpgsqlConnection(_connectionstring);
            connection.Open();

            string sql = @"SELECT 
                    id AS Id, 
                    room_id AS RoomId, 
                    title AS Title, 
                    is_completed AS IsCompleted, 
                    due_date AS DueDate 
                   FROM tasks;";

            return connection.Query<TaskItem>(sql).ToList();
        }

        public List<TaskItem> GetByRoomId(int roomid)
        {
            using var connection = new NpgsqlConnection(_connectionstring);
            connection.Open();

            string sql = @"SELECT id AS Id, 
                    room_id AS RoomId, 
                    title AS Title, 
                    is_completed AS IsCompleted, 
                    due_date AS DueDate 
                    FROM tasks WHERE room_id = @RoomId;";
            return connection.Query<TaskItem>(sql, new { RoomId = roomid }).ToList();
        }

        public List<TaskItem> GetCalendarTask()
        {
            using var connection = new NpgsqlConnection(_connectionstring);
            connection.Open();

            string sql = @"SELECT id AS Id, 
                    room_id AS RoomId, 
                    title AS Title, 
                    is_completed AS IsCompleted, 
                    due_date AS DueDate 
                    FROM tasks WHERE due_date IS NOT NULL
                    ORDER BY due_date ASC;";

            return connection.Query<TaskItem>(sql).ToList();
        }

        public void Update(TaskItem task)
        {
            using var connection = new NpgsqlConnection(_connectionstring);
            connection.Open();

            string sql = @"UPDATE tasks 
                   SET title = @Title, 
                       is_completed = @IsCompleted, 
                       due_date = @DueDate, 
                       room_id = @RoomId 
                   WHERE id = @Id;";

            connection.Execute(sql, task);
        }

        public void Delete(int id)
        {
            using var connection = new NpgsqlConnection(_connectionstring);
            connection.Open();

            string sql = @"DELETE FROM tasks WHERE id = @Id;";

            connection.Execute(sql, new { Id = id });
        }
    }
}
