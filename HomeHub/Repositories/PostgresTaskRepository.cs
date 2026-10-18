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
                        user_id INT NOT NULL REFERENCES users(id) ON DELETE CASCADE,
                        room_id INT NOT NULL,
                        title TEXT NOT NULL,
                        is_completed BOOLEAN NOT NULL,
                        due_date TIMESTAMP,
                        end_date TIMESTAMP,
                        recurrence TEXT NOT NULL DEFAULT 'none',
                        FOREIGN KEY (room_id) REFERENCES rooms(id) ON DELETE CASCADE
                        );";

            connection.Execute(sql);
            connection.Execute("ALTER TABLE tasks ADD COLUMN IF NOT EXISTS recurrence TEXT NOT NULL DEFAULT 'none';");
            connection.Execute("ALTER TABLE tasks ADD COLUMN IF NOT EXISTS end_date TIMESTAMP;");
            connection.Execute("ALTER TABLE tasks ADD COLUMN IF NOT EXISTS user_id INT;");
        }

        public int Save(TaskItem task)
        {
            using var connection = new NpgsqlConnection(_connectionstring);
            connection.Open();

            string sql = @"INSERT INTO tasks (user_id, room_id, title, is_completed, due_date, end_date, recurrence) VALUES (@UserId, @RoomId, @Title, @IsCompleted, @DueDate, @EndDate, @Recurrence) RETURNING id;";

            return connection.ExecuteScalar<int>(sql, task);
        }

        public List<TaskItem> GetAll(int userId)
        {
            using var connection = new NpgsqlConnection(_connectionstring);
            connection.Open();

            string sql = @"SELECT 
                    id AS Id, 
                    user_id AS UserId,
                    room_id AS RoomId, 
                    title AS Title, 
                    is_completed AS IsCompleted, 
                    due_date AS DueDate, 
                    end_date AS EndDate, 
                    recurrence AS Recurrence 
                   FROM tasks WHERE user_id = @UserId;";

            return connection.Query<TaskItem>(sql, new { UserId = userId }).ToList();
        }

        public TaskItem? GetById(int id, int userId)
        {
            using var connection = new NpgsqlConnection(_connectionstring);
            connection.Open();

            string sql = @"SELECT id AS Id, 
                    user_id AS UserId,
                    room_id AS RoomId, 
                    title AS Title, 
                    is_completed AS IsCompleted, 
                    due_date AS DueDate, 
                    end_date AS EndDate, 
                    recurrence AS Recurrence 
                    FROM tasks WHERE id = @Id AND user_id = @UserId;";
            return connection.QueryFirstOrDefault<TaskItem>(sql, new { Id = id, UserId = userId });
        }

        public List<TaskItem> GetByRoomId(int roomid, int userId)
        {
            using var connection = new NpgsqlConnection(_connectionstring);
            connection.Open();

            string sql = @"SELECT id AS Id, 
                    user_id AS UserId,
                    room_id AS RoomId, 
                    title AS Title, 
                    is_completed AS IsCompleted, 
                    due_date AS DueDate, 
                    end_date AS EndDate, 
                    recurrence AS Recurrence 
                    FROM tasks WHERE room_id = @RoomId AND user_id = @UserId ORDER BY id ASC;";
            return connection.Query<TaskItem>(sql, new { RoomId = roomid, UserId = userId }).ToList();
        }

        public List<TaskItem> GetCalendarTask(int userId)
        {
            using var connection = new NpgsqlConnection(_connectionstring);
            connection.Open();

            string sql = @"SELECT id AS Id, 
                    user_id AS UserId,
                    room_id AS RoomId, 
                    title AS Title, 
                    is_completed AS IsCompleted, 
                    due_date AS DueDate, 
                    end_date AS EndDate, 
                    recurrence AS Recurrence 
                    FROM tasks WHERE due_date IS NOT NULL AND user_id = @UserId
                    ORDER BY due_date ASC;";

            return connection.Query<TaskItem>(sql, new { UserId = userId }).ToList();
        }

        public void Update(TaskItem task)
        {
            using var connection = new NpgsqlConnection(_connectionstring);
            connection.Open();

            string sql = @"UPDATE tasks 
                   SET title = @Title, 
                       is_completed = @IsCompleted, 
                       due_date = @DueDate, 
                       end_date = @EndDate, 
                       recurrence = @Recurrence, 
                       room_id = @RoomId 
                   WHERE id = @Id;";

            connection.Execute(sql, task);
        }

        public void Delete(int id, int userId)
        {
            using var connection = new NpgsqlConnection(_connectionstring);
            connection.Open();

            string sql = @"DELETE FROM tasks WHERE id = @Id AND user_id = @UserId;";

            connection.Execute(sql, new { Id = id, UserId = userId });
        }
    }
}
