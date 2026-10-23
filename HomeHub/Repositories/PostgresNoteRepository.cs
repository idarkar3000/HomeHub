using Dapper;
using HomeHub.Models;
using Npgsql;

namespace HomeHub.Repositories
{
    public class PostgresNoteRepository : INoteRepository
    {
        private readonly string _connectionString;

        public PostgresNoteRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void InitializeDatabase()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            string sql = @"CREATE TABLE IF NOT EXISTS notes (
                        id SERIAL PRIMARY KEY,
                        user_id INT NOT NULL REFERENCES users(id) ON DELETE CASCADE,
                        content TEXT NOT NULL,
                        is_done BOOLEAN NOT NULL DEFAULT false,
                        kind TEXT NOT NULL DEFAULT 'note',
                        room_id INT NULL REFERENCES rooms(id) ON DELETE SET NULL,
                        category_id INT NULL REFERENCES shopping_categories(id) ON DELETE SET NULL,
                        created_at TIMESTAMP NOT NULL DEFAULT now()
                        );";

            connection.Execute(sql);
            connection.Execute("ALTER TABLE notes ADD COLUMN IF NOT EXISTS user_id INT;");
            connection.Execute("ALTER TABLE notes ADD COLUMN IF NOT EXISTS category_id INT REFERENCES shopping_categories(id) ON DELETE SET NULL;");
        }

        public int Save(Note note)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            string sql = @"INSERT INTO notes (user_id, content, is_done, kind, room_id, category_id) VALUES (@UserId, @Content, @IsDone, @Kind, @RoomId, @CategoryId) RETURNING id;";
            return connection.ExecuteScalar<int>(sql, note);
        }

        public List<Note> GetAll(string? kind, int userId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            string sql = @"SELECT id AS Id, user_id AS UserId, content AS Content, is_done AS IsDone, kind AS Kind, room_id AS RoomId, category_id AS CategoryId, created_at AS CreatedAt
                   FROM notes
                   WHERE user_id = @UserId AND (@Kind IS NULL OR kind = @Kind)
                   ORDER BY is_done ASC, id DESC;";

            return connection.Query<Note>(sql, new { UserId = userId, Kind = kind }).ToList();
        }

        public Note? GetById(int id, int userId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            string sql = @"SELECT id AS Id, user_id AS UserId, content AS Content, is_done AS IsDone, kind AS Kind, room_id AS RoomId, category_id AS CategoryId, created_at AS CreatedAt
                   FROM notes WHERE id = @Id AND user_id = @UserId;";
            return connection.QueryFirstOrDefault<Note>(sql, new { Id = id, UserId = userId });
        }

        public void Update(Note note)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            string sql = @"UPDATE notes SET content = @Content, is_done = @IsDone, kind = @Kind, room_id = @RoomId, category_id = @CategoryId WHERE id = @Id;";
            connection.Execute(sql, note);
        }

        public void Delete(int id, int userId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            connection.Execute("DELETE FROM notes WHERE id = @Id AND user_id = @UserId;", new { Id = id, UserId = userId });
        }
    }
}
